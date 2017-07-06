using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.Dictionary;
using ERP.BLL.ERP.ShipmentOrder;
using ERP.BLL.InspectionReceipt;
using ERP.BLL.Workflow;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.InspectionReceipt;
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
    /// 报检
    /// </summary>
    [UserTrackerLog]
    [ERP.WebNew.Attributes.AuthorizedUser(PageID = (int)ERPPage.DocumentsManagement_InspectionReceipt)]
    public class InspectionReceiptController : Controller
    {
        private UserServices _userService = new UserServices();
        private DictionaryServices _dictionaryServices = new DictionaryServices();
        private ShipmentOrderService _shipmentOrderService = new ShipmentOrderService();
        private InspectionReceiptService _inspectionReceiptService = new InspectionReceiptService();

        private static SelectList GetInspectionReceiptStatus(Dictionary<int, string> dictionary)
        {
            List<SelectListItem> list = new List<SelectListItem>() { };
            list.Add(new SelectListItem() { Text = "", Value = "" });

            foreach (var item in dictionary)
            {
                list.Add(new SelectListItem() { Text = item.Value, Value = item.Key.ToString() });
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
                case "PurchaseNumber":
                    dbColumnName = "PurchaseContractID";
                    break;

                case "FactoryAbbreviation":
                    dbColumnName = "Purchase_Contract.Factory.Abbreviation";
                    break;

                case "CustomerCode":
                    dbColumnName = "Purchase_Contract.Orders_Customers.CustomerCode";
                    break;

                case "StatusName":
                    dbColumnName = "StatusID";
                    break;

                case "UpdateDateForamtter":
                    dbColumnName = "DT_MODIFYDATE";
                    break;

                default:
                    dbColumnName = name;
                    break;
            }
            return dbColumnName;
        }

        /// <summary>
        /// 待审核的报检单据列表
        /// </summary>
        /// <param name="vm_search"></param>
        /// <returns></returns>
        public ActionResult Index(VMFilterInspectionReceipt vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DocumentsManagement_InspectionReceipt_Index);
            vm_search.PageType = PageTypeEnum.PendingApproval;

            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            dictionary.Add((int)InspectionReceiptStatusEnum.PendingMaintenance, EnumHelper.GetCustomEnumDesc(typeof(InspectionReceiptStatusEnum), InspectionReceiptStatusEnum.PendingMaintenance));
            dictionary.Add((int)InspectionReceiptStatusEnum.OutLine, EnumHelper.GetCustomEnumDesc(typeof(InspectionReceiptStatusEnum), InspectionReceiptStatusEnum.OutLine));
            dictionary.Add((int)InspectionReceiptStatusEnum.PendingCheck, EnumHelper.GetCustomEnumDesc(typeof(InspectionReceiptStatusEnum), InspectionReceiptStatusEnum.PendingCheck));
            dictionary.Add((int)InspectionReceiptStatusEnum.NotPassCheck, EnumHelper.GetCustomEnumDesc(typeof(InspectionReceiptStatusEnum), InspectionReceiptStatusEnum.NotPassCheck));

            ViewData["InspectionReceiptStatus"] = GetInspectionReceiptStatus(dictionary);

            return View(vm_search);
        }

        /// <summary>
        /// 已审核的报检单据列表
        /// </summary>
        /// <param name="vm_search"></param>
        /// <returns></returns>
        public ActionResult PassedApprovalList(VMFilterInspectionReceipt vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DocumentsManagement_InspectionReceipt_PassedApprovalList);
            vm_search.PageType = PageTypeEnum.PassedApproval;

            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            dictionary.Add((int)InspectionReceiptStatusEnum.PassedCheck, EnumHelper.GetCustomEnumDesc(typeof(InspectionReceiptStatusEnum), InspectionReceiptStatusEnum.PassedCheck));
            dictionary.Add((int)InspectionReceiptStatusEnum.Sended, EnumHelper.GetCustomEnumDesc(typeof(InspectionReceiptStatusEnum), InspectionReceiptStatusEnum.Sended));
            dictionary.Add((int)InspectionReceiptStatusEnum.Uploaded, EnumHelper.GetCustomEnumDesc(typeof(InspectionReceiptStatusEnum), InspectionReceiptStatusEnum.Uploaded));

            ViewData["InspectionReceiptStatus"] = GetInspectionReceiptStatus(dictionary);

            return View(vm_search);
        }

        public ActionResult GetAll(VMFilterInspectionReceipt vm_search)
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
                (int)ERPPage.DocumentsManagement_InspectionReceipt_Index,
                (int)ERPPage.DocumentsManagement_InspectionReceipt_PassedApprovalList });

            #endregion 筛选条件

            List<DTOInspectionReceiptList> listModel = _inspectionReceiptService.GetAll(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows, vm_search);

            if (vm_search.PageType == PageTypeEnum.PendingApproval && listModel != null)
            {
                listModel.Where(d => d.StatusID == (int)InspectionReceiptStatusEnum.PendingCheck).ToList().ForEach(p => p.NextApproverInfos = ApprovalService.GetNextApproverInfos(WorkflowTypes.ApprovalInspectionReceipt, p.ST_CREATEUSER, p.ApproverIndex, p.CustomerID));
            }

            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }

        public ActionResult Add(int id)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DocumentsManagement_InspectionReceipt_Index);
            if ((pageElementPrivileges & (int)InspectionReceiptElementPrivileges.Receipt) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            var list_vm = _inspectionReceiptService.GetDetailByID_Add(CurrentUserServices.Me, id);
            foreach (var item in list_vm)
            {
                item.InspectionReceiptListID = id;
            }

            return View(list_vm);
        }

        [HttpPost]
        public ActionResult Add(VMShipmentOrder vm)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();

            result = _inspectionReceiptService.Add(CurrentUserServices.Me, vm);

            return Json(result);
        }

        public ActionResult Edit(int ID)
        {

            var dictionaries = _dictionaryServices.GetDictionaryInfos(CurrentUserServices.Me.UserID, null);
            ViewBag.TradeType = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.TradeType).ToList();

            List<int> pageList = new List<int>();

            pageList.Add((int)ERPPage.DocumentsManagement_InspectionReceipt_Index);
            pageList.Add((int)ERPPage.DocumentsManagement_InspectionReceipt_PassedApprovalList);

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, pageList);
            int iPrivilegeswResult = 0;

            var list_vm = _inspectionReceiptService.GetDetailByID(CurrentUserServices.Me, ID);
            ViewBag.ID = ID;

            string Type = DTRequest.GetQueryString("Type");

            switch (Type)
            {
                case "Add":
                    iPrivilegeswResult = pageElementPrivileges & (int)InspectionReceiptElementPrivileges.Receipt;
                    ViewBag.Title = "新建报检";

                    ViewBag.PageTypeID = PageTypeEnum.Add;
                    break;

                case "Edit":
                    iPrivilegeswResult = pageElementPrivileges & (int)InspectionReceiptElementPrivileges.Edit;
                    ViewBag.Title = "编辑报检";
                    ViewBag.PageTypeID = PageTypeEnum.Edit;
                    break;

                case "Detail":
                    iPrivilegeswResult = pageElementPrivileges & (int)InspectionReceiptElementPrivileges.Watch;
                    ViewBag.Title = "查看报检";
                    ViewBag.PageTypeID = PageTypeEnum.Details;
                    break;

                case "Approval":
                    iPrivilegeswResult = pageElementPrivileges & (int)InspectionReceiptElementPrivileges.Audit;
                    ViewBag.Title = "审核报检";
                    ViewBag.PageTypeID = PageTypeEnum.Approval;
                    if (iPrivilegeswResult > 0)
                    {
                        bool isHasApprovalPermission = ApprovalService.HasApprovalPermission(WorkflowTypes.ApprovalInspectionReceipt, CurrentUserServices.Me, list_vm.FirstOrDefault().CreateUserID, list_vm.FirstOrDefault().ApproverIndex, list_vm.FirstOrDefault().CustomerID);

                        if (!isHasApprovalPermission)
                        {
                            return RedirectToAction("NoAuthPop", "Account");
                        }
                    }

                    break;

                default:
                    ViewBag.Title = "未定义状态";

                    break;
            }

            if (iPrivilegeswResult == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }


            return View(list_vm);
        }

        [HttpPost]
        public ActionResult Edit(List<DTOInspectionReceipt> ThisModel)
        {
            var result = _inspectionReceiptService.Save(CurrentUserServices.Me, ThisModel);
            return new CustomJsonResult((short)result);
        }

        public ActionResult SendEmail(int ID)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int>{
                    (int)ERPPage.DocumentsManagement_InspectionReceipt_PassedApprovalList,});
            if ((pageElementPrivileges & (int)InspectionReceiptElementPrivileges.Sending) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            DTOInspectionReceipt vm = new DTOInspectionReceipt();
            vm.PageTitle = "发送工厂";
            vm.InspectionReceiptListID = ID;
            ViewBag.FromAddress = CurrentUserServices.Me.Email;
            ViewBag.ToAddress = _inspectionReceiptService.GetFactoryEmail(ID);
            return View(vm);
        }

        [HttpPost]
        public ActionResult SendEmail(int id, VMSendEmail vm)
        {
            DBOperationStatus result = _inspectionReceiptService.SendEmail(CurrentUserServices.Me, id, vm);
            return Json(result);
        }

        public ActionResult Upload(int ID)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int>{
                    (int)ERPPage.DocumentsManagement_InspectionReceipt_PassedApprovalList,});
            if ((pageElementPrivileges & (int)InspectionReceiptElementPrivileges.UploadFile) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            var list_vm = _inspectionReceiptService.GetUploadList(CurrentUserServices.Me, ID);
            ViewBag.Title = "上传凭条";

            return View(list_vm);
        }

        [HttpPost]
        public ActionResult Upload(List<DTOInspectionReceipt> ThisModel)
        {
            DBOperationStatus result = _inspectionReceiptService.Upload(CurrentUserServices.Me, ThisModel);

            return new CustomJsonResult((short)result);
        }

        [HttpPost]
        public ActionResult CreateShippingMark(int InspectionReceiptID)
        {
            VMAjaxProcessResult result = _inspectionReceiptService.CreateShippingMark(CurrentUserServices.Me, InspectionReceiptID);
            return Json(result);
        }

        [HttpPost]
        public JsonResult DownLoad(int id)
        {
            List<string> list_InspectionReceiptID_InvoiceNo = new List<string>();
            List<string> makeFileList = _inspectionReceiptService.DownLoad(CurrentUserServices.Me, id, ref list_InspectionReceiptID_InvoiceNo);
            return Json(makeFileList);
        }

        [HttpPost]
        public JsonResult DownLoad_PDFAndExcel(int id)
        {
            List<string> makeFileList = _inspectionReceiptService.DownLoad_PDFAndExcel(CurrentUserServices.Me, id);
            return Json(makeFileList);
        }

        [HttpPost]
        public JsonResult IsNeedInspection(int ID, int ShipmentOrderID, bool IsNeedInspection)
        {
            var result = _inspectionReceiptService.IsNeedInspection(CurrentUserServices.Me, ID, ShipmentOrderID, IsNeedInspection);
            return Json(result);
        }
    }
}