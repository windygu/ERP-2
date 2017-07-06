using ERP.BLL.Consts;
using ERP.BLL.ERP.Consts;
using ERP.BLL.ERP.Dictionary;
using ERP.BLL.Workflow;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.Factory;
using ERP.Models.InspectionClearance;
using ERP.Models.Order;
using ERP.Models.Product;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ERP.BLL.InspectionClearance
{
    public class InspectionClearanceService
    {
        private DictionaryServices _dictionaryServices = new DictionaryServices();

        public List<DTOInspectionClearance> GetAll(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows, VMInspectionClearanceSearch vm_search)
        {
            List<DTOInspectionClearance> listModel = null;
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Inspection_InspectionClearance.Where(p => !p.IsDelete);

                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForInspectionClearance);

                    switch (vm_search.PageType)
                    {
                        case PageTypeEnum.PendingApproval:
                            query = query.Where(d => d.StatusID < (short)InspectionClearanceStatusEnum.PassedCheck);
                            break;

                        case PageTypeEnum.PassedApproval:
                            query = query.Where(d => d.StatusID >= (short)InspectionClearanceStatusEnum.PassedCheck);
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
                        listModel = new List<DTOInspectionClearance>();

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

                                List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());

                                #region 判断是否有审批流的权限

                                if (vm_search.PageType == PageTypeEnum.PendingApproval)//待审核
                                {
                                    IsHasApprovalPermission = ApprovalService.HasApprovalPermission(WorkflowTypes.ApprovalInspectionClearance, currentUser, item.ST_CREATEUSER, item.ApproverIndex, Order.CustomerID);
                                }

                                #endregion 判断是否有审批流的权限

                                listModel.Add(new DTOInspectionClearance()
                                {
                                    OrderNumber = Order.OrderNumber,
                                    CustomerCode = Order.Orders_Customers.CustomerCode,
                                    PortName = _dictionaryServices.GetDictionary_PortEnName(item.Delivery_ShipmentOrder.PortID, list_Com_DataDictionary),
                                    DestinationPortName = _dictionaryServices.GetDictionary_DestinationPortName(item.Delivery_ShipmentOrder.Delivery_ShipmentOrderCabinet.FirstOrDefault().ShipToPortID, list_Com_DataDictionary),
                                    OrderDateStartFormatter = Utils.DateTimeToStr(Order.OrderDateStart),
                                    OrderDateEndFormatter = Utils.DateTimeToStr(Order.OrderDateEnd),

                                    InspectionClearanceID = item.InspectionClearanceID,
                                    ShipmentOrderID = item.ShipmentOrderID,
                                    Merge = tempMerge,
                                    OrderNumberList = OrderNumberList,
                                    StatusID = item.StatusID,
                                    StatusName = CommonCode.GetStatusEnumName(item.StatusID, typeof(InspectionClearanceStatusEnum)),
                                    DT_MODIFYDATEFormatter = Utils.DateTimeToStr2(item.DT_MODIFYDATE),
                                    ST_CREATEUSER = item.ST_CREATEUSER,
                                    ApproverIndex = item.ApproverIndex,
                                    CustomerID = Order.CustomerID,
                                    IsHasApprovalPermission = IsHasApprovalPermission,
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

        public VMInspectionClearance GetDetailByID(VMERPUser currentUser, int id)
        {
            VMInspectionClearance vm = new VMInspectionClearance();

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Inspection_InspectionClearance.Find(id);
                    if (query != null)
                    {
                        int ShipmentOrderID = query.ShipmentOrderID;

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

                                    //订舱的箱号/柜号/封号
                                    string BoxNumber_Cabinet_SealingNumber = BoxNumber + " / " + item2.CabinetNumber + " / " + SealingNumber;
                                    if (!list_BoxNumber_CabinetNumber_SealingNumber.Contains(BoxNumber_Cabinet_SealingNumber))
                                    {
                                        list_BoxNumber_CabinetNumber_SealingNumber.Add(BoxNumber_Cabinet_SealingNumber);
                                    }
                                }

                                #region 获取目的港的地址

                                var query_Com_DataDictionary = list_Com_DataDictionary.Where(c => c.Code == item2.ShipToPortID && c.TableKind == (int)DictionaryTableKind.GoalPort).FirstOrDefault();
                                if (query_Com_DataDictionary != null)
                                {
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
                                }

                                #endregion 获取目的港的地址

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

                                    vm.ShipmentOrderID = ShipmentOrderID;
                                    //发票
                                    vm.CustomerName = query_Customers.CustomerName;
                                    vm.CustomerStreet = query_Customers.StreetAddress;

                                    vm.CustomerDate2 = query_Order.CustomerDate;
                                    vm.CustomerDate = Utils.DateTimeToStr(query_Order.CustomerDate);
                                    vm.OrderSaleDate = Utils.DateTimeToStr(query_Order.OrderDateStart);
                                    vm.CustomerReg = query_Customers.City + ","
                                        + query_Customers.Reg_Area.AreaName + ","
                                        + query_Customers.PostalCode + ","
                                        + query_Customers.Reg_Area.Reg_Country.CountryName;

                                    vm.PaymentType = _dictionaryServices.GetDictionaryByName2(query_Customers.PaymentType, list_Com_DataDictionary);

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
                                    vm_OrderProduct.POID = query_OrderProduct.Order.POID;
                                    vm_OrderProduct.SalePrice = query_OrderProduct.SalePrice ?? 0;
                                    if (query_OrderProduct.InspectionClearance_SalePrice.HasValue)
                                    {
                                        vm_OrderProduct.SalePrice = query_OrderProduct.InspectionClearance_SalePrice ?? 0;
                                    }

                                    vm_OrderProduct.OuterLength = OuterLength;
                                    vm_OrderProduct.OuterHeight = OuterHeight;
                                    vm_OrderProduct.OuterWidth = OuterWidth;
                                    vm_OrderProduct.OuterWeightGross = OuterWeightGross;
                                    vm_OrderProduct.OuterWeightNet = OuterWeightNet;

                                    vm_OrderProduct.SumSalePrice = vm_OrderProduct.SalePrice * item3.Qty;
                                    vm_OrderProduct.CustomerCode = query_OrderProduct.Order.Orders_Customers.CustomerCode;
                                    vm_OrderProduct.SkuNumber = query_OrderProduct.SkuNumber;
                                    vm_OrderProduct.No = query_OrderProduct.No;
                                    vm_OrderProduct.Desc = query_OrderProduct.Desc;
                                    vm_OrderProduct.Qty = item3.Qty;
                                    vm_OrderProduct.BoxQty = item3.BoxQty;
                                    vm_OrderProduct.OuterVolume = query_OrderProduct.OuterVolume;
                                    vm_OrderProduct.SumOuterWeightGross = OuterWeightGross * item3.BoxQty;
                                    vm_OrderProduct.SumOuterWeightNet = OuterWeightNet * item3.BoxQty;
                                    vm_OrderProduct.SumOuterVolume = CalculateHelper.GetSumOuterVolume(OuterLength, OuterWidth, OuterHeight, item3.BoxQty);
                                    vm_OrderProduct.SkuCode = query_OrderProduct.SkuCode;
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

                                    var query_InspectionClearance_Factory = context.Factories.Find(query_OrderProduct.InspectionClearance_FactoryID);
                                    if (query_InspectionClearance_Factory != null)
                                    {
                                        vm_OrderProduct.InspectionClearance_Factory_EnglishName = query_InspectionClearance_Factory.EnglishName;
                                        vm_OrderProduct.InspectionClearance_Factory_EnglishAddress = query_InspectionClearance_Factory.EnglishAddress;
                                    }

                                    vm_OrderProduct.RetailPrice = query_OrderProduct.RetailPrice;
                                    vm_OrderProduct.OuterBoxRate = query_OrderProduct.OuterBoxRate;
                                    vm_OrderProduct.UPC = query_OrderProduct.UPC;

                                    if (query_OrderProduct.Purchase_PackProductsUPC != null && query_OrderProduct.Purchase_PackProductsUPC.Count > 0)
                                    {
                                        vm_OrderProduct.ProductUPC = query_OrderProduct.Purchase_PackProductsUPC.First().ProductUPC;
                                    }

                                    if (query.StatusID == (int)InspectionClearanceStatusEnum.PendingMaintenance)
                                    {
                                        vm_OrderProduct.MiscPercent = query_Customers.MiscImportLoadAmount;
                                        vm_OrderProduct.InspectionClearance_FactoryID = query_OrderProduct.FactoryID;
                                    }
                                    else
                                    {
                                        vm_OrderProduct.MiscPercent = query_OrderProduct.InspectionClearance_MiscPercent;
                                        vm_OrderProduct.InspectionClearance_FactoryID = query_OrderProduct.InspectionClearance_FactoryID;
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
                                    else
                                    {
                                        var product = list_OrderProduct_Invoice.Find(d => d.ID == vm_OrderProduct_Invoice.ID);
                                        product.Qty += vm_OrderProduct_Invoice.Qty;
                                    }
                                }
                            }
                        }
                        vm.InspectionClearanceID = query.InspectionClearanceID;

                        vm.InvoiceNO = query.InvoiceNo;
                        if (list_InvoiceNO.Count > 0)
                        {
                            vm.InvoiceNO = list_InvoiceNO.First();
                        }
                        //装箱单
                        vm.POID = CommonCode.GetPOIDList(list_POID);
                        vm.list_OrderProduct_PackingList = list_OrderProduct_PackingList;
                        vm.list_OrderProduct_Invoice = list_OrderProduct_Invoice;

                        vm.list_ClearanceOther = ConstsMethod.GetUploadFileList(query.InspectionClearanceID, UploadFileType.InspectionClearance);

                        vm.CreateDate = query.DT_CREATEDATE;
                        vm.CreateDateForamtter = Utils.DateTimeToStr(query.DT_CREATEDATE);
                        vm.SelectCustomer = SelectCustomer;
                        vm.CustomerID = query.Delivery_ShipmentOrder.Order.CustomerID;

                        vm.ShipDateStart = query.ShipDateStart;
                        vm.ShipDateStartForamtter = Utils.DateTimeToStr(query.ShipDateStart);
                        vm.PortOfEntry = query.PortOfEntry;
                        vm.PortOfEntryName = _dictionaryServices.GetDictionary_DestinationPortName(query.PortOfEntry, list_Com_DataDictionary);
                        vm.InvoiceOF = query.InvoiceOF;
                        vm.BoxNumber = query.BoxNumber;
                        vm.SealNumber = query.SealNumber;
                        vm.TransshipmentPortID = query.TransshipmentPortID;
                        vm.TransshipmentPortName = _dictionaryServices.GetDictionary_DestinationPortName(query.TransshipmentPortID, list_Com_DataDictionary);

                        vm.CreditNumber = query.CreditNumber;
                        vm.ActualShippingDate = query.ActualShippingDate;
                        vm.ActualShippingDateFormatter = Utils.DateTimeToStr(query.ActualShippingDate);
                        vm.FactoryIDList = query.FactoryIDList;
                        vm.IsTest = query.IsTest;
                        vm.list_Factory = list_Factory;

                        var query_Shipment_Agencies = query.Delivery_ShipmentOrder.Shipment_Agencies;
                        if (query_Shipment_Agencies != null)
                        {
                            vm.WarehouseAddress = query_Shipment_Agencies.WarehouseAddress;
                            vm.AgencyAddress = query_Shipment_Agencies.AgencyAddress;
                        }

                        var query_history = context.Inspection_InspectionClearanceHis.Where(d => d.InspectionClearanceID == query.InspectionClearanceID);
                        List<VMInspectionClearanceHis> list_history = new List<VMInspectionClearanceHis>();
                        foreach (var item2 in query_history)
                        {
                            list_history.Add(new VMInspectionClearanceHis()
                            {
                                DT_CREATEDATE = Utils.DateTimeToStr2(item2.DT_CREATEDATE),
                                CheckSuggest = item2.CheckSuggest,
                                UserName = item2.SystemUser.UserName,
                                StatusName = CommonCode.GetStatusEnumName(item2.StatusID, typeof(InspectionClearanceStatusEnum))
                            });
                        };
                        vm.InspectionClearanceHis = list_history;

                        if (query.StatusID == (int)InspectionClearanceStatusEnum.PendingMaintenance)
                        {
                            vm.ShipDateStart = query.Delivery_ShipmentOrder.Order.OrderDateStart;
                            vm.ShipDateStartForamtter = Utils.DateTimeToStr(vm.ShipDateStart);

                            var query_InspectionReceipt = context.Inspection_InspectionReceipt.Where(d => d.ShipmentOrderID == query.ShipmentOrderID);
                            if (query_InspectionReceipt != null && query_InspectionReceipt.Count() > 0)//待清关的时候 贸易方式从报检里面取
                            {
                                vm.TradeType = query_InspectionReceipt.First().TradeType;
                            }
                        }
                        else
                        {
                            vm.TradeType = query.TradeType;
                        }

                        vm.TradeTypeName = _dictionaryServices.GetDictionaryByName(vm.TradeType, list_Com_DataDictionary);

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

        public DBOperationStatus Save(VMERPUser currentUser, VMInspectionClearance vm)
        {
            DBOperationStatus result = default(DBOperationStatus);
            try
            {
                int StatusID = vm.InspectionClearanceStatusID;
                string AuditIdea = vm.AuditIdea;

                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Inspection_InspectionClearance.Find(vm.InspectionClearanceID);
                    if (query != null)
                    {
                        ConstsMethod.SaveFileUpload(currentUser, query.InspectionClearanceID, vm.list_ClearanceOther, context, UploadFileType.InspectionClearance);
                        if (query.StatusID == (int)InspectionClearanceStatusEnum.PendingMaintenance)
                        {
                            query.ST_CREATEUSER = currentUser.UserID;
                            query.DT_CREATEDATE = DateTime.Now;
                        }
                        query.ST_MODIFYUSER = currentUser.UserID;
                        query.DT_MODIFYDATE = DateTime.Now;

                        if (!string.IsNullOrEmpty(vm.ShipDateStartForamtter))
                        {
                            query.ShipDateStart = Utils.StrToDateTime(vm.ShipDateStartForamtter);
                        }
                        query.PortOfEntry = vm.PortOfEntry;
                        query.InvoiceOF = vm.InvoiceOF;
                        query.BoxNumber = vm.BoxNumber;
                        query.SealNumber = vm.SealNumber;
                        query.TradeType = vm.TradeType;
                        query.ShipTo = vm.ShipTo;

                        query.TransshipmentPortID = vm.TransshipmentPortID;
                        query.CreditNumber = vm.CreditNumber;
                        if (vm.ActualShippingDateFormatter != null)
                        {
                            query.ActualShippingDate = Utils.StrToDateTime(vm.ActualShippingDateFormatter);
                        }
                        query.FactoryIDList = vm.FactoryIDList;
                        query.IsTest = vm.IsTest;

                        foreach (var item in vm.list_OrderProduct_Invoice)
                        {
                            var query_OrderProduct = context.OrderProducts.Find(item.ID);
                            query_OrderProduct.InspectionClearance_SalePrice = item.SalePrice;
                            query_OrderProduct.InspectionClearance_MiscPercent = item.MiscPercent;
                            query_OrderProduct.InspectionClearance_FactoryID = item.InspectionClearance_FactoryID;
                            query_OrderProduct.DT_MODIFYDATE = DateTime.Now;
                        }

                        if (StatusID != (int)InspectionClearanceStatusEnum.PassedCheck && StatusID != (int)InspectionClearanceStatusEnum.NotPassCheck)
                        {
                            query.StatusID = StatusID;
                        }

                        Inspection_InspectionClearanceHis history = new Inspection_InspectionClearanceHis()
                        {
                            StatusID = StatusID,
                            ST_CREATEUSER = currentUser.UserID,
                            CheckSuggest = AuditIdea,
                            DT_CREATEDATE = DateTime.Now,
                            IPAddress = CommonCode.GetIP(),
                        };
                        query.Inspection_InspectionClearanceHis.Add(history);

                        //提交审核时，清关直接审核通过
                        var SelectCustomer = query.Delivery_ShipmentOrder.Order.Orders_Customers.SelectCustomer;
                        if (StatusID == (int)InspectionClearanceStatusEnum.PendingCheck)
                        {
                            if (SelectCustomer == SelectCustomerEnum.S60.ToString() || SelectCustomer == SelectCustomerEnum.S52.ToString())
                            {
                                query.StatusID = 5;
                                query.ApproverIndex = int.MinValue;
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
                            if (StatusID == (int)InspectionClearanceStatusEnum.NotPassCheck)
                            {
                                bIsPass = false;
                            }

                            List<int> listStatus = new List<int>();
                            listStatus.Add((int)InspectionClearanceStatusEnum.PendingCheck);
                            listStatus.Add((int)InspectionClearanceStatusEnum.NotPassCheck);
                            int[] iArrLogicStatus = { (int)InspectionClearanceStatusEnum.PendingCheck, (int)InspectionClearanceStatusEnum.NotPassCheck, (int)InspectionClearanceStatusEnum.PassedCheck };

                            ConstsMethod.InsertAuditStream(query.InspectionClearanceID, bIsPass, WorkflowTypes.ApprovalInspectionClearance, currentUser.UserID, listStatus, iArrLogicStatus, AuditIdea);

                            result = DBOperationStatus.Success;

                            SaveOther(currentUser, query.InspectionClearanceID);
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

        private void SaveOther(VMERPUser currentUser, int InspectionClearanceID)
        {
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                var query2 = context.Inspection_InspectionClearance.Find(InspectionClearanceID);
                if (query2 != null)
                {
                    if (query2.StatusID == (int)InspectionClearanceStatusEnum.PassedCheck)
                    {
                        //其中DG、S05、S235、F20、S52、S56、S164客户清关提交审核，添加结汇，并且直接审核通过。其他客户需要上传FCR后，才添加结汇。

                        #region 审核通过后，添加结汇列表的数据

                        var query_InspectionExchange = context.Inspection_InspectionExchange.Where(d => d.ShipmentOrderID == query2.ShipmentOrderID && !d.IsDelete);
                        if (query_InspectionExchange.Count() == 0)
                        {
                            DAL.Inspection_InspectionExchange query = new Inspection_InspectionExchange()
                            {
                                ST_CREATEUSER = currentUser.UserID,
                                DT_CREATEDATE = DateTime.Now,
                                ST_MODIFYUSER = currentUser.UserID,
                                DT_MODIFYDATE = DateTime.Now,
                                IsDelete = false,
                                IPAddress = CommonCode.GetIP(),

                                OrderID = query2.Delivery_ShipmentOrder.OrderID,
                                ShipmentOrderID = query2.ShipmentOrderID,
                                StatusID = (int)InspectionExchangeStatusEnum.PendingMaintenance,
                            };

                            //提交审核时，结汇直接审核通过
                            var SelectCustomer = query2.Delivery_ShipmentOrder.Order.Orders_Customers.SelectCustomer;
                            if (SelectCustomer == SelectCustomerEnum.DG.ToString()
                                || SelectCustomer == SelectCustomerEnum.S05.ToString()
                                || SelectCustomer == SelectCustomerEnum.S235.ToString()
                                || SelectCustomer == SelectCustomerEnum.F20.ToString()
                                || SelectCustomer == SelectCustomerEnum.S52.ToString()
                                || SelectCustomer == SelectCustomerEnum.S56.ToString()
                                || SelectCustomer == SelectCustomerEnum.S164.ToString())
                            {
                                query.StatusID = (int)InspectionExchangeStatusEnum.PassedCheck;
                                query.ApproverIndex = int.MinValue;

                                query.CreditNumber = query2.CreditNumber;

                                List<string> list_InvoiceNO = new List<string>();

                                foreach (var item2 in query2.Delivery_ShipmentOrder.Delivery_ShipmentOrderCabinet.Where(d => !d.IsDelete))//TODO 为了兼容旧数据
                                {
                                    if (string.IsNullOrEmpty(query2.InvoiceNo))//TODO 为了兼容旧数据
                                    {
                                        foreach (var item3 in item2.Delivery_ShipmentOrderProduct.Where(d => !d.IsDelete))
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
                                    }

                                    query.InvoiceNo = query2.InvoiceNo;

                                    if (list_InvoiceNO.Count > 0)
                                    {
                                        query.InvoiceNo = list_InvoiceNO.First();
                                    }

                                    if (query2.ShipDateStart.HasValue)
                                    {
                                        query.ShipDateStart = query2.ShipDateStart;
                                    }
                                    query.PortOfEntry = query2.PortOfEntry;
                                    query.InvoiceOF = query2.InvoiceOF;
                                    query.MiscPercent = query2.MiscPercent;
                                    query.BoxNumber = query2.BoxNumber;
                                    query.SealNumber = query2.SealNumber;
                                    query.TransshipmentPortID = query2.TransshipmentPortID;
                                    query.ActualShippingDate = query2.ActualShippingDate;
                                    query.FactoryIDList = query2.FactoryIDList;
                                    query.IsTest = query2.IsTest;

                                    query.TradeType = query2.TradeType;
                                    query.ShipTo = query2.ShipTo;
                                }

                                context.Inspection_InspectionExchange.Add(query);
                            }
                        }

                        #endregion 审核通过后，添加结汇列表的数据

                        context.SaveChanges();
                    }
                    else if (query2.StatusID == (int)InspectionClearanceStatusEnum.UploadedFCR)
                    {
                        #region 审核通过后，添加结汇列表的数据

                        var query_InspectionExchange = context.Inspection_InspectionExchange.Where(d => d.ShipmentOrderID == query2.ShipmentOrderID && !d.IsDelete);
                        if (query_InspectionExchange.Count() == 0)
                        {
                            DAL.Inspection_InspectionExchange query = new Inspection_InspectionExchange()
                            {
                                ST_CREATEUSER = currentUser.UserID,
                                DT_CREATEDATE = DateTime.Now,
                                ST_MODIFYUSER = currentUser.UserID,
                                DT_MODIFYDATE = DateTime.Now,
                                IsDelete = false,
                                IPAddress = CommonCode.GetIP(),

                                OrderID = query2.Delivery_ShipmentOrder.OrderID,
                                ShipmentOrderID = query2.ShipmentOrderID,
                                StatusID = (int)InspectionExchangeStatusEnum.PendingMaintenance,

                                PortOfEntry = query2.PortOfEntry,
                                InvoiceOF = query2.InvoiceOF,
                                MiscPercent = query2.MiscPercent,
                                BoxNumber = query2.BoxNumber,
                                SealNumber = query2.SealNumber,
                                TransshipmentPortID = query2.TransshipmentPortID,
                                ActualShippingDate = query2.ActualShippingDate,
                                FactoryIDList = query2.FactoryIDList,
                                IsTest = query2.IsTest,

                                InvoiceNo = query2.InvoiceNo,
                                TradeType = query2.TradeType,
                            };

                            context.Inspection_InspectionExchange.Add(query);
                        }

                        #endregion 审核通过后，添加结汇列表的数据

                        #region 添加普通产品的单证索引，不是配件产品

                        var query_ShipmentOrder = context.Delivery_ShipmentOrder.Find(query2.ShipmentOrderID);

                        List<int> list_OrderID = new List<int>();
                        if (query_ShipmentOrder.IsMerge)
                        {
                            list_OrderID = CommonCode.IdListToList(query_ShipmentOrder.OrderIDList);
                        }
                        else
                        {
                            list_OrderID.Add(query_ShipmentOrder.OrderID);
                        }
                        foreach (var item in list_OrderID)
                        {
                            var query_temp = context.DocumentsIndexings.Where(d => d.OrderID == item && !d.IsDelete && d.DocumentsIndexingType == (int)DocumentsIndexingTypeEnum.Default);
                            if (query_temp.Count() == 0)
                            {
                                var query_DocumentsIndexing = new DAL.DocumentsIndexing()
                                {
                                    ST_CREATEUSER = currentUser.UserID,
                                    DT_CREATEDATE = DateTime.Now,
                                    ST_MODIFYUSER = currentUser.UserID,
                                    DT_MODIFYDATE = DateTime.Now,
                                    IsDelete = false,
                                    IPAddress = CommonCode.GetIP(),

                                    OrderID = item,
                                    ShipmentOrderID = query2.ShipmentOrderID,
                                    StatusID = (int)DocumentsIndexingStatusEnum.PendingMaintenance,
                                };
                                context.DocumentsIndexings.Add(query_DocumentsIndexing);
                            }
                        }

                        #endregion 添加普通产品的单证索引，不是配件产品

                        context.SaveChanges();
                    }
                }
            }
        }

        public VMInspectionClearance GetUploadList(VMERPUser currentUser, int InspectionClearanceID)
        {
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Inspection_InspectionClearance.Find(InspectionClearanceID);
                    if (query != null)
                    {
                        VMInspectionClearance vm = new VMInspectionClearance();
                        vm.InspectionClearanceID = query.InspectionClearanceID;

                        vm.FCRReceiveDateStartFormatter = Utils.DateTimeToStr(query.FCRReceiveDateStart);
                        vm.FCRReceiveDateEndFormatter = Utils.DateTimeToStr(query.FCRReceiveDateEnd);

                        vm.list_UploadFCR = ConstsMethod.GetUploadFileList(query.InspectionClearanceID, UploadFileType.InspectionClearance_FCR);
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

        public DBOperationStatus UploadFCR(VMERPUser currentUser, VMInspectionClearance vm)
        {
            DBOperationStatus result = default(DBOperationStatus);
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Inspection_InspectionClearance.Find(vm.InspectionClearanceID);
                    if (query != null)
                    {
                        query.StatusID = (int)InspectionClearanceStatusEnum.UploadedFCR;

                        if (!string.IsNullOrEmpty(vm.FCRReceiveDateStartFormatter))
                        {
                            query.FCRReceiveDateStart = Utils.StrToDateTime(vm.FCRReceiveDateStartFormatter);
                        }

                        if (!string.IsNullOrEmpty(vm.FCRReceiveDateEndFormatter))
                        {
                            query.FCRReceiveDateEnd = Utils.StrToDateTime(vm.FCRReceiveDateEndFormatter);
                        }

                        query.DT_MODIFYDATE = DateTime.Now;
                        query.ST_MODIFYUSER = currentUser.UserID;
                        query.IPAddress = CommonCode.GetIP();

                        Inspection_InspectionClearanceHis history = new Inspection_InspectionClearanceHis();
                        history.ST_CREATEUSER = currentUser.UserID;
                        history.DT_CREATEDATE = DateTime.Now;
                        history.IPAddress = CommonCode.GetIP();
                        history.StatusID = (int)InspectionClearanceStatusEnum.UploadedFCR;

                        query.Inspection_InspectionClearanceHis.Add(history);

                        ConstsMethod.SaveFileUpload(currentUser, query.InspectionClearanceID, vm.list_UploadFCR, context, UploadFileType.InspectionClearance_FCR);

                        int affectRows = context.SaveChanges();
                        if (affectRows == 0)
                        {
                            result = DBOperationStatus.NoAffect;
                        }
                        else
                        {
                            result = DBOperationStatus.Success;

                            SaveOther(currentUser, query.InspectionClearanceID);
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
                        //vm.SelectCustomer = SelectCustomerEnum.S164.ToString();//TODO 暂时的

                        List<string> list_PdfFile = new List<string>();
                        MakerExcel_Documents.Maker(vm, MakerTypeEnum.InspectionClearance_CommercialInvoice, vm.SelectCustomer, vm.InspectionClearanceID.ToString(), list_PdfFile);

                        #region 装箱单

                        if (vm.SelectCustomer == SelectCustomerEnum.S288.ToString())//S288一个柜子一个装箱单
                        {
                            var list_Product = vm.list_OrderProduct_PackingList;
                            var temp = list_Product.GroupBy(d => d.CabinetIndex);

                            int index = 1;

                            foreach (var item in temp)
                            {
                                var vm2 = new VMInspectionClearance();
                                vm2 = vm;
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

                                MakerExcel_Documents.Maker(vm2, MakerTypeEnum.InspectionClearance_PackingList, vm.SelectCustomer, vm.InspectionClearanceID.ToString(), list_PdfFile, index.ToString());
                                index++;
                            }
                        }
                        else
                        {
                            MakerExcel_Documents.Maker(vm, MakerTypeEnum.InspectionClearance_PackingList, vm.SelectCustomer, vm.InspectionClearanceID.ToString(), list_PdfFile);
                        }

                        #endregion 装箱单

                        if (vm.SelectCustomer == SelectCustomerEnum.S135.ToString())
                        {
                            MakerExcel_Documents.Maker(vm, MakerTypeEnum.InspectionClearance_ExcludingWoodPackagingDeclaration, vm.SelectCustomer, vm.InspectionClearanceID.ToString(), list_PdfFile);

                            MakerExcel_Documents.Maker(vm, MakerTypeEnum.InspectionClearance_CertificateOfOrigin, vm.SelectCustomer, vm.InspectionClearanceID.ToString(), list_PdfFile);
                        }

                        if (vm.SelectCustomer == SelectCustomerEnum.S220.ToString())
                        {
                            MakerExcel_Documents.Maker(vm, MakerTypeEnum.InspectionClearance_BeneficiaryStatement, vm.SelectCustomer, vm.InspectionClearanceID.ToString(), list_PdfFile);
                        }

                        if (vm.SelectCustomer == SelectCustomerEnum.S235.ToString())
                        {
                            MakerExcel_Documents.Maker(vm, MakerTypeEnum.InspectionClearance_CertificateOfOrigin, vm.SelectCustomer, vm.InspectionClearanceID.ToString(), list_PdfFile);
                            MakerExcel_Documents.Maker(vm, MakerTypeEnum.InspectionClearance_PackingListBuContainer, vm.SelectCustomer, vm.InspectionClearanceID.ToString(), list_PdfFile);
                        }

                        if (vm.list_ClearanceOther != null && vm.list_ClearanceOther.Count > 0)
                        {
                            foreach (var item in vm.list_ClearanceOther)
                            {
                                ConstsMethod.GetUploadToPdfList(list_PdfFile, item.ServerFileName);
                            }
                        }

                        makeFileList.Add(CommonCode.CreatePdfList(list_PdfFile, "InspectionClearance", vm.InspectionClearanceID.ToString()));
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

            string path = "/data/Template/Out/InspectionClearance/" + ID + "/PDFAndExcel";
            string zipPath = "/data/Template/Out/InspectionClearance/" + ID + "/" + CommonCode.GetTimeStamp() + ".zip";
            ExcelHelper.Zip(Utils.GetMapPath(path), Utils.GetMapPath(zipPath));//生成压缩文件

            makeFileList.Add(zipPath);

            return makeFileList;
        }

        public VMInspectionClearance GetUploadList_Modify(VMERPUser currentUser, int InspectionClearanceID)
        {
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Inspection_InspectionClearance.Find(InspectionClearanceID);
                    if (query != null)
                    {
                        VMInspectionClearance vm = new VMInspectionClearance();
                        vm.InspectionClearanceID = query.InspectionClearanceID;

                        vm.list_UploadModify = ConstsMethod.GetUploadFileList(query.InspectionClearanceID, UploadFileType.InspectionClearance_Modify);
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

        public DBOperationStatus UploadModify(VMERPUser currentUser, VMInspectionClearance vm)
        {
            DBOperationStatus result = default(DBOperationStatus);
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Inspection_InspectionClearance.Find(vm.InspectionClearanceID);
                    if (query != null)
                    {
                        ConstsMethod.SaveFileUpload(currentUser, query.InspectionClearanceID, vm.list_UploadModify, context, UploadFileType.InspectionClearance_Modify);

                        int affectRows = context.SaveChanges();
                        if (affectRows == 0)
                        {
                            result = DBOperationStatus.NoAffect;
                        }
                        else
                        {
                            result = DBOperationStatus.Success;

                            SaveOther(currentUser, query.InspectionClearanceID);
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

        /// <summary>
        /// 生成信用证文件
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public List<string> DownLoad_CreditNumber(VMERPUser currentUser, int ID)
        {
            List<string> makeFileList = new List<string>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var vm = GetDetailByID(currentUser, ID);
                    if (vm != null)
                    {
                        string filePath = "";
                        filePath = MakerExcel_Documents.Maker(vm, MakerTypeEnum.InspectionClearance_CreditNumber1, vm.SelectCustomer, vm.InspectionClearanceID.ToString());
                        filePath = MakerExcel_Documents.Maker(vm, MakerTypeEnum.InspectionClearance_CreditNumber2, vm.SelectCustomer, vm.InspectionClearanceID.ToString());
                        filePath = MakerExcel_Documents.Maker(vm, MakerTypeEnum.InspectionClearance_CreditNumber3, vm.SelectCustomer, vm.InspectionClearanceID.ToString());
                        filePath = MakerExcel_Documents.Maker(vm, MakerTypeEnum.InspectionClearance_CreditNumber4, vm.SelectCustomer, vm.InspectionClearanceID.ToString());
                        filePath = MakerExcel_Documents.Maker(vm, MakerTypeEnum.InspectionClearance_CreditNumber5, vm.SelectCustomer, vm.InspectionClearanceID.ToString());

                        ExcelHelper.Zip(Path.GetDirectoryName(filePath));//生成压缩文件
                        makeFileList.Add("/data/Template/Out/InspectionClearance/" + vm.InspectionClearanceID.ToString() + "/PDFAndExcel.zip");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return makeFileList;
        }
    }
}