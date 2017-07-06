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

namespace ERP.BLL.ERP.DocumentsIndexing
{
    /// <summary>
    /// 单证索引
    /// </summary>
    public class DocumentsIndexingService
    {
        private ERP.Dictionary.DictionaryServices _dictionaryServices = new Dictionary.DictionaryServices();

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
                    var OrderIDList = context.Delivery_ShipmentOrder.Where(d => !d.IsDelete && d.StatusID == (int)DocumentsIndexingStatusEnum.PassedCheck).Select(d => d.OrderID);//TODO 待完善

                    var query = context.DocumentsIndexings.Where(d => !d.IsDelete && OrderIDList.Contains(d.OrderID) && d.DocumentsIndexingType == (int)DocumentsIndexingTypeEnum.Default);
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

                            #region 获取合并/分批订舱、合并订舱的订单编号

                            string OrderNumberList = "";
                            int tempOrderID = 0;
                            string tempOrderNumber = "";
                            string tempMerge = "";
                            List<string> listOrderNumberList = new List<string>();
                            var query_Delivery_ShipmentOrder = context.Delivery_ShipmentOrder.Where(d => d.ID == item.ShipmentOrderID).FirstOrDefault();
                            if (query_Delivery_ShipmentOrder != null)
                            {
                                tempOrderID = item.OrderID;
                                tempOrderNumber = context.Orders.Find(tempOrderID).OrderNumber;
                                tempMerge = query_Delivery_ShipmentOrder.IsMerge ? "合并订舱" : (query_Delivery_ShipmentOrder.IsBatchShipped ? "分批订舱" : "");

                                if (query_Delivery_ShipmentOrder.IsMerge)
                                {
                                    foreach (var OrderID in CommonCode.IdListToList(query_Delivery_ShipmentOrder.OrderIDList))
                                    {
                                        if (OrderID != tempOrderID)
                                        {
                                            listOrderNumberList.Add(context.Orders.Find(OrderID).OrderNumber);
                                        }
                                    }
                                }
                            }
                            if (listOrderNumberList.Count > 0)
                            {
                                OrderNumberList = string.Join(",", listOrderNumberList.ToArray());
                            }

                            #endregion 获取合并/分批订舱、合并订舱的订单编号

                            #region 判断是否有审批流的权限

                            if (vm_search.PageType == PageTypeEnum.PendingApproval)//待审核
                            {
                                IsHasApprovalPermission = ApprovalService.HasApprovalPermission(WorkflowTypes.ApprovalDocumentsIndexing, currentUser, item.ST_CREATEUSER, item.ApproverIndex, item.Order.CustomerID);
                            }

                            #endregion 判断是否有审批流的权限

                            #region FCR收到日期

                            string FCRReceiveDateFormatter = "";

                            var query_InspectionClearance = context.Inspection_InspectionClearance.Where(d => d.Delivery_ShipmentOrder.OrderID == item.Delivery_ShipmentOrder.OrderID && !d.IsDelete && d.StatusID == (int)InspectionClearanceStatusEnum.UploadedFCR);
                            if (item.Delivery_ShipmentOrder.IsMerge)//合并订舱
                            {
                                query_InspectionClearance = context.Inspection_InspectionClearance.Where(d => d.Delivery_ShipmentOrder.OrderIDList == item.Delivery_ShipmentOrder.OrderIDList && !d.IsDelete && d.StatusID == (int)InspectionClearanceStatusEnum.UploadedFCR);
                            }

                            if (query_InspectionClearance.Count() > 0)
                            {
                                if (query_InspectionClearance.First().FCRReceiveDateEnd.HasValue)
                                {
                                    FCRReceiveDateFormatter = Utils.DateTimeToStr(query_InspectionClearance.First().FCRReceiveDateEnd);
                                }
                            }

                            #endregion FCR收到日期

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
                                Merge = tempMerge,
                                OrderNumberList = OrderNumberList,
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
                                FCRReceiveDateFormatter = FCRReceiveDateFormatter,
                                //GuaranteeShipments = item.GuaranteeShipments ? "否" : "是",
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
                        vm.ShipmentOrderID = dataFromDB.ShipmentOrderID;

                        var query_Order = dataFromDB.Order;

                        List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());

                        List<VMUpLoadFile> list_UploadFile_ShipmentNotification = new List<VMUpLoadFile>();
                        List<string> list_InvoiceNo = new List<string>();
                        bool isPullCabinet = false;//是否拉柜
                        bool isPutInStorage = false;//是否入仓

