using ERP.BLL.Consts;
using ERP.BLL.ERP.Consts;
using ERP.BLL.ERP.Dictionary;
using ERP.BLL.ERP.ShipmentOrder;
using ERP.BLL.Workflow;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.InspectionCustoms;
using ERP.Models.ShipmentOrder;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP.BLL.InspectionCustoms
{
    public class InspectionCustomsService
    {
        private DictionaryServices _dictionaryServices = new DictionaryServices();
        private ShipmentOrderService _shipmentOrderService = new ShipmentOrderService();

        public List<DTOInspectionCustoms> GetAll(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows, VMInspectionCustomsSearch vm_search)
        {
            List<DTOInspectionCustoms> listModel = null;
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Inspection_InspectionCustoms.Where(p => !p.IsDelete);

                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForInspectionCustoms);

                    switch (vm_search.PageType)
                    {
                        case PageTypeEnum.PendingApproval:
                            query = query.Where(d => d.StatusID != (short)InspectionCustomsStatusEnum.PassedCheck);
                            break;

                        case PageTypeEnum.PassedApproval:
                            query = query.Where(d => d.StatusID == (short)InspectionCustomsStatusEnum.PassedCheck);
                            break;

                        default:
                            break;
                    }

                    #region 筛选条件

                    if (!string.IsNullOrEmpty(vm_search.OrderNumber))
                    {
                        query = query.Where(p => p.Delivery_ShipmentOrder.Order.OrderNumber.Contains(vm_search.OrderNumber));
                    }

                    if (!string.IsNullOrEmpty(vm_search.CustomerCode))
                    {
                        query = query.Where(p => p.Delivery_ShipmentOrder.Order.Orders_Customers.CustomerCode == vm_search.CustomerCode);
                    }

                    if (!string.IsNullOrEmpty(vm_search.DeliveryDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.DeliveryDateStart);
                        query = query.Where(p => p.Delivery_ShipmentOrder.Order.OrderDateStart >= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.DeliveryDateEnd))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.DeliveryDateEnd);
                        query = query.Where(p => p.Delivery_ShipmentOrder.Order.OrderDateEnd <= dt);
                    }

                    if (!string.IsNullOrEmpty(vm_search.StatusID))
                    {
                        int i = Utils.StrToInt(vm_search.StatusID, 0);
                        query = query.Where(d => d.StatusID == i);
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
                        query = query.OrderByDescending(p => p.DT_MODIFYDATE);
                    }

                    #endregion 排序

                    totalRows = query.Count();//获取总条数

                    var dataFromDB = query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();//分页

                    #region 给Model赋值

                    if (dataFromDB != null && dataFromDB.Count > 0)
                    {
                        listModel = new List<DTOInspectionCustoms>();

                        foreach (var item in dataFromDB)
                        {
                            bool IsHasApprovalPermission = true;

                            if (item.Delivery_ShipmentOrder != null)
                            {
                                DAL.Order Order = item.Delivery_ShipmentOrder.Order;

                                #region 获取合并/分批订舱、合并订舱的订单编号

                                string OrderNumberList = "";
                                int tempOrderID = 0;
                                string tempOrderNumber = "";
                                string tempMerge = "";
                                List<string> listOrderNumberList = new List<string>();
                                var query_Delivery_ShipmentOrder = context.Delivery_ShipmentOrder.Where(d => d.ID == item.ShipmentOrderID).FirstOrDefault();
                                if (query_Delivery_ShipmentOrder != null)
                                {
                                    tempOrderID = Order.OrderID;
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

                                #region 判断是否需要我司报关

                                string IsNeedInspectionName = "";
                                if (item.IsNeedInspection.HasValue)
                                {
                                    IsNeedInspectionName = item.IsNeedInspection.Value ? "是" : "否";
                                }

                                #endregion 判断是否需要我司报关

                                List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());

                                #region 判断是否有审批流的权限

                                if (vm_search.PageType == PageTypeEnum.PendingApproval)//待审核
                                {
                                    IsHasApprovalPermission = ApprovalService.HasApprovalPermission(WorkflowTypes.ApprovalInspectionCustoms, currentUser, item.ST_CREATEUSER, item.ApproverIndex, Order.CustomerID);
                                }

                                #endregion 判断是否有审批流的权限

                                listModel.Add(new DTOInspectionCustoms()
                                {
                                    OrderNumber = Order.OrderNumber,
                                    CustomerCode = Order.Orders_Customers.CustomerCode,
                                    PortName = _dictionaryServices.GetDictionary_PortEnName(item.Delivery_ShipmentOrder.PortID, list_Com_DataDictionary),
                                    DestinationPortName = _dictionaryServices.GetDictionary_DestinationPortName(item.Delivery_ShipmentOrder.Delivery_ShipmentOrderCabinet.FirstOrDefault().ShipToPortID, list_Com_DataDictionary),
                                    OrderDateStartFormatter = Utils.DateTimeToStr(Order.OrderDateStart),
                                    OrderDateEndFormatter = Utils.DateTimeToStr(Order.OrderDateEnd),

                                    ID = item.InspectionCustomsID,
                                    ShipmentOrderID = item.ShipmentOrderID,
                                    Merge = tempMerge,
                                    OrderNumberList = OrderNumberList,
                                    StatusID = item.StatusID,
                                    StatusName = CommonCode.GetStatusEnumName(item.StatusID, typeof(InspectionCustomsStatusEnum)),
                                    DT_MODIFYDATEFormatter = Utils.DateTimeToStr2(item.DT_MODIFYDATE),
                                    ST_CREATEUSER = item.ST_CREATEUSER,
                                    ApproverIndex = item.ApproverIndex,
                                    CustomerID = Order.CustomerID,
                                    IsHasApprovalPermission = IsHasApprovalPermission,
                                    ShipmentOrderProductIDList = item.ShipmentOrderProductIDList,
                                    IsNeedInspectionName = IsNeedInspectionName,
                                });
                            }
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

        public List<VMUpLoadFile> GetUploadReceipt(VMERPUser currentUser, int id)
        {
            List<VMUpLoadFile> list_upload = new List<VMUpLoadFile>();
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {

                List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                List<DAL.HarmonizedSystem> list_HarmonizedSystem = context.HarmonizedSystems.Where(p => !p.IsDelete).ToList();

                List<string> list_ShippingDateStart = new List<string>();
                List<string> list_OrderDateStart = new List<string>();

                var query_InspectionCustoms = context.Inspection_InspectionCustoms.Find(id);
                if (query_InspectionCustoms != null)
                {
                    int ShipmentOrderID = query_InspectionCustoms.ShipmentOrderID;

                    var ShipmentOrderIDlist = _shipmentOrderService.GetShipmentOrderIDlist(currentUser, ShipmentOrderID);
                    var query_InspectionReceiptList = context.Inspection_InspectionReceiptList.Where(d => !d.IsDelete && ShipmentOrderIDlist.Contains(d.ShipmentOrderID ?? 0));
                    foreach (var item in query_InspectionReceiptList)
                    {
                        foreach (var item2 in item.Inspection_InspectionReceipt)
                        {
                            list_upload.AddRange(ConstsMethod.GetUploadFileList(item2.InspectionReceiptID, UploadFileType.InspectionReceiptUploadReceipt));
                        }
                    }
                }
            }
            return list_upload;
        }

        public List<VMInspectionCustoms> GetDetailByID(VMERPUser currentUser, int id)
        {
            List<VMInspectionCustoms> list_vm = new List<VMInspectionCustoms>();

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                    List<DAL.HarmonizedSystem> list_HarmonizedSystem = context.HarmonizedSystems.Where(p => !p.IsDelete).ToList();

                    List<string> list_ShippingDateStart = new List<string>();
                    List<string> list_OrderDateStart = new List<string>();

                    var query_InspectionCustoms = context.Inspection_InspectionCustoms.Find(id);
                    if (query_InspectionCustoms != null)
                    {
                        var query_InspectionCustomsDetail = query_InspectionCustoms.Inspection_InspectionCustomsDetail.Where(d => !d.IsDelete);
                        foreach (var item in query_InspectionCustomsDetail)
                        {
                            VMInspectionCustoms vm = new VMInspectionCustoms();

                            List<int> list_HSCode = new List<int>();
                            Dictionary<int, List<int>> di = new Dictionary<int, List<int>>();

                            List<string> list_InvoiceNO = new List<string>();
                            List<string> list_SCNo = new List<string>();
                            List<string> list_POID = new List<string>();
                            List<int> list_ContractID = new List<int>();

                            DAL.Order query_Order = null;
                            int ShipmentOrderID = 0;
                            int? PortID = 0;
                            int? DestinationPortID = 0;

                            string CallPeople = "";
                            string TerritoryOfOriginOfGoods = "";

                            List<VMShipmentOrderProduct> list_ShipmentOrderProduct = new List<VMShipmentOrderProduct>();
                            List<VMOrderProducts> list_OrderProducts = new List<VMOrderProducts>();

                            foreach (var item2 in item.Inspection_InspectionCustomsProduct.Where(d => !d.IsDelete))
                            {
                                string HsCode = "";
                                string HsEngName = "";

                                var query_HarmonizedSystem = list_HarmonizedSystem.Where(p => p.ID == item2.HSID && !p.IsDelete).FirstOrDefault();
                                if (query_HarmonizedSystem != null)
                                {
                                    HsCode = query_HarmonizedSystem.HSCode;
                                    HsEngName = query_HarmonizedSystem.CodeEngName;
                                }

                                int Qty = 0;
                                int ProductsBoxNum = 0;
                                decimal? SumOuterVolume = 0;
                                decimal SumOuterWeightGross = 0;
                                decimal SumOuterWeightNet = 0;

                                foreach (var item3 in item2.Inspection_InspectionCustomsProduct2.Where(d => !d.IsDelete))
                                {
                                    #region 获取相关的数据信息

                                    var query_ShipmentOrderProduct = item3.Delivery_ShipmentOrderProduct;

                                    int HSCode = query_ShipmentOrderProduct.OrderProduct.HSCode ?? 0;
                                    if (!list_HSCode.Contains(HSCode))
                                    {
                                        list_HSCode.Add(HSCode);

                                        query_Order = query_ShipmentOrderProduct.OrderProduct.Order;
                                        ShipmentOrderID = query_ShipmentOrderProduct.Delivery_ShipmentOrderCabinet.ShipmentOrderID ?? 0;

                                        PortID = query_ShipmentOrderProduct.Delivery_ShipmentOrderCabinet.Delivery_ShipmentOrder.PortID;
                                        DestinationPortID = query_ShipmentOrderProduct.Delivery_ShipmentOrderCabinet.ShipToPortID;
                                    }

                                    var InvoiceNo = query_ShipmentOrderProduct.InvoiceNo;
                                    if (!string.IsNullOrEmpty(InvoiceNo))
                                    {
                                        InvoiceNo = InvoiceNo.Substring(0, 7);
                                        if (!list_InvoiceNO.Contains(InvoiceNo))
                                        {
                                            list_InvoiceNO.Add(InvoiceNo);
                                        }
                                    }

                                    var SCNo = query_ShipmentOrderProduct.SCNo;
                                    if (!string.IsNullOrEmpty(SCNo))
                                    {
                                        SCNo = SCNo.Substring(0, 9);
                                        if (!list_SCNo.Contains(SCNo))
                                        {
                                            list_SCNo.Add(SCNo);
                                        }
                                    }

                                    var POID = query_ShipmentOrderProduct.OrderProduct.Order.POID;
                                    if (!list_POID.Contains(POID))
                                    {
                                        list_POID.Add(POID);
                                    }

                                    int ContractID = query_ShipmentOrderProduct.OrderProduct.Purchase_ContractProduct.Where(d => !d.IsDelete).FirstOrDefault().Purchase_ContractBatch.PurchaseContractID;
                                    if (!list_ContractID.Contains(ContractID))
                                    {
                                        list_ContractID.Add(ContractID);
                                    }

                                    #endregion 获取相关的数据信息

                                    DAL.OrderProduct query_OrderProduct = item3.Delivery_ShipmentOrderProduct.OrderProduct;

                                    CallPeople = query_OrderProduct.Factory.CallPeople;
                                    TerritoryOfOriginOfGoods = query_OrderProduct.Factory.Province + query_OrderProduct.Factory.CityArea;

                                    #region 产品信息

                                    #region 出运明细的产品信息

                                    decimal OuterLength = 0, OuterWidth = 0, OuterHeight = 0, OuterWeightGross = 0, OuterWeightNet = 0;

                                    var dbEncasementProductInfo = query_OrderProduct.Delivery_EncasementsProducts.FirstOrDefault();

                                    if (dbEncasementProductInfo != null)
                                    {
                                        OuterWeightGross = dbEncasementProductInfo.WeightGross ?? 0;//产品单箱毛重
                                        OuterWeightNet = dbEncasementProductInfo.WeightNet ?? 0;//产品单箱净重
                                        OuterLength = dbEncasementProductInfo.OuterLength ?? 0;
                                        OuterHeight = dbEncasementProductInfo.OuterHeight ?? 0;
                                        OuterWidth = dbEncasementProductInfo.OuterWidth ?? 0;
                                    }

                                    #endregion 出运明细的产品信息

                                    var ShippingDateStart = CommonCode.GetDateTime3(item3.Delivery_ShipmentOrderProduct.Delivery_ShipmentOrderCabinet.ShippingDateStart);
                                    if (!string.IsNullOrEmpty(ShippingDateStart) && !list_ShippingDateStart.Contains(ShippingDateStart))
                                    {
                                        list_ShippingDateStart.Add(ShippingDateStart);
                                    }

                                    var OrderDateStart = CommonCode.GetDateTime3(item3.Delivery_ShipmentOrderProduct.OrderProduct.Order.OrderDateStart);
                                    if (!string.IsNullOrEmpty(OrderDateStart) && !list_OrderDateStart.Contains(OrderDateStart))
                                    {
                                        list_OrderDateStart.Add(OrderDateStart);
                                    }

                                    var shipmentOrderProduct = new ShipmentOrderService().GetProductInfo(context, query_OrderProduct, item3.SelectBoxQty, item3.Qty, list_Com_DataDictionary, list_HarmonizedSystem);
                                    shipmentOrderProduct.ProductPrice = item2.ICUnitPrice;
                                    shipmentOrderProduct.SumPrice = item3.Qty ?? 0 * item2.ICUnitPrice;
                                    shipmentOrderProduct.HsEngName = HsEngName;

                                    #region 获取季节

                                    string SeasonPrefix = "";
                                    string SeasonSuffix = "";
                                    string SeasonName = "";
                                    string SeasonAlias = "";
                                    string SeasonDepartmentNumber = "";

                                    var query_TempSeason = list_Com_DataDictionary.Where(d => d.TableKind == (int)DictionaryTableKind.Season && d.Code == query_OrderProduct.Season);
                                    if (query_TempSeason != null && query_TempSeason.Count() > 0)
                                    {
                                        SeasonName = query_TempSeason.First().Name;
                                        SeasonAlias = query_TempSeason.First().Alias;
                                        SeasonDepartmentNumber = query_TempSeason.First().DepartmentNumber;

                                        if (!string.IsNullOrEmpty(SeasonName))
                                        {
                                            SeasonPrefix = SeasonName;
                                            SeasonSuffix = SeasonAlias;

                                            if (!string.IsNullOrEmpty(SeasonAlias))
                                            {
                                                SeasonName = SeasonName + " " + SeasonAlias;
                                            }
                                        }
                                    }

                                    shipmentOrderProduct.SeasonPrefix = SeasonPrefix;

                                    #endregion 获取季节

                                    shipmentOrderProduct.Desc = query_OrderProduct.Desc;
                                    shipmentOrderProduct.SalePrice = query_OrderProduct.SalePrice;

                                    #endregion 产品信息

                                    list_ShipmentOrderProduct.Add(shipmentOrderProduct);

                                    Qty += item3.Qty ?? 0;//数量
                                    ProductsBoxNum += item3.SelectBoxQty ?? 0;//箱数

                                    SumOuterWeightGross += Math.Round(OuterWeightGross * item3.SelectBoxQty ?? 0, 2);
                                    SumOuterWeightNet += Math.Round(OuterWeightNet * item3.SelectBoxQty ?? 0, 2);
                                    SumOuterVolume += CalculateHelper.GetSumOuterVolume(OuterLength, OuterWidth, OuterHeight, item3.SelectBoxQty);
                                }

                                #region 产品信息 HScode

                                VMOrderProducts vm_OrderProducts = new VMOrderProducts();

                                vm_OrderProducts.HsCode = HsCode;
                                vm_OrderProducts.HsEngName = HsEngName;

                                vm_OrderProducts.ID = item2.ID;
                                vm_OrderProducts.HSID = item2.HSID ?? 0;
                                vm_OrderProducts.HsName = item2.HSCodeName;

                                if (string.IsNullOrEmpty(item2.HSCodeEngName))
                                {
                                    if (item2.HSID.HasValue)
                                    {
                                        var query_HS = context.HarmonizedSystems.Find(item2.HSID);
                                        if (query_HS != null)
                                        {
                                            vm_OrderProducts.HsEngName = query_HS.CodeEngName;
                                        }
                                    }
                                }
                                else
                                {
                                    vm_OrderProducts.HsEngName = item2.HSCodeEngName;
                                }


                                vm_OrderProducts.ProductsBoxNum = item2.SelectBoxQty ?? 0;
                                vm_OrderProducts.ProductPrice = item2.ICUnitPrice;
                                vm_OrderProducts.Qty = item2.Qty ?? 0;
                                vm_OrderProducts.Amount = item2.ICUnitPrice * item2.Qty ?? 0;

                                vm_OrderProducts.SumOuterVolume = SumOuterVolume;
                                vm_OrderProducts.SumOuterWeightGross = SumOuterWeightGross;
                                vm_OrderProducts.SumOuterWeightNet = SumOuterWeightNet;

                                list_OrderProducts.Add(vm_OrderProducts);

                                #endregion 产品信息 HScode
                            }

                            #region 详情信息

                            vm.list_ShipmentOrderProduct = list_ShipmentOrderProduct;
                            vm.list_OrdersProducts = list_OrderProducts;
                            vm.list_InvoiceNO = list_InvoiceNO;
                            vm.list_SCNo = list_SCNo;

                            var query_Orders_Contacts = query_Order.Orders_Customers.Orders_Contacts.Where(d => d.IsDefault && !d.IsDelete);
                            if (query_Orders_Contacts.Count() > 0)
                            {
                                var query_this = query_Orders_Contacts.First();
                                vm.TheBuyer = query_this.FirstName + query_this.LastName;
                            }

                            vm.CallPeople = CallPeople;
                            vm.TerritoryOfOriginOfGoods = TerritoryOfOriginOfGoods;

                            //vm.ProductsBoxNum = ProductsBoxNum;
                            //vm.WeightGrossSum = SumOuterWeightGross;
                            //vm.WeightNetSum = SumOuterWeightNet;

                            var query_Orders_Customers = query_Order.Orders_Customers;

                            vm.SelectCustomer = query_Orders_Customers.SelectCustomer;

                            vm.CustomerCode = query_Orders_Customers.CustomerCode;
                            vm.ShipmentOrderID = ShipmentOrderID;
                            //发票
                            vm.CustomerName = query_Orders_Customers.CustomerName;

                            vm.CustomerStreet = query_Orders_Customers.StreetAddress;
                            vm.CustomerDate = query_Order.CustomerDate;
                            vm.CustomerDateFormatter = Utils.DateTimeToStr(query_Order.CustomerDate);
                            vm.OrderSaleDate = Utils.DateTimeToStr(query_Order.OrderDateStart);
                            vm.CustomerReg = query_Orders_Customers.City + ","
                                + query_Orders_Customers.Reg_Area.AreaName + ","
                                + query_Orders_Customers.PostalCode + ","
                                + query_Orders_Customers.Reg_Area.Reg_Country.CountryName;


                            //出运港和目的港从订舱里面取
                            vm.PortID = PortID;
                            vm.PortName = _dictionaryServices.GetDictionary_PortEnName(PortID, list_Com_DataDictionary);
                            vm.DestinationPortID = DestinationPortID;
                            vm.DestinationPortName = _dictionaryServices.GetDictionary_DestinationPortName(DestinationPortID, list_Com_DataDictionary);

                            #region 获取目的港的地址

                            var query_Com_DataDictionary = context.Com_DataDictionary.Where(d => d.Code == DestinationPortID).First();
                            CountryEnum countryEnum = CountryEnum.USA;
                            if (query_Com_DataDictionary.Country.HasValue)
                            {
                                countryEnum = (CountryEnum)query_Com_DataDictionary.Country.Value;
                            }
                            vm.DestinationPort_Country = EnumHelper.GetCustomEnumDesc(typeof(CountryEnum), countryEnum);
                            var query_Reg_Area = context.Reg_Area.Where(d => d.ARID == query_Com_DataDictionary.Province);
                            if (query_Reg_Area != null && query_Reg_Area.Count() > 0)
                            {
                                vm.DestinationPort_Province = query_Reg_Area.First().AreaName;
                            }

                            vm.DestinationPort_City = query_Com_DataDictionary.City;
                            vm.DestinationPort_StreetAddress = query_Com_DataDictionary.StreetAddress;
                            vm.DestinationPort_ZipCode = query_Com_DataDictionary.ZipCode;
                            vm.DestinationPort_CompanyName = query_Com_DataDictionary.CompanyName;
                            vm.DestinationPort_Address = vm.DestinationPort_City + "," + vm.DestinationPort_Province + " " + vm.DestinationPort_ZipCode + " ,US";

                            #endregion 获取目的港的地址

                            //装箱单
                            vm.POID = CommonCode.GetPOIDList(list_POID);

                            //报关单
                            vm.CountryName = query_Orders_Customers.Reg_Country.CountryName;

                            vm.InspectionCustomsDetailID = item.ID;
                            vm.InspectionCustomsID = item.InspectionCustomsID;
                            vm.list_DeclareElements = ConstsMethod.GetUploadFileList(item.ID, UploadFileType.InspectionCustomsDeclareElements);
                            vm.list_CustomsCommission = ConstsMethod.GetUploadFileList(item.ID, UploadFileType.InspectionCustomsCommission);

                            vm.TransportType = item.TransportType;
                            vm.TransportTypeName = CommonCode.GetStatusEnumName(item.TransportType, typeof(TransportTypeEnum));
                            vm.TradeType = item.TradeType;
                            vm.TradeTypeName = _dictionaryServices.GetDictionaryByName(item.TradeType, list_Com_DataDictionary);
                            vm.ExchangeType = item.ExchangeType;
                            vm.ExchangeTypeName = _dictionaryServices.GetDictionaryByName(vm.ExchangeType, list_Com_DataDictionary);
                            vm.SourceAreaWithin = item.SourceAreaWithin;
                            vm.TransactionType = item.TransactionType;
                            vm.SCNo = item.SalesContractNO;
                            vm.InvoiceNO = item.InvoiceNO;
                            if (item.InvoiceNO.Length == 8)
                            {
                                vm.Letter = item.InvoiceNO.Substring(7, 1);
                            }
                            if (item.SalesContractNO.Length == 10)
                            {
                                vm.Letter_ForSCNo = item.SalesContractNO.Substring(9, 1);
                            }
                            vm.TeamOfThePayment = _dictionaryServices.GetDictionaryByName2(query_Orders_Customers.PaymentType, list_Com_DataDictionary);
                            vm.SaleContractContent = item.SaleContractContent;

                            var query_history = context.Inspection_InspectionCustomsHis.Where(d => d.InspectionCustomsID == item.InspectionCustomsID);
                            List<VMInspectionCustomsHis> list_history = new List<VMInspectionCustomsHis>();
                            foreach (var item4 in query_history)
                            {
                                list_history.Add(new VMInspectionCustomsHis()
                                {
                                    AuditCreateDate = Utils.DateTimeToStr2(item4.CreateDate),
                                    AuditIdea = item4.AuditIdea,
                                    AuditUserName = item4.SystemUser.UserName,
                                    InspectionCustomsStatus = CommonCode.GetStatusEnumName(item4.InspectionCustomsStatus, typeof(InspectionCustomsStatusEnum))
                                });
                            };
                            vm.InspectionCustomsHis = list_history;

                            vm.CreateDate = item.DT_CREATEDATE;
                            vm.CreateDateFormatter = Utils.DateTimeToStr(item.DT_CREATEDATE);

                            //报检凭条

                            vm.list_UploadReceipts = ConstsMethod.GetUploadFileList(item.ID, UploadFileType.InspectionCustomsUploadReceipt);

                            #endregion 详情信息

                            list_vm.Add(vm);
                        }

                        foreach (var item in list_vm)
                        {
                            item.ShippingDateStart = CommonCode.ListToString(list_ShippingDateStart);
                            item.OrderDateStart = CommonCode.ListToString(list_OrderDateStart);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteError(e);
            }

            return list_vm;
        }

        public DBOperationStatus Save(VMERPUser currentUser, List<VMInspectionCustoms> list_vm)
        {
            DBOperationStatus result = default(DBOperationStatus);
            try
            {
                int affectRows = 0;
                int StatusID = list_vm.FirstOrDefault().InspectionCustomsStatusID;
                string AuditIdea = list_vm.FirstOrDefault().AuditIdea;

                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    DAL.Inspection_InspectionCustoms query = context.Inspection_InspectionCustoms.Find(list_vm.First().InspectionCustomsID);
                    if (query != null)
                    {
                        query.ST_MODIFYUSER = currentUser.UserID;
                        query.DT_MODIFYDATE = DateTime.Now;
                        query.IsDelete = false;
                        query.IPAddress = CommonCode.GetIP();

                        foreach (var item in list_vm)
                        {
                            #region 修改

                            ConstsMethod.SaveFileUpload(currentUser, item.InspectionCustomsDetailID, item.list_DeclareElements, context, UploadFileType.InspectionCustomsDeclareElements);
                            ConstsMethod.SaveFileUpload(currentUser, item.InspectionCustomsDetailID, item.list_CustomsCommission, context, UploadFileType.InspectionCustomsCommission);

                            if (StatusID != (int)InspectionCustomsStatusEnum.PassedCheck && StatusID != (int)InspectionCustomsStatusEnum.NotPassCheck)
                            {
                                query.StatusID = StatusID;
                            }

                            var query_InspectionCustomsDetail = context.Inspection_InspectionCustomsDetail.Find(item.InspectionCustomsDetailID);
                            if (query_InspectionCustomsDetail != null)
                            {
                                query_InspectionCustomsDetail.DT_MODIFYDATE = DateTime.Now;
                                query_InspectionCustomsDetail.ExchangeType = item.ExchangeType;
                                query_InspectionCustomsDetail.InvoiceNO = item.InvoiceNO.Substring(0, 7) + item.Letter;
                                query_InspectionCustomsDetail.SalesContractNO = item.SCNo.Substring(0, 9) + item.Letter_ForSCNo;
                                query_InspectionCustomsDetail.IPAddress = CommonCode.GetIP();
                                query_InspectionCustomsDetail.IsDelete = false;
                                query_InspectionCustomsDetail.SaleContractContent = item.SaleContractContent;
                                query_InspectionCustomsDetail.SourceAreaWithin = item.SourceAreaWithin;
                                query_InspectionCustomsDetail.ST_MODIFYUSER = currentUser.UserID;
                                query_InspectionCustomsDetail.TransactionType = item.TransactionType;
                                query_InspectionCustomsDetail.TransportType = item.TransportType;
                                query_InspectionCustomsDetail.TradeType = item.TradeType ?? 0;
                            }

                            foreach (var item2 in item.list_OrdersProducts)
                            {
                                var query_InspectionCustomsProduct = context.Inspection_InspectionCustomsProduct.Find(item2.ID);
                                if (query_InspectionCustomsProduct != null)
                                {
                                    query_InspectionCustomsProduct.DT_MODIFYDATE = DateTime.Now;
                                    query_InspectionCustomsProduct.ST_MODIFYUSER = currentUser.UserID;
                                    query_InspectionCustomsProduct.IPAddress = CommonCode.GetIP();
                                    query_InspectionCustomsProduct.IsDelete = false;

                                    query_InspectionCustomsProduct.HSCodeName = item2.HsName;
                                    query_InspectionCustomsProduct.HSCodeEngName = item2.HsEngName;
                                    query_InspectionCustomsProduct.ICUnitPrice = item2.ProductPrice;
                                }
                            }


                            #endregion 修改
                        }

                        Inspection_InspectionCustomsHis history = new Inspection_InspectionCustomsHis()
                        {
                            InspectionCustomsStatus = StatusID,
                            AuditUserID = currentUser.UserID,
                            AuditIdea = AuditIdea,
                            CreateDate = DateTime.Now,
                        };
                        query.Inspection_InspectionCustomsHis.Add(history);

                        affectRows = context.SaveChanges();

                        if (affectRows == 0)
                        {
                            result = DBOperationStatus.NoAffect;
                        }
                        else
                        {
                            bool bIsPass = true;
                            if (StatusID == (int)InspectionCustomsStatusEnum.NotPassCheck)
                            {
                                bIsPass = false;
                            }

                            List<int> listStatus = new List<int>();
                            listStatus.Add((int)InspectionCustomsStatusEnum.PendingCheck);
                            listStatus.Add((int)InspectionCustomsStatusEnum.NotPassCheck);
                            int[] iArrLogicStatus = { (int)InspectionCustomsStatusEnum.PendingCheck, (int)InspectionCustomsStatusEnum.NotPassCheck, (int)InspectionCustomsStatusEnum.PassedCheck };

                            ConstsMethod.InsertAuditStream(query.InspectionCustomsID, bIsPass, WorkflowTypes.ApprovalInspectionCustoms, currentUser.UserID, listStatus, iArrLogicStatus, AuditIdea);

                            result = DBOperationStatus.Success;

                            //SaveOther(currentUser, item.ShipmentOrderID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
                result = DBOperationStatus.NoAffect;
            }

            return result;
        }

        //private void SaveOther(VMERPUser currentUser, int ShipmentOrderID)
        //{
        //    using (ERPEntitiesNew context = new ERPEntitiesNew())
        //    {
        //        #region 该订舱的所有报关都审核通过了，添加清关列表的数据

        //        var query2 = context.Inspection_InspectionCustoms.Where(d => d.ShipmentOrderID == ShipmentOrderID && !d.IsDelete);
        //        var list = query2.Select(d => d.StatusID);
        //        if (list.Where(d => d == (int)InspectionCustomsStatusEnum.PassedCheck).Count() == list.Count())//如果该订舱的所有报关都审核通过了
        //        {
        //            var query_ShipmentOrder = context.Delivery_ShipmentOrder.Where(d => d.ID == ShipmentOrderID && !d.IsDelete);
        //            if (query_ShipmentOrder.First().IsMerge)
        //            {
        //                var OrderIDList = query_ShipmentOrder.First().OrderIDList;
        //                query_ShipmentOrder = context.Delivery_ShipmentOrder.Where(d => d.OrderIDList == OrderIDList && !d.IsDelete);
        //            }
        //            var list_ShipmentOrderID = query_ShipmentOrder.Select(d => d.ID);

        //            var query_InspectionClearance = context.Inspection_InspectionClearance.Where(d => list_ShipmentOrderID.Contains(d.ShipmentOrderID) && !d.IsDelete);
        //            if (query_InspectionClearance.Count() == 0)
        //            {
        //                DAL.Inspection_InspectionClearance query = new Inspection_InspectionClearance()
        //                {
        //                    ST_CREATEUSER = currentUser.UserID,
        //                    DT_CREATEDATE = DateTime.Now,
        //                    ST_MODIFYUSER = currentUser.UserID,
        //                    DT_MODIFYDATE = DateTime.Now,
        //                    IsDelete = false,
        //                    IPAddress = CommonCode.GetIP(),

        //                    ShipmentOrderID = ShipmentOrderID,
        //                    StatusID = (int)InspectionClearanceStatusEnum.PendingMaintenance,
        //                };

        //                context.Inspection_InspectionClearance.Add(query);
        //            }
        //            context.SaveChanges();
        //        }

        //        #endregion 该订舱的所有报关都审核通过了，添加清关列表的数据
        //    }
        //}

        public VMAjaxProcessResult CreateShippingMark(VMERPUser currentUser, int InspectionCustomsDetailID)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    List<string> filesAsolutePathList = new List<string>();
                    List<string> filesPhysicalPathList = new List<string>();
                    List<string> list_ShippingMark = new List<string>();

                    List<int?> list_ShipmentOrderProductID = new List<int?>();

                    List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());

                    var query = context.Inspection_InspectionCustomsDetail.Find(InspectionCustomsDetailID);
                    foreach (var item in query.Inspection_InspectionCustomsProduct.Where(d => !d.IsDelete))
                    {
                        foreach (var item2 in item.Inspection_InspectionCustomsProduct2.Where(d => !d.IsDelete))
                        {
                            if (item2.ShipmentOrderProductID.HasValue && !list_ShipmentOrderProductID.Contains(item2.ShipmentOrderProductID))
                            {
                                list_ShipmentOrderProductID.Add(item2.ShipmentOrderProductID);

                                var item_product = context.Delivery_ShipmentOrderProduct.Find(item2.ShipmentOrderProductID);
                                if (item_product != null)
                                {
                                    var r = ConstsMethod.CreateShippingMark(context, filesAsolutePathList, filesPhysicalPathList, list_Com_DataDictionary, item_product, list_ShippingMark);
                                    if (!r.IsSuccess)
                                    {
                                        return r;
                                    }
                                }
                            }
                        }
                    }



                    result.Msg = CommonCode.CreatePdfList(filesAsolutePathList, filesPhysicalPathList, list_ShippingMark);
                    result.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                LogHelper.WriteError(ex);
            }

            return result;
        }

        public List<string> DownLoad(VMERPUser currentUser, int id, ref List<string> list_InspectionCustomsID_InvoiceNo)
        {
            List<string> makeFileList = new List<string>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var vm_list = GetDetailByID(currentUser, id);
                    if (vm_list != null)
                    {
                        foreach (var vm in vm_list)
                        {
                            list_InspectionCustomsID_InvoiceNo.Add(vm.InspectionCustomsID + "_" + vm.InvoiceNO);

                            List<string> list_PdfFile = new List<string>();
                            MakerExcel_Documents.Maker(vm, MakerTypeEnum.InspectionCustoms_SaleContract, vm.SelectCustomer, vm.InspectionCustomsID.ToString(), list_PdfFile);
                            MakerExcel_Documents.Maker(vm, MakerTypeEnum.InspectionCustoms_CommercialInvoice, vm.SelectCustomer, vm.InspectionCustomsID.ToString(), list_PdfFile);

                            if (vm.list_CustomsCommission != null && vm.list_CustomsCommission.Count > 0)
                            {
                                foreach (var item in vm.list_CustomsCommission)
                                {
                                    ConstsMethod.GetUploadToPdfList(list_PdfFile, item.ServerFileName);
                                }
                            }

                            MakerExcel_Customs.CreateCustoms(vm, vm.SelectCustomer, list_PdfFile);//报关单

                            if (vm.list_DeclareElements != null && vm.list_DeclareElements.Count > 0)
                            {
                                foreach (var item in vm.list_DeclareElements)
                                {
                                    ConstsMethod.GetUploadToPdfList(list_PdfFile, item.ServerFileName);
                                }
                            }

                            if (vm.list_UploadReceipts != null && vm.list_UploadReceipts.Count > 0)
                            {
                                foreach (var item in vm.list_UploadReceipts)
                                {
                                    ConstsMethod.GetUploadToPdfList(list_PdfFile, item.ServerFileName);
                                }
                            }

                            MakerExcel_Documents.Maker(vm, MakerTypeEnum.InspectionCustoms_PackingList, vm.SelectCustomer, vm.InspectionCustomsID.ToString(), list_PdfFile);

                            makeFileList.Add(CommonCode.CreatePdfList(list_PdfFile, "InspectionReceipt", vm.InspectionCustomsID.ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return makeFileList;
        }


        public List<string> DownLoad_PDFAndExcel(VMERPUser currentUser, int ID)
        {
            List<string> list_InspectionCustomsID_InvoiceNo = new List<string>();
            DownLoad(currentUser, ID, ref list_InspectionCustomsID_InvoiceNo);
            List<string> makeFileList = new List<string>();

            foreach (var item in list_InspectionCustomsID_InvoiceNo)
            {
                string path = "/data/Template/Out/InspectionCustoms/" + item.Split('_')[0] + "/PDFAndExcel";
                string zipPath = "/data/Template/Out/InspectionCustoms/" + item.Split('_')[0] + "/" + item.Split('_')[1] + "_" + CommonCode.GetTimeStamp() + ".zip";
                ExcelHelper.Zip(Utils.GetMapPath(path), Utils.GetMapPath(zipPath));//生成压缩文件

                makeFileList.Add(zipPath);

            }

            return makeFileList;
        }


        /// <summary>
        /// 是否需要美金工厂报关/需要我司报关
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="shipmentOrderID"></param>
        /// <param name="IsNeedInspection"></param>
        /// <returns></returns>
        public DBOperationStatus IsNeedInspection(VMERPUser currentUser, int shipmentOrderID, bool IsNeedInspection)
        {
            DBOperationStatus result = default(DBOperationStatus);
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Inspection_InspectionCustoms.Where(d => d.ShipmentOrderID == shipmentOrderID && !d.IsDelete);
                    if (query != null && query.Count() > 0)
                    {
                        foreach (var item in query)
                        {
                            item.IsNeedInspection = IsNeedInspection;
                            item.ST_MODIFYUSER = currentUser.UserID;
                            item.DT_MODIFYDATE = DateTime.Now;
                        }

                        int affectRows = context.SaveChanges();
                        if (affectRows > 0)
                        {
                            result = DBOperationStatus.Success;
                        }
                        else
                        {
                            result = DBOperationStatus.Failed;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
                result = DBOperationStatus.NoAffect;
            }

            return result;
        }

        public VMAjaxProcessResult Add(VMERPUser currentUser, VMShipmentOrder vm)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            try
            {
                int affectRows = 0;
                if (vm.list_cabinet != null)
                {

                    foreach (var item in vm.list_cabinet)
                    {
                        using (ERPEntitiesNew context = new ERPEntitiesNew())
                        {
                            var query = context.Inspection_InspectionCustoms.Find(vm.InspectionCustomsID);
                            if (query != null)
                            {
                                query.ST_MODIFYUSER = currentUser.UserID;
                                query.DT_MODIFYDATE = DateTime.Now;
                                query.IPAddress = CommonCode.GetIP();
                                query.StatusID = (int)InspectionCustomsStatusEnum.OutLine;

                                #region 新建详情

                                DAL.Inspection_InspectionCustomsDetail query_InspectionCustomsDetail = new Inspection_InspectionCustomsDetail()
                                {
                                    DT_CREATEDATE = DateTime.Now,
                                    DT_MODIFYDATE = DateTime.Now,
                                    ExchangeType = 0,
                                    InvoiceNO = item.list_product.First().InvoiceNo,
                                    SalesContractNO = item.list_product.First().SCNo,

                                    IPAddress = CommonCode.GetIP(),
                                    IsDelete = false,
                                    SaleContractContent = CommonCode.GetInspectionComment(),
                                    SourceAreaWithin = "",
                                    ST_CREATEUSER = currentUser.UserID,
                                    ST_MODIFYUSER = currentUser.UserID,
                                    TransactionType = "",
                                    TradeType = 0,
                                    TransportType = 0,
                                };

                                query.Inspection_InspectionCustomsDetail.Add(query_InspectionCustomsDetail);

                                #endregion 新建详情

                                var ShipmentOrderIDlist = _shipmentOrderService.GetShipmentOrderIDlist(currentUser, query.ShipmentOrderID);

                                var temp = item.list_product.GroupBy(d => d.HSID);
                                foreach (var item_temp in temp)
                                {
                                    int? HSID = item_temp.First().HSID;
                                    string HSCodeName = item_temp.First().HSCodeName;
                                    string HSCodeEngName = item_temp.First().HSCodeEngName;
                                    decimal ICUnitPrice = 0;

                                    var query_InspectionReceipt = context.Inspection_InspectionReceipt.Where(d => d.HSID == HSID && ShipmentOrderIDlist.Contains(d.ShipmentOrderID ?? 0));
                                    if (query_InspectionReceipt != null)
                                    {
                                        if (query_InspectionReceipt.Count() > 0 && query_InspectionReceipt.First() != null)
                                        {
                                            HSCodeName = query_InspectionReceipt.First().HSCodeName;
                                            HSCodeEngName = query_InspectionReceipt.First().HSCodeEngName;
                                            ICUnitPrice = query_InspectionReceipt.First().INProductPrice;
                                        }
                                    }

                                    int? SelectQty = 0;
                                    int? Qty = 0;

                                    foreach (var item2 in item_temp)
                                    {
                                        SelectQty += item2.SelectBoxQty;
                                        Qty += item2.SelectQty;
                                    }
                                    #region 新建产品——根据HSCode分组

                                    DAL.Inspection_InspectionCustomsProduct query_InspectionCustomsProduct = new Inspection_InspectionCustomsProduct()
                                    {
                                        DT_CREATEDATE = DateTime.Now,
                                        DT_MODIFYDATE = DateTime.Now,
                                        IPAddress = CommonCode.GetIP(),
                                        IsDelete = false,
                                        ST_CREATEUSER = currentUser.UserID,
                                        ST_MODIFYUSER = currentUser.UserID,

                                        HSID = item_temp.First().HSID,
                                        HSCodeName = HSCodeName,
                                        HSCodeEngName = HSCodeEngName,
                                        ICUnitPrice = ICUnitPrice,
                                        Qty = Qty,
                                        SelectBoxQty = SelectQty,
                                    };
                                    query_InspectionCustomsDetail.Inspection_InspectionCustomsProduct.Add(query_InspectionCustomsProduct);

                                    #endregion 新建产品——根据HSCode分组


                                    foreach (var item2 in item_temp)
                                    {

                                        #region 新建产品

                                        DAL.Inspection_InspectionCustomsProduct2 query_InspectionCustomsProduct2 = new Inspection_InspectionCustomsProduct2();
                                        query_InspectionCustomsProduct2.ShipmentOrderProductID = item2.ID;
                                        query_InspectionCustomsProduct2.SelectBoxQty = item2.SelectBoxQty;
                                        query_InspectionCustomsProduct2.Qty = item2.SelectQty;
                                        query_InspectionCustomsProduct2.IsDelete = false;

                                        query_InspectionCustomsProduct2.DT_CREATEDATE = DateTime.Now;
                                        query_InspectionCustomsProduct2.ST_CREATEUSER = currentUser.UserID;
                                        query_InspectionCustomsProduct2.IPAddress = CommonCode.GetIP();
                                        query_InspectionCustomsProduct2.DT_MODIFYDATE = DateTime.Now;
                                        query_InspectionCustomsProduct2.ST_MODIFYUSER = currentUser.UserID;

                                        query_InspectionCustomsProduct.Inspection_InspectionCustomsProduct2.Add(query_InspectionCustomsProduct2);

                                        #endregion 新建产品
                                    }
                                }

                                affectRows += context.SaveChanges();

                                int InspectionCustomsDetailID = query_InspectionCustomsDetail.ID;

                                var list_UploadReceipt_ID = CommonCode.IdListToList(item.list_product.First().list_UploadReceipt_ID);
                                if (list_UploadReceipt_ID != null && list_UploadReceipt_ID.Count > 0)
                                {
                                    foreach (var item2 in list_UploadReceipt_ID)
                                    {
                                        var query_UpLoadFiles = context.UpLoadFiles.Find(item2);
                                        if (query_UpLoadFiles != null)
                                        {
                                            context.UpLoadFiles.Add(new UpLoadFile()
                                            {
                                                DisplayFileName = query_UpLoadFiles.DisplayFileName,
                                                ServerFileName = query_UpLoadFiles.ServerFileName,
                                                DT_CREATEDATE = DateTime.Now,
                                                DT_MODIFYDATE = DateTime.Now,
                                                ST_CREATEUSER = currentUser.UserID,
                                                ST_MODIFYUSER = currentUser.UserID,
                                                IPAddress = CommonCode.GetIP(),
                                                IsDelete = false,
                                                LinkID = InspectionCustomsDetailID,
                                                ModuleType = (short)UploadFileType.InspectionCustomsUploadReceipt,
                                            });

                                        }
                                    }
                                    affectRows += context.SaveChanges();
                                }


                            }
                        }
                    }
                }
                else
                {
                    using (ERPEntitiesNew context = new ERPEntitiesNew())
                    {

                        var query = context.Inspection_InspectionCustoms.Find(vm.InspectionCustomsID);
                        if (query != null)
                        {
                            query.ST_MODIFYUSER = currentUser.UserID;
                            query.DT_MODIFYDATE = DateTime.Now;
                            query.IPAddress = CommonCode.GetIP();
                            query.IsDelete = true;
                            query.IsShowNeedInspection = false;

                            affectRows = context.SaveChanges();
                        }
                    }
                }

                if (affectRows > 0)
                {
                    result.IsSuccess = true;
                }
                else
                {
                    result.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                LogHelper.WriteError(ex);
            }

            return result;
        }
    }
}