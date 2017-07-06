using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.Dictionary;
using ERP.BLL.ERP.Factory;
using ERP.BLL.ERP.ShipmentOrder;
using ERP.BLL.InspectionExchange;
using ERP.BLL.Workflow;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.InspectionExchange;
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
    /// 结汇
    /// </summary>
    [UserTrackerLog]
    [ERP.WebNew.Attributes.AuthorizedUser(PageID = (int)ERPPage.DocumentsManagement_InspectionExchange)]
    public class InspectionExchangeController : Controller
    {
        private UserServices _userService = new UserServices();
        private ShipmentOrderService _shipmentOrderService = new ShipmentOrderService();
        private FactoryServices _factoryServices = new FactoryServices();
        private DictionaryServices _dictionaryServices = new DictionaryServices();

        private InspectionExchangeService _inspectionExchangeService = new InspectionExchangeService();

        #region HelperMethod

        /// <summary>
        /// 绑定ViewBag
        /// </summary>
        private void BindViewBag()
        {
            //获取结汇方式下拉框数据
            var ExchangeTypeList = _dictionaryServices.GetDictionaryInfos(CurrentUserServices.Me.UserID, DictionaryTableKind.ExchangeType);


            var dictionaries = _dictionaryServices.GetDictionaryInfos(CurrentUserServices.Me.UserID, null);
            ViewBag.PortOfEntry = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.GoalPort).ToList();

            Dictionary<string, string> di2 = new Dictionary<string, string>();
            di2.Add("", "");
            for (int i = 65; i <= 90; i++)
            {
                char letter = (char)i;
                di2.Add(letter.ToString(), letter.ToString());
            }
            ViewData["list_Letter"] = CommonCode.GetSelectDataList(di2);

            ViewBag.TransshipmentPort = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.TransshipmentPort).ToList();

            Dictionary<int, string> di = _factoryServices.GetAllFactoryKeyInfo2();
            ViewBag.FactoryList = CommonCode.GetSelectDataList(di);
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

        public ActionResult Index(VMInspectionExchangeSearch vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DocumentsManagement_InspectionExchange_Index);
            vm_search.PageType = PageTypeEnum.PendingApproval;

            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            dictionary.Add((int)InspectionExchangeStatusEnum.PendingMaintenance, EnumHelper.GetCustomEnumDesc(typeof(InspectionExchangeStatusEnum), InspectionExchangeStatusEnum.PendingMaintenance));
            dictionary.Add((int)InspectionExchangeStatusEnum.OutLine, EnumHelper.GetCustomEnumDesc(typeof(InspectionExchangeStatusEnum), InspectionExchangeStatusEnum.OutLine));
            dictionary.Add((int)InspectionExchangeStatusEnum.PendingCheck, EnumHelper.GetCustomEnumDesc(typeof(InspectionExchangeStatusEnum), InspectionExchangeStatusEnum.PendingCheck));
            dictionary.Add((int)InspectionExchangeStatusEnum.NotPassCheck, EnumHelper.GetCustomEnumDesc(typeof(InspectionExchangeStatusEnum), InspectionExchangeStatusEnum.NotPassCheck));

            ViewData["InspectionExchangeStatus"] = CommonCode.GetSelectDataList(dictionary);

            return View(vm_search);
        }

        public ActionResult PassedApprovalList(VMInspectionExchangeSearch vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DocumentsManagement_InspectionExchange_PassedApprovalList);
            vm_search.PageType = PageTypeEnum.PassedApproval;

            return View(vm_search);
        }

        public ActionResult GetAll(VMInspectionExchangeSearch vm_search)
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
                (int)ERPPage.DocumentsManagement_InspectionExchange_Index,
                (int)ERPPage.DocumentsManagement_InspectionExchange_PassedApprovalList });

            #endregion 筛选条件

            List<DTOInspectionExchange> listModel = _inspectionExchangeService.GetAll(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows, vm_search);

            if (vm_search.PageType == PageTypeEnum.PendingApproval && listModel != null)
            {
                listModel.Where(d => d.StatusID == (int)InspectionExchangeStatusEnum.PendingCheck).ToList().ForEach(p => p.NextApproverInfos = ApprovalService.GetNextApproverInfos(WorkflowTypes.ApprovalInspectionExchange, p.ST_CREATEUSER, p.ApproverIndex, p.CustomerID));
            }

            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }

        public ActionResult Edit(int id)
        {
            BindViewBag();

            if (DTRequest.GetQueryString("Type") == "Detail")
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> {
                        (int)ERPPage.DocumentsManagement_InspectionExchange_Index,
                        (int)ERPPage.DocumentsManagement_InspectionExchange_PassedApprovalList});
                if ((pageElementPrivileges & (int)InspectionExchangeElementPrivileges.Watch) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                ViewBag.Title = "查看结汇";
                ViewBag.PageTypeID = PageTypeEnum.Details;
            }
            else if (DTRequest.GetQueryString("Type") == "Add")
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DocumentsManagement_InspectionExchange_Index);
                if ((pageElementPrivileges & (int)InspectionExchangeElementPrivileges.Exchange) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                ViewBag.Title = "新建结汇";
                ViewBag.PageTypeID = PageTypeEnum.Add;
            }
            else if (DTRequest.GetQueryString("Type") == "Approval")
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DocumentsManagement_InspectionExchange_Index);
                if ((pageElementPrivileges & (int)InspectionExchangeElementPrivileges.Approval) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                ViewBag.Title = "审核结汇";
                ViewBag.PageTypeID = PageTypeEnum.Approval;
            }
            else
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DocumentsManagement_InspectionExchange_Index);
                if ((pageElementPrivileges & (int)InspectionExchangeElementPrivileges.Edit) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                ViewBag.Title = "编辑结汇";
                ViewBag.PageTypeID = PageTypeEnum.Edit;
            }

            var result = _inspectionExchangeService.GetDetailByID(CurrentUserServices.Me, id);

            ViewBag.AcceptInformations = new CustomerController().GetAcceptInformations(result.CustomerID);

            return View(result);
        }

        [HttpPost]
        public ActionResult Edit(VMInspectionExchange ThisModel)
        {
            var result = _inspectionExchangeService.Save(CurrentUserServices.Me, ThisModel);
            return new CustomJsonResult((short)result);
        }

        [HttpPost]
        public JsonResult SelectIsCheck(int InspectionExchangeID,bool IsCheck)
        {
            var result = _inspectionExchangeService.SelectIsCheck(CurrentUserServices.Me, InspectionExchangeID,IsCheck);
            return Json(result);
        }
        
        [HttpPost]
        public JsonResult DownLoad(int id)
        {
            List<string> makeFileList = _inspectionExchangeService.DownLoad(CurrentUserServices.Me, id);
            return Json(makeFileList);
        }

        [HttpPost]
        public JsonResult DownLoad_PDFAndExcel(int id)
        {
            List<string> makeFileList = _inspectionExchangeService.DownLoad_PDFAndExcel(CurrentUserServices.Me, id);
            return Json(makeFileList);
        }


        public ActionResult UploadModify(int id)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int>{
                    (int)ERPPage.DocumentsManagement_InspectionExchange_PassedApprovalList,});
            if ((pageElementPrivileges & (int)InspectionExchangeElementPrivileges.UpLoadModify) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            var vm = _inspectionExchangeService.GetUploadList_Modify(CurrentUserServices.Me, id);
            return View(vm);
        }

        [HttpPost]
        public ActionResult UploadModify(VMInspectionExchange ThisModel)
        {
            DBOperationStatus result = _inspectionExchangeService.UploadModify(CurrentUserServices.Me, ThisModel);

            return new CustomJsonResult((short)result);
        }
        #endregion UserMethod
    }
}