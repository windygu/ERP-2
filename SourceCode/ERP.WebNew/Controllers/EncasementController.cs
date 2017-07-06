using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.Encasement;
using ERP.BLL.Workflow;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.Encasement;
using ERP.Tools;
using ERP.WebNew.Attributes;
using ERP.WebNew.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.WebNew.Controllers
{
    [UserTrackerLog]
    [ERP.WebNew.Attributes.AuthorizedUser(PageID = (int)ERPPage.DeliveryManagement_Details)]
    public class EncasementController : Controller
    {
        private UserServices _userService = new UserServices();
        private EncasementServices _EncasementService = new EncasementServices();
        
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
                case "FactoryAbbreviation":
                    dbColumnName = "Factory.Abbreviation";
                    break;

                case "CustomerCode":
                    dbColumnName = "Orders_Customers.CustomerCode";
                    break;
                    
                case "ContractAmountSymbol":
                    dbColumnName = "AllAmount";
                    break;

                case "PortName":
                    dbColumnName = "PortID";
                    break;

                case "EncasementUpdateDateFormatter":
                    dbColumnName = "EncasementUpdateDate";
                    break;
                default:
                    dbColumnName = name;
                    break;
            }
            return dbColumnName;
        }

        public ActionResult Index(VMFilterEncasement vm)
        {
            vm.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DeliveryManagement_Details_Index);
            vm.PageType = PageTypeEnum.PendingCheckList;

            ViewData["EncasementStatus"] = GetEncasementsStatus();

            return View(vm);
        }

        public ActionResult PassedApprovalList(VMFilterEncasement vm)
        {
            vm.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DeliveryManagement_Details_PassedApprovalList);
            vm.PageType = PageTypeEnum.PassedCheckList;

            return View(vm);
        }

        /// <summary>
        /// 查询并返回出运明细列表数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAll(VMFilterEncasement vm_search)
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

            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> { (int)ERPPage.DeliveryManagement_Details_Index,
                (int)ERPPage.DeliveryManagement_Details_PassedApprovalList, });

            #endregion 筛选条件

            List<DTOEncasement> listModel = _EncasementService.GetAll(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows,
             vm_search);

            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }

        public ActionResult Edit(int ContractID, int EncasementID, int PageType)
        {
            List<int> pageList = new List<int>();
            pageList.Add((int)ERPPage.DeliveryManagement_Details_Index);

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, pageList);
            string sPageTitle = string.Empty;//页面标题
            int iPrivilegeswResult = 0;

            switch (PageType)
            {
                case 1:
                    pageList = new List<int>();
                    pageList.Add((int)ERPPage.DeliveryManagement_Details_Index);
                    pageList.Add((int)ERPPage.DeliveryManagement_Details_PassedApprovalList);

                    pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, pageList);
                    iPrivilegeswResult = pageElementPrivileges & (int)EncasementElementPrivileges.EncasementWatch;
                    sPageTitle = "查看出运明细";
                    break;

                case 2:
                    iPrivilegeswResult = pageElementPrivileges & (int)EncasementElementPrivileges.EncasementEdit;
                    sPageTitle = "编辑出运明细";
                    break;

                case 3:
                    iPrivilegeswResult = pageElementPrivileges & (int)EncasementElementPrivileges.EncasementCheck;
                    sPageTitle = "审核出运明细";
                    if (iPrivilegeswResult > 0)
                    {
                        DTOEncasement vmEncasement = new DTOEncasement();
                        vmEncasement = _EncasementService.GetEncasementInfo(CurrentUserServices.Me, vmEncasement, ContractID);

                        bool isHasApprovalPermission = ApprovalService.HasApprovalPermission(WorkflowTypes.ApprovalShipping, CurrentUserServices.Me, vmEncasement.CreateUserID, vmEncasement.ApproverIndex);

                        if (!isHasApprovalPermission)
                        {
                            return RedirectToAction("NoAuthPop", "Account");
                        }
                    }

                    break;

                default:
                    sPageTitle = "未定义状态";

                    break;
            }

            if (iPrivilegeswResult == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            DTOEncasement vm = new DTOEncasement();
            vm.ContractID = ContractID;
            vm.EncasementID = EncasementID;
            vm.PageTitle = sPageTitle;
            vm.PageTypeID = PageType;
            vm = _EncasementService.GetDetailByID(CurrentUserServices.Me, vm);

            return View(vm);
        }
                
        [HttpPost]
        public ActionResult Save(DTOEncasement vmEncasement)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            if (vmEncasement == null)
            {
                result.IsSuccess = false;
            }
            else
            {
                if (vmEncasement.EncasementProducts == null)
                {
                    result.IsSuccess = false;
                }
                else
                {
                    result = _EncasementService.Save(CurrentUserServices.Me,  vmEncasement);
                }
            }

            return Json(result);
        }

        public JsonResult ClearData(int id)
        {
            var result = _EncasementService.ClearData(CurrentUserServices.Me, id);
            return Json(result);
        }

        /// <summary>
        /// 生成包装资料数据状态下拉框数据
        /// </summary>
        /// <returns></returns>
        private static SelectList GetEncasementsStatus()
        {
            List<SelectListItem> list = new List<SelectListItem>() { };
            list.Add(new SelectListItem() { Text = "", Value = "" });

            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            dictionary.Add(1, "待维护");
            dictionary.Add(2, "草稿");
            dictionary.Add(3, "待审核");
            dictionary.Add(4, "审核未通过");

            foreach (var item in dictionary)
            {
                list.Add(new SelectListItem() { Text = item.Value, Value = item.Key.ToString() });
            }
            SelectList generateList = new SelectList(list, "Value", "Text");

            return generateList;
        }
    }
}