                        List<int> list_OrderIDList = new List<int>();

                        var query_ShipmentOrder = context.Delivery_ShipmentOrder.Where(d => d.OrderID == query_Order.OrderID && !d.IsDelete);
                        if (query_ShipmentOrder.First().IsMerge)
                        {
                            var OrderIDList = query_ShipmentOrder.First().OrderIDList;
                            list_OrderIDList.AddRange(CommonCode.IdListToList(OrderIDList));
                            query_ShipmentOrder = context.Delivery_ShipmentOrder.Where(d => d.OrderIDList == OrderIDList && !d.IsDelete);
                        }
                        else
                        {
                            list_OrderIDList.Add(query_Order.OrderID);
                        }

                        decimal DesignatedAgencyAmount = 0;
                        decimal OurAgencyAmount = 0;
                        List<VMUpLoadFile> list_UploadFile_PortChargesInvoice = new List<VMUpLoadFile>();
                        vm.IsPortChargesInvoice = false;

                        decimal AllAmount_USD = 0;
                        decimal AllAmount_RMB = 0;

                        List<VMDocumentIndexing_RegisterFees> list_RegisterFee = new List<VMDocumentIndexing_RegisterFees>();
                        DateTime? PaymentDate = null;
                        if (query_ShipmentOrder.Count() > 0)
                        {
                            List<VMDocumentIndexing_RegisterFees> list_RegisterFee_Temp = new List<VMDocumentIndexing_RegisterFees>();
                            Dictionary<int, decimal?> di_CabinetIndex_RegisterFees = new Dictionary<int, decimal?>();
                            Dictionary<int, int?> di_CabinetIndex_GatherType = new Dictionary<int, int?>();
                            Dictionary<int, decimal?> di_CabinetIndex_SumOuterVolume = new Dictionary<int, decimal?>();

                            foreach (var item in query_ShipmentOrder)
                            {
                                foreach (var item2 in item.Delivery_ShipmentOrderCabinet.Where(d => !d.IsDelete))
                                {
                                    if (!di_CabinetIndex_GatherType.Keys.Contains(item2.CabinetIndex) && item2.GatherType.HasValue)
                                    {
                                        di_CabinetIndex_GatherType.Add(item2.CabinetIndex, item2.GatherType);
                                    }
                                    if (item.NotificationStatusID == (int)ShipmentNotificationStatusEnum.PassedCheck)
                                    {
                                        list_UploadFile_ShipmentNotification.AddRange(ConstsMethod.GetUploadFileList(item2.ID, UploadFileType.ShipmentNotification_DeliveryNotification));
                                    }

                                    if (item2.GatherType == (int)GatherTypeEnum.PullCabinet)
                                    {
                                        isPullCabinet = true;
                                    }
                                    else if (item2.GatherType == (int)GatherTypeEnum.PutInStorage)
                                    {
                                        isPutInStorage = true;
                                    }
                                }

                                #region 报关的发票号码和合同总金额

                                foreach (var item2 in item.Inspection_InspectionCustoms.Where(d => !d.IsDelete))
                                {
                                    if (item2.Inspection_InspectionCustomsDetail != null && item2.Inspection_InspectionCustomsDetail.Count > 0)
                                    {
                                        string InvoiceNO = item2.Inspection_InspectionCustomsDetail.First().InvoiceNO;
                                        if (!list_InvoiceNo.Contains(InvoiceNO) && !string.IsNullOrEmpty(InvoiceNO))
                                        {
                                            list_InvoiceNo.Add(InvoiceNO);
                                        }
                                    }
                                }

                                #endregion 报关的发票号码和合同总金额
                            }

                            if (query_ShipmentOrder.First().IsMerge)
                            {
                                #region 合并订舱时的拖柜费

                                foreach (var item in query_ShipmentOrder)
                                {
                                    foreach (var item2 in item.Delivery_ShipmentOrderCabinet.Where(d => !d.IsDelete))
                                    {
                                        if (di_CabinetIndex_GatherType.Count > 0 && di_CabinetIndex_GatherType[item2.CabinetIndex] == (int)GatherTypeEnum.PullCabinet)
                                        {
                                            if (!di_CabinetIndex_RegisterFees.Keys.Contains(item2.CabinetIndex) && item2.RegisterFees.HasValue && item.RegisterFeesStatusID == (int)ShipmentRegisterFeesStatusEnum.PassedCheck)
                                            {
                                                di_CabinetIndex_RegisterFees.Add(item2.CabinetIndex, item2.RegisterFees);
                                            }

                                            foreach (var item3 in item2.Delivery_ShipmentOrderProduct.Where(d => !d.IsDelete))
                                            {
                                                decimal? OuterLength = 0;
                                                decimal? OuterWidth = 0;
                                                decimal? OuterHeight = 0;

                                                #region 从出运明细的产品里面获取外箱长宽高、毛重、净重

                                                var Encasement_Product = context.Delivery_EncasementsProducts.Where(d => d.OrderProductID == item3.OrderProductID).FirstOrDefault();
                                                if (Encasement_Product != null)
                                                {
                                                    OuterLength = Encasement_Product.OuterLength;
                                                    OuterWidth = Encasement_Product.OuterWidth;
                                                    OuterHeight = Encasement_Product.OuterHeight;
                                                }

                                                #endregion 从出运明细的产品里面获取外箱长宽高、毛重、净重

                                                decimal? SumOuterVolume = CalculateHelper.GetSumOuterVolume(OuterLength, OuterWidth, OuterHeight, item3.BoxQty);
                                                if (!di_CabinetIndex_SumOuterVolume.Keys.Contains(item2.CabinetIndex))
                                                {
                                                    di_CabinetIndex_SumOuterVolume.Add(item2.CabinetIndex, SumOuterVolume);
                                                }
                                                else
                                                {
                                                    di_CabinetIndex_SumOuterVolume[item2.CabinetIndex] += SumOuterVolume;
                                                }

                                                var FactoryID = item3.OrderProduct.FactoryID;

                                                if (list_RegisterFee_Temp.Select(d => d.FactoryID).ToList().Contains(FactoryID ?? 0))
                                                {
                                                    list_RegisterFee_Temp.Where(d => d.FactoryID == FactoryID).First().SumOuterVolume += SumOuterVolume;
                                                }
                                                else
                                                {
                                                    list_RegisterFee_Temp.Add(new VMDocumentIndexing_RegisterFees()
                                                    {
                                                        CabinetIndex = item2.CabinetIndex,
                                                        FactoryID = FactoryID,
                                                        SumOuterVolume = SumOuterVolume,
                                                        FactoryAbbreviation = item3.OrderProduct.Factory.Abbreviation,
                                                    });
                                                }
                                            }
                                        }
                                    }
                                }

                                foreach (var item2 in list_RegisterFee_Temp)
                                {
                                    if (di_CabinetIndex_RegisterFees.Count > 0)
                                    {
                                        var FactoryID = item2.FactoryID;
                                        decimal? thisPrice = 0;
                                        if (di_CabinetIndex_SumOuterVolume[item2.CabinetIndex] * di_CabinetIndex_RegisterFees[item2.CabinetIndex] != 0)
                                        {
                                            thisPrice = item2.SumOuterVolume / di_CabinetIndex_SumOuterVolume[item2.CabinetIndex] * di_CabinetIndex_RegisterFees[item2.CabinetIndex];
                                        }
                                        thisPrice = thisPrice.Round(2);
                                        if (list_RegisterFee.Select(d => d.FactoryID).ToList().Contains(FactoryID ?? 0))
                                        {
                                            if (list_RegisterFee.Where(d => d.CabinetIndex == item2.CabinetIndex).Count() == 0)//同一个销售订单，不是同一批次
                                            {
                                                list_RegisterFee.Where(d => d.FactoryID == FactoryID).First().AllAmount += thisPrice;
                                            }
                                        }
                                        else
                                        {
                                            list_RegisterFee.Add(new VMDocumentIndexing_RegisterFees()
                                            {
                                                CabinetIndex = item2.CabinetIndex,
                                                FactoryID = FactoryID,
                                                AllAmount = thisPrice,
                                                FactoryAbbreviation = context.Factories.Where(d => d.ID == FactoryID).First().Abbreviation,
                                            });
                                        }
                                    }
                                }

                                #endregion 合并订舱时的拖柜费

                                #region 收款日期

                                var listShipmentOrderID = query_ShipmentOrder.Select(d => d.ID).ToList();
                                var query_InspectionExchange = context.Inspection_InspectionExchange.Where(d => listShipmentOrderID.Contains(dataFromDB.ShipmentOrderID ?? 0) && d.StatusID == (int)InspectionExchangeStatusEnum.PassedCheck);
                                if (query_InspectionExchange.Count() > 0)
                                {
                                    PaymentDate = query_InspectionExchange.Max(d => d.LargestSettlementDate);
                                }

                                #endregion 收款日期
                            }
                            else
                            {
                                #region 分批订舱的拖柜费

                                decimal? AllRegisterFees = 0;
                                decimal? AllSumOuterVolume = 0;

                                foreach (var item in query_ShipmentOrder)
                                {
                                    foreach (var item2 in item.Delivery_ShipmentOrderCabinet.Where(d => !d.IsDelete))
                                    {
                                        if (item2.GatherType == (int)GatherTypeEnum.PullCabinet)
                                        {
                                            foreach (var item3 in item2.Delivery_ShipmentOrderProduct.Where(d => !d.IsDelete))
                                            {
                                                decimal? OuterLength = 0;
                                                decimal? OuterWidth = 0;
                                                decimal? OuterHeight = 0;

                                                #region 从出运明细的产品里面获取外箱长宽高、毛重、净重

                                                var Encasement_Product = context.Delivery_EncasementsProducts.Where(d => d.OrderProductID == item3.OrderProductID).FirstOrDefault();
                                                if (Encasement_Product != null)
                                                {
                                                    OuterLength = Encasement_Product.OuterLength;
                                                    OuterWidth = Encasement_Product.OuterWidth;
                                                    OuterHeight = Encasement_Product.OuterHeight;
                                                }

                                                #endregion 从出运明细的产品里面获取外箱长宽高、毛重、净重

                                                decimal? SumOuterVolume = CalculateHelper.GetSumOuterVolume(OuterLength, OuterWidth, OuterHeight, item3.BoxQty);
                                                AllSumOuterVolume += SumOuterVolume;

                                                var FactoryID = item3.OrderProduct.FactoryID;

                                                if (item.RegisterFeesStatusID == (int)ShipmentRegisterFeesStatusEnum.PassedCheck)
                                                {
                                                    if (list_RegisterFee_Temp.Select(d => d.FactoryID).ToList().Contains(FactoryID ?? 0))
                                                    {
                                                        list_RegisterFee_Temp.Where(d => d.FactoryID == FactoryID).First().SumOuterVolume += SumOuterVolume;
                                                    }
                                                    else
                                                    {
                                                        list_RegisterFee_Temp.Add(new VMDocumentIndexing_RegisterFees()
                                                        {
                                                            CabinetIndex = item2.CabinetIndex,
                                                            FactoryID = FactoryID,
                                                            SumOuterVolume = SumOuterVolume,
                                                            FactoryAbbreviation = item3.OrderProduct.Factory.Abbreviation,
                                                        });
                                                    }
                                                }
                                            }
                                            AllRegisterFees += item2.RegisterFees;
                                        }
                                    }
                                }

                                foreach (var item2 in list_RegisterFee_Temp)
                                {
                                    var FactoryID = item2.FactoryID;
                                    decimal? thisPrice = 0;
                                    if (AllSumOuterVolume * AllRegisterFees != 0)
                                    {
                                        thisPrice = item2.SumOuterVolume / AllSumOuterVolume * AllRegisterFees;
                                        thisPrice = thisPrice.Round(2);
                                    }

                                    list_RegisterFee.Add(new VMDocumentIndexing_RegisterFees()
                                    {
                                        CabinetIndex = item2.CabinetIndex,
                                        FactoryID = FactoryID,
                                        AllAmount = thisPrice,
                                        FactoryAbbreviation = context.Factories.Where(d => d.ID == FactoryID).First().Abbreviation,
                                    });
                                }

                                #endregion 分批订舱的拖柜费

                                #region 收款日期

                                var query_InspectionExchange = context.Inspection_InspectionExchange.Where(d => d.OrderID == dataFromDB.OrderID);
                                if (query_InspectionExchange.Count() > 0)
                                {
                                    PaymentDate = query_InspectionExchange.Max(d => d.LargestSettlementDate);
                                }

                                #endregion 收款日期
                            }
                        }

