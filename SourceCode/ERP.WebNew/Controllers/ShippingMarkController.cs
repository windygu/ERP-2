using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.Customer;
using ERP.BLL.ERP.Dictionary;
using ERP.BLL.ERP.Purchase;
using ERP.BLL.ERP.ShippingMark;
using ERP.BLL.Workflow;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.Purchase;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.WebNew.Attributes;
using ERP.WebNew.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ERP.WebNew.Controllers
{
    [UserTrackerLog]
    [ERP.WebNew.Attributes.AuthorizedUser(PageID = (int)ERPPage.PurchaseManagement_PackManagement)]
    public class ShippingMarkController : Controller
    {
        private ShippingMarkService _shippingMarkService = new ShippingMarkService();
        private OrderCustomerServices _orderCustomerServices = new OrderCustomerServices();
        private DictionaryServices _dictionaryServices = new DictionaryServices();
        private UserServices _userService = new UserServices();

        #region HelperMethod

        /// <summary>
        /// 绑定ViewBag
        /// </summary>
        private void BindViewBag()
        {
            ViewBag.ShippingMarkInfos = GetSelectionShippingMark();

            ViewBag.list_CabinetType = CommonCode.GetSelectDataList_Enum(typeof(PurchaseProduct_CarbinetTypeEnum));

        }

        public static SelectList GetSelectionShippingMark()
        {
            List<SelectListItem> list = new List<SelectListItem>() { };
            list.Add(new SelectListItem());
            Dictionary<string, string> di = EnumHelper.GetCustomKeyEnums(typeof(ShippingMarkEnum));
            foreach (var item in di)
            {
                list.Add(new SelectListItem() { Text = item.Key, Value = item.Value });
            }
            SelectList generateList = new SelectList(list, "Value", "Text");

            return generateList;
        }

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

                case "ShippingMark_StatusName":
                    dbColumnName = "ShippingMark_StatusID";
                    break;

                case "ShippingMark_ModifyDateFormatter":
                    dbColumnName = "ShippingMark_ModifyDate";
                    break;

                default:
                    dbColumnName = name;
                    break;
            }
            return dbColumnName;
        }

        #endregion HelperMethod

        #region UserMethod

        public ActionResult Index(VMPurchaseSearch vm_search)
        {
            vm_search.PageType = PageTypeEnum.PendingCheckList;
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ShippingMark_Index);

            return View(vm_search);
        }

        public ActionResult PassedCheck(VMPurchaseSearch vm_search)
        {
            vm_search.PageType = PageTypeEnum.PassedCheckList;
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ShippingMark_PassedCheck);

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

            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> { (int)ERPPage.PurchaseManagement_ShippingMark_Index,
                (int)ERPPage.PurchaseManagement_ShippingMark_PassedCheck, });

            #endregion 筛选条件

            List<DTOPurchaseContract> listModel = _shippingMarkService.GetAll(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows,
             vm_search);
            if (vm_search.PageType == PageTypeEnum.PendingCheckList && listModel != null)
            {
                listModel.Where(d => d.ShippingMark_StatusID == (int)ShippingMarkStatusEnum.PendingCheck).ToList().ForEach(p => p.NextApproverInfos = ApprovalService.GetNextApproverInfos(WorkflowTypes.ApprovalShippingMark, (int)p.ShippingMark_CreateUser, p.ApproverIndexShippingMark, p.CustomerID));
            }

            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }

        public ActionResult Edit(int id)
        {
            BindViewBag();

            VMPurchase vm = _shippingMarkService.GetDetailByID(CurrentUserServices.Me, id);

            ViewBag.AcceptInformations = new CustomerController().GetAcceptInformations(vm.CustomerID);

            if (DTRequest.GetQueryString("Type") == "Detail")
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> {
                        (int)ERPPage.PurchaseManagement_ShippingMark_Index,
                        (int)ERPPage.PurchaseManagement_ShippingMark_PassedCheck});
                if ((pageElementPrivileges & (int)ShippingMarkElementPrivileges.View) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                ViewBag.Title = "查看唛头资料";
                vm.PageType = PageTypeEnum.Details;
            }
            else
            {
                vm.PageType = PageTypeEnum.Edit;
                switch (vm.ShippingMark_StatusID)
                {
                    case (int)ShippingMarkStatusEnum.PendingMaintenance:

                        #region 添加权限

                        int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ShippingMark_Index);
                        if ((pageElementPrivileges & (int)ShippingMarkElementPrivileges.Maintenance) == 0)
                        {
                            return RedirectToAction("NoAuthPop", "Account");
                        }

                        #endregion 添加权限

                        ViewBag.Title = "维护唛头资料";
                        break;

                    case (int)ShippingMarkStatusEnum.OutLine:
                    case (int)ShippingMarkStatusEnum.NotPassCheck:

                        #region 添加权限

                        pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ShippingMark_Index);
                        if ((pageElementPrivileges & (int)ShippingMarkElementPrivileges.Edit) == 0)
                        {
                            return RedirectToAction("NoAuthPop", "Account");
                        }

                        #endregion 添加权限

                        ViewBag.Title = "编辑唛头资料";
                        break;

                    case (int)ShippingMarkStatusEnum.PendingCheck:

                        #region 添加权限

                        pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ShippingMark_Index);
                        if ((pageElementPrivileges & (int)ShippingMarkElementPrivileges.Approval) == 0)
                        {
                            return RedirectToAction("NoAuthPop", "Account");
                        }

                        #endregion 添加权限

                        #region 判断是否有审批流的权限

                        bool isHasApprovalPermission = ApprovalService.HasApprovalPermission(WorkflowTypes.ApprovalShippingMark, CurrentUserServices.Me, (int)vm.ShippingMark_CreateUser, vm.ApproverIndexShippingMark, vm.CustomerID);
                        if (!isHasApprovalPermission)
                        {
                            return RedirectToAction("NoAuthPop", "Account");
                        }

                        #endregion 判断是否有审批流的权限

                        ViewBag.Title = "审核唛头资料";
                        ViewBag.ShowSuggest = true;
                        vm.PageType = PageTypeEnum.Approval;
                        break;
                }
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult Edit([System.Web.Http.FromBody]VMPurchase vm)
        {
            switch ((ShippingMarkStatusEnum)vm.ShippingMark_StatusID)
            {
                case ShippingMarkStatusEnum.PendingMaintenance:

                    #region 添加权限

                    int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ShippingMark_Index);
                    if ((pageElementPrivileges & (int)ShippingMarkElementPrivileges.Maintenance) == 0)
                    {
                        return RedirectToAction("NoAuthPop", "Account");
                    }

                    #endregion 添加权限

                    break;

                case ShippingMarkStatusEnum.OutLine:
                case ShippingMarkStatusEnum.PendingCheck:

                    #region 添加权限

                    pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ShippingMark_Index);
                    if ((pageElementPrivileges & (int)ShippingMarkElementPrivileges.Edit) == 0)
                    {
                        return RedirectToAction("NoAuthPop", "Account");
                    }

                    #endregion 添加权限

                    break;

                case ShippingMarkStatusEnum.NotPassCheck:
                case ShippingMarkStatusEnum.PassedCheck:

                    #region 添加权限

                    pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ShippingMark_Index);
                    if ((pageElementPrivileges & (int)ShippingMarkElementPrivileges.Approval) == 0)
                    {
                        return RedirectToAction("NoAuthPop", "Account");
                    }

                    #endregion 添加权限

                    break;

                default:
                    break;
            }
            VMAjaxProcessResult result = _shippingMarkService.Save(CurrentUserServices.Me, vm);
            ViewBag.status = DBOperationStatus.Success;

            BindViewBag();

            return Json(result);
        }

        public ActionResult ChangeStatus(int id)
        {
            #region 添加权限

            //int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ShippingMark_PassedCheck);
            //if ((pageElementPrivileges & (int)ShippingMarkElementPrivileges.NotificationPrinted) == 0)
            //{
            //    return RedirectToAction("NoAuthPop", "Account");
            //}

            #endregion 添加权限

            DBOperationStatus result = _shippingMarkService.Save_ChangeStatus(CurrentUserServices.Me, id);

            return new CustomJsonResult(result);
        }

        public ActionResult UpLoad(int id)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ShippingMark_PassedCheck);
            if ((pageElementPrivileges & (int)ShippingMarkElementPrivileges.UpLoad) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            VMUpLoad vm = _shippingMarkService.GetUploadDetial(id);
            return View(vm);
        }

        [HttpPost]
        public ActionResult UpLoad(int id, VMUpLoad vm)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ShippingMark_PassedCheck);
            if ((pageElementPrivileges & (int)ShippingMarkElementPrivileges.UpLoad) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            DBOperationStatus result = _shippingMarkService.SaveUpLoad(CurrentUserServices.Me, vm);
            return Json(result);
        }

        [HttpPost]
        public ActionResult CreateShippingMark([System.Web.Http.FromBody]VMPurchase vm)
        {
            VMAjaxProcessResult result = _shippingMarkService.CreateShippingMark(CurrentUserServices.Me, vm);
            return Json(result);
        }

        #endregion UserMethod
    }
}