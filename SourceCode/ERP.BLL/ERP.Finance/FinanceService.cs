using ERP.BLL.ERP.Consts;
using ERP.BLL.Workflow;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.Finance;
using ERP.Models.Order;
using ERP.Models.Purchase;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.Tools.Logs;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP.BLL.ERP.Finance
{
    public class FinanceService
    {
        private ERP.Dictionary.DictionaryServices _dictionaryServices = new Dictionary.DictionaryServices();
        private ERP.Product.ProductServices _productServices = new Product.ProductServices();

        #region HelperMethod

        /// <summary>
        /// 获取状态描述的内容
        /// </summary>
        /// <param name="StatusID"></param>
        /// <returns></returns>
        private static string GetFinanceStatusEnum_Description(int StatusID)
        {
            return EnumHelper.GetCustomEnumDesc(typeof(FinanceStatusEnum), (FinanceStatusEnum)StatusID);
        }

        #endregion HelperMethod

        #region UserMethod

        /// <summary>
        /// 获取列表数据
        /// </summary>
        public List<DTOOrder> GetAll(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows,
          VMOrderSearch vm_search)
        {
            List<DTOOrder> listModel = null;
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var OrderIDList = context.Delivery_ShipmentOrder.Where(d => !d.IsDelete && d.StatusID == (int)ShipmentOrderStatusEnum.PassedCheck).Select(d => d.OrderID).ToList();

                    var query = context.Orders.Where(d => !d.IsDelete && OrderIDList.Contains(d.OrderID));
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForFinance);

                    switch (vm_search.PageType)
                    {
                        case PageTypeEnum.PendingCheckList:

                            break;

                        default:
                            break;
                    }

                    #region 筛选条件

                    if (!string.IsNullOrEmpty(vm_search.OrderNumber))
                    {
                        query = query.Where(d => d.OrderNumber.Contains(vm_search.OrderNumber));
                    }
                    if (!string.IsNullOrEmpty(vm_search.CustomerDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.CustomerDateStart);
                        query = query.Where(d => d.CustomerDate >= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.CustomerDateEnd))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.CustomerDateEnd);
                        query = query.Where(d => d.CustomerDate <= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.CustomerCode))
                    {
                        query = query.Where(d => d.Orders_Customers.CustomerCode == vm_search.CustomerCode);
                    }
                    if (!string.IsNullOrEmpty(vm_search.OrderDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.OrderDateStart);
                        query = query.Where(d => d.OrderDateStart >= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.OrderDateEnd))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.OrderDateEnd);
                        query = query.Where(d => d.OrderDateEnd <= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.POID))
                    {
                        query = query.Where(d => d.POID.Contains(vm_search.POID));
                    }
                    if (!string.IsNullOrEmpty(vm_search.OrderOrigin))
                    {
                        query = query.Where(d => d.OrderOrigin.Contains(vm_search.OrderOrigin));
                    }
                    if (!string.IsNullOrEmpty(vm_search.OrderStatusID))
                    {
                        int i = Utils.StrToInt(vm_search.OrderStatusID, 0);
                        query = query.Where(d => d.OrderStatusID == i);
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
                        query = query.OrderByDescending(d => d.Finance_DT_MODIFYDATE);
                    }

                    #endregion 排序

                    totalRows = query.Count();//获取总条数

                    var dataFromDB = query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();//分页

                    #region 给Model赋值

                    if (dataFromDB != null && dataFromDB.Count > 0)
                    {
                        listModel = new List<DTOOrder>();
                        foreach (var item in dataFromDB)
                        {
                            listModel.Add(new DTOOrder()
                            {
                                OrderID = item.OrderID,
                                OrderNumber = item.OrderNumber,
                                CustomerNo = item.Orders_Customers.CustomerCode,
                                POID = item.POID,
                                EHIPO = item.EHIPO,
                                CustomerDate = Utils.DateTimeToStr(item.CustomerDate),
                                OrderAmount = item.OrderAmount,
                                OrderRate = item.OrderRate,
                                OrderRate_En = item.OrderRate_En,
                                OrderDateStart = Utils.DateTimeToStr(item.OrderDateStart),
                                OrderDateEnd = Utils.DateTimeToStr(item.OrderDateEnd),
                                OrderOrigin = item.OrderOrigin,
                                Finance_StatusID = item.Finance_StatusID,
                                Finance_StatusName = GetFinanceStatusEnum_Description(item.Finance_StatusID),
                                Finance_DT_MODIFYDATEFormatter = Utils.DateTimeToStr2(item.Finance_DT_MODIFYDATE),
                                ST_CREATEUSER = item.ST_CREATEUSER,
                                ApproverIndex = item.ApproverIndex,
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
        /// 获取列表数据——工厂往来账
        /// </summary>
        public List<DTOPurchaseContract> GetAll_ForFactory(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows,
          VMOrderSearch vm_search)
        {
            List<DTOPurchaseContract> listModel = null;
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var OrderIDList = context.Delivery_ShipmentOrder.Where(d => !d.IsDelete && d.StatusID == (int)ShipmentOrderStatusEnum.PassedCheck).Select(d => d.OrderID).ToList();

                    var query = context.Orders.Where(d => !d.IsDelete && OrderIDList.Contains(d.OrderID));

                    switch (vm_search.PageType)
                    {
                        case PageTypeEnum.PendingCheckList:

                            break;

                        default:
                            break;
                    }

                    #region 筛选条件

                    if (!string.IsNullOrEmpty(vm_search.OrderNumber))
                    {
                        query = query.Where(d => d.OrderNumber.Contains(vm_search.OrderNumber));
                    }
                    if (!string.IsNullOrEmpty(vm_search.CustomerDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.CustomerDateStart);
                        query = query.Where(d => d.CustomerDate >= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.CustomerDateEnd))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.CustomerDateEnd);
                        query = query.Where(d => d.CustomerDate <= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.CustomerCode))
                    {
                        query = query.Where(d => d.Orders_Customers.CustomerCode == vm_search.CustomerCode);
                    }
                    if (!string.IsNullOrEmpty(vm_search.OrderDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.OrderDateStart);
                        query = query.Where(d => d.OrderDateStart >= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.OrderDateEnd))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.OrderDateEnd);
                        query = query.Where(d => d.OrderDateEnd <= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.POID))
                    {
                        query = query.Where(d => d.POID.Contains(vm_search.POID));
                    }
                    if (!string.IsNullOrEmpty(vm_search.OrderOrigin))
                    {
                        query = query.Where(d => d.OrderOrigin.Contains(vm_search.OrderOrigin));
                    }
                    if (!string.IsNullOrEmpty(vm_search.OrderStatusID))
                    {
                        int i = Utils.StrToInt(vm_search.OrderStatusID, 0);
                        query = query.Where(d => d.OrderStatusID == i);
                    }

                    #endregion 筛选条件

                    var OrderIDList2 = query.Select(d => d.OrderID).ToList();
                    var query_purchase = context.Purchase_Contract.Where(d => !d.IsDelete && OrderIDList2.Contains(d.OrderID ?? 0) && d.ContractType == (int)ContractTypeEnum.Default);

                    if (!string.IsNullOrEmpty(vm_search.FactoryAbbreviation))
                    {
                        query_purchase = query_purchase.Where(d => d.Factory.Abbreviation.Contains(vm_search.FactoryAbbreviation));
                    }
                    #region 排序

                    if (sortColumnsNames != null && sortColumnsNames.Count > 0
                        && sortColumnOrders != null && sortColumnOrders.Count > 0
                        && sortColumnsNames.Count == sortColumnOrders.Count)
                    {
                        query_purchase = query_purchase.OrderBy(sortColumnsNames, sortColumnOrders);
                    }
                    else
                    {
                        query_purchase = query_purchase.OrderBy(d => d.OrderID);
                    }

                    #endregion 排序

                    totalRows = query_purchase.Count();//获取总条数

                    var dataFromDB = query_purchase.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();//分页

                    #region 给Model赋值

                    if (dataFromDB != null && dataFromDB.Count > 0)
                    {
                        listModel = new List<DTOPurchaseContract>();
                        foreach (var item in dataFromDB)
                        {
                            var query_order = context.Orders.Where(d => d.OrderID == item.OrderID).FirstOrDefault();
                            string OrderNumber = "";
                            string POID = "";
                            string CustomerDate = "";
                            decimal OrderAmount = 0;
                            if (query_order != null)
                            {
                                OrderNumber = query_order.OrderNumber;
                                POID = query_order.POID;
                                CustomerDate = Utils.DateTimeToStr(query_order.CustomerDate);
                                OrderAmount = query_order.OrderAmount;
                            }

                            listModel.Add(new DTOPurchaseContract()
                            {
                                ID = item.ID,
                                OrderNumber = OrderNumber,
                                PurchaseNumber = item.PurchaseNumber,
                                CustomerCode = item.Orders_Customers.CustomerCode,
                                POID = POID,
                                CustomerDate = CustomerDate,
                                OrderAmount = OrderAmount,
                                FactoryAbbreviation = item.Factory.Abbreviation,
                                PurchaseDate = Utils.DateTimeToStr(item.PurchaseDate),
                                AllAmount = item.AllAmount,
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
        /// 获取列表数据
        /// </summary>
        public List<VMFinanceProduct> GetAll_ForOther(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows,
          VMOrderSearch vm_search)
        {
            List<VMFinanceProduct> listModel = null;
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var OrderIDList = context.Delivery_ShipmentOrder.Where(d => !d.IsDelete && d.StatusID == (int)ShipmentOrderStatusEnum.PassedCheck).Select(d => d.OrderID).ToList();

                    var query = context.Orders.Where(d => !d.IsDelete && OrderIDList.Contains(d.OrderID));

                    switch (vm_search.PageType)
                    {
                        case PageTypeEnum.PendingCheckList:

                            break;

                        default:
                            break;
                    }

                    #region 筛选条件

                    if (!string.IsNullOrEmpty(vm_search.OrderNumber))
                    {
                        query = query.Where(d => d.OrderNumber.Contains(vm_search.OrderNumber));
                    }
                    if (!string.IsNullOrEmpty(vm_search.CustomerDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.CustomerDateStart);
                        query = query.Where(d => d.CustomerDate >= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.CustomerDateEnd))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.CustomerDateEnd);
                        query = query.Where(d => d.CustomerDate <= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.CustomerCode))
                    {
                        query = query.Where(d => d.Orders_Customers.CustomerCode == vm_search.CustomerCode);
                    }
                    if (!string.IsNullOrEmpty(vm_search.POID))
                    {
                        query = query.Where(d => d.POID.Contains(vm_search.POID));
                    }


                    #endregion 筛选条件

                    var OrderIDList2 = query.Select(d => d.OrderID).ToList();
                    var query_OrderProduct = context.OrderProducts.Where(d => !d.IsDelete && OrderIDList2.Contains(d.OrderID));

                    #region 筛选条件

                    if (!string.IsNullOrEmpty(vm_search.FactoryAbbreviation))
                    {
                        query_OrderProduct = query_OrderProduct.Where(d => d.Factory.Abbreviation.Contains(vm_search.FactoryAbbreviation));
                    }
                    if (!string.IsNullOrEmpty(vm_search.PurchaseNumber))
                    {
                        query_OrderProduct = query_OrderProduct.Where(d => d.Purchase_ContractProduct.Where(dd => !dd.IsDelete).FirstOrDefault().Purchase_ContractBatch.Purchase_Contract.PurchaseNumber.Contains(vm_search.PurchaseNumber));
                    }
                    if (!string.IsNullOrEmpty(vm_search.No))
                    {
                        query_OrderProduct = query_OrderProduct.Where(d => d.No.Contains(vm_search.No));
                    }
                    if (!string.IsNullOrEmpty(vm_search.FactoryAbbreviation))
                    {
                        query_OrderProduct = query_OrderProduct.Where(d => d.Factory.Abbreviation.Contains(vm_search.FactoryAbbreviation));
                    }

                    if (!string.IsNullOrEmpty(vm_search.InvoiceNo))
                    {
                        query_OrderProduct = query_OrderProduct.Where(d => d.Delivery_ShipmentOrderProduct.Where(dd => !dd.IsDelete).FirstOrDefault().InvoiceNo.Contains(vm_search.InvoiceNo));
                    }

                    #endregion 筛选条件

                    #region 排序

                    if (sortColumnsNames != null && sortColumnsNames.Count > 0
                        && sortColumnOrders != null && sortColumnOrders.Count > 0
                        && sortColumnsNames.Count == sortColumnOrders.Count)
                    {
                        query_OrderProduct = query_OrderProduct.OrderBy(sortColumnsNames, sortColumnOrders);
                    }
                    else
                    {
                        query_OrderProduct = query_OrderProduct.OrderBy(d => d.OrderID);
                    }

                    #endregion 排序

                    totalRows = query_OrderProduct.Count();//获取总条数

                    var dataFromDB = query_OrderProduct.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();//分页

                    #region 给Model赋值

                    if (dataFromDB != null && dataFromDB.Count > 0)
                    {
                        List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                        listModel = new List<VMFinanceProduct>();
                        listModel.AddRange(GetFinanceProduct(context, dataFromDB, list_Com_DataDictionary));
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
        public VMOrderEdit GetDetailByID(VMERPUser currentUser, int OrderID)
        {
            VMOrderEdit vm = null;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Orders.Where(p => p.OrderID == OrderID && !p.IsDelete);

                    var dataFromDB = query.FirstOrDefault();
                    if (dataFromDB != null)
                    {
                        var query_OrderProduct = new List<DAL.OrderProduct>();
                        foreach (var item in dataFromDB.OrderProducts)
                        {
                            query_OrderProduct.Add(item);
                        }
                        return GetFinance(context, OrderID, query_OrderProduct);
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
        /// 获取详情
        /// </summary>
        public VMOrderEdit GetDetailByID_ForFactory(VMERPUser currentUser, int PurchaseID)
        {
            VMOrderEdit vm = null;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Purchase_Contract.Where(d => d.ID == PurchaseID && !d.IsDelete && d.ContractType == (int)ContractTypeEnum.Default);

                    var dataFromDB = query.FirstOrDefault();
                    if (dataFromDB != null)
                    {
                        var query_OrderProduct = new List<DAL.OrderProduct>();
                        foreach (var item in dataFromDB.Purchase_ContractBatch)
                        {
                            foreach (var item2 in item.Purchase_ContractProduct)
                            {
                                query_OrderProduct.Add(item2.OrderProduct);
                            }
                        }

                        return GetFinance(context, dataFromDB.OrderID ?? 0, query_OrderProduct);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return vm;
        }

        public VMOrderEdit GetFinance(ERPEntitiesNew context, int OrderID, List<DAL.OrderProduct> query_product)
        {
            List<DAL.Orders_Customers> list_Orders_Customers = context.Orders_Customers.Where(d => !d.IsDelete).ToList();
            List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
            List<VMFinanceProduct> list_FinanceProduct = GetFinanceProduct(context, query_product, list_Com_DataDictionary);

            var query_Order = context.Orders.Find(OrderID);
            return new VMOrderEdit()
            {
                OrderID = query_Order.OrderID,
                OrderNumber = query_Order.OrderNumber,
                POID = query_Order.POID,
                EHIPO = query_Order.EHIPO,
                PortID = query_Order.PortID,
                PortName = _dictionaryServices.GetDictionary_PortName(query_Order.PortID, list_Com_DataDictionary),
                DestinationPortID = query_Order.DestinationPortID,
                DestinationPortName = _dictionaryServices.GetDictionary_DestinationPortName(query_Order.DestinationPortID, list_Com_DataDictionary),
                CustomerID = query_Order.CustomerID,
                CustomerNo = list_Orders_Customers.Where(d => d.OCID == query_Order.CustomerID).FirstOrDefault() == null ? "" : list_Orders_Customers.Where(d => d.OCID == query_Order.CustomerID).FirstOrDefault().CustomerCode,
                CustomerDate = Utils.DateTimeToStr(query_Order.CustomerDate),
                OrderStatusID = query_Order.OrderStatusID,
                OrderAmount = query_Order.OrderAmount,
                OrderDateStart = Utils.DateTimeToStr(query_Order.OrderDateStart),
                OrderDateEnd = Utils.DateTimeToStr(query_Order.OrderDateEnd),
                OrderOrigin = query_Order.OrderOrigin,
                OrderStatusName = GetFinanceStatusEnum_Description(query_Order.OrderStatusID),
                Comment = query_Order.Comment,
                CheckSuggest = query_Order.CheckSuggest,
                IsThirdAudits = query_Order.IsThirdAudits,
                IsThirdVerification = query_Order.IsThirdVerification,
                IsThirdTest = query_Order.IsThirdTest,
                ShippingType = query_Order.ShippingType,
                CabinetRemark = query_Order.CabinetRemark,
                QuotID = query_Order.QuotID,
                CurrencyExchange = query_Order.CurrencyExchange,
                Additional = query_Order.Additional,
                Finance_StatusID = query_Order.Finance_StatusID,
                ST_CREATEUSER = query_Order.ST_CREATEUSER,
                ApproverIndex = query_Order.ApproverIndex,

                list_FinanceProduct = list_FinanceProduct,
            };
        }

        private List<VMFinanceProduct> GetFinanceProduct(ERPEntitiesNew context, List<OrderProduct> query_product, List<Com_DataDictionary> list_Com_DataDictionary)
        {
            List<VMFinanceProduct> list_FinanceProduct = new List<VMFinanceProduct>();

            decimal? AllPOAmount = query_product.Sum(d => d.SalePrice * d.Qty);

            foreach (var item_product in query_product.OrderBy(d => d.SortIndex))
            {
                string CurrencySign = _dictionaryServices.GetDictionary_CurrencySign(item_product.CurrencyType, list_Com_DataDictionary);//货币符号

                decimal? POPercent = item_product.SalePrice * item_product.Qty / AllPOAmount;

                var query_FinanceProduct = item_product.FinanceProducts.Where(d => d.ModuleType == 0).FirstOrDefault();

                VMFinanceProduct row = new VMFinanceProduct();

                var PurchaseIDList = context.Purchase_Contract.Where(d => !d.IsDelete && d.OrderID == item_product.OrderID && d.ContractType == (int)ContractTypeEnum.Default).Select(d => d.ID).ToList();

                #region 从第三方检验中获取数据

                var ThirdParty_Inspection = context.ThirdParty_Inspection.Where(d => !d.IsDelete && PurchaseIDList.Contains(d.PurchaseID));
                if (ThirdParty_Inspection.Count() > 0)
                {
                    #region 第三方验厂

                    decimal InspectionAuditFee = ThirdParty_Inspection.Sum(d => d.InspectionAuditFee);
                    decimal? InspectionAuditFee_ForFactory = ThirdParty_Inspection.Sum(d => d.InspectionAuditFee_ForFactory);

                    row.InspectionAuditFee = InspectionAuditFee * POPercent;
                    row.InspectionAuditFee_ForFactory = InspectionAuditFee_ForFactory * POPercent;

                    row.InspectionAuditFee = row.InspectionAuditFee.Round(2);
                    row.InspectionAuditFee_ForFactory = row.InspectionAuditFee_ForFactory.Round(2);

                    #endregion 第三方验厂

                    #region 第三方检测

                    //财务的检测费，直接取第三方检测的值，产品可能有多个相同的数据，用Sum汇总。
                    var list_ID = ThirdParty_Inspection.Where(d => d.TypeID == (int)InspectionTypeEnum.DetectNotice).Select(d => d.ID).ToList();
                    var queryTemp = context.ThirdParty_InspectionDetectNotice.Where(d => list_ID.Contains(d.InspectionID) && d.ProductID == item_product.ProductID && !d.IsDelete);

                    decimal? InspectionDetectFee = queryTemp.Sum(d => d.ProductDetectFee);
                    decimal? InspectionDetectFee_ForFactory = queryTemp.Sum(d => d.ProductDetectFee_ForFactory);

                    row.InspectionDetectFee = InspectionDetectFee;
                    row.InspectionDetectFee_ForFactory = InspectionDetectFee_ForFactory;


                    #endregion 第三方检测

                    #region 第三方抽检

                    decimal? InspectionSamplingFee = ThirdParty_Inspection.Sum(d => d.InspectionSamplingFee);
                    decimal? InspectionSamplingFee_ForFactory = ThirdParty_Inspection.Sum(d => d.InspectionSamplingFee_ForFactory);

                    row.InspectionSamplingFee = InspectionSamplingFee * POPercent;
                    row.InspectionSamplingFee_ForFactory = InspectionSamplingFee_ForFactory * POPercent;

                    row.InspectionSamplingFee = row.InspectionSamplingFee.Round(2);
                    row.InspectionSamplingFee_ForFactory = row.InspectionSamplingFee_ForFactory.Round(2);

                    #endregion 第三方抽检
                }

                #endregion 从第三方检验中获取数据

                #region 从第三方验货中获取数据

                var query_ThirdPartyVerification = context.Purchase_ThirdPartyVerification.Where(d => d.OrderID == item_product.OrderID).FirstOrDefault();
                if (query_ThirdPartyVerification != null)
                {
                    row.InspectionVerificationFee = query_ThirdPartyVerification.InspectionVerificationFee * POPercent;
                    row.InspectionVerificationFee_ForFactory = query_ThirdPartyVerification.InspectionVerificationFee_ForFactory * POPercent;

                    row.InspectionVerificationFee = row.InspectionVerificationFee.Round(2);
                    row.InspectionVerificationFee_ForFactory = row.InspectionVerificationFee_ForFactory.Round(2);
                }

                #endregion 从第三方验货中获取数据

                #region 从代购合同获取数据——待印及快递费

                var query_OutContracts = context.Purchase_OutContracts.Where(d => !(bool)d.IsDelete && PurchaseIDList.Contains(d.ContractsID));
                if (query_OutContracts != null)
                {
                    foreach (var item in query_OutContracts)
                    {
                        foreach (var item2 in item.Purchase_OutContractsPacks.Where(d => !d.IsDelete))
                        {
                            foreach (var item3 in item2.Purchase_OutContractProduct.Where(d => !d.IsDelete))
                            {
                                if (item3.OrderProductID == item_product.ID)
                                {
                                    row.AllOutContractAmount = (item.OutContractSum + item.OthersFee) * POPercent;
                                }
                            }
                        }
                    }
                }

                #endregion 从代购合同获取数据——待印及快递费

                #region 从订舱中获取数据——拖柜费(￥)、从报关中获取发票号、报关金额

                var query_ShipmentOrderProduct = item_product.Delivery_ShipmentOrderProduct.Where(d => !d.IsDelete).FirstOrDefault();
                if (query_ShipmentOrderProduct != null)
                {
                    row.RegisterFees = query_ShipmentOrderProduct.Delivery_ShipmentOrderCabinet.RegisterFees * POPercent;

                    var query_InspectionCustoms = context.Inspection_InspectionCustomsProduct2.Where(d => d.ShipmentOrderProductID == query_ShipmentOrderProduct.ID && !d.IsDelete);
                    if (query_InspectionCustoms.Count() > 0)
                    {
                        List<string> list_InvoiceNo = new List<string>();

                        decimal? CustomsAmount = 0;
                        foreach (var item in query_InspectionCustoms)
                        {
                            list_InvoiceNo.Add(item.Inspection_InspectionCustomsProduct.Inspection_InspectionCustomsDetail.InvoiceNO);
                            CustomsAmount += item.Inspection_InspectionCustomsProduct.Qty * item.Inspection_InspectionCustomsProduct.ICUnitPrice;
                        }
                        row.InvoiceNo = CommonCode.ListToString(list_InvoiceNo);
                        row.CustomsAmount = CustomsAmount.ToString();
                    }

                }

                #endregion 从订舱中获取数据——拖柜费(￥)、发票号

                #region 从采购合同中获取数据

                var query_PurchaseProduct = item_product.Purchase_ContractProduct.Where(d => !d.IsDelete).FirstOrDefault();
                if (query_PurchaseProduct != null)
                {
                    var query_PurchaseContract = query_PurchaseProduct.Purchase_ContractBatch.Purchase_Contract;

                    row.PurchaseNumber = query_PurchaseContract.PurchaseNumber;
                    row.PurchaseDate = query_PurchaseContract.PurchaseDate;
                    row.PurchaseDateFormatter = Utils.DateTimeToStr(query_PurchaseContract.PurchaseDate);
                    row.DateStart = query_PurchaseContract.DateStart;
                    row.DateStartFormatter = Utils.DateTimeToStr(query_PurchaseContract.DateStart);
                    row.AllAmount = query_PurchaseContract.AllAmount;

                    #region 结算期

                    //结算期
                    //1、结关后30天付款 ===》实际船期 + 30天
                    //2、进仓后30天付款 ===》实际进仓日期 + 30天（出运通知里需要有“进仓日期”的字段）
                    //3、发货后30天付款 ===》实际发货日期 + 30天（出运通知里需要有“发货日期”的字段）
                    //4、收到正本单据后14天付款 ===》FCR收到日期 + 14天（单证模块里需要有“FCR收到日期”的字段）

                    int AfterDate = query_PurchaseContract.AfterDate;
                    switch ((PaymentTypeEnum)Utils.StrToInt(query_PurchaseContract.PaymentType, 0))
                    {
                        case PaymentTypeEnum.AfterClearance:
                            if (query_FinanceProduct != null)
                            {
                                if (query_FinanceProduct.ShippingDate.HasValue)
                                {
                                    row.SettlementPeriod = Utils.DateTimeToStr(query_FinanceProduct.ShippingDate.Value.AddDays(AfterDate));
                                }
                            }
                            break;

                        case PaymentTypeEnum.AfterShipToStock:
                            var GatherEndDate = query_PurchaseProduct.OrderProduct.Delivery_ShipmentOrderProduct.Where(d => !d.IsDelete).First().Delivery_ShipmentOrderCabinet.GatherEndDate;
                            if (GatherEndDate.HasValue)
                            {
                                row.SettlementPeriod = Utils.DateTimeToStr(GatherEndDate.Value.AddDays(AfterDate));
                            }
                            break;

                        case PaymentTypeEnum.AfterDelivery:
                            var ShippingDateEnd = query_PurchaseProduct.OrderProduct.Delivery_ShipmentOrderProduct.Where(d => !d.IsDelete).First().Delivery_ShipmentOrderCabinet.ShippingDateEnd;
                            if (ShippingDateEnd.HasValue)
                            {
                                row.SettlementPeriod = Utils.DateTimeToStr(ShippingDateEnd.Value.AddDays(AfterDate));
                            }
                            break;

                        case PaymentTypeEnum.AfterReceivingTheOriginalDocuments:
                            var temp = context.Inspection_InspectionClearance.Where(d => d.Delivery_ShipmentOrder.OrderIDList.Contains(item_product.OrderID.ToString()) || d.Delivery_ShipmentOrder.OrderID == item_product.OrderID);

                            if (temp.Count() > 0)
                            {
                                if (temp.First().FCRReceiveDateEnd.HasValue)
                                {
                                    row.SettlementPeriod = Utils.DateTimeToStr(temp.First().FCRReceiveDateEnd.Value.AddDays(AfterDate));
                                }
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

                    #endregion 结算期

                    row.SettlementPeriod = "";
                }

                #endregion 从采购合同中获取数据

                #region 从出运明细中获取数据——立方数(cuft)

                var query_EncasementsProducts = item_product.Delivery_EncasementsProducts.FirstOrDefault();
                if (query_EncasementsProducts != null)
                {
                    //立方数CUFT=((外箱长×宽×高)/1000000) × 产品箱数，四舍五入
                    var BoxNumber = item_product.Qty / item_product.OuterBoxRate;//箱数
                    row.VolumeCUFT = query_EncasementsProducts.OuterLength * query_EncasementsProducts.OuterWidth * query_EncasementsProducts.OuterHeight / 1000000 * BoxNumber;
                }

                #endregion 从出运明细中获取数据——立方数(cuft)

                #region 从销售订单获取数据

                var this_Order = item_product.Order;
                row.OrderNumber = this_Order.OrderNumber;
                row.CustomerDate = this_Order.CustomerDate;
                row.CustomerCode = this_Order.Orders_Customers.CustomerCode;
                row.POID = this_Order.POID;

                #endregion 从销售订单获取数据

                #region 从销售订单的产品获取数据

                row.CurrencySign = CurrencySign;
                row.OrderProductID = item_product.ID;
                row.SortIndex = item_product.SortIndex;
                row.ProductID = item_product.ProductID;
                row.Image = item_product.Image;
                row.No = item_product.No;
                row.FactoryID = item_product.FactoryID;
                row.FactoryAbbreviation = item_product.Factory.Abbreviation;

                row.Name = item_product.Name;
                row.Qty = item_product.Qty;
                row.PriceFactory = item_product.PriceFactory;

                row.POPrice = item_product.SalePrice;
                row.POAmount = item_product.SalePrice * item_product.Qty;
                row.CommissionPercent = item_product.Commission;
                row.CommissionAmount = row.POAmount * row.CommissionPercent / 100m;
                row.FOBUSDAmount = row.POAmount - row.CommissionAmount;

                //报关金额
                row.HSCode = item_product.HSCode.HasValue ? context.HarmonizedSystems.Find(item_product.HSCode).HSCode : null;
                row.HSCodeName = item_product.HSCode.HasValue ? context.HarmonizedSystems.Find(item_product.HSCode).CodeName : null;

                if (item_product.Season.HasValue)
                {
                    row.SeasonName = _dictionaryServices.GetDictionary_AllSeasonName(item_product.Season, list_Com_DataDictionary);
                }

                #endregion 从销售订单的产品获取数据

                #region 从财务中获取数据

                //结算期
                if (query_FinanceProduct != null)
                {
                    row.ID = query_FinanceProduct.ID;
                    row.ShippingDate = query_FinanceProduct.ShippingDate;
                    row.ShippingDateFormatter = Utils.DateTimeToStr(query_FinanceProduct.ShippingDate);
                    row.FactoryDate = query_FinanceProduct.FactoryDate;
                    row.FactoryDateFormatter = Utils.DateTimeToStr(query_FinanceProduct.FactoryDate);

                    row.InvoicedAmount = query_FinanceProduct.InvoicedAmount;
                    row.PaymentDate = query_FinanceProduct.PaymentDate;
                    row.PaymentDateFormatter = Utils.DateTimeToStr(query_FinanceProduct.PaymentDate);
                    row.PaymentAmount = query_FinanceProduct.PaymentAmount;
                    row.CertifiedInvoiceDate = query_FinanceProduct.CertifiedInvoiceDate;
                    row.CertifiedInvoiceDateFormatter = Utils.DateTimeToStr(query_FinanceProduct.CertifiedInvoiceDate);

                    row.USDExchangeRate = query_FinanceProduct.USDExchangeRate;
                    row.FOBRMBAmount = row.FOBUSDAmount * row.USDExchangeRate;
                    row.ClearanceDate = query_FinanceProduct.ClearanceDate;
                    row.ClearanceDateFormatter = Utils.DateTimeToStr(query_FinanceProduct.ClearanceDate);
                    row.ClearanceAmount = query_FinanceProduct.ClearanceAmount;
                    row.BankFees = query_FinanceProduct.BankFees;

                    //出口发票号
                    row.CustomsNumber = query_FinanceProduct.CustomsNumber;
                    row.PreRecordedStatus = (int)PreRecordedStatusEnum.Empty;
                    if (query_FinanceProduct.PreRecordedStatus.HasValue)
                    {
                        if (query_FinanceProduct.PreRecordedStatus.Value)
                        {
                            row.PreRecordedStatus = (int)PreRecordedStatusEnum.Yes;
                        }
                        else
                        {
                            row.PreRecordedStatus = (int)PreRecordedStatusEnum.No;
                        }
                    }
                    row.OtherExpensesDeduction = query_FinanceProduct.OtherExpensesDeduction;
                    row.FactoryFees = query_FinanceProduct.FactoryFees;
                    row.PacksDetectFees = query_FinanceProduct.PacksDetectFees;
                    row.PortCharges = query_FinanceProduct.PortCharges;
                    row.InternationalCourierFees = query_FinanceProduct.InternationalCourierFees;
                    row.OtherFees = query_FinanceProduct.OtherFees;
                    row.CompanyManagementRate = query_FinanceProduct.CompanyManagementRate;
                    row.RefundRate = query_FinanceProduct.RefundRate;
                    row.Comment = query_FinanceProduct.Comment;
                    //需要计算的
                    row.CompanyManagementAmount = row.POAmount * row.USDExchangeRate * row.CompanyManagementRate;

                    row.AllAmount_CompanyManagement = row.PacksDetectFees.ToNumber() + row.InspectionAuditFee.ToNumber() + row.InspectionDetectFee.ToNumber() + row.InspectionSamplingFee.ToNumber() + row.InspectionVerificationFee.ToNumber() + row.PortCharges.ToNumber() + row.InternationalCourierFees.ToNumber() + row.OtherFees.ToNumber() + row.CompanyManagementAmount.ToNumber();

                    if (CurrencySign == Keys.USD_Sign)
                    {
                        row.RefundAmount = row.AllAmount * row.USDExchangeRate ?? 0 / 1.17m * row.RefundRate / 100;

                        //美元 毛利 = 结汇金额*美元汇率-合计我司成本(￥) + 退税金额(￥)-采购合同金额*美元汇率
                        row.GrossProfitAmount = row.ClearanceAmount * row.USDExchangeRate - row.AllAmount_CompanyManagement + row.RefundAmount - row.AllAmount * row.USDExchangeRate;
                    }
                    else
                    {
                        row.RefundAmount = row.AllAmount / 1.17m * row.RefundRate / 100;

                        //人民币 毛利 = 结汇金额*美元汇率-合计我司成本(￥) + 退税金额(￥)-采购合同金额
                        row.GrossProfitAmount = row.ClearanceAmount * row.USDExchangeRate - row.AllAmount_CompanyManagement + row.RefundAmount - row.AllAmount;
                    }
                    //毛利率(%)=毛利(￥) / (结汇金额（$）*美元汇率 - 合计我司成本(￥) +退税金额(￥) )
                    row.GrossProfitPercent = row.GrossProfitAmount / (row.ClearanceAmount * row.USDExchangeRate - row.AllAmount_CompanyManagement + row.RefundAmount) * 100;

                    row.FOBRMBAmount = row.FOBRMBAmount.Round(2);
                    row.CompanyManagementAmount = row.CompanyManagementAmount.Round(2);
                    row.AllAmount_CompanyManagement = row.AllAmount_CompanyManagement.Round(2);

                    row.RefundAmount = row.RefundAmount.Round(2);
                    row.GrossProfitAmount = row.GrossProfitAmount.Round(2);
                    row.GrossProfitPercent = row.GrossProfitPercent.Round(2);
                    row.IsMaintain = true;
                }
                else
                {
                    row.CompanyManagementRate = 10;
                    row.IsMaintain = false;
                }

                #endregion 从财务中获取数据

                //买手简称
                var query_Orders_Contacts = item_product.Order.Orders_Customers.Orders_Contacts.Where(d => d.IsDefault && !d.IsDelete);
                if (query_Orders_Contacts.Count() > 0)
                {
                    var query_this = query_Orders_Contacts.First();
                    row.BuyersAbbreviation = query_this.FirstName + query_this.LastName;
                }

                row.OrderDateStart = Utils.DateTimeToStr(item_product.Order.OrderDateStart);
                row.OrderDateEnd = Utils.DateTimeToStr(item_product.Order.OrderDateEnd);

                //单证索引上传日期
                var query_DocumentsIndexings = context.DocumentsIndexings.Where(d => d.OrderID == item_product.OrderID && d.StatusID == (int)DocumentsIndexingStatusEnum.PassedCheck);
                if (query_DocumentsIndexings.Count() > 0)
                {
                    row.DocumentIndexUpLoadDate = Utils.DateTimeToStr(query_DocumentsIndexings.First().DocumentsIndexingUploadDate);
                }

                row.AllAmount_Factory = row.AllOutContractAmount ?? 0 + row.InspectionAuditFee_ForFactory ?? 0 + row.InspectionDetectFee_ForFactory ?? 0 + row.InspectionSamplingFee_ForFactory ?? 0 + row.InspectionVerificationFee_ForFactory ?? 0 + row.RegisterFees ?? 0 + row.OtherExpensesDeduction ?? 0;

                row.POPrice = row.POPrice.Round(2);
                row.POAmount = row.POAmount.Round(2);
                row.CommissionAmount = row.CommissionAmount.Round(2);
                row.FOBUSDAmount = row.FOBUSDAmount.Round(2);
                row.VolumeCUFT = row.VolumeCUFT.Round(2);
                row.AllAmount_Factory = row.AllAmount_Factory.Round(2);
                row.AllOutContractAmount = row.AllOutContractAmount.Round(2);

                if (row.RegisterFees.HasValue)
                {
                    row.RegisterFees = row.RegisterFees.Round(2);
                }

                list_FinanceProduct.Add(row);
            }
            return list_FinanceProduct;
        }

        public VMAjaxProcessResult Save(VMERPUser currentUser, VMOrderEdit vm)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Orders.Find(vm.OrderID);

                    List<string> list_RoleNames = currentUser.RoleNames;
                    bool IsFinancialOfficer = list_RoleNames.Contains("财务主管");

                    foreach (var item in vm.list_FinanceProduct)
                    {
                        DAL.FinanceProduct financeProduct = new FinanceProduct();
                        if (query.Finance_StatusID != (int)FinanceStatusEnum.PendingMaintenance)
                        {
                            financeProduct = context.FinanceProducts.Find(item.ID);
                        }

                        financeProduct.OrderProductID = item.OrderProductID;
                        if (!string.IsNullOrEmpty(item.ShippingDateFormatter))
                        {
                            financeProduct.ShippingDate = Utils.StrToDateTime(item.ShippingDateFormatter);
                        }
                        if (!string.IsNullOrEmpty(item.FactoryDateFormatter))
                        {
                            financeProduct.FactoryDate = Utils.StrToDateTime(item.FactoryDateFormatter);
                        }
                        if (IsFinancialOfficer)//只有财务主管可以保存
                        {
                            financeProduct.InvoicedAmount = item.InvoicedAmount;

                            if (!string.IsNullOrEmpty(item.CertifiedInvoiceDateFormatter))
                            {
                                financeProduct.CertifiedInvoiceDate = Utils.StrToDateTime(item.CertifiedInvoiceDateFormatter);
                            }
                            financeProduct.CompanyManagementRate = item.CompanyManagementRate;
                        }
                        if (!string.IsNullOrEmpty(item.PaymentDateFormatter))
                        {
                            financeProduct.PaymentDate = Utils.StrToDateTime(item.PaymentDateFormatter);
                        }
                        financeProduct.PaymentAmount = item.PaymentAmount;
                        financeProduct.USDExchangeRate = item.USDExchangeRate;
                        if (!string.IsNullOrEmpty(item.ClearanceDateFormatter))
                        {
                            financeProduct.ClearanceDate = Utils.StrToDateTime(item.ClearanceDateFormatter);
                        }
                        financeProduct.ClearanceAmount = item.ClearanceAmount;
                        financeProduct.BankFees = item.BankFees;
                        financeProduct.CustomsNumber = item.CustomsNumber;

                        switch (item.PreRecordedStatus)
                        {
                            case (int)PreRecordedStatusEnum.Empty:
                                financeProduct.PreRecordedStatus = null;
                                break;

                            case (int)PreRecordedStatusEnum.Yes:
                                financeProduct.PreRecordedStatus = true;
                                break;

                            case (int)PreRecordedStatusEnum.No:
                                financeProduct.PreRecordedStatus = false;
                                break;

                            default:
                                break;
                        }
                        financeProduct.OtherExpensesDeduction = item.OtherExpensesDeduction;
                        financeProduct.FactoryFees = item.FactoryFees;
                        financeProduct.PacksDetectFees = item.PacksDetectFees;
                        financeProduct.PortCharges = item.PortCharges;
                        financeProduct.InternationalCourierFees = item.InternationalCourierFees;
                        financeProduct.OtherFees = item.OtherFees;
                        financeProduct.RefundRate = item.RefundRate;
                        financeProduct.Comment = item.Comment;

                        if (query.Finance_StatusID == (int)FinanceStatusEnum.PendingMaintenance)
                        {
                            context.FinanceProducts.Add(financeProduct);
                        }
                    }

                    query.Finance_StatusID = vm.Finance_StatusID;
                    query.Finance_DT_MODIFYDATE = DateTime.Now;
                    query.Finance_ST_MODIFYUSER = currentUser.UserID;

                    int affectRows = context.SaveChanges();
                    if (affectRows == 0)
                    {
                        result.IsSuccess = false;
                        result.Msg = "出错了！";
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
                result.IsSuccess = false;
                result.Msg = "出错了！";
            }
            return result;
        }

        #region 生成Excel

        /// <summary>
        /// 生成Excel
        /// </summary>
        /// <param name="list"></param>
        public string ExportExcel_Factory(VMERPUser currentUser, List<int> PurchaseIDList)
        {
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Purchase_Contract.Where(d => !d.IsDelete && PurchaseIDList.Contains(d.ID) && d.ContractType == (int)ContractTypeEnum.Default);
                    var query_OrderProduct = new List<DAL.OrderProduct>();
                    foreach (var item in query)
                    {
                        foreach (var item2 in item.Purchase_ContractBatch)
                        {
                            foreach (var item3 in item2.Purchase_ContractProduct)
                            {
                                query_OrderProduct.Add(item3.OrderProduct);
                            }
                        }
                    }

                    List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                    List<VMFinanceProduct> list_vm = GetFinanceProduct(context, query_OrderProduct, list_Com_DataDictionary);
                    return MakerExcel_Finance.BuildFinance_Factory(list_vm);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return null;
        }

        /// <summary>
        /// 生成Excel
        /// </summary>
        /// <param name="list"></param>
        public string ExportExcel_Analysis(VMERPUser currentUser, List<int> OrderProductIDList)
        {
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.OrderProducts.Where(d => !d.IsDelete && OrderProductIDList.Contains(d.ID));

                    List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                    List<VMFinanceProduct> list_vm = GetFinanceProduct(context, query.ToList(), list_Com_DataDictionary);
                    return MakerExcel_Finance.BuildFinance_Analysis(list_vm);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return null;
        }

        /// <summary>
        /// 生成Excel
        /// </summary>
        /// <param name="list"></param>
        public string ExportExcel_SelfExportList(VMERPUser currentUser, List<int> OrderProductIDList)
        {
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.OrderProducts.Where(d => !d.IsDelete && OrderProductIDList.Contains(d.ID));

                    List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                    List<VMFinanceProduct> list_vm = GetFinanceProduct(context, query.ToList(), list_Com_DataDictionary);
                    return MakerExcel_Finance.BuildFianace_SelfExportList(list_vm);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return null;
        }

        /// <summary>
        /// 生成Excel
        /// </summary>
        /// <param name="list"></param>
        public string ExportExcel_DetailList(VMERPUser currentUser, List<int> OrderProductIDList)
        {
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.OrderProducts.Where(d => !d.IsDelete && OrderProductIDList.Contains(d.ID));

                    List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                    List<VMFinanceProduct> list_vm = GetFinanceProduct(context, query.ToList(), list_Com_DataDictionary);
                    return MakerExcel_Finance.BuildFinance_DetailList(list_vm);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return null;
        }

        #endregion 生成Excel

        #endregion UserMethod
    }
}