                        #region 港杂费发票

                        if (query_Order.PortChargesInvoice_StatusID == (int)PortChargesInvoiceStatusEnum.PassedCheck)
                        {
                            DesignatedAgencyAmount = query_Order.DesignatedAgencyAmount ?? 0;
                            OurAgencyAmount = query_Order.OurAgencyAmount ?? 0;
                            list_UploadFile_PortChargesInvoice.AddRange(ConstsMethod.GetUploadFileList(query_Order.OrderID, UploadFileType.PortChargesInvoice));
                            if (list_UploadFile_PortChargesInvoice != null && list_UploadFile_PortChargesInvoice.Count > 0)
                            {
                                vm.IsPortChargesInvoice = true;
                            }
                        }

                        #endregion 港杂费发票

                        //单证索引信息
                        vm.CustomerCode = query_Order.Orders_Customers.CustomerCode;
                        vm.InvoiceNoList = CommonCode.ListToString(list_InvoiceNo.OrderBy(d => d).ToList());
                        vm.CustomsUnit = dataFromDB.CustomsUnit;
                        vm.ShippingDate = Utils.DateTimeToStr(dataFromDB.ShippingDate);
                        if (query_Order.Orders_Customers.PaymentType.HasValue)
                        {
                            vm.PaymentType = _dictionaryServices.GetDictionaryByName2(query_Order.Orders_Customers.PaymentType, list_Com_DataDictionary);
                        }

