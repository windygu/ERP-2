using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.Dictionary;
using ERP.BLL.ERP.ThreeTimesQC;
using ERP.BLL.Workflow;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.ThreeTimesQC;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.WebNew.Attributes;
using ERP.WebNew.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace ERP.WebNew.Controllers
{
    [UserTrackerLog]
    [ERP.WebNew.Attributes.AuthorizedUser(PageID = (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement)]
    public class ThreeTimesQCController : Controller
    {
        private UserServices _userService = new UserServices();
        private DictionaryServices _dictionaryServices = new DictionaryServices();
        private ThreeTimesQCService _threeTimesQCService = new ThreeTimesQCService();

        /// <summary>
        /// 替换排序的字段
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string ReplaceSortNames(string name)
        {
            string dbColumnName = string.Empty;

            switch (name)
            {
                case "PurchaseNumber":
                    dbColumnName = "Purchase_Contract.PurchaseNumber";
                    break;

                case "FactoryAbbreviation":
                    dbColumnName = "Purchase_Contract.Factory.Abbreviation";
                    break;

                case "PurchaseDate":
                    dbColumnName = "Purchase_Contract.PurchaseDate";
                    break;

                case "CustomerCode":
                    dbColumnName = "Purchase_Contract.Orders_Customers.CustomerCode";
                    break;

                case "DateStart":
                    dbColumnName = "Purchase_Contract.DateStart";
                    break;

                case "DateEnd":
                    dbColumnName = "Purchase_Contract.DateEnd";
                    break;

                default:
                    dbColumnName = name;
                    break;
            }
            return dbColumnName;
        }

        #region UserMethod

        public ActionResult Index(VMPurchaseSearch vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Index);
            vm_search.PageType = PageTypeEnum.PendingCheckList;

            return View(vm_search);
        }

        public ActionResult Two(VMPurchaseSearch vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Two);
            vm_search.PageType = PageTypeEnum.PendingCheckList_Two;

            return View(vm_search);
        }

        public ActionResult Three(VMPurchaseSearch vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Three);
            vm_search.PageType = PageTypeEnum.PendingCheckList_Three;

            return View(vm_search);
        }

        public ActionResult PassedApproval(VMPurchaseSearch vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_PassedApproval);
            vm_search.PageType = PageTypeEnum.PassedCheckList;

            return View(vm_search);
        }

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAll(VMPurchaseSearch vm_search)
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
                    sortColumnsNames.Add(ReplaceSortNames(n));
                }
            }

            List<string> sortColumnOrders = new List<string>();
            if (!string.IsNullOrEmpty(DTRequest.GetQueryString(EasyuiPagenationConsts.SORT_COLUMN_ORDERS)))
            {
                sortColumnOrders = DTRequest.GetQueryString(EasyuiPagenationConsts.SORT_COLUMN_ORDERS).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            #endregion 排序

            #region 筛选条件

            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> {
                (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Index,
                (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Two,
                (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Three,
                (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_PassedApproval, });

            #endregion 筛选条件


            List<DTOPurchaseContract> listModel = _threeTimesQCService.GetAll(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows,
             vm_search);
            if (vm_search.PageType != PageTypeEnum.PassedCheckList && listModel != null)
            {
                listModel.Where(d => d.PurchaseStatusID == (int)ThreeTimesQCStatusEnum.PendingCheck).ToList().ForEach(p => p.NextApproverInfos = ApprovalService.GetNextApproverInfos(WorkflowTypes.ApprovalThirdPeriodQC, p.ST_CREATEUSER, p.ApproverIndex, p.CustomerID));
            }


            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }

        public ActionResult Edit(int id)
        {
            VMThreeTimesQC vm = _threeTimesQCService.GetDetailByID(CurrentUserServices.Me, id);

            string Type = DTRequest.GetQueryString("Type");
            if (Type == "Detail")
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> {
                    (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Index,
                    (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Two,
                    (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Three,
                    (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_PassedApproval });
                if ((pageElementPrivileges & (int)ThreeTimesQCElementPricileges.View) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                vm.PageType = PageTypeEnum.Details;
                ViewBag.Title = "查看";
            }
            else if (Type == "Check")
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> {
                    (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Index,
                    (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Two,
                    (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Three });
                if ((pageElementPrivileges & (int)ThreeTimesQCElementPricileges.Approval) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                vm.PageType = PageTypeEnum.Approval;
                ViewBag.Title = "审核";
            }
            else if (Type == "ReplySuggest")
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> {
                    (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Index,
                    (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Two,
                    (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Three });
                if ((pageElementPrivileges & (int)ThreeTimesQCElementPricileges.ReplySuggest) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                vm.PageType = PageTypeEnum.ReplySuggest;
                ViewBag.Title = "回复意见——";
            }
            else
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> {
                    (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Index,
                    (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Two,
                    (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Three });
                if ((pageElementPrivileges & (int)ThreeTimesQCElementPricileges.Edit) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                vm.PageType = PageTypeEnum.Edit;
                ViewBag.Title = "上传";

            }
            switch (vm.ApprovalStatus)
            {
                case 0:
                    ViewBag.Title += "前期QC";
                    break;
                case 1:
                    ViewBag.Title += "中期QC";
                    break;
                case 2:
                    ViewBag.Title += "尾期QC";
                    break;
                default:
                    break;
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult Edit([System.Web.Http.FromBody]VMThreeTimesQC vm)
        {
            if (vm.PageType == PageTypeEnum.Edit)
            {

                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> {
                    (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Index,
                    (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Two,
                    (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Three });
                if ((pageElementPrivileges & (int)ThreeTimesQCElementPricileges.Edit) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限
            }
            else if (vm.PageType == PageTypeEnum.Approval)
            {

                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> {
                    (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Index,
                    (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Two,
                    (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Three });
                if ((pageElementPrivileges & (int)ThreeTimesQCElementPricileges.Approval) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限
            }

            if (vm.StatusID == (int)ThreeTimesQCStatusEnum.ViewPDF)
            {
                string path = _threeTimesQCService.CreatePDF(vm);
                return Json(path);

            }
            var result = _threeTimesQCService.Save(CurrentUserServices.Me, vm);

            return Json(result);
        }

        public ActionResult SendContract(int id)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> {
                    (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Index,
                    (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Two,
                    (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Three });
            if ((pageElementPrivileges & (int)ThreeTimesQCElementPricileges.SendContract) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            var vm = _threeTimesQCService.GetDetailByID(CurrentUserServices.Me, id);
            ViewBag.FromAddress = CurrentUserServices.Me.Email;
            ViewBag.ToAddress = _threeTimesQCService.GetToAddress(vm.PurchaseContract.CustomerID);
            ViewBag.UserName = CurrentUserServices.Me.UserName;

            return View(vm);
        }

        [HttpPost]
        public ActionResult SendContract(int id, VMSendEmail vm)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> {
                    (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Index,
                    (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Two,
                    (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Three });
            if ((pageElementPrivileges & (int)ThreeTimesQCElementPricileges.SendContract) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            var result = _threeTimesQCService.SendContract(CurrentUserServices.Me, id, vm);
            return Json(result);
        }

        [HttpPost]
        public ActionResult MakeExcel(int id)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> {
                    (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Index,
                    (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Two,
                    (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement_Three });
            if ((pageElementPrivileges & (int)ThreeTimesQCElementPricileges.SendContract) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            string result = _threeTimesQCService.MakeExcel(CurrentUserServices.Me, id);
            return Json(result);
        }
        #endregion UserMethod
    }
}