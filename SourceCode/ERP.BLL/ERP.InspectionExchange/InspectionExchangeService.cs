using ERP.BLL.Consts;
using ERP.BLL.ERP.Consts;
using ERP.BLL.ERP.Dictionary;
using ERP.BLL.Workflow;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.Factory;
using ERP.Models.InspectionExchange;
using ERP.Models.Order;
using ERP.Models.Product;
using ERP.Tools;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ERP.BLL.InspectionExchange
{
    public class InspectionExchangeService
    {
        private DictionaryServices _dictionaryServices = new DictionaryServices();

        public List<DTOInspectionExchange> GetAll(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows, VMInspectionExchangeSearch vm_search)
        {
            List<DTOInspectionExchange> listModel = null;
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Inspection_InspectionExchange.Where(p => !p.IsDelete);

                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForInspectionExchange);

                    switch (vm_search.PageType)
                    {
                        case PageTypeEnum.PendingApproval:
                            query = query.Where(d => d.StatusID != (short)InspectionExchangeStatusEnum.PassedCheck);
                            break;

                        case PageTypeEnum.PassedApproval:
                            query = query.Where(d => d.StatusID == (short)InspectionExchangeStatusEnum.PassedCheck);
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
                        listModel = new List<DTOInspectionExchange>();

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
                                    tempOrderID = item.OrderID ?? 0;
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

                                List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());

                                #region 判断是否有审批流的权限

                                if (vm_search.PageType == PageTypeEnum.PendingApproval)//待审核
                                {
                                    IsHasApprovalPermission = ApprovalService.HasApprovalPermission(WorkflowTypes.ApprovalInspectionExchange, currentUser, item.ST_CREATEUSER, item.ApproverIndex, Order.CustomerID);
                                }

                                #endregion 判断是否有审批流的权限

                                bool IsCheck = true;
                                if (item.Delivery_ShipmentOrder.IsMerge && !item.IsCheck.HasValue && item.StatusID == (int)InspectionExchangeStatusEnum.PendingMaintenance)
                                {
                                    IsCheck = false;
                                }
                                string CheckName = "";
                                if (item.IsCheck.HasValue)
                                {
                                    CheckName = item.IsCheck.Value ? "订单号" : "订舱号";
                                }

                                listModel.Add(new DTOInspectionExchange()
                                {
                                    OrderNumber = context.Orders.Find(item.OrderID).OrderNumber,
                                    CustomerCode = Order.Orders_Customers.CustomerCode,
                                    PortName = _dictionaryServices.GetDictionary_PortEnName(item.Delivery_ShipmentOrder.PortID, list_Com_DataDictionary),
                                    DestinationPortName = _dictionaryServices.GetDictionary_DestinationPortName(item.Delivery_ShipmentOrder.Delivery_ShipmentOrderCabinet.FirstOrDefault().ShipToPortID, list_Com_DataDictionary),
                                    OrderDateStartFormatter = Utils.DateTimeToStr(Order.OrderDateStart),
                                    OrderDateEndFormatter = Utils.DateTimeToStr(Order.OrderDateEnd),

                                    InspectionExchangeID = item.InspectionExchangeID,
                                    ShipmentOrderID = item.ShipmentOrderID,
                                    Merge = tempMerge,
                                    OrderNumberList = OrderNumberList,
                                    StatusID = item.StatusID,
                                    StatusName = CommonCode.GetStatusEnumName(item.StatusID, typeof(InspectionExchangeStatusEnum)),
                                    DT_MODIFYDATEFormatter = Utils.DateTimeToStr2(item.DT_MODIFYDATE),
                                    ST_CREATEUSER = item.ST_CREATEUSER,
                                    ApproverIndex = item.ApproverIndex,
                                    CustomerID = Order.CustomerID,
                                    IsHasApprovalPermission = IsHasApprovalPermission,
                                    IsCheck = IsCheck,
                                    CheckName = CheckName,
                                    SelectCustomer = Order.Orders_Customers.SelectCustomer,
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

        public VMInspectionExchange GetDetailByID(VMERPUser currentUser, int id)
        {
            VMInspectionExchange vm = new VMInspectionExchange();

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Inspection_InspectionExchange.Find(id);
                    if (query != null)
                    {
                        int ShipmentOrderID = query.ShipmentOrderID;

                        var List_HarmonizedSystems = context.HarmonizedSystems.Where(p => !p.IsDelete).ToList();
                        List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());

                        var query_ShipmentOrder = context.Delivery_ShipmentOrder.Where(d => d.ID == ShipmentOrderID && !d.IsDelete);
                        if (query_ShipmentOrder.First().IsMerge)
                        {
                            var OrderIDList = query_ShipmentOrder.First().OrderIDList;
                            query_ShipmentOrder = context.Delivery_ShipmentOrder.Where(d => d.OrderIDList == OrderIDList && !d.IsDelete);
                        }

                        List<string> list_InvoiceNO = new List<string>();
                        List<string> list_SCNo = new List<string>();
                        List<string> list_POID = new List<string>();
                        List<string> list_PO2ID = new List<string>();
                        List<string> list_CabinetNumber = new List<string>();
                        List<string> list_OceanVessel = new List<string>();
                        List<string> list_CabinetNumber_Cabinet_SealingNumber = new List<string>();//订舱的柜号/柜型/封号,箱号
                        List<string> list_BoxNumber_CabinetNumber_SealingNumber = new List<string>();//订舱的箱号/柜号/封号

                        List<VMOrderProduct> list_OrderProduct_PackingList = new List<VMOrderProduct>();
                        List<VMOrderProduct> list_OrderProduct_Invoice = new List<VMOrderProduct>();//发票上同一个PO下的同一个货号不需根据分柜情况显示多行，1个货号只需显示一行即可。

                        List<DTOFactory> list_Factory = new List<DTOFactory>();
                        string FactoryIDList = query.FactoryIDList;
                        if (!string.IsNullOrEmpty(FactoryIDList))
                        {
                            List<int> list_FactoryID = CommonCode.IdListToList(FactoryIDList);
                            foreach (var item in list_FactoryID)
                            {
                                var query_Factory_temp = context.Factories.Find(item);
                                if (query_Factory_temp != null)
                                {
                                    DTOFactory vm_Factory = new DTOFactory();
                                    vm_Factory.EnglishName = query_Factory_temp.EnglishName;
                                    vm_Factory.EnglishAddress = query_Factory_temp.EnglishAddress;
                                    list_Factory.Add(vm_Factory);
                                }
                            }
                        }

                        string SelectCustomer = query.Delivery_ShipmentOrder.Order.Orders_Customers.SelectCustomer;
                        List<string> list_OrderProductID = new List<string>();

                        foreach (var item in query_ShipmentOrder)
                        {
                            foreach (var item2 in item.Delivery_ShipmentOrderCabinet.Where(d => !d.IsDelete))
                            {
                                //出运港和目的港从订舱里面取
                                vm.PortID = item.PortID;
                                vm.PortName = _dictionaryServices.GetDictionary_PortEnName(item.PortID, list_Com_DataDictionary);
                                vm.DestinationPortID = item2.ShipToPortID;
                                vm.DestinationPortName = _dictionaryServices.GetDictionary_DestinationPortName(item2.ShipToPortID, list_Com_DataDictionary);

                                if (!string.IsNullOrEmpty(item2.CabinetNumber))
                                {
                                    if (!list_CabinetNumber.Contains(item2.CabinetNumber))
                                    {
                                        list_CabinetNumber.Add(item2.CabinetNumber);
                                    }
                                }

                                if (!string.IsNullOrEmpty(item2.OceanVessel))
                                {
                                    if (!list_OceanVessel.Contains(item2.OceanVessel))
                                    {
                                        list_OceanVessel.Add(item2.OceanVessel);
                                    }
                                }

                                if (item2.Shipment_CabinetID.HasValue)
                                {
                                    string CabinetNumber_Cabinet_SealingNumber = item2.CabinetNumber + " / " + item2.Shipment_Cabinet.Name + " / " + item2.SealingNumber;
                                    if (!list_CabinetNumber_Cabinet_SealingNumber.Contains(CabinetNumber_Cabinet_SealingNumber))
                                    {
                                        list_CabinetNumber_Cabinet_SealingNumber.Add(CabinetNumber_Cabinet_SealingNumber);
                                    }

                                    string BoxNumber = "";
                                    string SealingNumber = "";
                                    if (item2.SealingNumber != null)
                                    {
                                        BoxNumber = item2.SealingNumber;
                                        if (item2.SealingNumber.Contains("/"))
                                        {
                                            BoxNumber = item2.SealingNumber.Split('/')[0];
                                            SealingNumber = item2.SealingNumber.Split('/')[1];
                                        }
                                    }

                                    //订舱的箱号/柜型/封号
                                    string BoxNumber_Cabinet_SealingNumber = BoxNumber + " / " + item2.Shipment_Cabinet.Name + " / " + SealingNumber;
                                    if (!list_BoxNumber_CabinetNumber_SealingNumber.Contains(BoxNumber_Cabinet_SealingNumber))
                                    {
                                        list_BoxNumber_CabinetNumber_SealingNumber.Add(BoxNumber_Cabinet_SealingNumber);
                                    }
                                }

                                foreach (var item3 in item2.Delivery_ShipmentOrderProduct.Where(d => !d.IsDelete && (!d.IsProductMixed || (d.IsProductMixed && d.ParentProductMixedID.HasValue))))
                                {
                                    if (string.IsNullOrEmpty(query.InvoiceNo))//TODO 为了兼容旧数据
                                    {
                                        //首先模糊搜索一下，比如编号为11的包含ShipmentOrderProductIDList包含211。然后在具体筛选
                                        var query_InspectionCustoms = context.Inspection_InspectionCustoms.Where(d => d.ShipmentOrderProductIDList.Contains(item3.ID.ToString()));
                                        if (query_InspectionCustoms.Count() > 0)
                                        {
                                            foreach (var item4 in query_InspectionCustoms)
                                            {
                                                var ShipmentOrderProductIDList = CommonCode.IdListToList(item4.ShipmentOrderProductIDList, '_');
                                                if (ShipmentOrderProductIDList.Contains(item3.ID))
                                                {
                                                    var InvoiceNo = item4.Inspection_InspectionCustomsDetail.First().InvoiceNO;
                                                    if (!string.IsNullOrEmpty(InvoiceNo))
                                                    {
                                                        InvoiceNo = item4.Inspection_InspectionCustomsDetail.First().InvoiceNO.Substring(0, 7);
                                                        if (!list_InvoiceNO.Contains(InvoiceNo))
                                                        {
                                                            list_InvoiceNO.Add(InvoiceNo);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    var query_OrderProduct = item3.OrderProduct;
                                    var query_Order = query_OrderProduct.Order;
                                    var query_Customers = query_Order.Orders_Customers;

                                    if (!list_POID.Contains(query_Order.POID))
                                    {
                                        list_POID.Add(query_Order.POID);
                                    }
                                    if (!list_PO2ID.Contains(query_Order.EHIPO))
                                    {
                                        list_PO2ID.Add(query_Order.EHIPO);
                                    }
                                    vm.CustomerID = query_Order.CustomerID;
                                    vm.CustomerCode = query_Order.Orders_Customers.CustomerCode;

                                    vm.ShipmentOrderID = ShipmentOrderID;
                                    //发票
                                    vm.CustomerName = query_Customers.CustomerName;
                                    vm.PaymentType = _dictionaryServices.GetDictionaryByName2(query_Customers.PaymentType, list_Com_DataDictionary);
                                    vm.CustomerStreet = query_Customers.StreetAddress;
                                    vm.CustomerDate = Utils.DateTimeToStr(query_Order.CustomerDate);
                                    vm.OrderSaleDate = Utils.DateTimeToStr(query_Order.OrderDateStart);
                                    vm.CustomerReg = query_Customers.City + ","
                                        + query_Customers.Reg_Area.AreaName + ","
                                        + query_Customers.PostalCode + ","
                                        + query_Customers.Reg_Area.Reg_Country.CountryName;

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

                                    VMOrderProduct vm_OrderProduct = new VMOrderProduct();

                                    vm_OrderProduct.OrderID = query_OrderProduct.OrderID;
                                    vm_OrderProduct.OrderProductQty = query_OrderProduct.Qty;
                                    vm_OrderProduct.CabinetID = item2.ID;
                                    vm_OrderProduct.CabinetIndex = item2.CabinetIndex;

                                    vm_OrderProduct.ID = query_OrderProduct.ID;
                                    vm_OrderProduct.Image = query_OrderProduct.Image;
                                    vm_OrderProduct.ProductID = query_OrderProduct.ProductID;

                                    vm_OrderProduct.OuterLength = OuterLength;
                                    vm_OrderProduct.OuterHeight = OuterHeight;
                                    vm_OrderProduct.OuterWidth = OuterWidth;
                                    vm_OrderProduct.OuterWeightGross = OuterWeightGross;
                                    vm_OrderProduct.OuterWeightNet = OuterWeightNet;

                                    vm_OrderProduct.POID = query_OrderProduct.Order.POID;
                                    vm_OrderProduct.SalePrice = query_OrderProduct.SalePrice ?? 0;

                                    vm_OrderProduct.SumSalePrice = vm_OrderProduct.SalePrice * item3.Qty;
                                    vm_OrderProduct.CustomerCode = query_OrderProduct.Order.Orders_Customers.CustomerCode;
                                    vm_OrderProduct.SkuNumber = query_OrderProduct.SkuNumber;
                                    vm_OrderProduct.No = query_OrderProduct.No;
                                    vm_OrderProduct.Desc = query_OrderProduct.Desc;
                                    vm_OrderProduct.Qty = item3.Qty;
                                    vm_OrderProduct.BoxQty = item3.BoxQty;
                                    vm_OrderProduct.OuterVolume = query_OrderProduct.OuterVolume;
                                    vm_OrderProduct.SumOuterWeightGross = OuterWeightGross * item3.BoxQty;
                                    vm_OrderProduct.SumOuterWeightGrossLBS = vm_OrderProduct.SumOuterWeightGross * 2.2m;
                                    vm_OrderProduct.SumOuterWeightNet = OuterWeightNet * item3.BoxQty;
                                    vm_OrderProduct.SumOuterWeightNetLBS = vm_OrderProduct.SumOuterWeightNet * 2.2m;
                                    vm_OrderProduct.SumOuterVolume = CalculateHelper.GetSumOuterVolume(OuterLength, OuterWidth, OuterHeight, item3.BoxQty);
                                    vm_OrderProduct.SkuCode = query_OrderProduct.SkuCode;

                                    if (query_OrderProduct.InspectionExchange_SumOuterVolume.HasValue)
                                    {
                                        vm_OrderProduct.SumOuterVolume = query_OrderProduct.InspectionExchange_SumOuterVolume;
                                    }
                                    vm_OrderProduct.SumOuterVolume_CUFT = vm_OrderProduct.SumOuterVolume * 35.315m;

                                    string HSCode = "";
                                    decimal Cess = 0;
                                    string HsEngName = "";
                                    if (query_OrderProduct.HTS.HasValue)
                                    {
                                        var query_HarmonizedSystems = context.HarmonizedSystems.Find(query_OrderProduct.HTS);
                                        if (query_HarmonizedSystems != null)
                                        {
                                            HSCode = query_HarmonizedSystems.HSCode;
                                            HsEngName = query_HarmonizedSystems.CodeEngName;
                                            Cess = query_HarmonizedSystems.Cess;
                                        }
                                    }
                                    vm_OrderProduct.HTS = query_OrderProduct.HTS;
                                    vm_OrderProduct.HsEngName = HsEngName;
                                    vm_OrderProduct.HSCode = HSCode;
                                    vm_OrderProduct.HSCode_Cess = Cess;
                                    vm_OrderProduct.FactoryID = query_OrderProduct.FactoryID;
                                    vm_OrderProduct.Factory_EnglishName = query_OrderProduct.Factory.EnglishName;
                                    vm_OrderProduct.Factory_EnglishAddress = query_OrderProduct.Factory.EnglishAddress;

                                    vm_OrderProduct.RetailPrice = query_OrderProduct.RetailPrice;
                                    vm_OrderProduct.OuterBoxRate = query_OrderProduct.OuterBoxRate;
                                    vm_OrderProduct.UPC = query_OrderProduct.UPC;

                                    if (query_OrderProduct.Purchase_PackProductsUPC != null && query_OrderProduct.Purchase_PackProductsUPC.Count > 0)
                                    {
                                        vm_OrderProduct.ProductUPC = query_OrderProduct.Purchase_PackProductsUPC.First().ProductUPC;
                                    }

                                    var HSCodeID = query_OrderProduct.HSCode;
                                    var dbHS = context.HarmonizedSystems.Find(HSCodeID);
                                    if (dbHS != null)
                                    {
                                        vm_OrderProduct.HSCode = dbHS.HSCode;
                                    }

                                    if (query.StatusID == (int)InspectionClearanceStatusEnum.PendingMaintenance)
                                    {
                                        vm_OrderProduct.MiscPercent = query_OrderProduct.InspectionClearance_MiscPercent;
                                        vm_OrderProduct.InspectionExchange_FactoryID = query_OrderProduct.InspectionClearance_FactoryID;
                                    }
                                    else
                                    {
                                        vm_OrderProduct.MiscPercent = query_OrderProduct.InspectionExchange_MiscPercent;
                                        vm_OrderProduct.InspectionExchange_FactoryID = query_OrderProduct.InspectionExchange_FactoryID;
                                    }

                                    var query_InspectionExchange_Factory = context.Factories.Find(query_OrderProduct.InspectionExchange_FactoryID);
                                    if (query_InspectionExchange_Factory != null)
                                    {
                                        vm_OrderProduct.InspectionExchange_Factory_EnglishName = query_InspectionExchange_Factory.EnglishName;
                                        vm_OrderProduct.InspectionExchange_Factory_EnglishAddress = query_InspectionExchange_Factory.EnglishAddress;
                                    }

                                    vm_OrderProduct.list_ProductIngredient = context.ProductIngredients.Where(d => d.ModuleType == (int)ModuleTypeEnum.Product && d.ProductID == query_OrderProduct.ProductID).Select(p => new VMProductIngredients
                                    {
                                        IngredientName = p.IngredientName,
                                        IngredientPercent = p.IngredientPercent,
                                        ProductID = p.ProductID,
                                    }).ToList();
                                    vm_OrderProduct.list_Ingredient = ExcelHelper.GetListIngredient(vm_OrderProduct.list_ProductIngredient);

                                    string BoxNumber = "";
                                    string SealingNumber = "";
                                    if (item2.SealingNumber != null)
                                    {
                                        BoxNumber = item2.SealingNumber;
                                        if (item2.SealingNumber.Contains("/"))
                                        {
                                            BoxNumber = item2.SealingNumber.Split('/')[0];
                                            SealingNumber = item2.SealingNumber.Split('/')[1];
                                        }
                                    }

                                    vm_OrderProduct.BoxNumber = BoxNumber;
                                    vm_OrderProduct.SealingNumber = SealingNumber;
                                    vm_OrderProduct.Department = query_OrderProduct.Department;
                                    vm_OrderProduct.DepartmentName = _dictionaryServices.GetDictionaryByAlias(query_OrderProduct.Department, list_Com_DataDictionary);

                                    list_OrderProduct_PackingList.Add(vm_OrderProduct);

                                    VMOrderProduct vm_OrderProduct_Invoice = ConstsMethod.Copy(vm_OrderProduct);

                                    vm_OrderProduct_Invoice.SumSalePrice = vm_OrderProduct_Invoice.Qty * vm_OrderProduct.SalePrice;
                                    vm_OrderProduct_Invoice.BoxQty = CalculateHelper.GetBoxQty(vm_OrderProduct_Invoice.Qty, vm_OrderProduct.OuterBoxRate);

                                    vm_OrderProduct_Invoice.SumOuterWeightGross = OuterWeightGross * vm_OrderProduct_Invoice.BoxQty;
                                    vm_OrderProduct_Invoice.SumOuterWeightNet = OuterWeightNet * vm_OrderProduct_Invoice.BoxQty;

                                    string OrderID_ProductID = vm_OrderProduct_Invoice.OrderID + "_" + vm_OrderProduct_Invoice.ID;
                                    if (!list_OrderProductID.Contains(OrderID_ProductID))
                                    {
                                        list_OrderProduct_Invoice.Add(vm_OrderProduct_Invoice);
                                        list_OrderProductID.Add(OrderID_ProductID);
                                    }
                                }
                            }
                        }
                        vm.InspectionExchangeID = query.InspectionExchangeID;

                        if (query.StatusID == (int)InspectionExchangeStatusEnum.PendingMaintenance)
                        {
                            vm.InvoiceNO = query.InvoiceNo;
                            if (list_InvoiceNO.Count > 0)
                            {
                                vm.InvoiceNO = list_InvoiceNO.First();
                            }

                            vm.ShipDateStartForamtter = Utils.DateTimeToStr(query_ShipmentOrder.First().Order.OrderDateStart);
                        }
                        else
                        {
                            vm.InvoiceNO = query.InvoiceNo;
                        }

                        if (vm.InvoiceNO != null && vm.InvoiceNO.Length == 8)
                        {
                            vm.Letter = vm.InvoiceNO.Substring(7, 1);
                        }

                        //装箱单
                        vm.POID = CommonCode.GetPOIDList(list_POID);
                        vm.EHIPO = string.Join(",", list_PO2ID);

                        vm.list_OrderProduct_PackingList = list_OrderProduct_PackingList;
                        vm.list_OrderProduct_Invoice = list_OrderProduct_Invoice;

                        vm.list_ExchangeOther = ConstsMethod.GetUploadFileList(query.InspectionExchangeID, UploadFileType.InspectionExchange);

                        vm.CreditNumber = query.CreditNumber;
                        vm.CreateDate = query.DT_MODIFYDATE;
                        vm.CreateDateForamtter = Utils.DateTimeToStr(query.DT_CREATEDATE);
                        vm.SelectCustomer = SelectCustomer;

                        vm.ShipDateStart = query.ShipDateStart;
                        vm.ShipDateStartForamtter = Utils.DateTimeToStr(query.ShipDateStart);
                        vm.PortOfEntry = query.PortOfEntry;
                        vm.PortOfEntryName = _dictionaryServices.GetDictionary_DestinationPortName(query.PortOfEntry, list_Com_DataDictionary);
                        vm.InvoiceOF = query.InvoiceOF;
                        vm.BoxNumber = query.BoxNumber;
                        vm.SealNumber = query.SealNumber;

                        var query_history = context.Inspection_InspectionExchangeHis.Where(d => d.InspectionExchangeID == query.InspectionExchangeID);
                        List<VMInspectionExchangeHis> list_history = new List<VMInspectionExchangeHis>();
                        foreach (var item2 in query_history)
                        {
                            list_history.Add(new VMInspectionExchangeHis()
                            {
                                DT_CREATEDATE = Utils.DateTimeToStr2(item2.DT_CREATEDATE),
                                CheckSuggest = item2.CheckSuggest,
                                UserName = item2.SystemUser.UserName,
                                StatusName = CommonCode.GetStatusEnumName(item2.StatusID, typeof(InspectionExchangeStatusEnum))
                            });
                        };
                        vm.InspectionExchangeHis = list_history;

                        if (query.StatusID == (int)InspectionExchangeStatusEnum.PendingMaintenance)
                        {
                            vm.ShipDateStart = query.Delivery_ShipmentOrder.Order.OrderDateStart;
                            vm.ShipDateStartForamtter = Utils.DateTimeToStr(vm.ShipDateStart);
                        }

                        vm.TransshipmentPortID = query.TransshipmentPortID;
                        vm.TransshipmentPortName = _dictionaryServices.GetDictionary_DestinationPortName(query.TransshipmentPortID, list_Com_DataDictionary);

                        vm.ActualShippingDate = query.ActualShippingDate;
                        vm.ActualShippingDateFormatter = Utils.DateTimeToStr(query.ActualShippingDate);

                        vm.FactoryIDList = query.FactoryIDList;
                        vm.list_Factory = list_Factory;
                        vm.InspectionExchangeStatusID = query.StatusID;

                        var query_Shipment_Agencies = query.Delivery_ShipmentOrder.Shipment_Agencies;
                        if (query_Shipment_Agencies != null)
                        {
                            vm.WarehouseAddress = query_Shipment_Agencies.WarehouseAddress;
                            vm.AgencyAddress = query_Shipment_Agencies.AgencyAddress;
                        }

                        var query_InspectionReceipt = context.Inspection_InspectionReceipt.Where(d => d.ShipmentOrderID == query.ShipmentOrderID);
                        if (query_InspectionReceipt != null && query_InspectionReceipt.Count() > 0)
                        {
                            vm.TradeTypeName = _dictionaryServices.GetDictionaryByName(query_InspectionReceipt.First().TradeType, list_Com_DataDictionary);
                        }

                        var query_Orders_Customers = query.Delivery_ShipmentOrder.Order.Orders_Customers;
                        vm.TeamOfThePayment = _dictionaryServices.GetDictionaryByName2(query_Orders_Customers.PaymentType, list_Com_DataDictionary);

                        vm.CabinetNumberList = CommonCode.ListToString(list_CabinetNumber);
                        vm.OceanVessel = CommonCode.ListToString(list_OceanVessel);
                        vm.list_CabinetNumber_Cabinet_SealingNumber = list_CabinetNumber_Cabinet_SealingNumber;
                        vm.list_BoxNumber_CabinetNumber_SealingNumber = list_BoxNumber_CabinetNumber_SealingNumber;

                        vm.ShipTo = query.ShipTo;

                        if (query.Orders_AcceptInformation != null)
                        {
                            vm.AcceptInformation_CompanyName = query.Orders_AcceptInformation.CompanyName;
                            vm.AcceptInformation_StreetAddress = query.Orders_AcceptInformation.StreetAddress;

                            vm.AcceptInformation_CustomerReg = query.Orders_AcceptInformation.City + ","
                                + context.Reg_Area.Where(p => p.ARID == query.Orders_AcceptInformation.Province).FirstOrDefault().AreaName + ","
                                + query.Orders_AcceptInformation.PostalCode + ","
                                + context.Reg_Country.Where(p => p.COID == query.Orders_AcceptInformation.Country).FirstOrDefault().CountryName;

                            vm.AcceptInformation_Comment = query.Orders_AcceptInformation.Comment;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteError(e);
            }

            return vm;
        }

        public DBOperationStatus SelectIsCheck(VMERPUser currentUser, int InspectionExchangeID, bool isCheck)
        {
            DBOperationStatus result = default(DBOperationStatus);
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query_InspectionExchange = context.Inspection_InspectionExchange.Find(InspectionExchangeID);
                    if (query_InspectionExchange != null)
                    {
                        //if (isCheck)//按照订单号结汇
                        //{
                        //    var query_ShipmentOrder = context.Delivery_ShipmentOrder.Where(d => d.ID == query_InspectionExchange.ShipmentOrderID && !d.IsDelete);
                        //    if (query_ShipmentOrder.First().IsMerge)
                        //    {
                        //        var OrderIDList = CommonCode.IdListToList(query_ShipmentOrder.First().OrderIDList);

                        //        foreach (var OrderID in OrderIDList)
                        //        {
                        //            if (OrderID != query_InspectionExchange.OrderID)
                        //            {
                        //                DAL.Inspection_InspectionExchange query = new Inspection_InspectionExchange()
                        //                {
                        //                    ST_CREATEUSER = currentUser.UserID,
                        //                    DT_CREATEDATE = DateTime.Now,
                        //                    ST_MODIFYUSER = currentUser.UserID,
                        //                    DT_MODIFYDATE = DateTime.Now,
                        //                    IsDelete = false,
                        //                    IPAddress = CommonCode.GetIP(),

                        //                    OrderID = OrderID,
                        //                    ShipmentOrderID = query_InspectionExchange.ShipmentOrderID,
                        //                    StatusID = (int)InspectionExchangeStatusEnum.PendingMaintenance,
                        //                    IsCheck = true,
                        //                };

                        //                context.Inspection_InspectionExchange.Add(query);
                        //            }
                        //            else
                        //            {
                        //                var query_InspectionExchange2 = context.Inspection_InspectionExchange.Where(d => d.OrderID == OrderID && !d.IsDelete);
                        //                if (query_InspectionExchange2 != null)
                        //                {
                        //                    query_InspectionExchange2.First().IsCheck = true;

                        //                    query_InspectionExchange2.First().ST_MODIFYUSER = currentUser.UserID;
                        //                    query_InspectionExchange2.First().DT_MODIFYDATE = DateTime.Now;
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //按照订舱号结汇
                        query_InspectionExchange.IsCheck = isCheck;
                        query_InspectionExchange.ST_MODIFYUSER = currentUser.UserID;
                        query_InspectionExchange.DT_MODIFYDATE = DateTime.Now;

                        //}
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

        public DBOperationStatus Save(VMERPUser currentUser, VMInspectionExchange vm)
        {
            DBOperationStatus result = default(DBOperationStatus);
            try
            {
                int StatusID = vm.InspectionExchangeStatusID;
                string AuditIdea = vm.AuditIdea;

                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Inspection_InspectionExchange.Find(vm.InspectionExchangeID);
                    if (query != null)
                    {
                        ConstsMethod.SaveFileUpload(currentUser, query.InspectionExchangeID, vm.list_ExchangeOther, context, UploadFileType.InspectionExchange);
                        if (query.StatusID == (int)InspectionExchangeStatusEnum.PendingMaintenance)
                        {
                            query.ST_CREATEUSER = currentUser.UserID;
                            query.DT_CREATEDATE = DateTime.Now;
                        }
                        query.ST_MODIFYUSER = currentUser.UserID;
                        query.DT_MODIFYDATE = DateTime.Now;
                        query.InvoiceNo = vm.InvoiceNO.Substring(0, 7) + vm.Letter;

                        if (!string.IsNullOrEmpty(vm.ShipDateStartForamtter))
                        {
                            query.ShipDateStart = Utils.StrToDateTime(vm.ShipDateStartForamtter);
                        }
                        query.PortOfEntry = vm.PortOfEntry;
                        query.InvoiceOF = vm.InvoiceOF;
                        query.BoxNumber = vm.BoxNumber;
                        query.SealNumber = vm.SealNumber;
                        query.ShipTo = vm.ShipTo;

                        if (vm.ActualShippingDateFormatter != null)
                        {
                            query.ActualShippingDate = Utils.StrToDateTime(vm.ActualShippingDateFormatter);
                        }

                        query.CreditNumber = vm.CreditNumber;
                        query.TransshipmentPortID = vm.TransshipmentPortID;
                        query.FactoryIDList = vm.FactoryIDList;

                        if (StatusID != (int)InspectionExchangeStatusEnum.PassedCheck && StatusID != (int)InspectionExchangeStatusEnum.NotPassCheck)
                        {
                            query.StatusID = StatusID;
                        }

                        Inspection_InspectionExchangeHis history = new Inspection_InspectionExchangeHis()
                        {
                            StatusID = StatusID,
                            ST_CREATEUSER = currentUser.UserID,
                            CheckSuggest = AuditIdea,
                            DT_CREATEDATE = DateTime.Now,
                            IPAddress = CommonCode.GetIP(),
                        };
                        query.Inspection_InspectionExchangeHis.Add(history);

                        if (vm.list_OrderProduct_Invoice != null)
                        {
                            foreach (var item in vm.list_OrderProduct_Invoice)
                            {
                                var query_OrderProduct = context.OrderProducts.Find(item.ID);
                                if (query_OrderProduct != null)
                                {
                                    query_OrderProduct.InspectionExchange_MiscPercent = item.MiscPercent;
                                    query_OrderProduct.InspectionExchange_FactoryID = item.InspectionExchange_FactoryID;
                                    query_OrderProduct.DT_MODIFYDATE = DateTime.Now;
                                }
                            }
                        }

                        if (vm.list_OrderProduct_PackingList != null)
                        {
                            foreach (var item in vm.list_OrderProduct_PackingList)
                            {
                                var query_OrderProduct = context.OrderProducts.Find(item.ID);
                                if (query_OrderProduct != null)
                                {
                                    query_OrderProduct.InspectionExchange_SumOuterVolume = item.SumOuterVolume;
                                    query_OrderProduct.DT_MODIFYDATE = DateTime.Now;
                                }
                            }
                        }

                        int affectRows = context.SaveChanges();

                        if (affectRows == 0)
                        {
                            result = DBOperationStatus.NoAffect;
                        }
                        else
                        {
                            bool bIsPass = true;
                            if (StatusID == (int)InspectionExchangeStatusEnum.NotPassCheck)
                            {
                                bIsPass = false;
                            }

                            List<int> listStatus = new List<int>();
                            listStatus.Add((int)InspectionExchangeStatusEnum.PendingCheck);
                            listStatus.Add((int)InspectionExchangeStatusEnum.NotPassCheck);
                            int[] iArrLogicStatus = { (int)InspectionExchangeStatusEnum.PendingCheck, (int)InspectionExchangeStatusEnum.NotPassCheck, (int)InspectionExchangeStatusEnum.PassedCheck };

                            ConstsMethod.InsertAuditStream(query.InspectionExchangeID, bIsPass, WorkflowTypes.ApprovalInspectionExchange, currentUser.UserID, listStatus, iArrLogicStatus, AuditIdea);

                            result = DBOperationStatus.Success;
                        }
                    }
                }

                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Inspection_InspectionExchange.Find(vm.InspectionExchangeID);
                    if (query != null && query.StatusID == (int)DocumentsIndexingStatusEnum.PassedCheck)
                    {
                        query.LargestSettlementDate = DateTime.Now; //收款日期 取最大的结汇日期，最大结汇日期=结汇审核通过的日期

                        context.SaveChanges();
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

        public List<string> DownLoad(VMERPUser currentUser, int ID)
        {
            List<string> makeFileList = new List<string>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var vm = GetDetailByID(currentUser, ID);
                    if (vm != null)
                    {
                        //vm.SelectCustomer = SelectCustomerEnum.S235.ToString();//TODO 暂时的

                        List<string> list_PdfFile = new List<string>();

                        List<VMOrderProduct> list_OrderProduct_vm = vm.list_OrderProduct_Invoice;
                        string InvoiceNO = vm.InvoiceNO;

                        #region 发票

                        if (vm.SelectCustomer == SelectCustomerEnum.S288.ToString())//S288的发票按照PO号分开
                        {
                            var list_Product = vm.list_OrderProduct_Invoice;
                            var temp = list_Product.GroupBy(d => d.OrderID).ToList();

                            int index = 1;
                            char letter = 'A';

                            foreach (var item in temp)
                            {
                                var vm2 = vm;
                                List<VMOrderProduct> list_OrderProduct_Invoice = new List<VMOrderProduct>();
                                List<string> list_POID = new List<string>();

                                foreach (var item_product in item)
                                {
                                    list_OrderProduct_Invoice.Add(item_product);

                                    if (!list_POID.Contains(item_product.POID))
                                    {
                                        list_POID.Add(item_product.POID);
                                    }
                                }

                                vm2.list_OrderProduct_Invoice = list_OrderProduct_Invoice;
                                vm2.POID = CommonCode.GetPOIDList(list_POID);

                                if (index == 1)
                                {
                                    if (vm.InvoiceNO.Length > 7)
                                    {
                                        letter = vm.InvoiceNO.Substring(7, 1)[0];
                                    }
                                }
                                else
                                {
                                    letter = (char)(letter + 1);
                                }

                                vm2.InvoiceNO = vm.InvoiceNO.Substring(0, 7) + letter;

                                MakerExcel_Documents.Maker(vm2, MakerTypeEnum.InspectionExchange_CommercialInvoice, vm.SelectCustomer, vm.InspectionExchangeID.ToString(), list_PdfFile, index.ToString());
                                index++;
                            }
                        }
                        else
                        {
                            MakerExcel_Documents.Maker(vm, MakerTypeEnum.InspectionExchange_CommercialInvoice, vm.SelectCustomer, vm.InspectionExchangeID.ToString(), list_PdfFile);
                        }

                        #endregion 发票

                        #region 装箱单

                        if (vm.SelectCustomer != SelectCustomerEnum.DG.ToString())//DG不需要装箱单
                        {
                            if (vm.SelectCustomer == SelectCustomerEnum.S288.ToString())//S288一个柜子一个装箱单
                            {
                                var list_Product = list_OrderProduct_vm;
                                var temp = list_Product.GroupBy(d => d.CabinetIndex).ToList();

                                int index = 1;
                                foreach (var item in temp)
                                {
                                    var vm2 = vm;
                                    List<VMOrderProduct> list_OrderProduct = new List<VMOrderProduct>();
                                    List<string> list_POID = new List<string>();

                                    foreach (var item_product in item)
                                    {
                                        list_OrderProduct.Add(item_product);

                                        if (!list_POID.Contains(item_product.POID))
                                        {
                                            list_POID.Add(item_product.POID);
                                        }
                                    }
                                    vm2.list_OrderProduct_PackingList = list_OrderProduct;
                                    vm2.POID = CommonCode.GetPOIDList(list_POID);

                                    #region 设置柜型

                                    var item2 = context.Delivery_ShipmentOrderCabinet.Find(item.First().CabinetID);
                                    if (item2 != null)
                                    {
                                        List<string> list_BoxNumber_CabinetNumber_SealingNumber = new List<string>();

                                        string BoxNumber = "";
                                        string SealingNumber = "";
                                        if (item2.SealingNumber != null)
                                        {
                                            BoxNumber = item2.SealingNumber;
                                            if (item2.SealingNumber.Contains("/"))
                                            {
                                                BoxNumber = item2.SealingNumber.Split('/')[0];
                                                SealingNumber = item2.SealingNumber.Split('/')[1];
                                            }
                                        }

                                        //订舱的箱号/柜号/封号
                                        string BoxNumber_Cabinet_SealingNumber = BoxNumber + " / " + item2.CabinetNumber + " / " + SealingNumber;
                                        if (!list_BoxNumber_CabinetNumber_SealingNumber.Contains(BoxNumber_Cabinet_SealingNumber))
                                        {
                                            list_BoxNumber_CabinetNumber_SealingNumber.Add(BoxNumber_Cabinet_SealingNumber);
                                        }

                                        vm2.list_BoxNumber_CabinetNumber_SealingNumber = list_BoxNumber_CabinetNumber_SealingNumber;
                                    }

                                    #endregion 设置柜型

                                    vm.InvoiceNO = InvoiceNO;
                                    MakerExcel_Documents.Maker(vm2, MakerTypeEnum.InspectionExchange_PackingList, vm.SelectCustomer, vm.InspectionExchangeID.ToString(), list_PdfFile, index.ToString());
                                    index++;
                                }
                            }
                            else
                            {
                                MakerExcel_Documents.Maker(vm, MakerTypeEnum.InspectionExchange_PackingList, vm.SelectCustomer, vm.InspectionExchangeID.ToString(), list_PdfFile);
                            }
                        }

                        #endregion 装箱单

                        if (vm.SelectCustomer == SelectCustomerEnum.S135.ToString())
                        {
                            MakerExcel_Documents.Maker(vm, MakerTypeEnum.InspectionExchange_ExcludingWoodPackagingDeclaration, vm.SelectCustomer, vm.InspectionExchangeID.ToString(), list_PdfFile);

                            MakerExcel_Documents.Maker(vm, MakerTypeEnum.InspectionExchange_CertificateOfOrigin, vm.SelectCustomer, vm.InspectionExchangeID.ToString(), list_PdfFile);
                        }

                        if (vm.SelectCustomer == SelectCustomerEnum.S220.ToString())
                        {
                            MakerExcel_Documents.Maker(vm, MakerTypeEnum.InspectionExchange_BeneficiaryStatement, vm.SelectCustomer, vm.InspectionExchangeID.ToString(), list_PdfFile);
                        }

                        if (vm.SelectCustomer == SelectCustomerEnum.S235.ToString())
                        {
                            MakerExcel_Documents.Maker(vm, MakerTypeEnum.InspectionExchange_CertificateOfOrigin, vm.SelectCustomer, vm.InspectionExchangeID.ToString(), list_PdfFile);
                            MakerExcel_Documents.Maker(vm, MakerTypeEnum.InspectionExchange_PackingListBuContainer, vm.SelectCustomer, vm.InspectionExchangeID.ToString(), list_PdfFile);
                        }

                        if (vm.list_ExchangeOther != null && vm.list_ExchangeOther.Count > 0)
                        {
                            foreach (var item in vm.list_ExchangeOther)
                            {
                                ConstsMethod.GetUploadToPdfList(list_PdfFile, item.ServerFileName);
                            }
                        }

                        if (vm.SelectCustomer == SelectCustomerEnum.S135.ToString())
                        {
                            list_PdfFile.Add(Utils.GetMapPath("/data/Documents/S135/S135 结汇公司账户信息.pdf"));
                        }

                        makeFileList.Add(CommonCode.CreatePdfList(list_PdfFile, "InspectionExchange", vm.InspectionExchangeID.ToString()));
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
            DownLoad(currentUser, ID);
            List<string> makeFileList = new List<string>();

            string path = "/data/Template/Out/InspectionExchange/" + ID + "/PDFAndExcel";
            string zipPath = "/data/Template/Out/InspectionExchange/" + ID + "/" + CommonCode.GetTimeStamp() + ".zip";
            ExcelHelper.Zip(Utils.GetMapPath(path), Utils.GetMapPath(zipPath));//生成压缩文件

            makeFileList.Add(zipPath);
            return makeFileList;
        }

        public VMInspectionExchange GetUploadList_Modify(VMERPUser currentUser, int InspectionExchangeID)
        {
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Inspection_InspectionExchange.Find(InspectionExchangeID);
                    if (query != null)
                    {
                        VMInspectionExchange vm = new VMInspectionExchange();
                        vm.InspectionExchangeID = query.InspectionExchangeID;

                        vm.list_UploadModify = ConstsMethod.GetUploadFileList(query.InspectionExchangeID, UploadFileType.InspectionExchange_Modify);
                        return vm;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return null;
        }

        public DBOperationStatus UploadModify(VMERPUser currentUser, VMInspectionExchange vm)
        {
            DBOperationStatus result = default(DBOperationStatus);
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Inspection_InspectionExchange.Find(vm.InspectionExchangeID);
                    if (query != null)
                    {
                        ConstsMethod.SaveFileUpload(currentUser, query.InspectionExchangeID, vm.list_UploadModify, context, UploadFileType.InspectionExchange_Modify);

                        int affectRows = context.SaveChanges();
                        if (affectRows == 0)
                        {
                            result = DBOperationStatus.NoAffect;
                        }
                        else
                        {
                            result = DBOperationStatus.Success;
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
    }
}