                        vm.PaymentDate = Utils.DateTimeToStr(PaymentDate);//收款日期 取最大的结汇日期
                        vm.IsGuaranteeShipments = dataFromDB.GuaranteeShipments;

                        //合约
                        vm.IsCustomsOrder = true;

                        //港杂费发票
                        vm.DesignatedAgencyAmount = DesignatedAgencyAmount;
                        vm.OurAgencyAmount = OurAgencyAmount;
                        vm.list_UploadFile_PortChargesInvoice = list_UploadFile_PortChargesInvoice;

                        //FCR
                        var query_InspectionClearance = context.Inspection_InspectionClearance.Where(d => d.Delivery_ShipmentOrder.OrderID == dataFromDB.Delivery_ShipmentOrder.OrderID && !d.IsDelete && d.StatusID == (int)InspectionClearanceStatusEnum.UploadedFCR);
                        if (dataFromDB.Delivery_ShipmentOrder.IsMerge)//合并订舱
                        {
                            query_InspectionClearance = context.Inspection_InspectionClearance.Where(d => d.Delivery_ShipmentOrder.OrderIDList == dataFromDB.Delivery_ShipmentOrder.OrderIDList && !d.IsDelete && d.StatusID == (int)InspectionClearanceStatusEnum.UploadedFCR);
                        }

