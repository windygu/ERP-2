using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.Dictionary;
using ERP.BLL.ERP.ShipmentOrder;
using ERP.BLL.InspectionCustoms;
using ERP.BLL.Workflow;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.InspectionCustoms;
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
    /// <summary>
    /// 报关
    /// </summary>
    [UserTrackerLog]
    [ERP.WebNew.Attributes.AuthorizedUser(PageID = (int)ERPPage.DocumentsManagement_InspectionCustoms)]
    public class InspectionCustomsController : Controller
    {
        private UserServices _userService = new UserServices();
        private InspectionCustomsService _inspectionCustomsService = new InspectionCustomsService();
        private ShipmentOrderService _shipmentOrderService = new ShipmentOrderService();

        private DictionaryServices _dictionaryServices = new DictionaryServices();

        #region HelperMethod

        /// <summary>
        /// 绑定ViewBag
        /// </summary>
        private void BindViewBag()
        {
            //获取结汇方式下拉框数据
            var ExchangeTypeList = _dictionaryServices.GetDictionaryInfos(CurrentUserServices.Me.UserID, DictionaryTableKind.ExchangeType);

            //生成结汇方式的键值对
            var di = new Dictionary<int, string>();
            foreach (var item1 in ExchangeTypeList)
            {
                di.Add((int)item1.Code, item1.Name);
            }
            ViewData["list_ExchangeTypeList"] = CommonCode.GetSelectDataList(di);

            Dictionary<string, string> di2 = new Dictionary<string, string>();
            di2.Add("", "");
            for (int i = 65; i <= 90; i++)
            {
                char letter = (char)i;
                di2.Add(letter.ToString(), letter.ToString());
            }
            ViewData["list_Letter"] = CommonCode.GetSelectDataList(di2);

            Dictionary<short, KeyValuePair<string, string>> enumValues = EnumHelper.GetEnumKeyValuesWithDescription<short>(typeof(TransportTypeEnum));
            Dictionary<int, string> di3 = new Dictionary<int, string>();
            foreach (var item in enumValues)
            {
                di3.Add(item.Key, item.Value.Value);
            }
            ViewData["list_TransportType"] = CommonCode.GetSelectDataList(di3);

            var dictionaries = _dictionaryServices.GetDictionaryInfos(CurrentUserServices.Me.UserID, null);
            ViewBag.TradeType = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.TradeType).ToList();
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
                case "PurchaseNumber":
                    dbColumnName = "PurchaseContractID";
                    break;

                case "OrderNumber":
                    dbColumnName = "Delivery_ShipmentOrder.Order.OrderNumber";
                    break;

                case "CustomerCode":
                    dbColumnName = "Delivery_ShipmentOrder.Order.Orders_Customers.CustomerCode";
                    break;

                case "PortName":
                    dbColumnName = "Delivery_ShipmentOrder.Order.PortID";
                    break;

                case "DestinationPortName":
                    dbColumnName = "Delivery_ShipmentOrder.Order.DestinationPortID";
                    break;

                case "OrderDateStartFormatter":
                    dbColumnName = "Delivery_ShipmentOrder.Order.OrderDateStart";
                    break;

                case "OrderDateEndFormatter":
                    dbColumnName = "Delivery_ShipmentOrder.Order.OrderDateEnd";
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

        public ActionResult Index(VMInspectionCustomsSearch vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DocumentsManagement_InspectionCustoms_Index);
            vm_search.PageType = PageTypeEnum.PendingApproval;

            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            dictionary.Add((int)InspectionCustomsStatusEnum.PendingMaintenance, EnumHelper.GetCustomEnumDesc(typeof(InspectionCustomsStatusEnum), InspectionCustomsStatusEnum.PendingMaintenance));
            dictionary.Add((int)InspectionCustomsStatusEnum.OutLine, EnumHelper.GetCustomEnumDesc(typeof(InspectionCustomsStatusEnum), InspectionCustomsStatusEnum.OutLine));
            dictionary.Add((int)InspectionCustomsStatusEnum.PendingCheck, EnumHelper.GetCustomEnumDesc(typeof(InspectionCustomsStatusEnum), InspectionCustomsStatusEnum.PendingCheck));
            dictionary.Add((int)InspectionCustomsStatusEnum.NotPassCheck, EnumHelper.GetCustomEnumDesc(typeof(InspectionCustomsStatusEnum), InspectionCustomsStatusEnum.NotPassCheck));

            ViewData["InspectionCustomsStatus"] = CommonCode.GetSelectDataList(dictionary);

            return View(vm_search);
        }

        public ActionResult PassedApprovalList(VMInspectionCustomsSearch vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DocumentsManagement_InspectionCustoms_PassedApprovalList);
            vm_search.PageType = PageTypeEnum.PassedApproval;

            return View(vm_search);
        }

        public ActionResult GetAll(VMInspectionCustomsSearch vm_search)
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
                (int)ERPPage.DocumentsManagement_InspectionCustoms_Index,
                (int)ERPPage.DocumentsManagement_InspectionCustoms_PassedApprovalList });

            #endregion 筛选条件

            List<DTOInspectionCustoms> listModel = _inspectionCustomsService.GetAll(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows, vm_search);

            if (vm_search.PageType == PageTypeEnum.PendingApproval && listModel != null)
            {
                listModel.Where(d => d.StatusID == (int)InspectionCustomsStatusEnum.PendingCheck).ToList().ForEach(p => p.NextApproverInfos = ApprovalService.GetNextApproverInfos(WorkflowTypes.ApprovalInspectionCustoms, p.ST_CREATEUSER, p.ApproverIndex, p.CustomerID));
            }

            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }

        public ActionResult Add(int id, int ShipmentOrderID, int Type)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DocumentsManagement_InspectionCustoms_Index);
            if ((pageElementPrivileges & (int)InspectionCustomsElementPrivileges.Customs) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            var list_vm = _shipmentOrderService.GetDetailByID_ForEditPage(CurrentUserServices.Me, ShipmentOrderID,0, -1, -1);

            ViewBag.ID = id;
            ViewBag.Type = Type;
            return View(list_vm);
        }

        [HttpPost]
        public ActionResult Add(VMShipmentOrder vm)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();

            result = _inspectionCustomsService.Add(CurrentUserServices.Me, vm);

            return Json(result);
        }

        [HttpGet]
        public ActionResult GetUploadReceipt(int id)
        {
            var list = _inspectionCustomsService.GetUploadReceipt(CurrentUserServices.Me, id);
            if (list != null)
            {
                foreach (var item in list)
                {
                    item.DT_CREATEDATE_Formatter = Utils.DateTimeToStr2(item.DT_CREATEDATE);
                }
            }
            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = list.Count, Rows = list });
        }

        public ActionResult Edit(int id)
        {
            BindViewBag();

            if (DTRequest.GetQueryString("Type") == "Detail")
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> {
                        (int)ERPPage.DocumentsManagement_InspectionCustoms_Index,
                        (int)ERPPage.DocumentsManagement_InspectionCustoms_PassedApprovalList});
                if ((pageElementPrivileges & (int)InspectionCustomsElementPrivileges.Watch) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                ViewBag.Title = "查看报关";
                ViewBag.PageTypeID = PageTypeEnum.Details;
            }
            else if (DTRequest.GetQueryString("Type") == "Add")
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DocumentsManagement_InspectionCustoms_Index);
                if ((pageElementPrivileges & (int)InspectionCustomsElementPrivileges.Customs) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                ViewBag.Title = "新建报关";
                ViewBag.PageTypeID = PageTypeEnum.Add;
            }
            else if (DTRequest.GetQueryString("Type") == "Approval")
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DocumentsManagement_InspectionCustoms_Index);
                if ((pageElementPrivileges & (int)InspectionCustomsElementPrivileges.Approval) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                ViewBag.Title = "审核报关";
                ViewBag.PageTypeID = PageTypeEnum.Approval;
            }
            else
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DocumentsManagement_InspectionCustoms_Index);
                if ((pageElementPrivileges & (int)InspectionCustomsElementPrivileges.Edit) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                ViewBag.Title = "编辑报关";
                ViewBag.PageTypeID = PageTypeEnum.Edit;
            }

            var result = _inspectionCustomsService.GetDetailByID(CurrentUserServices.Me, id);

            ViewBag.ID = id;

            #region 获取下拉框

            int index = 0;
            foreach (var item in result)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                foreach (var item2 in item.list_InvoiceNO)
                {
                    dictionary.Add(item2, item2);
                }
                ViewData["list_InvoiceNO" + index] = CommonCode.GetSelectDataList(dictionary);

                Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
                foreach (var item2 in item.list_SCNo)
                {
                    dictionary2.Add(item2, item2);
                }
                ViewData["list_SCNo" + index] = CommonCode.GetSelectDataList(dictionary2);

                ++index;
            }

            #endregion 获取下拉框

            return View(result);
        }

        [HttpPost]
        public ActionResult Edit(List<VMInspectionCustoms> ThisModel)
        {
            var result = _inspectionCustomsService.Save(CurrentUserServices.Me, ThisModel);
            return new CustomJsonResult((short)result);
        }

        [HttpPost]
        public ActionResult CreateShippingMark(int InspectionCustomsDetailID)
        {
            VMAjaxProcessResult result = _inspectionCustomsService.CreateShippingMark(CurrentUserServices.Me, InspectionCustomsDetailID);
            return Json(result);
        }

        [HttpPost]
        public JsonResult DownLoad(int id)
        {
            List<string> list_InspectionCustomsID_InvoiceNo = new List<string>();
            List<string> makeFileList = _inspectionCustomsService.DownLoad(CurrentUserServices.Me, id, ref list_InspectionCustomsID_InvoiceNo);
            return Json(makeFileList);
        }

        [HttpPost]
        public JsonResult DownLoad_PDFAndExcel(int id)
        {
            List<string> makeFileList = _inspectionCustomsService.DownLoad_PDFAndExcel(CurrentUserServices.Me, id);
            return Json(makeFileList);
        }

        [HttpPost]
        public JsonResult IsNeedInspection(int ShipmentOrderID, bool IsNeedInspection)
        {
            var result = _inspectionCustomsService.IsNeedInspection(CurrentUserServices.Me, ShipmentOrderID, IsNeedInspection);
            return Json(result);
        }

        #endregion UserMethod
    }
}