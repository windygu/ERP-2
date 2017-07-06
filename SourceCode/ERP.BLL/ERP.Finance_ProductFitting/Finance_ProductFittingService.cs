using ERP.BLL.ERP.Consts;
using ERP.BLL.ERP.ProductFitting;
using ERP.BLL.Workflow;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.Finance;
using ERP.Models.Order;
using ERP.Models.ProductFitting;
using ERP.Models.Purchase;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.Tools.Logs;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP.BLL.ERP.Finance_ProductFitting
{
    public class Finance_ProductFittingService
    {
        private ERP.Dictionary.DictionaryServices _dictionaryServices = new Dictionary.DictionaryServices();
        private ERP.Product.ProductServices _productServices = new Product.ProductServices();

        private ProductFittingServices _productFittingServices = new ProductFittingServices();

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
                    var OrderIDList = context.Purchase_Contract.Where(d => !d.IsDelete && d.ContractType == (int)ContractTypeEnum.ProductFitting && d.PurchaseStatus >= (int)PurchaseStatusEnum.PassedCheck).Select(d => d.OrderID).ToList();

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
        /// 获取详情
        /// </summary>
        public VMOrderEdit GetDetailByID(VMERPUser currentUser, int OrderID)
        {
            VMOrderEdit vm = null;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Purchase_Contract.Where(d => d.OrderID == OrderID && !d.IsDelete && d.ContractType == (int)ContractTypeEnum.ProductFitting);
                    var list = new List<VMProductFittingInfo>();

                    foreach (var item in query)
                    {
                        foreach (var item2 in item.Purchase_ContractBatch)
                        {
                            var listProductFitting = _productFittingServices.GetProductFittingInfo(currentUser, item2.ID, (int)ModuleTypeEnum.PurchaseContract);
                            list.AddRange(listProductFitting);
                        }

                    }
                    return GetFinance(context, OrderID, list);
                }

            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return vm;
        }

        public VMOrderEdit GetFinance(ERPEntitiesNew context, int OrderID, List<VMProductFittingInfo> query_ProductFitting)
        {
            List<DAL.Orders_Customers> list_Orders_Customers = context.Orders_Customers.Where(d => !d.IsDelete).ToList();
            List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
            List<VMFinanceProduct> list_FinanceProduct = GetFinanceProduct(context, OrderID, query_ProductFitting, list_Com_DataDictionary);

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

        private List<VMFinanceProduct> GetFinanceProduct(ERPEntitiesNew context, int OrderID, List<VMProductFittingInfo> query_ProductFitting, List<Com_DataDictionary> list_Com_DataDictionary)
        {
            List<VMFinanceProduct> list_FinanceProduct = new List<VMFinanceProduct>();

            foreach (var item_product in query_ProductFitting)
            {
                var this_Order = context.Orders.Find(OrderID);

                string CurrencySign = _dictionaryServices.GetDictionary_CurrencySign(item_product.CurrencyType, list_Com_DataDictionary);//货币符号

                decimal? POPercent = 0;//TODO 没值

                var query_FinanceProduct = context.FinanceProducts.Where(d => d.ParentID == item_product.ID && d.ModuleType == 1).FirstOrDefault();

                VMFinanceProduct row = new VMFinanceProduct();
                row.ParentID = item_product.ID;
                row.RootID = item_product.RootID;

                var PurchaseIDList = context.Purchase_Contract.Where(d => !d.IsDelete && d.OrderID == OrderID && d.ContractType == (int)ContractTypeEnum.ProductFitting).Select(d => d.ID).ToList();

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

                    #region 第三方验货

                    decimal InspectionDetectFee = ThirdParty_Inspection.Sum(d => d.InspectionDetectFee);
                    decimal? InspectionDetectFee_ForFactory = ThirdParty_Inspection.Sum(d => d.InspectionDetectFee_ForFactory);

                    row.InspectionDetectFee = InspectionDetectFee * POPercent;
                    row.InspectionDetectFee_ForFactory = InspectionDetectFee_ForFactory * POPercent;

                    row.InspectionDetectFee = row.InspectionDetectFee.Round(2);
                    row.InspectionDetectFee_ForFactory = row.InspectionDetectFee_ForFactory.Round(2);

                    #endregion 第三方验货

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

                var query_ThirdPartyVerification = context.Purchase_ThirdPartyVerification.Where(d => d.OrderID == OrderID).FirstOrDefault();
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

                #region 从采购合同中获取数据
                var query_Purchase_ContractBatch = context.Purchase_ContractBatch.Where(d => d.ID == item_product.ParentID).FirstOrDefault();

                if (query_Purchase_ContractBatch != null)
                {
                    var query_PurchaseContract = query_Purchase_ContractBatch.Purchase_Contract;

                    row.PurchaseNumber = query_PurchaseContract.PurchaseNumber;
                    row.PurchaseDate = query_PurchaseContract.PurchaseDate;
                    row.PurchaseDateFormatter = Utils.DateTimeToStr(query_PurchaseContract.PurchaseDate);
                    row.DateStart = query_PurchaseContract.DateStart;
                    row.DateStartFormatter = Utils.DateTimeToStr(query_PurchaseContract.DateStart);
                    row.AllAmount = query_PurchaseContract.AllAmount;


                    row.SettlementPeriod = "";
                }

                #endregion 从采购合同中获取数据

                #region 从销售订单获取数据

                row.OrderNumber = this_Order.OrderNumber;
                row.CustomerDate = this_Order.CustomerDate;
                row.CustomerCode = this_Order.Orders_Customers.CustomerCode;
                row.POID = this_Order.POID;

                #endregion 从销售订单获取数据

                #region 从销售订单的产品获取数据

                row.CurrencySign = CurrencySign;
                row.OrderProductID = item_product.ID;
                row.ProductID = item_product.ProductID;
                row.Image = item_product.Image;
                row.No = item_product.No;
                row.FactoryID = item_product.FactoryID;
                row.FactoryAbbreviation = item_product.FactoryName;

                row.Name = item_product.Name;
                row.Qty = item_product.Qty ?? 0;
                row.PriceFactory = item_product.PriceFactory ?? 0;

                //row.POPrice = item_product.SalePrice;
                //row.POAmount = item_product.SalePrice * item_product.Qty;
                //row.CommissionPercent = item_product.Commission;
                //row.CommissionAmount = row.POAmount * row.CommissionPercent / 100m;
                //row.FOBUSDAmount = row.POAmount - row.CommissionAmount;

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

                    row.AllAmount_CompanyManagement = row.PacksDetectFees ?? 0 + row.InspectionAuditFee ?? 0 + row.InspectionDetectFee ?? 0 + row.InspectionSamplingFee ?? 0 + row.InspectionVerificationFee ?? 0 + row.PortCharges ?? 0 + row.InternationalCourierFees ?? 0 + row.OtherFees ?? 0 + row.CompanyManagementAmount ?? 0;

                    if (CurrencySign == Keys.USD_Sign)
                    {
                        row.RefundAmount = row.AllAmount * row.USDExchangeRate ?? 0 / 1.17m * row.RefundRate;
                        row.GrossProfitAmount = row.FOBRMBAmount - row.AllAmount_CompanyManagement + row.RefundAmount - row.AllAmount * row.USDExchangeRate;
                    }
                    else
                    {
                        row.RefundAmount = row.AllAmount / 1.17m * row.RefundRate;
                        row.GrossProfitAmount = row.FOBRMBAmount - row.AllAmount_CompanyManagement + row.RefundAmount - row.AllAmount;
                    }
                    row.GrossProfitPercent = row.GrossProfitAmount / (row.FOBRMBAmount - row.AllAmount_CompanyManagement + row.RefundAmount);

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
                var query_Orders_Contacts = this_Order.Orders_Customers.Orders_Contacts.Where(d => d.IsDefault && !d.IsDelete);
                if (query_Orders_Contacts.Count() > 0)
                {
                    var query_this = query_Orders_Contacts.First();
                    row.BuyersAbbreviation = query_this.FirstName + query_this.LastName;
                }

                row.OrderDateStart = Utils.DateTimeToStr(this_Order.OrderDateStart);
                row.OrderDateEnd = Utils.DateTimeToStr(this_Order.OrderDateEnd);

                //单证索引上传日期
                var query_DocumentsIndexings = context.DocumentsIndexings.Where(d => d.OrderID == OrderID && d.StatusID == (int)DocumentsIndexingStatusEnum.PassedCheck);
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

                        financeProduct.ModuleType = 1;
                        financeProduct.ParentID = item.ParentID;

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

        #endregion UserMethod
    }
}