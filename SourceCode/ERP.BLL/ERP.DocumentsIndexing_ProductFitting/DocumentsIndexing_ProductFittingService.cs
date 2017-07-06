using ERP.BLL.Consts;
using ERP.BLL.ERP.Consts;
using ERP.BLL.InspectionCustoms;
using ERP.BLL.Workflow;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.DocumentIndexing;
using ERP.Models.Order;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP.BLL.ERP.DocumentsIndexing_ProductFitting
{
    /// <summary>
    /// 单证索引
    /// </summary>
    public class DocumentsIndexing_ProductFittingService
    {
        private Dictionary.DictionaryServices _dictionaryServices = new Dictionary.DictionaryServices();

        #region HelperMethod

        /// <summary>
        /// 获取状态描述的内容
        /// </summary>
        /// <param name="StatusID"></param>
        /// <returns></returns>
        private static string GetDocumentsIndexingStatusEnum_Description(int StatusID)
        {
            return EnumHelper.GetCustomEnumDesc(typeof(DocumentsIndexingStatusEnum), (DocumentsIndexingStatusEnum)StatusID);
        }

        #endregion HelperMethod

        #region UserMethod

        /// <summary>
        /// 获取列表数据
        /// </summary>
        public List<VMDocumentIndexing> GetAll(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows,
          VMOrderSearch vm_search)
        {
            List<VMDocumentIndexing> listModel = null;
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.DocumentsIndexings.Where(d => !d.IsDelete && d.DocumentsIndexingType == (int)DocumentsIndexingTypeEnum.ProductFitting);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForDocumentsIndexing);

                    switch (vm_search.PageType)
                    {
                        case PageTypeEnum.PendingMaintenanceList:
                            query = query.Where(d => d.StatusID != (short)DocumentsIndexingStatusEnum.PassedCheck && d.StatusID != (short)DocumentsIndexingStatusEnum.PendingCheck);
                            break;

                        case PageTypeEnum.PendingApproval:
                            query = query.Where(d => d.StatusID == (short)DocumentsIndexingStatusEnum.PendingCheck);
                            break;

                        case PageTypeEnum.PassedApproval:
                            query = query.Where(d => d.StatusID == (short)DocumentsIndexingStatusEnum.PassedCheck);
                            break;

                        default:
                            break;
                    }

                    #region 筛选条件

                    if (!string.IsNullOrEmpty(vm_search.OrderNumber))
                    {
                        query = query.Where(d => d.Order.OrderNumber.Contains(vm_search.OrderNumber));
                    }
                    if (!string.IsNullOrEmpty(vm_search.CustomerDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.CustomerDateStart);
                        query = query.Where(d => d.Order.CustomerDate >= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.CustomerDateEnd))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.CustomerDateEnd);
                        query = query.Where(d => d.Order.CustomerDate <= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.CustomerCode))
                    {
                        query = query.Where(d => d.Order.Orders_Customers.CustomerCode == vm_search.CustomerCode);
                    }
                    if (!string.IsNullOrEmpty(vm_search.OrderDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.OrderDateStart);
                        query = query.Where(d => d.Order.OrderDateStart >= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.OrderDateEnd))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.OrderDateEnd);
                        query = query.Where(d => d.Order.OrderDateEnd <= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.POID))
                    {
                        query = query.Where(d => d.Order.POID.Contains(vm_search.POID));
                    }
                    if (!string.IsNullOrEmpty(vm_search.OrderOrigin))
                    {
                        query = query.Where(d => d.Order.OrderOrigin.Contains(vm_search.OrderOrigin));
                    }
                    if (!string.IsNullOrEmpty(vm_search.OrderStatusID))
                    {
                        int i = Utils.StrToInt(vm_search.OrderStatusID, 0);
                        query = query.Where(d => d.Order.OrderStatusID == i);
                    }

                    #endregion 筛选条件

                    #region 排序

                    if (sortColumnsNames != null && sortColumnsNames.Count > 0
                        && sortColumnOrders != null && sortColumnOrders.Count > 0
                        && sortColumnsNames.Count == sortColumnOrders.Count)
                    {
                        query = query.OrderBy(sortColumnsNames, sortColumnOrders);
                    }
                    else
                    {
                        query = query.OrderByDescending(d => d.DT_MODIFYDATE);
                    }

                    #endregion 排序

                    totalRows = query.Count();//获取总条数

                    var dataFromDB = query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();//分页

                    #region 给Model赋值

                    if (dataFromDB != null && dataFromDB.Count > 0)
                    {
                        listModel = new List<VMDocumentIndexing>();

                        List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());

                        foreach (var item in dataFromDB)
                        {
                            bool IsHasApprovalPermission = true;

                            #region 判断是否有审批流的权限

                            if (vm_search.PageType == PageTypeEnum.PendingApproval)//待审核
                            {
                                IsHasApprovalPermission = ApprovalService.HasApprovalPermission(WorkflowTypes.ApprovalDocumentsIndexing, currentUser, item.ST_CREATEUSER, item.ApproverIndex, item.Order.CustomerID);
                            }

                            #endregion 判断是否有审批流的权限

                            listModel.Add(new VMDocumentIndexing()
                            {
                                ID = item.ID,
                                OrderID = item.OrderID,
                                OrderNumber = item.Order.OrderNumber,
                                CustomerID = item.Order.CustomerID,
                                CustomerNo = item.Order.Orders_Customers.CustomerCode,
                                POID = item.Order.POID,
                                EHIPO = item.Order.EHIPO,
                                ShipmentOrderID = item.ShipmentOrderID,
                                OrderDateStart = Utils.DateTimeToStr(item.Order.OrderDateStart),
                                OrderDateEnd = Utils.DateTimeToStr(item.Order.OrderDateEnd),
                                StatusID = item.StatusID,
                                StatusName = GetDocumentsIndexingStatusEnum_Description(item.StatusID),
                                Managers_Date = Utils.DateTimeToStr(item.Managers_Date),
                                Managers_UserName = context.SystemUsers.Find(item.Managers_UserID) == null ? null : context.SystemUsers.Find(item.Managers_UserID).UserName,
                                DT_MODIFYDATE = Utils.DateTimeToStr2(item.DT_MODIFYDATE),
                                ST_CREATEUSER = item.ST_CREATEUSER,
                                IsHasApprovalPermission = IsHasApprovalPermission,
                                ApproverIndex = item.ApproverIndex,
                                ShippingDate = Utils.DateTimeToStr(item.ShippingDate),
                                GuaranteeShipments = item.GuaranteeShipments ? "否" : "是",
                                PortName = _dictionaryServices.GetDictionary_PortName(item.Order.PortID, list_Com_DataDictionary),
                                //订单描述
                            });
                        }
                    }

                    #endregion 给Model赋值
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return listModel;
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        public VMDocumentIndexing GetDetailByID(VMERPUser currentUser, int id)
        {
            VMDocumentIndexing vm = new VMDocumentIndexing();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.DocumentsIndexings.Where(d => d.ID == id && !d.IsDelete);

                    var dataFromDB = query.FirstOrDefault();
                    if (dataFromDB != null)
                    {
                        vm.ID = dataFromDB.ID;
                        vm.OrderID = dataFromDB.OrderID;

                        var query_Order = dataFromDB.Order;

                        List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());

                        List<VMUpLoadFile> list_UploadFile_PortChargesInvoice = new List<VMUpLoadFile>();
                        vm.IsPortChargesInvoice = false;

                        decimal AllAmount_USD = 0;
                        decimal AllAmount_RMB = 0;

                        List<VMDocumentIndexing_RegisterFees> list_RegisterFee = new List<VMDocumentIndexing_RegisterFees>();

                        //单证索引信息
                        vm.CustomerCode = query_Order.Orders_Customers.CustomerCode;
                        vm.CustomsUnit = dataFromDB.CustomsUnit;
                        vm.ShippingDate = Utils.DateTimeToStr(dataFromDB.ShippingDate);
                        if (query_Order.Orders_Customers.PaymentType.HasValue)
                        {
                            vm.PaymentType = _dictionaryServices.GetDictionaryByName2(query_Order.Orders_Customers.PaymentType, list_Com_DataDictionary);
                        }

                        vm.IsGuaranteeShipments = dataFromDB.GuaranteeShipments;

                        List<VMUpLoadFile> list_UploadFile_PurchaseContract = new List<VMUpLoadFile>();
                        List<VMUpLoadFile> list_UploadFile_PurchaseContract_MakeMoney = new List<VMUpLoadFile>();
                        List<VMDocumentIndexing_Purchase> list_Purchase = new List<VMDocumentIndexing_Purchase>();
                        List<VMDocumentIndexing_Outsourcing> list_Outsourcing = new List<VMDocumentIndexing_Outsourcing>();
                        List<VMDocumentIndexing_Inspection> list_Inspection = new List<VMDocumentIndexing_Inspection>();
                        List<VMDocumentIndexing_OtherFees> list_OtherFees = new List<VMDocumentIndexing_OtherFees>();
                        List<VMUpLoadFile> list_Upload_Order = new List<VMUpLoadFile>();

                        var query_Purchase = context.Purchase_Contract.Where(d => d.OrderID == dataFromDB.OrderID && d.ContractType == (int)ContractTypeEnum.ProductFitting && !d.IsDelete);
                        foreach (var item in query_Purchase)
                        {
                            #region 工厂合同

                            var Purchase_ContractBatchID = item.Purchase_ContractBatch.First().ID;
                            int CurrencyType = context.ProductFittings.Where(d => d.ParentID == Purchase_ContractBatchID && !d.Deleted && d.ModuleType == (int)ModuleTypeEnum.PurchaseContract).First().CurrencyType;
                            string CurrentSign = _dictionaryServices.GetDictionary_CurrencySign(CurrencyType, list_Com_DataDictionary);
                            var temp = ConstsMethod.GetUploadFileList(item.ID, UploadFileType.PurchaseContract);
                            list_UploadFile_PurchaseContract.AddRange(temp);

                            var temp2 = ConstsMethod.GetUploadFileList(item.ID, UploadFileType.PurchaseContract_MakeMoney);
                            list_UploadFile_PurchaseContract_MakeMoney.AddRange(temp2);

                            if (CurrentSign == Keys.RMB_Sign)
                            {
                                AllAmount_RMB += item.AllAmount;
                            }
                            else
                            {
                                AllAmount_USD += item.AllAmount;
                            }

                            DateTime? FCRReceiveDateEnd = null;
                            var temp3 = context.Inspection_InspectionClearance.Where(d => d.Delivery_ShipmentOrder.OrderIDList.Contains(item.OrderID.ToString()) || d.Delivery_ShipmentOrder.OrderID == item.OrderID);
                            if (temp3 != null && temp3.Count() > 0)
                            {
                                FCRReceiveDateEnd = temp3.First().FCRReceiveDateEnd;
                            }

                            list_Purchase.Add(new VMDocumentIndexing_Purchase()
                            {
                                ID = item.ID,
                                AllAmount = CurrentSign + item.AllAmount,
                                FactoryAbbreviation = item.Factory.Abbreviation,
                                PurchaseID = item.ID,
                                IsUpload = temp.Count > 0,
                                IsUpload_MakeMoney = temp2.Count > 0,
                                DocumentsIndexing_IsGuaranteeShipments = item.DocumentsIndexing_IsGuaranteeShipments.HasValue ? item.DocumentsIndexing_IsGuaranteeShipments.Value : false,
                            });

                            #endregion 工厂合同
                        }

                        list_Upload_Order.AddRange(ConstsMethod.GetUploadFileList(dataFromDB.OrderID, UploadFileType.Order));

                        vm.SelectCustomer = query_Order.Orders_Customers.SelectCustomer;

                        //如果核算单那里上传了附件，附件显示上传的核算单的附件；如果没上传附件，就显示报关SC的总金额和报关的SC附件
                        vm.IsUpload_Order = false;
                        if (list_Upload_Order != null && list_Upload_Order.Count > 0)
                        {
                            vm.list_Upload_Order = list_Upload_Order;
                            vm.IsUpload_Order = true;
                        }
                        vm.AllAmount = query_Order.OrderAmount;

                        //工厂合同
                        vm.list_Purchase = list_Purchase;
                        vm.list_UploadFile_PurchaseContract = list_UploadFile_PurchaseContract;
                        if (vm.list_UploadFile_PurchaseContract != null && vm.list_UploadFile_PurchaseContract.Count > 0)
                        {
                            vm.IsPurchaseContract = true;
                        }
                        else
                        {
                            vm.IsPurchaseContract = false;
                        }
                        vm.list_UploadFile_PurchaseContract_MakeMoney = list_UploadFile_PurchaseContract_MakeMoney;

                        //其他和备注
                        vm.IsOther = dataFromDB.IsOther;
                        vm.OtherFee = dataFromDB.OtherFee;
                        vm.Comment = dataFromDB.Comment;

                        vm.AllAmount_USD = AllAmount_USD;
                        vm.AllAmount_RMB = AllAmount_RMB;

                        #region 获取历史记录列表

                        var query_history = dataFromDB.DocumentsIndexingHistories;
                        List<VMDocumentIndexingHistory> list_history = new List<VMDocumentIndexingHistory>();
                        foreach (var item_history in query_history)
                        {
                            list_history.Add(new VMDocumentIndexingHistory()
                            {
                                ST_CREATEUSER = context.SystemUsers.Find(item_history.ST_CREATEUSER).UserName,
                                DT_CREATEDATE = item_history.DT_CREATEDATE,
                                Comment = GetDocumentsIndexingStatusEnum_Description(item_history.StatusID),
                                CheckSuggest = item_history.CheckSuggest,
                            });
                        }

                        #endregion 获取历史记录列表

                        vm.list_history = list_history;
                        vm.ShippingDateForamtter = CommonCode.GetDateTime3(dataFromDB.ShippingDate);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return vm;
        }
        
        public VMAjaxProcessResult Save(VMERPUser currentUser, VMDocumentIndexing vm)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.DocumentsIndexings.Where(p => p.ID == vm.ID);

                    var dataFromDB = query.FirstOrDefault();
                    if (dataFromDB != null)
                    {
                        var item = context.DocumentsIndexings.Find(vm.ID);
                        if (item != null)
                        {
                            if (item.StatusID == (int)DocumentsIndexingStatusEnum.PendingMaintenance)
                            {
                                item.ST_CREATEUSER = currentUser.UserID;
                                item.DT_CREATEDATE = DateTime.Now;
                            }

                            if (vm.StatusID == (int)DocumentsIndexingStatusEnum.PendingCheck && !item.Managers_UserID.HasValue)//第一次提交审批时
                            {
                                item.Managers_UserID = currentUser.UserID;
                                item.Managers_Date = DateTime.Now;
                            }

                            item.CustomsUnit = vm.CustomsUnit;
                            item.GuaranteeShipments = vm.IsGuaranteeShipments;
                            item.IsOther = vm.IsOther;
                            item.OtherFee = vm.OtherFee;
                            item.Comment = vm.Comment;

                            if (!string.IsNullOrEmpty(vm.ShippingDate))
                            {
                                item.ShippingDate = Utils.StrToDateTime(vm.ShippingDate);
                            }

                            item.ST_MODIFYUSER = currentUser.UserID;
                            item.DT_MODIFYDATE = DateTime.Now;
                            item.IPAddress = CommonCode.GetIP();

                            if (vm.StatusID != (int)DocumentsIndexingStatusEnum.NotPassCheck && vm.StatusID != (int)DocumentsIndexingStatusEnum.PassedCheck)
                            {
                                item.StatusID = vm.StatusID;
                            }

                            string CheckSuggest = "";
                            if (vm.StatusID == (int)DocumentsIndexingStatusEnum.PassedCheck || vm.StatusID == (int)DocumentsIndexingStatusEnum.NotPassCheck)
                            {
                                CheckSuggest = vm.CheckSuggest;
                            }

                            context.DocumentsIndexingHistories.Add(new DocumentsIndexingHistory()
                            {
                                DocumentsIndexingID = vm.ID,
                                DT_CREATEDATE = DateTime.Now,
                                ST_CREATEUSER = currentUser.UserID,
                                IPAddress = CommonCode.GetIP(),
                                StatusID = vm.StatusID,
                                CheckSuggest = vm.CheckSuggest,
                            });

                            foreach (var item2 in vm.list_Purchase)
                            {
                                var query_Purchase = context.Purchase_Contract.Find(item2.PurchaseID);
                                if (query_Purchase != null)
                                {
                                    query_Purchase.DocumentsIndexing_IsGuaranteeShipments = item2.DocumentsIndexing_IsGuaranteeShipments;
                                }
                            }

                            int affectRows = context.SaveChanges();
                            if (affectRows == 0)
                            {
                                result.IsSuccess = false;
                            }
                            else
                            {
                                result.IsSuccess = true;

                                ExecuteApproval(item.ST_CREATEUSER, item.ID, "", vm.StatusID, currentUser.UserID, false);//执行审批流
                            }
                        }
                    }

                    using (ERPEntitiesNew context2 = new ERPEntitiesNew())
                    {
                        var item = context2.DocumentsIndexings.Find(vm.ID);
                        if (item != null && item.StatusID == (int)DocumentsIndexingStatusEnum.PassedCheck)
                        {
                            item.DocumentsIndexingUploadDate = DateTime.Now; //单证索引上传日期=单证索引审核通过的日期

                            context2.SaveChanges();
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
                result.IsSuccess = false;
            }

            return result;
        }


        public List<string> MakerExcel(VMERPUser currentUser, int ID, MakerTypeEnum makerTypeEnum, int PurchaseID)
        {
            List<string> makeFileList = new List<string>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    VMDocumentIndexing vm = GetDetailByID(currentUser, ID);
                    if (vm != null)
                    {
                        List<string> list_PdfFile = new List<string>();

                        List<string> filesAsolutePathList = new List<string>();
                        List<string> filesPhysicalPathList = new List<string>();

                        if (makerTypeEnum == MakerTypeEnum.DocumentsIndexing_SaleOrder || makerTypeEnum == MakerTypeEnum.DocumentsIndexing_AllDocuments)
                        {
                            if (vm.list_Upload_Order != null && vm.list_Upload_Order.Count > 0)
                            {
                                foreach (var item in vm.list_Upload_Order)
                                {
                                    ConstsMethod.GetUploadToPdfList(list_PdfFile, item.ServerFileName);
                                }
                            }
                            else
                            {
                                #region 合约

                                var query_ShipmentOrder = context.Delivery_ShipmentOrder.Where(d => d.OrderID == vm.OrderID && !d.IsDelete);
                                if (query_ShipmentOrder.First().IsMerge)
                                {
                                    var OrderIDList = query_ShipmentOrder.First().OrderIDList;
                                    query_ShipmentOrder = context.Delivery_ShipmentOrder.Where(d => d.OrderIDList == OrderIDList && !d.IsDelete);
                                }

                                if (query_ShipmentOrder.Count() > 0)
                                {
                                    InspectionCustomsService _inspectionCustomsService = new InspectionCustomsService();
                                    foreach (var item_temp in query_ShipmentOrder)
                                    {
                                        int ShipmentOrderID = item_temp.ID;
                                        var query_InspectionCustoms = context.Inspection_InspectionCustoms.Where(d => d.ShipmentOrderID == ShipmentOrderID && !d.IsDelete);
                                        if (query_InspectionCustoms.Count() > 0)
                                        {
                                            foreach (var item_temp2 in query_InspectionCustoms)
                                            {
                                                var vm2 = _inspectionCustomsService.GetDetailByID(currentUser, item_temp2.InspectionCustomsID);
                                                foreach (var item in vm2)
                                                {
                                                    item.ShippingDateStart = vm.ShippingDateForamtter;//SHIPMENT DATE取单证索引的船期

                                                    MakerExcel_Documents.Maker(item, MakerTypeEnum.InspectionCustoms_SaleContract, item.SelectCustomer, item.InspectionCustomsID.ToString(), list_PdfFile);
                                                }
                                            }
                                        }
                                    }
                                }

                                #endregion 合约
                            }
                        }
                        if (makerTypeEnum == MakerTypeEnum.DocumentsIndexing_PortChargesInvoice || makerTypeEnum == MakerTypeEnum.DocumentsIndexing_AllDocuments)
                        {
                            #region 港杂费发票

                            if (vm.list_UploadFile_PortChargesInvoice.Count > 0)
                            {
                                foreach (var item in vm.list_UploadFile_PortChargesInvoice)
                                {
                                    ConstsMethod.GetUploadToPdfList(list_PdfFile, item.ServerFileName);
                                }
                            }

                            #endregion 港杂费发票
                        }
                        if (makerTypeEnum == MakerTypeEnum.DocumentsIndexing_FCR || makerTypeEnum == MakerTypeEnum.DocumentsIndexing_AllDocuments)
                        {
                            #region FCR

                            if (vm.list_UploadFile_FCRUploadFile.Count > 0)
                            {
                                foreach (var item in vm.list_UploadFile_FCRUploadFile)
                                {
                                    ConstsMethod.GetUploadToPdfList(list_PdfFile, item.ServerFileName);
                                }
                            }

                            #endregion FCR
                        }
                        if (makerTypeEnum == MakerTypeEnum.DocumentsIndexing_Notification || makerTypeEnum == MakerTypeEnum.DocumentsIndexing_AllDocuments)
                        {
                            #region 配仓单

                            if (vm.list_UploadFile_ShipmentNotification.Count > 0)
                            {
                                foreach (var item in vm.list_UploadFile_ShipmentNotification)
                                {
                                    ConstsMethod.GetUploadToPdfList(list_PdfFile, item.ServerFileName);
                                }
                            }

                            #endregion 配仓单
                        }
                        if (makerTypeEnum == MakerTypeEnum.DocumentsIndexing_Purchase)
                        {
                            #region 合同

                            List<VMUpLoadFile> list_UploadFile_PurchaseContract = ConstsMethod.GetUploadFileList(PurchaseID, UploadFileType.PurchaseContract);
                            if (list_UploadFile_PurchaseContract.Count > 0)
                            {
                                foreach (var item in list_UploadFile_PurchaseContract)
                                {
                                    ConstsMethod.GetUploadToPdfList(list_PdfFile, item.ServerFileName);
                                }
                            }

                            #endregion 合同
                        }
                        if (makerTypeEnum == MakerTypeEnum.DocumentsIndexing_AllDocuments)
                        {
                            #region 合同

                            List<VMUpLoadFile> list_UploadFile_PurchaseContract = ConstsMethod.GetUploadFileList(PurchaseID, UploadFileType.PurchaseContract);
                            if (vm.list_UploadFile_PurchaseContract.Count > 0)
                            {
                                foreach (var item in vm.list_UploadFile_PurchaseContract)
                                {
                                    ConstsMethod.GetUploadToPdfList(list_PdfFile, item.ServerFileName);
                                }
                            }

                            #endregion 合同
                        }

                        if (makerTypeEnum == MakerTypeEnum.DocumentsIndexing_Purchase_MakeMoney)
                        {
                            #region 请款合同

                            List<VMUpLoadFile> list_UploadFile_PurchaseContract = ConstsMethod.GetUploadFileList(PurchaseID, UploadFileType.PurchaseContract_MakeMoney);
                            if (vm.list_UploadFile_PurchaseContract_MakeMoney.Count > 0)
                            {
                                foreach (var item in vm.list_UploadFile_PurchaseContract_MakeMoney)
                                {
                                    ConstsMethod.GetUploadToPdfList(list_PdfFile, item.ServerFileName);
                                }
                            }

                            #endregion 请款合同
                        }

                        makeFileList.Add(CommonCode.CreatePdfList(list_PdfFile, "DocumentsIndex", ID.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return makeFileList;
        }

        #endregion UserMethod

        /// <summary>
        /// 执行审批流
        /// </summary>
        /// <param name="createUserID">创建人</param>
        /// <param name="identityID">主键ID</param>
        /// <param name="CheckSuggest">审批意见</param>
        /// <param name="StatusID">报价单的状态</param>
        /// <param name="UserID">当前用户ID</param>
        private static void ExecuteApproval(int createUserID, int identityID, string CheckSuggest, int StatusID, int UserID, bool historyAdded)
        {
            bool isPass = true;
            if (StatusID == (int)DocumentsIndexingStatusEnum.NotPassCheck)
            {
                isPass = false;
            }

            List<int> validWaitingApproveStatus = new List<int>(){
                            (int)DocumentsIndexingStatusEnum.PendingCheck,
                            (int)DocumentsIndexingStatusEnum.NotPassCheck
                        };
            ApprovalService.ExcuteApproval(new ApprovalInfo
            {
                WorkflowType = WorkflowTypes.ApprovalDocumentsIndexing,
                IsPass = isPass,
                IdentityID = identityID,
                ValidWaitingApproveStatus = validWaitingApproveStatus,
                StatusApproving = (int)DocumentsIndexingStatusEnum.PendingCheck,
                StatusNextTo = (int)DocumentsIndexingStatusEnum.PassedCheck,
                StatusRejected = (int)DocumentsIndexingStatusEnum.NotPassCheck,
                ApproveOpinion = CheckSuggest,
                ApproveUserID = UserID,
                LogMethod = () =>
                {
                    return null;
                }
            });
        }
    }
}