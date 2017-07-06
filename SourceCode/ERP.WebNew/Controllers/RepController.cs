using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.Rep;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.Rep;
using ERP.Tools;
using ERP.WebNew.Attributes;
using ERP.WebNew.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ERP.WebNew.Controllers
{
    [UserTrackerLog]
    [ERP.WebNew.Attributes.AuthorizedUser(PageID = (int)ERPPage.RepManagement_List)]
    public class RepController : Controller
    {
        private RepServices _repService = new RepServices();

        private UserServices _userService = new UserServices();

        /// <summary>
        /// 验证请求
        /// </summary>
        /// <returns></returns>
        private VMAjaxProcessResult ValidRequest(RepElementPrivileges repElementPrivileges)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();

            if (CurrentUserServices.Me == null)
            {
                result.IsSuccess = false;
                result.Msg = "未登录！";
                return result;
            }
            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> { (int)ERPPage.RepManagement_List });
            if ((pageElementPrivileges & (int)repElementPrivileges) == 0)
            {
                result.IsSuccess = false;
                result.Msg = "没权限！";
                return result;
            }

            result.IsSuccess = true;
            return result;
        }

        #region UserMethod

        public ActionResult Index(VMRepSearch vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.RepManagement_List);
            return View(vm_search);
        }

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAll(VMRepSearch vm_search)
        {
            int currentPage = DTRequest.GetQueryInt(EasyuiPagenationConsts.CURRENT_PAGE, 1);
            int pageSize = DTRequest.GetQueryInt(EasyuiPagenationConsts.PAGE_SIZE, Keys.DEFAULT_PAGE_SIZE);
            int totalRows = 0;

            #region 排序

            List<string> sortColumnsNames = new List<string>();
            if (!string.IsNullOrEmpty(DTRequest.GetQueryString(EasyuiPagenationConsts.SORT_COLUMN_NAMES)))
            {
                var sortColumnsNamesTemp = HttpContext.Request[EasyuiPagenationConsts.SORT_COLUMN_NAMES].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                // 需要替换转换过的属性
                foreach (var n in sortColumnsNamesTemp)
                {
                    sortColumnsNames.Add(n);
                }
            }
            List<string> sortColumnOrders = new List<string>();
            if (!string.IsNullOrEmpty(DTRequest.GetQueryString(EasyuiPagenationConsts.SORT_COLUMN_ORDERS)))
            {
                sortColumnOrders = DTRequest.GetQueryString(EasyuiPagenationConsts.SORT_COLUMN_ORDERS).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            #endregion 排序

            #region 筛选条件

            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> { (int)ERPPage.RepManagement_List });

            #endregion 筛选条件

            List<VMRep> listModel = _repService.GetAll(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows,
             vm_search);

            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public ActionResult BatchDelete([System.Web.Http.FromBody]string idList)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.RepManagement_List);
            if ((pageElementPrivileges & (int)RepElementPrivileges.BatchDelete) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            List<int> deleteList = null;
            if (!string.IsNullOrEmpty(idList))
            {
                deleteList = idList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
            }
            DBOperationStatus result = _repService.Delete(CurrentUserServices.Me, deleteList);
            return new CustomJsonResult((short)result);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public ActionResult Delete([System.Web.Http.FromBody]int id)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.RepManagement_List);
            if ((pageElementPrivileges & (int)RepElementPrivileges.Delete) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            List<int> list = new List<int>();
            list.Add(id);

            DBOperationStatus result = _repService.Delete(CurrentUserServices.Me, list);
            return new CustomJsonResult((short)result);
        }

        public ActionResult Edit(int id)
        {
            VMRep vm = null;
            if (id == -1)
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.RepManagement_List);
                if ((pageElementPrivileges & (int)RepElementPrivileges.Create) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                ViewBag.Title = "新建Rep";

                vm = new VMRep();
                vm.PageType = PageTypeEnum.Add;
                vm.RepID = -1;
            }
            else
            {
                vm = _repService.GetDetailByID(CurrentUserServices.Me, id);

                if (DTRequest.GetQueryString("Type") == "Detail")
                {
                    #region 添加权限

                    int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.RepManagement_List);
                    if ((pageElementPrivileges & (int)RepElementPrivileges.View) == 0)
                    {
                        return RedirectToAction("NoAuthPop", "Account");
                    }

                    #endregion 添加权限

                    ViewBag.Title = "查看Rep";
                    vm.PageType = PageTypeEnum.Details;
                }
                else
                {
                    #region 添加权限

                    int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.RepManagement_List);
                    if ((pageElementPrivileges & (int)RepElementPrivileges.Edit) == 0)
                    {
                        return RedirectToAction("NoAuthPop", "Account");
                    }

                    #endregion 添加权限

                    ViewBag.Title = "编辑Rep";
                    vm.PageType = PageTypeEnum.Edit;
                }
            }

            return View(vm);
        }

        [HttpPost]
        public JsonResult Edit(VMRep vm)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            if (vm.RepID == -1)//添加
            {
                #region 添加权限

                VMAjaxProcessResult validResult = ValidRequest(RepElementPrivileges.Create);
                if (!validResult.IsSuccess)
                {
                    return Json(validResult, JsonRequestBehavior.AllowGet);
                }

                #endregion 添加权限

                result = _repService.Add(CurrentUserServices.Me, vm);
            }
            else
            {
                #region 添加权限

                VMAjaxProcessResult validResult = ValidRequest(RepElementPrivileges.Edit);
                if (!validResult.IsSuccess)
                {
                    return Json(validResult, JsonRequestBehavior.AllowGet);
                }

                #endregion 添加权限

                result = _repService.Save(CurrentUserServices.Me, vm);
            }

            return Json(result);
        }

        [HttpGet]
        public JsonResult GetRep(int id, bool openCached = false)
        {
            #region 添加权限

            VMAjaxProcessResult validResult = ValidRequest(RepElementPrivileges.View);
            if (!validResult.IsSuccess)
            {
                return Json(validResult, JsonRequestBehavior.AllowGet);
            }

            #endregion 添加权限

            var vm = _repService.GetDetailByID(CurrentUserServices.Me, id);

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        #endregion UserMethod
    }
}