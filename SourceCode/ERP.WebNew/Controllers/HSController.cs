using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.HS;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.Dictionary;
using ERP.Models.HS;
using ERP.Tools;
using ERP.WebNew.Attributes;
using ERP.WebNew.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace ERP.WebNew.Controllers
{
    [UserTrackerLog]
    [ERP.WebNew.Attributes.AuthorizedUser(PageID = (int)ERPPage.SystemManagement_HSCode)]
    public class HSController : Controller
    {
        private HSService _HSServices = new HSService();
        private UserServices _userService = new UserServices();

        ///主页面
        public ActionResult Index(VMHSContract vm_search)
        {
            vm_search.PageType = PageTypeEnum.PendingCheckList;
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SystemManagement_HSCode);

            if (vm_search.DataFlag == 2)
            {
                vm_search.DataFlag = 2;
            }
            else
            {
                vm_search.DataFlag = 1;
            }
            ViewData["IsCheck"] = GetOutStatus();
            ViewData["HSTypes"] = GetHSTypes();
            return View(vm_search);
        }

        /// <summary>
        /// 待审核的采购合同——合同状态
        /// </summary>
        /// <returns></returns>
        public static SelectList GetOutStatus()
        {
            List<SelectListItem> list = new List<SelectListItem>() { };
            list.Add(new SelectListItem() { Text = "", Value = "" });

            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            dictionary.Add("1", "是");
            dictionary.Add("2", "否");

            foreach (var item in dictionary)
            {
                list.Add(new SelectListItem() { Text = item.Value, Value = item.Key.ToString() });
            }
            SelectList generateList = new SelectList(list, "Value", "Text");

            return generateList;
        }

        public static SelectList GetHSTypes()
        {
            List<SelectListItem> list = new List<SelectListItem>() { };
            //list.Add(new SelectListItem() { Text = "", Value = "" });
            list.Add(new SelectListItem() { Text = "报关编码", Value = "1" });
            list.Add(new SelectListItem() { Text = "HS Code", Value = "2" });

            SelectList generateList = new SelectList(list, "Value", "Text");

            return generateList;
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SystemManagement_HSCode);
            if ((pageElementPrivileges & (int)HsElementsPrivileges.HsDelete) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            DBOperationStatus result = _HSServices.Delete(id);
            return new CustomJsonResult((short)result);
        }

        public ActionResult GetAll(VMHSContract vm_search)
        {
            int currentPage = ERP.Tools.Utils.StrToInt(HttpContext.Request[EasyuiPagenationConsts.CURRENT_PAGE], 1);
            int pageSize = ERP.Tools.Utils.StrToInt(HttpContext.Request[EasyuiPagenationConsts.PAGE_SIZE], 1);
            int totalRows = 0;

            #region 排序

            List<string> sortColumnsNames = new List<string>();
            if (!string.IsNullOrEmpty(HttpContext.Request[EasyuiPagenationConsts.SORT_COLUMN_NAMES]))
            {
                sortColumnsNames = HttpContext.Request[EasyuiPagenationConsts.SORT_COLUMN_NAMES].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            List<string> sortColumnOrders = new List<string>();
            if (!string.IsNullOrEmpty(HttpContext.Request[EasyuiPagenationConsts.SORT_COLUMN_ORDERS]))
            {
                sortColumnOrders = HttpContext.Request[EasyuiPagenationConsts.SORT_COLUMN_ORDERS].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            #endregion 排序

            #region 筛选条件

            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> { (int)ERPPage.SystemManagement_HSCode });

            if (vm_search.DataFlag == 0)
            {
                vm_search.DataFlag = 1; //设置默认值
            }

            #endregion 筛选条件

            List<DTOHSContract> listModel = _HSServices.GetAll(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows, vm_search);

            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit(int id, int DataFlag)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SystemManagement_HSCode);
            if ((id == 0 && (pageElementPrivileges & (int)HsElementsPrivileges.HsAdd) == 0) ||
                (id != 0 && (pageElementPrivileges & (int)HsElementsPrivileges.HsEdit) == 0))
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            if (DataFlag == 1)
            {
                //报关的（原来做过的） 1是报关的HSCODE类型，进口Hscode类型是2

                ViewBag.title = "修改报关信息";
                ViewBag.Name = "报关品名";
                ViewBag.EngName = "报关英文品名";
                ViewBag.Code = "海关编码";
            }
            else
            {
                //进口关税
                ViewBag.title = "修改HS Code";
                ViewBag.Name = "中文品名";
                ViewBag.EngName = "英文品名";
                ViewBag.Code = "HS 编码";
            }
            ViewBag.Cesss = "退税率";

            HSService service = new HSService();
            DTOHSContract list = service.selectByID(id);
            ViewBag.DataFlag = DataFlag;
            ViewBag.ID = list.ID;
            ViewBag.Hscode = list.HSCode;
            ViewBag.Cess = list.Cess;
            ViewBag.DutyPercentList = list.DutyPercentList;
            ViewBag.IsCheck = list.IsCheck;
            ViewBag.ProjectName = list.ProjectName;
            ViewBag.CodeName = list.CodeName;
            ViewBag.CodeEngName = list.CodeEngName;
            List<DTODictionary> listt = _HSServices.selectAllDictionary();
            ViewData["Data"] = listt;
            return View();
        }

        [HttpPost]
        public ActionResult Edit(string hscode, string cess, string DutyPercentList, int id, string CodeList, string name, string engName, string dataflag)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SystemManagement_HSCode);
            if ((id == 0 && (pageElementPrivileges & (int)HsElementsPrivileges.HsAdd) == 0) ||
                (id != 0 && (pageElementPrivileges & (int)HsElementsPrivileges.HsEdit) == 0))
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            DTOHSContract dto = new DTOHSContract();
            dto.ID = id;
            dto.HSCode = hscode;
            if (!string.IsNullOrEmpty(cess))
            {
                dto.Cess = decimal.Parse(cess);
            }
            dto.DutyPercentList = DutyPercentList;
            dto.CodeName = name;
            dto.CodeEngName = engName;
            dto.DataFlag = int.Parse(dataflag);
            DBOperationStatus result = _HSServices.UpdateResult(dto, CurrentUserServices.Me.UserID, CodeList);
            ViewBag.ShowMessage = result.ToString();
            return new CustomJsonResult((short)result);
        }

        //添加页面（根据传进来的直，改变页面元素的显示）
        public ActionResult AddCode(int id)
        {
            if (id == 1)
            {
                //报关的（原来做过的） 1是报关的HSCODE类型，进口Hscode类型是2

                ViewBag.title = "新建报关编码";
                ViewBag.Name = "报关品名";
                ViewBag.EngName = "报关英文品名";
                ViewBag.Code = "海关编码";
                ViewBag.Cess = "退税率";
                ViewBag.DataFlag = 1;
            }
            else
            {
                //进口关税
                ViewBag.title = "新建HS Code";
                ViewBag.Name = "中文品名";
                ViewBag.EngName = "英文品名";
                ViewBag.Code = "HS 编码";
                ViewBag.DataFlag = 2;
            }
            ViewBag.Cess = "退税率";
            ViewBag.DutyPercentList = "Duty";

            List<DTODictionary> listt = _HSServices.selectAllDictionary();
            ViewData["Data"] = listt;
            return View();
        }

        [HttpPost]
        public ActionResult AddCode(string hscode, string cess, string DutyPercentList, string tagid, string name, string engName, string dataflag)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SystemManagement_HSCode);
            if ((pageElementPrivileges & (int)HsElementsPrivileges.HsAdd) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            DTOHSContract dto = new DTOHSContract();
            dto.HSCode = hscode;
            if (!string.IsNullOrEmpty(cess))
            {
                dto.Cess = decimal.Parse(cess);
            }
            dto.DutyPercentList = DutyPercentList;
            dto.CodeName = name;
            dto.CodeEngName = engName;
            dto.DataFlag = int.Parse(dataflag);
            DBOperationStatus result = _HSServices.AddCode(dto, CurrentUserServices.Me.UserID, tagid);
            //ViewBag.ShowMessage = result.ToString();
            //return View();

            return new CustomJsonResult((short)result);
        }
    }
}