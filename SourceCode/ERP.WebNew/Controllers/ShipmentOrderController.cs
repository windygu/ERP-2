using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.Dictionary;
using ERP.BLL.ERP.Shipment;
using ERP.BLL.ERP.ShipmentOrder;
using ERP.BLL.Workflow;
using ERP.Models.Cabinet;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.Shipment;
using ERP.Models.ShipmentOrder;
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
    [ERP.WebNew.Attributes.AuthorizedUser(PageID = (int)ERPPage.DeliveryManagement_ShipmentOrder)]
    public class ShipmentOrderController : Controller
    {
        private UserServices _userService = new UserServices();
        private DictionaryServices _dictionaryServices = new DictionaryServices();
        private ShipmentAgencyServices _shipmentAgencyServices = new ShipmentAgencyServices();

        private CabinetService _cabinetService = new CabinetService();
        private ShipmentOrderService _shipmentOrderService = new ShipmentOrderService();

        #region HelperMethod

        /// <summary>
        /// 绑定ViewBag
        /// </summary>
        private void BindViewBag()
        {
            List<DTOCabinet> listModel = _cabinetService.GetAll();
            Dictionary<int, string> di = new Dictionary<int, string>();
            foreach (var item in listModel)
            {
                di.Add(item.ID, item.Name + " [" + item.Size + "(m³)]");
            }
            ViewBag.CabinetList = CommonCode.GetSelectDataList(di);

            var dictionaries = _dictionaryServices.GetDictionaryInfos(CurrentUserServices.Me.UserID, null);
            ViewBag.Ports = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.OutPort).ToList();
            ViewBag.GoalPort = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.GoalPort).ToList();

            ViewBag.list_CabinetType = CommonCode.GetSelectDataList_Enum(typeof(CabinetTypeEnum));
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
                    dbColumnName = "Order.OrderID";
                    break;

                case "POID":
                    dbColumnName = "Order.POID";
                    break;

                case "EHIPO":
                    dbColumnName = "Order.EHIPO";
                    break;

                case "DestinationPortName":
                    dbColumnName = "Order.DestinationPortID";
                    break;

                case "CustomerCode":
                    dbColumnName = "Order.Orders_Customers.CustomerCode";
                    break;

                case "OrderDateStartFormatter":
                    dbColumnName = "Order.OrderDateStart";
                    break;

                case "OrderDateEndFormatter":
                    dbColumnName = "Order.OrderDateEnd";
                    break;

                case "StatusName":
                    dbColumnName = "StatusID";
                    break;

                case "DT_MODIFYDATEFormatter":
                    dbColumnName = "DT_MODIFYDATE";
                    break;

                default:
                    dbColumnName = name;
                    break;
            }
            return dbColumnName;
        }

        #endregion HelperMethod

        #region UserMethod

        #region 列表页面

        public ActionResult Index(VMShipmentOrderSearch vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DeliveryManagement_ShipmentOrder_Index);
            vm_search.PageType = PageTypeEnum.PendingApproval;

            return View(vm_search);
        }

        public ActionResult PassedApprovalList(VMShipmentOrderSearch vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DeliveryManagement_ShipmentOrder_PassedApprovalList);
            vm_search.PageType = PageTypeEnum.PassedApproval;

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
                (int)ERPPage.DeliveryManagement_ShipmentOrder_Index,
                (int)ERPPage.DeliveryManagement_ShipmentOrder_PassedApprovalList });

            #endregion 筛选条件

            List<VMShipmentOrder> listModel = _shipmentOrderService.GetAll(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows, vm_search);

            if (vm_search.PageType == PageTypeEnum.PendingApproval && listModel != null)
            {
                listModel.Where(d => d.StatusID == (int)ShipmentOrderStatusEnum.PendingCheck).ToList().ForEach(p => p.NextApproverInfos = ApprovalService.GetNextApproverInfos(WorkflowTypes.ApprovalShipmentOrder, p.ST_CREATEUSER, p.ApproverIndex, p.CustomerID));
            }

            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }

        #endregion 列表页面

        public ActionResult Edit(string id)
        {
            BindViewBag();

            if (DTRequest.GetQueryString("Type") == "Add")
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DeliveryManagement_ShipmentOrder_Index);
                if ((pageElementPrivileges & (int)ShipmentOrderElementPrivileges.Add) == 0 && (pageElementPrivileges & (int)ShipmentOrderElementPrivileges.Merge) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                List<int> idList = CommonCode.IdListToList(id);
                var list_vm = _shipmentOrderService.GetDetailByID(CurrentUserServices.Me, idList);

                ViewBag.PageType = PageTypeEnum.Add;
                ViewBag.Title = "新建订舱信息";

                return View(list_vm);
            }
            else
            {
                var tempID = Utils.StrToInt(id, 0);
                var list_vm = _shipmentOrderService.GetDetailByID_ForEditPage(CurrentUserServices.Me, tempID,0);
                int StatusID = list_vm.FirstOrDefault().StatusID;
                int ST_CREATEUSER = list_vm.FirstOrDefault().ST_CREATEUSER;
                int? ApproverIndex = list_vm.FirstOrDefault().ApproverIndex;
                int? CustomerID = list_vm.FirstOrDefault().CustomerID;

                if (DTRequest.GetQueryString("Type") == "Detail")
                {
                    #region 添加权限

                    int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int>{
                        (int)ERPPage.DeliveryManagement_ShipmentOrder_Index,
                        (int)ERPPage.DeliveryManagement_ShipmentOrder_PassedApprovalList,
                    });
                    if ((pageElementPrivileges & (int)ShipmentOrderElementPrivileges.View) == 0)
                    {
                        return RedirectToAction("NoAuthPop", "Account");
                    }

                    #endregion 添加权限

                    ViewBag.PageType = PageTypeEnum.Details;
                    ViewBag.Title = "查看订舱信息";
                }
                else if (StatusID == (short)ShipmentOrderStatusEnum.OutLine || StatusID == (short)ShipmentOrderStatusEnum.NotPassCheck)
                {
                    #region 添加权限

                    int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DeliveryManagement_ShipmentOrder_Index);
                    if ((pageElementPrivileges & (int)ShipmentOrderElementPrivileges.Edit) == 0)
                    {
                        return RedirectToAction("NoAuthPop", "Account");
                    }

                    #endregion 添加权限

                    ViewBag.PageType = PageTypeEnum.Edit;
                    ViewBag.Title = "编辑订舱信息";
                }
                else if (StatusID == (short)ShipmentOrderStatusEnum.PendingCheck)
                {
                    #region 添加权限

                    int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DeliveryManagement_ShipmentOrder_Index);
                    if ((pageElementPrivileges & (int)ShipmentOrderElementPrivileges.Approval) == 0)
                    {
                        return RedirectToAction("NoAuthPop", "Account");
                    }

                    #endregion 添加权限

                    #region 判断是否有审批流的权限

                    bool isHasApprovalPermission = ApprovalService.HasApprovalPermission(WorkflowTypes.ApprovalShipmentOrder, CurrentUserServices.Me, ST_CREATEUSER, ApproverIndex, CustomerID);
                    if (!isHasApprovalPermission)
                    {
                        return RedirectToAction("NoAuthPop", "Account");
                    }

                    #endregion 判断是否有审批流的权限

                    ViewBag.PageType = PageTypeEnum.Check;
                    ViewBag.Title = "审核订舱信息";
                }
                return View(list_vm);
            }
        }

        [HttpPost]
        public ActionResult Edit(VMShipmentOrder vm)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();

            if (vm.IsAddPage)
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DeliveryManagement_ShipmentOrder_Index);
                if ((pageElementPrivileges & (int)ShipmentOrderElementPrivileges.Add) == 0 && (pageElementPrivileges & (int)ShipmentOrderElementPrivileges.Merge) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限
            }
            else
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DeliveryManagement_ShipmentOrder_Index);
                if ((pageElementPrivileges & (int)ShipmentOrderElementPrivileges.Edit) == 0 && (pageElementPrivileges & (int)ShipmentOrderElementPrivileges.Approval) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限
            }
            result = _shipmentOrderService.Save(CurrentUserServices.Me, vm);

            return Json(result);
        }

        //保存订舱的柜号、箱号、封号
        [HttpPost]
        public ActionResult Edit2(VMShipmentOrder vm)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            
            result = _shipmentOrderService.Save2(CurrentUserServices.Me, vm);

            return Json(result);
        }

        [HttpGet]
        public JsonResult GetOrderProducts(string OrderIDList, int ID)
        {
            List<int> tempList = CommonCode.IdListToList(OrderIDList);

            var list = _shipmentOrderService.GetOrderProducts(tempList, ID);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取船运公司列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetSelectAllShipmentAgency()
        {
            List<VMShipmentAgency> listModel = _shipmentAgencyServices.SelectAll();
            if (listModel.Count() > 0)
            {
                var dataForEasyUI = listModel.Select(p => new
                {
                    ShippingAgencyID = p.ShippingAgencyID,
                    ShippingAgencyName = p.ShippingAgencyName,
                    Currency = p.CurrentShipmentAgentFees == null ? null : p.CurrentShipmentAgentFees.Currency,
                    FeeDocument = p.CurrentShipmentAgentFees == null ? 0 : p.CurrentShipmentAgentFees.FeeDocument,
                    FeeDockOperation = p.CurrentShipmentAgentFees == null ? 0 : p.CurrentShipmentAgentFees.FeeDockOperation,
                    FeeYangShanPicking = p.CurrentShipmentAgentFees == null ? 0 : p.CurrentShipmentAgentFees.FeeYangShanPicking,
                    FeeFacilityManagement = p.CurrentShipmentAgentFees == null ? 0 : p.CurrentShipmentAgentFees.FeeFacilityManagement,
                    FeePortSecurity = p.CurrentShipmentAgentFees == null ? 0 : p.CurrentShipmentAgentFees.FeePortSecurity,
                    FeeImporterSecurityClassify = p.CurrentShipmentAgentFees == null ? 0 : p.CurrentShipmentAgentFees.FeeImporterSecurityClassify,
                    FeeWarehousing = p.CurrentShipmentAgentFees == null ? 0 : p.CurrentShipmentAgentFees.FeeWarehousing,
                    FeePicking = p.CurrentShipmentAgentFees == null ? 0 : p.CurrentShipmentAgentFees.FeePicking,
                    FeeCustomDeclaration = p.CurrentShipmentAgentFees == null ? 0 : p.CurrentShipmentAgentFees.FeeCustomDeclaration,
                });
                return Json(dataForEasyUI, JsonRequestBehavior.AllowGet);
            }
            return Json(listModel, JsonRequestBehavior.AllowGet);
        }

        #endregion UserMethod
    }
}