                        List<VMUpLoadFile> list_UploadFile_FCRUploadFile = new List<VMUpLoadFile>();

                        if (query_InspectionClearance.Count() > 0)
                        {
                            foreach (var item in query_InspectionClearance)
                            {
                                list_UploadFile_FCRUploadFile.AddRange(ConstsMethod.GetUploadFileList(item.InspectionClearanceID, UploadFileType.InspectionClearance_FCR));
                            }
                        }

                        vm.list_UploadFile_FCRUploadFile = list_UploadFile_FCRUploadFile;
                        if (vm.list_UploadFile_FCRUploadFile != null && vm.list_UploadFile_FCRUploadFile.Count > 0)
                        {
                            vm.IsFCRUploadFile = true;
                        }
                        else
                        {
                            vm.IsFCRUploadFile = false;
                        }

                        //配仓单
                        vm.list_UploadFile_ShipmentNotification = list_UploadFile_ShipmentNotification;
                        if (vm.list_UploadFile_ShipmentNotification != null && vm.list_UploadFile_ShipmentNotification.Count > 0)
                        {
                            vm.IsShipmentNotification = true;
                        }
                        else
                        {
                            vm.IsShipmentNotification = false;
                        }

                        List<VMUpLoadFile> list_UploadFile_PurchaseContract = new List<VMUpLoadFile>();
                        List<VMUpLoadFile> list_UploadFile_PurchaseContract_MakeMoney = new List<VMUpLoadFile>();
                        List<VMDocumentIndexing_Purchase> list_Purchase = new List<VMDocumentIndexing_Purchase>();
                        List<VMDocumentIndexing_Outsourcing> list_Outsourcing = new List<VMDocumentIndexing_Outsourcing>();
                        List<VMDocumentIndexing_Inspection> list_Inspection = new List<VMDocumentIndexing_Inspection>();
                        List<VMDocumentIndexing_OtherFees> list_OtherFees = new List<VMDocumentIndexing_OtherFees>();
                        List<VMUpLoadFile> list_Upload_Order = new List<VMUpLoadFile>();
                        foreach (var item_OrderID in list_OrderIDList)
                        {
                            var query_Purchase = context.Purchase_Contract.Where(d => d.OrderID == item_OrderID && !d.IsDelete && d.ContractType == (int)ContractTypeEnum.Default);
                            foreach (var item in query_Purchase)
                            {
                                #region 工厂合同

                                string CurrentSign = _dictionaryServices.GetDictionary_CurrencySign(item.Purchase_ContractBatch.First().Purchase_ContractProduct.First().OrderProduct.CurrencyType, list_Com_DataDictionary);
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

                                DateTime? GatherEndDate = null;
                                DateTime? ShippingDateEnd = null;
                                DateTime? ActualPaymentTime = null;
                                foreach (var item2 in item.Purchase_ContractBatch)
                                {
                                    foreach (var item3 in item2.Purchase_ContractProduct)
                                    {
                                        var tempDate = item3.OrderProduct.Delivery_ShipmentOrderProduct.Where(d => !d.IsDelete).First().Delivery_ShipmentOrderCabinet.GatherEndDate;
                                        if (tempDate.HasValue)
                                        {
                                            GatherEndDate = tempDate;
                                        }

                                        tempDate = item3.OrderProduct.Delivery_ShipmentOrderProduct.Where(d => !d.IsDelete).First().Delivery_ShipmentOrderCabinet.ShippingDateEnd;
                                        if (tempDate.HasValue)
                                        {
                                            ShippingDateEnd = tempDate;
                                        }

                                        if (item3.OrderProduct.FinanceProducts != null && item3.OrderProduct.FinanceProducts.Count > 0)
                                        {
                                            tempDate = item3.OrderProduct.FinanceProducts.First().PaymentDate;
                                            if (tempDate.HasValue)
                                            {
                                                ActualPaymentTime = tempDate;
                                            }
                                        }
                                    }
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
                                    LatestTimePaymentFactory = GetSettlementPeriod(item.AfterDate, item.PaymentType, dataFromDB.ShippingDate, GatherEndDate, ShippingDateEnd, FCRReceiveDateEnd),
                                    ActualPaymentTime = Utils.DateTimeToStr(ActualPaymentTime),
                                    DocumentsIndexing_IsGuaranteeShipments = item.DocumentsIndexing_IsGuaranteeShipments.HasValue ? item.DocumentsIndexing_IsGuaranteeShipments.Value : false,
                                });

                                #endregion 工厂合同

                                list_OtherFees.Add(new VMDocumentIndexing_OtherFees()
                                {
                                    DocumentsIndexing_OtherFees = item.DocumentsIndexing_OtherFees,
                                    FactoryAbbreviation = item.Factory.Abbreviation,
                                    PurchaseContractID = item.ID,
                                });

                                #region 吊卡费、标签费

                                if (item.Purchase_OutContracts.Count > 0)
                                {
                                    if (item.Purchase_OutContracts.First().OutContractStatus >= (int)OutContractStatusEnum.PassedCheck)
                                    {
                                        decimal OutContractSum = item.Purchase_OutContracts.First().OutContractSum ?? 0;
                                        decimal OthersFee = item.Purchase_OutContracts.First().OthersFee;
                                        decimal AllAmount = OutContractSum + OthersFee;

                                        list_Outsourcing.Add(new VMDocumentIndexing_Outsourcing()
                                        {
                                            AllAmount = Keys.RMB_Sign + AllAmount,
                                            FactoryAbbreviation = item.Factory.Abbreviation,
                                            PurchaseContractID = item.ID,
                                        });
                                    }
                                }

                                #endregion 吊卡费、标签费

                                #region 第三方检测费

                                decimal? AllAmount_ThirdAudits = 0;
                                decimal? AllAmount_ThirdTest = 0;
                                decimal? AllAmount_ThirdVerification = 0;
                                decimal? AllAmount_ThirdSampling = 0;

                                var query_ThirdParty_Inspection = item.ThirdParty_Inspection.Where(d => !d.IsDelete);
                                if (query_ThirdParty_Inspection.Count() > 0)
                                {
                                    foreach (var item2 in query_ThirdParty_Inspection)
                                    {
                                        if (item2.StatusID == (int)InspectionStatusEnum.Passed)
                                        {
                                            switch ((InspectionTypeEnum)item2.TypeID)
                                            {
                                                case InspectionTypeEnum.AuditNotice:
                                                    AllAmount_ThirdAudits = item2.InspectionAuditFee_ForFactory;
                                                    break;

                                                case InspectionTypeEnum.DetectNotice:
                                                    AllAmount_ThirdTest = item2.InspectionDetectFee_ForFactory;
                                                    break;

                                                case InspectionTypeEnum.SamplingNotice:
                                                    AllAmount_ThirdSampling = item2.InspectionSamplingFee_ForFactory;
                                                    break;

                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                }
                                if (query_Order.Purchase_ThirdPartyVerification.Count > 0 && query_Order.Purchase_ThirdPartyVerification.First().StatusID == (int)ThirdPartyVerificationStatusEnum.HadUpload)
                                {
                                    AllAmount_ThirdVerification = query_Order.OrderProducts.Where(d => d.FactoryID == item.FactoryID).Sum(d => d.InspectionVerificationFee_ForFactory);
                                }

                                list_Inspection.Add(new VMDocumentIndexing_Inspection()
                                {
                                    AllAmount_ThirdAudits = Keys.RMB_Sign + AllAmount_ThirdAudits,
                                    AllAmount_ThirdTest = Keys.RMB_Sign + (AllAmount_ThirdTest ?? 0),
                                    AllAmount_ThirdVerification = Keys.RMB_Sign + AllAmount_ThirdVerification,
                                    AllAmount_ThirdSampling = Keys.RMB_Sign + AllAmount_ThirdSampling,
                                    FactoryAbbreviation = item.Factory.Abbreviation,
                                    PurchaseContractID = item.ID,
                                });

                                #endregion 第三方检测费
                            }

                            list_Upload_Order.AddRange(ConstsMethod.GetUploadFileList(item_OrderID, UploadFileType.Order));
                        }
                        vm.SelectCustomer = query_Order.Orders_Customers.SelectCustomer;

                        //如果核算单那里上传了附件，附件显示上传的核算单的附件；如果没上传附件，就显示报关SC的总金额和报关的SC附件
                        vm.IsUpload_Order = false;
                        if (list_Upload_Order != null && list_Upload_Order.Count > 0)
                        {
                            vm.list_Upload_Order = list_Upload_Order;
                            vm.IsUpload_Order = true;
                        }
                        else if (string.IsNullOrEmpty(vm.InvoiceNoList))
                        {
                            vm.IsCustomsOrder = false;
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

                        //出运方式
                        vm.list_RegisterFee = list_RegisterFee;
                        vm.IsPullCabinet = isPullCabinet;
                        vm.IsPutInStorage = isPutInStorage;

                        //吊卡费、标签费
                        vm.list_Outsourcing = list_Outsourcing;
                        if (vm.list_Outsourcing != null && vm.list_Outsourcing.Count > 0)
                        {
                            vm.IsOutsourcing = true;
                        }
                        else
                        {
                            vm.IsOutsourcing = false;
                        }

                        //第三方检验费
                        vm.list_Inspection = list_Inspection;
                        if (vm.list_Inspection != null && vm.list_Inspection.Count > 0)
                        {
                            vm.IsInspection = true;
                        }
                        else
                        {
                            vm.IsInspection = false;
                        }

                        //其他和备注
                        vm.IsOther = dataFromDB.IsOther;
                        vm.OtherFee = dataFromDB.OtherFee;
                        vm.Comment = dataFromDB.Comment;

                        vm.AllAmount_USD = AllAmount_USD;
                        vm.AllAmount_RMB = AllAmount_RMB;

                        vm.list_OtherFees = list_OtherFees;

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

        /// <summary>
        /// 获取结算期
        /// </summary>
        /// <param name="AfterDate"></param>
        /// <param name="PaymentType"></param>
        /// <param name="ShippingDate">实际船期</param>
        /// <param name="GatherEndDate">实际进仓日期</param>
        /// <param name="ShippingDateEnd">实际发货日期</param>
        /// <param name="FCRReceiveDateEnd">FCR收到日期</param>
        /// <returns></returns>
        public string GetSettlementPeriod(int AfterDate, string PaymentType, DateTime? ShippingDate, DateTime? GatherEndDate, DateTime? ShippingDateEnd, DateTime? FCRReceiveDateEnd)
        {
            #region 结算期

            //结算期
            //1、结关后30天付款 ===》实际船期 + 30天
            //2、进仓后30天付款 ===》实际进仓日期 + 30天（出运通知里需要有“进仓日期”的字段）
            //3、发货后30天付款 ===》实际发货日期 + 30天（出运通知里需要有“发货日期”的字段）
            //4、收到正本单据后14天付款 ===》FCR收到日期 + 14天（单证模块里需要有“FCR收到日期”的字段）

            string SettlementPeriod = "";

            switch ((PaymentTypeEnum)Utils.StrToInt(PaymentType, 0))
            {
                case PaymentTypeEnum.AfterClearance:

                    if (ShippingDate.HasValue)
                    {
                        SettlementPeriod = Utils.DateTimeToStr(ShippingDate.Value.AddDays(AfterDate));
                    }

                    break;

                case PaymentTypeEnum.AfterShipToStock:
                    if (GatherEndDate.HasValue)
                    {
                        SettlementPeriod = Utils.DateTimeToStr(GatherEndDate.Value.AddDays(AfterDate));
                    }
                    break;

                case PaymentTypeEnum.AfterDelivery:
                    if (ShippingDateEnd.HasValue)
                    {
                        SettlementPeriod = Utils.DateTimeToStr(ShippingDateEnd.Value.AddDays(AfterDate));
                    }
                    break;

                case PaymentTypeEnum.AfterReceivingTheOriginalDocuments:

                    if (FCRReceiveDateEnd.HasValue)
                    {
                        SettlementPeriod = Utils.DateTimeToStr(FCRReceiveDateEnd.Value.AddDays(AfterDate));
                    }

                    break;

                case PaymentTypeEnum.AfterReceivingTheOriginalDocumentsScannedCopy:
                    //TODO 待定
                    break;

                case PaymentTypeEnum.LC_SIGHT:
                    //TODO 待定
                    break;

                case PaymentTypeEnum.AfterReceiptOfTheOriginalFCR:
                    //TODO 待定
                    break;

                default:
                    break;
            }
            return SettlementPeriod;

            #endregion 结算期
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
                        if (dataFromDB.Delivery_ShipmentOrder.IsMerge)//合并订舱，按照订舱号
                        {
                            query = context.DocumentsIndexings.Where(d => d.Delivery_ShipmentOrder.OrderIDList == dataFromDB.Delivery_ShipmentOrder.OrderIDList);
                        }

                        foreach (var item in query)
                        {
                            vm.ID = item.ID;
                            Save2(currentUser, vm);
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

        public VMAjaxProcessResult Save2(VMERPUser currentUser, VMDocumentIndexing vm)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
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

                        foreach (var item2 in vm.list_OtherFees)
                        {
                            var query_Purchase = context.Purchase_Contract.Find(item2.PurchaseContractID);
                            if (query_Purchase != null)
                            {
                                query_Purchase.DocumentsIndexing_OtherFees = item2.DocumentsIndexing_OtherFees;
                            }
                        }

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

                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var item = context.DocumentsIndexings.Find(vm.ID);
                    if (item != null && item.StatusID == (int)DocumentsIndexingStatusEnum.PassedCheck)
                    {
                        item.DocumentsIndexingUploadDate = DateTime.Now; //单证索引上传日期=单证索引审核通过的日期

                        context.SaveChanges();
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

                            List<VMUpLoadFile> list_temp = ConstsMethod.GetUploadFileList(PurchaseID, UploadFileType.PurchaseContract);
                            if (list_temp.Count > 0)
                            {
                                foreach (var item in list_temp)
                                {
                                    ConstsMethod.GetUploadToPdfList(list_PdfFile, item.ServerFileName);
                                }
                            }

                            #endregion 合同
                        }
                        if (makerTypeEnum == MakerTypeEnum.DocumentsIndexing_AllDocuments)
                        {
                            #region 合同

                            //List<VMUpLoadFile> list_UploadFile_PurchaseContract = ConstsMethod.GetUploadFileList(PurchaseID, UploadFileType.PurchaseContract);
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

                            List<VMUpLoadFile> list_temp = ConstsMethod.GetUploadFileList(PurchaseID, UploadFileType.PurchaseContract_MakeMoney);
                            if (list_temp.Count > 0)
                            {
                                foreach (var item in list_temp)
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