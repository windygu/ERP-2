using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.Dictionary;
using ERP.BLL.ERP.Shipment;
using ERP.BLL.ERP.ShipmentNotification;
using ERP.BLL.ERP.ShipmentOrder;
using ERP.BLL.Workflow;
using ERP.Models.Cabinet;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.Shipment;
using ERP.Models.ShipmentOrder;
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
    [ERP.WebNew.Attributes.AuthorizedUser(PageID = (int)ERPPage.DeliveryManagement_ShipmentNotification)]
    public class ShipmentNotificationController : Controller
    {
        private UserServices _userService = new UserServices();
        private DictionaryServices _dictionaryServices = new DictionaryServices();
        private ShipmentAgencyServices _shipmentAgencyServices = new ShipmentAgencyServices();
        private CabinetService _cabinetService = new CabinetService();

        private ShipmentOrderService _shipmentOrderService = new ShipmentOrderService();
        private ShipmentNotificationService _shipmentNotificationService = new ShipmentNotificationService();

        #region HelperMethod

        /// <summary>
        /// 绑定ViewBag
        /// </summary>
        private void BindViewBag()
        {
            var dictionaries = _dictionaryServices.GetDictionaryInfos(CurrentUserServices.Me.UserID, null);
            ViewBag.Ports = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.OutPort).ToList();

            ViewBag.ShipmentAgency = GetSelectionShipmentAgency();
        }

        /// <summary>
        /// 获取船运公司列表
        /// </summary>
        /// <returns></returns>
        public SelectList GetSelectionShipmentAgency()
        {
            List<SelectListItem> list = new List<SelectListItem>() { };
            List<VMShipmentAgency> listModel = _shipmentAgencyServices.GetList();
            foreach (var item in listModel)
            {
                list.Add(new SelectListItem() { Text = item.ShippingAgencyName, Value = item.ShippingAgencyID.ToString() });
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
                case "OrderNumber":
                    dbColumnName = "Order.OrderNumber";
                    break;

                case "CustomerCode":
                    dbColumnName = "Order.Orders_Customers.CustomerCode";
                    break;

                case "CustomerDate":
                    dbColumnName = "Order.CustomerDate";
                    break;

                case "OrderAmount":
                    dbColumnName = "Order.OrderAmount";
                    break;

                case "NotificationStatusName":
                    dbColumnName = "NotificationStatusID";
                    break;

                default:
                    dbColumnName = name;
                    break;
            }
            return dbColumnName;
        }

        #endregion HelperMethod

        #region UserMethod

        public ActionResult Index(VMShipmentOrderSearch vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DeliveryManagement_ShipmentNotification_Index);
            vm_search.PageType = PageTypeEnum.PendingApproval;

            return View(vm_search);
        }

        public ActionResult PassedApprovalList(VMShipmentOrderSearch vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DeliveryManagement_ShipmentNotification_PassedApprovalList);
            vm_search.PageType = PageTypeEnum.PassedApproval;

            return View(vm_search);
        }

        /// <summary>
        /// 拉柜费用登记列表
        /// </summary>
        /// <param name="vm_search"></param>
        /// <returns></returns>
        public ActionResult RegisterFeesList(VMShipmentOrderSearch vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DeliveryManagement_ShipmentNotification_RegisterFees);
            vm_search.PageType = PageTypeEnum.RegisterFeesList;

            return View(vm_search);
        }

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetAll(VMShipmentOrderSearch vm_search)
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
                (int)ERPPage.DeliveryManagement_ShipmentNotification_Index,
                (int)ERPPage.DeliveryManagement_ShipmentNotification_PassedApprovalList,
                (int)ERPPage.DeliveryManagement_ShipmentNotification_RegisterFees });

            #endregion 筛选条件

            List<VMShipmentOrder> listModel = _shipmentNotificationService.GetAll(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows, vm_search);

            if (vm_search.PageType == PageTypeEnum.PendingApproval && listModel != null)
            {
                listModel.Where(d => d.NotificationStatusID == (int)ShipmentNotificationStatusEnum.PendingCheck).ToList().ForEach(p => p.NextApproverInfos = ApprovalService.GetNextApproverInfos(WorkflowTypes.ApprovalShipmentNotification, (int)p.ST_CREATEUSERNotification, p.ApproverIndexNotification, p.CustomerID));
            }

            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }

        public ActionResult Edit(int id)
        {
            BindViewBag();

            var list_vm = _shipmentOrderService.GetDetailByID_ForEditPage(CurrentUserServices.Me, id,0);

            int NotificationStatusID = list_vm.FirstOrDefault().NotificationStatusID;
            int ST_CREATEUSERNotification = list_vm.FirstOrDefault().ST_CREATEUSERNotification.HasValue ? (int)list_vm.FirstOrDefault().ST_CREATEUSERNotification : 0;
            int? ApproverIndexNotification = list_vm.FirstOrDefault().ApproverIndexNotification;
            int? CustomerID = list_vm.FirstOrDefault().CustomerID;

            string Type = DTRequest.GetQueryString("Type");

            if (Type == "Detail")
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int>{
                    (int)ERPPage.DeliveryManagement_ShipmentNotification_Index,
                    (int)ERPPage.DeliveryManagement_ShipmentNotification_PassedApprovalList});
                if ((pageElementPrivileges & (int)ShipmentNotificationElementPrivileges.View) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                ViewBag.PageType = PageTypeEnum.Details;
                ViewBag.Title = "查看出运通知";
            }
            else if (Type == "View_RegisterFees")
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DeliveryManagement_ShipmentNotification_RegisterFees);
                if ((pageElementPrivileges & (int)ShipmentNotificationElementPrivileges.View_RegisterFees) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                ViewBag.PageType = PageTypeEnum.RegisterFees;
                ViewBag.Title = "查看拉柜费用登记";
                ViewBag.Page = "View_RegisterFees";
            }
            else if (Type == "Maintenance_RegisterFees")
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DeliveryManagement_ShipmentNotification_RegisterFees);
                if ((pageElementPrivileges & (int)ShipmentNotificationElementPrivileges.Maintenance_RegisterFees) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                ViewBag.PageType = PageTypeEnum.RegisterFees;
                ViewBag.Title = "维护拉柜费用登记";
                ViewBag.Page = "Maintenance_RegisterFees";
            }
            else if (Type == "Edit_RegisterFees")
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DeliveryManagement_ShipmentNotification_RegisterFees);
                if ((pageElementPrivileges & (int)ShipmentNotificationElementPrivileges.Edit_RegisterFees) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                ViewBag.PageType = PageTypeEnum.RegisterFees;
                ViewBag.Title = "编辑拉柜费用登记";
                ViewBag.Page = "Edit_RegisterFees";
            }
            else
            {
                if (NotificationStatusID == (short)ShipmentNotificationStatusEnum.PendingMaintenance)
                {
                    #region 添加权限

                    int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DeliveryManagement_ShipmentNotification_Index);
                    if ((pageElementPrivileges & (int)ShipmentNotificationElementPrivileges.Maintenance) == 0)
                    {
                        return RedirectToAction("NoAuthPop", "Account");
                    }

                    #endregion 添加权限

                    ViewBag.PageType = PageTypeEnum.Edit;
                    ViewBag.Title = "维护出运通知";
                }
                else if (NotificationStatusID == (short)ShipmentNotificationStatusEnum.OutLine || NotificationStatusID == (short)ShipmentNotificationStatusEnum.NotPassCheck)
                {
                    #region 添加权限

                    int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DeliveryManagement_ShipmentNotification_Index);
                    if ((pageElementPrivileges & (int)ShipmentNotificationElementPrivileges.Edit) == 0)
                    {
                        return RedirectToAction("NoAuthPop", "Account");
                    }

                    #endregion 添加权限

                    ViewBag.PageType = PageTypeEnum.Edit;
                    ViewBag.Title = "编辑出运通知";
                }
                else if (NotificationStatusID == (short)ShipmentNotificationStatusEnum.PendingCheck)
                {
                    #region 添加权限

                    int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DeliveryManagement_ShipmentNotification_Index);
                    if ((pageElementPrivileges & (int)ShipmentNotificationElementPrivileges.Approval) == 0)
                    {
                        return RedirectToAction("NoAuthPop", "Account");
                    }

                    #endregion 添加权限

                    #region 判断是否有审批流的权限

                    bool isHasApprovalPermission = ApprovalService.HasApprovalPermission(WorkflowTypes.ApprovalShipmentNotification, CurrentUserServices.Me, ST_CREATEUSERNotification, ApproverIndexNotification, CustomerID);
                    if (!isHasApprovalPermission)
                    {
                        return RedirectToAction("NoAuthPop", "Account");
                    }

                    #endregion 判断是否有审批流的权限

                    ViewBag.PageType = PageTypeEnum.Check;
                    ViewBag.Title = "审核出运通知";
                }
            }

            return View(list_vm);
        }

        [HttpPost]
        public ActionResult Edit(VMShipmentOrder vm)
        {
            if (vm.NotificationStatusID > 0)
            {
                if (vm.NotificationStatusID == (int)ShipmentNotificationStatusEnum.NotPassCheck || vm.NotificationStatusID == (int)ShipmentNotificationStatusEnum.PassedCheck)
                {
                    #region 添加权限

                    int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DeliveryManagement_ShipmentNotification_Index);
                    if ((pageElementPrivileges & (int)ShipmentNotificationElementPrivileges.Approval) == 0)
                    {
                        return RedirectToAction("NoAuthPop", "Account");
                    }

                    #endregion 添加权限
                }
                else
                {
                    #region 添加权限

                    int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DeliveryManagement_ShipmentNotification_Index);
                    if ((pageElementPrivileges & (int)ShipmentNotificationElementPrivileges.Edit) == 0 && (pageElementPrivileges & (int)ShipmentNotificationElementPrivileges.Maintenance) == 0)
                    {
                        return RedirectToAction("NoAuthPop", "Account");
                    }

                    #endregion 添加权限
                }

                VMAjaxProcessResult result = _shipmentNotificationService.Save(CurrentUserServices.Me, vm);
                return Json(result);
            }
            else
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DeliveryManagement_ShipmentNotification_RegisterFees);
                if ((pageElementPrivileges & (int)ShipmentNotificationElementPrivileges.Edit_RegisterFees) == 0 && (pageElementPrivileges & (int)ShipmentNotificationElementPrivileges.Maintenance_RegisterFees) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                VMAjaxProcessResult result = _shipmentNotificationService.Save_RegisterFees(CurrentUserServices.Me, vm);

                return Json(result);
            }
        }

        #endregion UserMethod

        public ActionResult GetAllCabinets()
        {
            List<DTOCabinet> cabinets = _cabinetService.GetAll();
            return new CustomJsonResult(cabinets);
        }
    }
}