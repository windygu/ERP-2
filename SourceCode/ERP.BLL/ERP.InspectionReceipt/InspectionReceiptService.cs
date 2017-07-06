using ERP.BLL.Consts;
using ERP.BLL.ERP.Consts;
using ERP.BLL.ERP.Dictionary;
using ERP.BLL.ERP.ShipmentOrder;
using ERP.BLL.Workflow;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.InspectionReceipt;
using ERP.Models.Order;
using ERP.Models.ShipmentOrder;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP.BLL.InspectionReceipt
{
    public class InspectionReceiptService
    {
        private DictionaryServices _dictionaryServices = new DictionaryServices();
        private ShipmentOrderService _shipmentOrderService = new ShipmentOrderService();

        public List<DTOInspectionReceiptList> GetAll(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows, VMFilterInspectionReceipt vm_search)
        {
            List<DTOInspectionReceiptList> listModel = null;
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Inspection_InspectionReceiptList.Where(p => !p.IsDelete && p.IsShowNeedInspection);

                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForInspectionReceipt);

                    if (currentUser.HierachyType == HierachyType.Agency)
                    {
                        var userHierachy = new List<int?>()
                        {
                            currentUser.HierachyID,
                            context.SystemUsers.FirstOrDefault(p => p.HierarchyID == currentUser.HierachyID).Hierarchy.Hierarchy2.HierarchyID
                        };

                        query = query.Where(p => userHierachy.Contains(p.Purchase_Contract.Factory.Hierarchy));
                    }

                    switch (vm_search.PageType)
                    {
                        case PageTypeEnum.PendingApproval:
                            query = query.Where(d => d.StatusID < (short)InspectionReceiptStatusEnum.PassedCheck);
                            break;

                        case PageTypeEnum.PassedApproval:
                            query = query.Where(d => d.StatusID >= (short)InspectionReceiptStatusEnum.PassedCheck);
                            break;

                        default:
                            break;
                    }

                    #region 筛选条件

                    if (!string.IsNullOrEmpty(vm_search.PurchaseNumber))
                    {
                        query = query.Where(p => p.Purchase_Contract.PurchaseNumber.Contains(vm_search.PurchaseNumber));
                    }
                    if (!string.IsNullOrEmpty(vm_search.FactoryAbbreviation))
                    {
                        query = query.Where(p => p.Purchase_Contract.Factory.Abbreviation.Contains(vm_search.FactoryAbbreviation));
                    }
                    if (!string.IsNullOrEmpty(vm_search.CustomerCode))
                    {
                        query = query.Where(p => p.Purchase_Contract.Orders_Customers.CustomerCode == vm_search.CustomerCode);
                    }
                    if (!string.IsNullOrEmpty(vm_search.CustomerCode))
                    {
                        query = query.Where(p => p.Purchase_Contract.Orders_Customers.CustomerCode == vm_search.CustomerCode);
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
                        listModel = new List<DTOInspectionReceiptList>();

                        List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());

                        foreach (var item in dataFromDB)
                        {
                            var inspectionReceipt = context.Inspection_InspectionReceipt.Where(p => p.ShipmentOrderID == item.ShipmentOrderID && p.HSID == item.HSCode).FirstOrDefault();

                            bool IsHasApprovalPermission = true;

                            #region 判断是否有审批流的权限

                            if (vm_search.PageType == PageTypeEnum.PendingApproval)//待审核
                            {
                                IsHasApprovalPermission = ApprovalService.HasApprovalPermission(WorkflowTypes.ApprovalInspectionReceipt, currentUser, item.ST_CREATEUSER, item.ApproverIndex, item.Purchase_Contract.CustomerID);
                            }

                            #endregion 判断是否有审批流的权限

                            #region 获取合并/分批订舱、合并订舱的订单编号

                            string OrderNumberList = "";
                            int tempOrderID = 0;
                            string tempOrderNumber = "";
                            string tempMerge = "";
                            List<string> listOrderNumberList = new List<string>();

                            var query_Delivery_ShipmentOrder = context.Delivery_ShipmentOrder.Where(d => d.ID == item.ShipmentOrderID).FirstOrDefault();
                            if (query_Delivery_ShipmentOrder != null)
                            {
                                tempOrderID = item.Purchase_Contract.OrderID ?? 0;
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

                            string CurrencyName = _dictionaryServices.GetDictionary_CurrencyName(item.Purchase_Contract.Factory.CurrencyType, list_Com_DataDictionary);

                            #region 判断是否需要我司报检报关

                            bool? IsNeedInspection = item.IsNeedInspection;

                            string IsNeedInspectionName = "";
                            if (IsNeedInspection.HasValue)
                            {
                                IsNeedInspectionName = IsNeedInspection.Value ? "是" : "否";
                            }

                            #endregion 判断是否需要我司报检报关

                            string HsCode = "";
                            string HsName = "";
                            if (inspectionReceipt != null && !string.IsNullOrEmpty(inspectionReceipt.HSCodeName))
                            {
                                HsName = inspectionReceipt.HSCodeName;
                            }
                            var query_HarmonizedSystems = context.HarmonizedSystems.Find(item.HSCode);
                            if (query_HarmonizedSystems != null)
                            {
                                HsCode = query_HarmonizedSystems.HSCode;
                                if (string.IsNullOrEmpty(HsName))
                                {
                                    HsName = query_HarmonizedSystems.CodeName;
                                }
                            }

                            listModel.Add(new DTOInspectionReceiptList()
                            {
                                ID = item.ID,
                                ShipmentOrderID = item.ShipmentOrderID,
                                ContractID = item.PurchaseContractID,
                                OrderID = tempOrderID,
                                OrderNumber = tempOrderNumber,
                                PurchaseNumber = item.Purchase_Contract.PurchaseNumber,
                                FactoryAbbreviation = item.Purchase_Contract.Factory.Abbreviation,
                                CustomerCode = item.Purchase_Contract.Orders_Customers.CustomerCode,
                                HSID = item.HSCode ?? 0,
                                HsCode = HsCode,
                                HsName = HsName,
                                Merge = tempMerge,
                                OrderNumberList = OrderNumberList,
                                StatusID = item.StatusID,
                                StatusName = CommonCode.GetStatusEnumName(item.StatusID, typeof(InspectionReceiptStatusEnum)),
                                UpdateDateForamtter = Utils.DateTimeToStr2(item.DT_MODIFYDATE),
                                ST_CREATEUSER = item.ST_CREATEUSER,
                                ApproverIndex = item.ApproverIndex,
                                CustomerID = item.Purchase_Contract.CustomerID,
                                IsHasApprovalPermission = IsHasApprovalPermission,
                                CurrencyName = CurrencyName,
                                IsNeedInspection = IsNeedInspection,
                                IsNeedInspectionName = IsNeedInspectionName,
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

        public List<DTOInspectionReceipt> GetDetailByID(VMERPUser currentUser, int ID)
        {
            List<DTOInspectionReceipt> list_vm = new List<DTOInspectionReceipt>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query_InspectionReceiptList = context.Inspection_InspectionReceiptList.Find(ID);
                    if (query_InspectionReceiptList != null)
                    {
                        List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                        List<DAL.HarmonizedSystem> list_HarmonizedSystem = context.HarmonizedSystems.Where(p => !p.IsDelete).ToList();

                        bool IsMerge = false;
                        List<string> list_POID = new List<string>();
                        var query_Delivery_ShipmentOrder = context.Delivery_ShipmentOrder.Find(query_InspectionReceiptList.ShipmentOrderID);
                        if (query_Delivery_ShipmentOrder != null)
                        {
                            if (query_Delivery_ShipmentOrder.IsMerge)
                            {
                                var list_OrderIDList = CommonCode.IdListToList(query_Delivery_ShipmentOrder.OrderIDList);
                                list_POID = context.Orders.Where(d => list_OrderIDList.Contains(d.OrderID) && !d.IsDelete).Select(d => d.POID).ToList();
                                IsMerge = true;
                            }
                            else
                            {
                                var query_Order = context.Orders.Find(query_Delivery_ShipmentOrder.OrderID);
                                if (query_Order != null)
                                {
                                    list_POID.Add(query_Order.POID);
                                }
                            }
                        }
                        List<string> list_ShippingDateStart = new List<string>();
                        List<string> list_OrderDateStart = new List<string>();


                        var query_InspectionReceipt = query_InspectionReceiptList.Inspection_InspectionReceipt;
                        if (query_InspectionReceipt.Count == 0 && IsMerge)
                        {
                            var query_temp = context.Inspection_InspectionReceiptList.Where(d => d.ShipmentOrderID == query_InspectionReceiptList.ShipmentOrderID && d.HSCode == query_InspectionReceiptList.HSCode && d.PurchaseContractID != query_InspectionReceiptList.PurchaseContractID && !d.IsDelete && d.ReceiptIndex == query_InspectionReceiptList.ReceiptIndex && d.Purchase_Contract.FactoryID == query_InspectionReceiptList.Purchase_Contract.FactoryID);
                            if (query_temp != null)
                            {
                                foreach (var item in query_temp)
                                {
                                    if (item.Inspection_InspectionReceipt.Count > 0)
                                    {
                                        query_InspectionReceipt = item.Inspection_InspectionReceipt;
                                    }
                                }
                            }
                        }
                        foreach (var item in query_InspectionReceipt)
                        {
                            DTOInspectionReceipt vm = new DTOInspectionReceipt();

                            vm.InvNo = item.InvNO;
                            vm.SCNO = item.SCNO;


                            #region 获取基本信息

                            vm.ApproverIndex = item.ApproverIndex;
                            vm.CreateUserID = item.CreateUserID;
                            vm.CustomerID = query_InspectionReceiptList.Purchase_Contract.CustomerID;
                            vm.TradeType = item.TradeType;

                            if (item.InspectionReceiptStatus == (int)InspectionReceiptStatusEnum.PendingMaintenance)//新建报检明细时
                            {
                                vm.CreateDate = DateTime.Now;
                                vm.CreateDateForamtter = Utils.DateTimeToStr(DateTime.Now);
                                vm.CreateUserName = currentUser.UserName;
                                vm.SaleContractContent = CommonCode.GetInspectionComment();
                                vm.ClaimFaxDate = "";
                            }
                            else
                            {
                                vm.CreateDate = item.CreateDate;
                                vm.CreateDateForamtter = Utils.DateTimeToStr(item.CreateDate);
                                vm.CreateUserName = item.SystemUser.UserName;
                                vm.SaleContractContent = item.SaleContractContent;
                                vm.ClaimFaxDate = Utils.DateTimeToStr(item.ClaimFaxDate);
                            }
                            vm.ShipmentOrderID = query_InspectionReceiptList.ShipmentOrderID ?? 0;
                            vm.InspectionReceiptStatusID = item.InspectionReceiptStatus;
                            vm.InspectionReceiptID = item.InspectionReceiptID;
                            vm.HSID = item.HSID;
                            vm.InspectionReceiptStatusID = item.InspectionReceiptStatus;
                            vm.ProductPrice = item.INProductPrice;
                            vm.INRemark = item.INRemark;
                            vm.AuditIdea = item.AuditIdea;

                            vm.ReceiptCommission = ConstsMethod.GetUploadFileList(item.InspectionReceiptID, UploadFileType.InspectionReceiptCommission);
                            vm.UploadReceipt = ConstsMethod.GetUploadFileList(item.InspectionReceiptID, UploadFileType.InspectionReceiptUploadReceipt);

                            vm.OrderID = item.OrderID;
                            vm.ContractID = query_InspectionReceiptList.PurchaseContractID ?? 0;//TODO 暂时的

                            var query_Purchase_Contract2 = query_InspectionReceiptList.Purchase_Contract;
                            var query_Orders_Customers = query_Purchase_Contract2.Orders_Customers;
                            vm.CustomerName = query_Orders_Customers.CustomerName;//客户名称
                            vm.CustomerCode = query_Purchase_Contract2.Orders_Customers.CustomerCode;

                            //客户基本地址
                            vm.CustomerStreet = query_Orders_Customers.StreetAddress + ",";
                            vm.CustomerReg = query_Orders_Customers.City + ","
                                + query_Orders_Customers.Reg_Area.AreaName + ","
                                + query_Orders_Customers.PostalCode + ","
                                + query_Orders_Customers.Reg_Country.CountryName;
                            vm.FactoryName = query_Purchase_Contract2.Factory.Name;
                            vm.FactoryContact = query_Purchase_Contract2.Factory.CallPeople;

                            var query_Orders_Contacts = query_Purchase_Contract2.Orders_Customers.Orders_Contacts.Where(d => d.IsDefault && !d.IsDelete);
                            if (query_Orders_Contacts.Count() > 0)
                            {
                                var query_this = query_Orders_Contacts.First();
                                vm.TheBuyer = query_this.FirstName + query_this.LastName;
                            }

                            vm.TeamOfThePayment = _dictionaryServices.GetDictionaryByName2(query_Orders_Customers.PaymentType, list_Com_DataDictionary);

                            vm.POID = CommonCode.ListToString(list_POID, ",");
                            vm.TradeType = item.TradeType;
                            vm.TradeTypeName = _dictionaryServices.GetDictionaryByName(item.TradeType, list_Com_DataDictionary);

                            var HSCodeID = item.HSID;
                            var dbHS = context.HarmonizedSystems.Find(HSCodeID);

                            if (dbHS != null)
                            {
                                vm.HSCodeID = HSCodeID;
                                vm.HsName = string.IsNullOrEmpty(item.HSCodeName) ? dbHS.CodeName : item.HSCodeName;
                                vm.HsEngName = string.IsNullOrEmpty(item.HSCodeEngName) ? dbHS.CodeEngName : item.HSCodeEngName;
                                vm.HSCodeName = dbHS.HSCode;
                                vm.INTypeName = ConstsMethod.GetHSCodeName(list_HarmonizedSystem, list_Com_DataDictionary, dbHS, HSCodeID);
                            }

                            #endregion 获取基本信息

                            #region 获取产品的信息

                            List<string> list_PurchaseNumber = new List<string>();
                            List<string> list_SCNo = new List<string>();
                            List<VMShipmentOrderProduct> list_ShipmentOrderProduct = new List<VMShipmentOrderProduct>();
                            var query_InspectionReceiptProduct = item.Inspection_InspectionReceiptProduct;
                            foreach (var item2 in query_InspectionReceiptProduct)
                            {
                                var item_product = item2.Delivery_ShipmentOrderProduct;

                                #region 从出运明细获取外箱长宽高

                                decimal ProductOuterLength = 0, ProductOuterWidth = 0, ProductOuterHeight = 0, OuterWeightGross = 0, OuterWeightNet = 0;

                                var dbEncasementProductInfo = item_product.OrderProduct.Delivery_EncasementsProducts.FirstOrDefault();
                                if (dbEncasementProductInfo != null)
                                {
                                    OuterWeightGross = dbEncasementProductInfo.WeightGross ?? 0;//产品单箱毛重
                                    OuterWeightNet = dbEncasementProductInfo.WeightNet ?? 0;//产品单箱净重
                                    ProductOuterLength = dbEncasementProductInfo.OuterLength ?? 0;
                                    ProductOuterHeight = dbEncasementProductInfo.OuterHeight ?? 0;
                                    ProductOuterWidth = dbEncasementProductInfo.OuterWidth ?? 0;
                                }

                                #endregion 从出运明细获取外箱长宽高


                                string PurchaseNumber = item_product.OrderProduct.Purchase_ContractProduct.First().Purchase_ContractBatch.Purchase_Contract.PurchaseNumber;
                                if (!list_PurchaseNumber.Contains(PurchaseNumber))
                                {
                                    list_PurchaseNumber.Add(PurchaseNumber);
                                }
                                if (!list_SCNo.Contains(item_product.SCNo))
                                {
                                    list_SCNo.Add(item_product.SCNo);
                                }


                                int OuterBoxRate = item_product.OrderProduct.OuterBoxRate ?? 0;//产品外箱率
                                int ProductsBoxNum = item2.SelectBoxQty ?? 0;//箱数

                                vm.ProductsQuauity += item2.Qty ?? 0;//数量=外箱率*箱数
                                vm.ProductsBoxNum += ProductsBoxNum;

                                vm.WeightGrossSum += Math.Round(OuterWeightGross * ProductsBoxNum, 0);//总毛重=单箱毛重×产品箱数
                                vm.WeightNetSum += Math.Round(OuterWeightNet * ProductsBoxNum, 0);//总净重=单箱净重×产品箱数

                                decimal molecular = ProductOuterLength * ProductOuterWidth * ProductOuterHeight;//外箱长×宽×高
                                molecular = (molecular * ProductsBoxNum) / 1000000;//转换为立方米
                                vm.CUFT += Math.Round(molecular, 2);

                                //只取一个产品货号、内盒率,并且多于一个产品时，赋空值（产品外箱率也是如此）

                                vm.OuterBoxRate = OuterBoxRate;
                                vm.SMCustoerPO = item_product.OrderProduct.Order.POID;

                                vm.CustomerDate = item_product.OrderProduct.Order.CustomerDate;
                                vm.CustomerDateFormatter = Utils.DateTimeToStr(item_product.OrderProduct.Order.CustomerDate);

                                vm.INPortName = _dictionaryServices.GetDictionary_PortEnName(item_product.Delivery_ShipmentOrderCabinet.Delivery_ShipmentOrder.PortID, list_Com_DataDictionary);

                                vm.PortName = _dictionaryServices.GetDictionary_PortName(item_product.Delivery_ShipmentOrderCabinet.Delivery_ShipmentOrder.PortID, list_Com_DataDictionary);

                                var ShippingDateStart = CommonCode.GetDateTime3(item_product.Delivery_ShipmentOrderCabinet.ShippingDateStart);
                                if (!string.IsNullOrEmpty(ShippingDateStart) && !list_ShippingDateStart.Contains(ShippingDateStart))
                                {
                                    list_ShippingDateStart.Add(ShippingDateStart);
                                }

                                var OrderDateStart = Utils.DateTimeToStr(item_product.OrderProduct.Order.OrderDateStart);
                                if (!string.IsNullOrEmpty(OrderDateStart) && !list_OrderDateStart.Contains(OrderDateStart))
                                {
                                    list_OrderDateStart.Add(OrderDateStart);
                                }

                                var query_Com_DataDictionary = list_Com_DataDictionary.Where(c => c.Code == item_product.Delivery_ShipmentOrderCabinet.ShipToPortID && c.TableKind == (int)DictionaryTableKind.GoalPort).FirstOrDefault();
                                if (query_Com_DataDictionary != null)
                                {
                                    vm.INEndPortName = query_Com_DataDictionary.Name;

                                    #region 获取目的港的地址

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
                                }

                                var shipmentOrderProduct = new ShipmentOrderService().GetProductInfo(context, item_product.OrderProduct, item_product.BoxQty, item_product.Qty, list_Com_DataDictionary, list_HarmonizedSystem);
                                shipmentOrderProduct.SumPrice = item_product.Qty * item.INProductPrice;
                                shipmentOrderProduct.ProductPrice = item.INProductPrice;
                                shipmentOrderProduct.HsEngName = vm.HsEngName;
                                list_ShipmentOrderProduct.Add(shipmentOrderProduct);

                                #region 获取季节

                                string SeasonPrefix = "";
                                string SeasonSuffix = "";
                                string SeasonName = "";
                                string SeasonAlias = "";
                                string SeasonDepartmentNumber = "";

                                var query_TempSeason = list_Com_DataDictionary.Where(d => d.TableKind == (int)DictionaryTableKind.Season && d.Code == item_product.OrderProduct.Season);
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

                                vm.SeasonPrefix = SeasonPrefix;

                                #endregion 获取季节
                            }
                            vm.list_ShipmentOrderProduct = list_ShipmentOrderProduct;

                            vm.ProductAmount = Math.Round(vm.ProductPrice * vm.ProductsQuauity, 2);
                            vm.PurchaseNumber = CommonCode.ListToString(list_PurchaseNumber);
                            vm.SCNOList = CommonCode.ListToString(list_SCNo);

                            #endregion 获取产品的信息

                            vm.SelectCustomer = query_Orders_Customers.SelectCustomer;

                            string CurrencyName2 = _dictionaryServices.GetDictionary_CurrencyName(query_Purchase_Contract2.Factory.CurrencyType, list_Com_DataDictionary);
                            var IsNeedInspection = query_InspectionReceiptList.IsNeedInspection;
                            if (CurrencyName2 == Keys.USD && IsNeedInspection.HasValue && !IsNeedInspection.Value)//如果是美金工厂 并且IsNeedInspection=false 就不需要报检
                            {
                                continue;
                            }

                            var query_history = context.Inspection_InspectionReceiptHis.Where(d => d.InspectionReceiptID == vm.InspectionReceiptID);
                            List<VMInspectionReceiptHis> list_history = new List<VMInspectionReceiptHis>();
                            foreach (var item2 in query_history)
                            {
                                list_history.Add(new VMInspectionReceiptHis()
                                {
                                    AuditCreateDate = Utils.DateTimeToStr2(item2.CreateDate),
                                    AuditIdea = item2.AuditIdea,
                                    AuditUserName = item2.SystemUser.UserName,
                                    InspectionReceiptStatus = CommonCode.GetStatusEnumName(item2.InspectionReceiptStatus, typeof(InspectionReceiptStatusEnum))
                                });
                            };
                            vm.InspectionReceiptHis = list_history;

                            list_vm.Add(vm);
                        }

                        foreach (var item in list_vm)
                        {
                            item.ShippingDateStart = CommonCode.ListToString(list_ShippingDateStart);
                            if (list_OrderDateStart.Count > 0)
                            {
                                item.OrderDateStart = list_OrderDateStart.First();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return list_vm;
        }

        public DBOperationStatus Save(VMERPUser currentUser, List<DTOInspectionReceipt> list_vm)
        {
            DBOperationStatus result = default(DBOperationStatus);
            try
            {
                int affectRows = 0;
                int StatusID = list_vm.FirstOrDefault().InspectionReceiptStatusID;
                string AuditIdea = list_vm.FirstOrDefault().AuditIdea;
                string CheckSuggest = "";

                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    foreach (var item in list_vm)
                    {
                        var query = context.Inspection_InspectionReceipt.Find(item.InspectionReceiptID);

                        if (query.InspectionReceiptStatus == (int)InspectionReceiptStatusEnum.PendingMaintenance)//待报检
                        {
                            query.CreateDate = DateTime.Now;
                            query.CreateUserID = currentUser.UserID;
                            query.CreateTerminal = CommonCode.GetIP();
                        }

                        query.UpdateUserID = currentUser.UserID;
                        query.UpdateTerminal = CommonCode.GetIP();
                        query.UpdateDate = DateTime.Now;

                        if (StatusID != (int)InspectionReceiptStatusEnum.PassedCheck && StatusID != (int)InspectionReceiptStatusEnum.NotPassCheck)
                        {
                            query.InspectionReceiptStatus = StatusID;
                            query.TradeType = item.TradeType;
                            query.HSCodeName = item.HsName;
                            query.HSCodeEngName = item.HsEngName;
                        }
                        if (StatusID == (int)InspectionReceiptStatusEnum.PassedCheck || StatusID == (int)InspectionReceiptStatusEnum.NotPassCheck)
                        {
                            CheckSuggest = AuditIdea;
                        }

                        if (StatusID == (int)InspectionReceiptStatusEnum.PendingMaintenance || StatusID == (int)InspectionReceiptStatusEnum.OutLine || StatusID == (int)InspectionReceiptStatusEnum.PendingCheck)
                        {
                            query.SCNO = item.SCNO;
                            query.ClaimFaxDate = Utils.StrToDateTime(item.ClaimFaxDate);
                            query.PortID = item.INPortID_1;
                            query.INProductPrice = item.ProductPrice;
                            query.INRemark = item.INRemark;
                            query.SaleContractContent = item.SaleContractContent;
                            query.AuditIdea = CheckSuggest;
                        }

                        Inspection_InspectionReceiptHis efIRHis = new Inspection_InspectionReceiptHis()
                        {
                            InspectionReceiptStatus = StatusID,
                            AuditUserID = currentUser.UserID,
                            AuditIdea = CheckSuggest,
                            CreateDate = DateTime.Now,
                        };

                        query.Inspection_InspectionReceiptHis.Add(efIRHis);

                        ConstsMethod.SaveFileUpload(currentUser, item.InspectionReceiptID, item.ReceiptCommission, context, UploadFileType.InspectionReceiptCommission);

                        affectRows = context.SaveChanges();

                        if (affectRows == 0)
                        {
                            result = DBOperationStatus.NoAffect;
                        }
                        else
                        {
                            bool bIsPass = true;
                            if (StatusID == (int)InspectionReceiptStatusEnum.NotPassCheck)
                            {
                                bIsPass = false;
                            }

                            List<int> listStatus = new List<int>();
                            listStatus.Add((int)InspectionReceiptStatusEnum.PendingCheck);
                            listStatus.Add((int)InspectionReceiptStatusEnum.NotPassCheck);
                            int[] iArrLogicStatus = { (int)InspectionReceiptStatusEnum.PendingCheck, (int)InspectionReceiptStatusEnum.NotPassCheck, (int)InspectionReceiptStatusEnum.PassedCheck };

                            ConstsMethod.InsertAuditStream(item.InspectionReceiptID, bIsPass, WorkflowTypes.ApprovalInspectionReceipt, currentUser.UserID, listStatus, iArrLogicStatus, AuditIdea);

                            result = DBOperationStatus.Success;
                        }
                    }
                }

                int tempID = 0;
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    #region 同步更新报检列表的创建人、ApproverIndex

                    foreach (var item in list_vm)
                    {
                        var query_InspectionReceipt = context.Inspection_InspectionReceipt.Find(item.InspectionReceiptID);

                        var query_InspectionReceiptList = query_InspectionReceipt.Inspection_InspectionReceiptList;

                        tempID = query_InspectionReceiptList.ID;

                        query_InspectionReceiptList.StatusID = query_InspectionReceipt.InspectionReceiptStatus;
                        query_InspectionReceiptList.ApproverIndex = query_InspectionReceipt.ApproverIndex;
                        query_InspectionReceiptList.ST_MODIFYUSER = query_InspectionReceipt.UpdateUserID ?? 0;
                        query_InspectionReceiptList.DT_MODIFYDATE = query_InspectionReceipt.UpdateDate;
                    }
                    context.SaveChanges();

                    #endregion 同步更新报检列表的创建人、ApproverIndex
                }

                SaveOther(tempID);
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
                result = DBOperationStatus.NoAffect;
            }

            return result;
        }

        /// <summary>
        /// 合并订舱，同一个工厂、同一个HSCode报检时，同步更新。
        /// </summary>
        /// <param name="InspectionReceiptID"></param>
        public void SaveOther(int InspectionReceiptID, int? tempReceiptIndex = null)
        {

            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                var query = context.Inspection_InspectionReceiptList.Find(InspectionReceiptID);
                if (query != null)
                {
                    bool IsMerge = context.Delivery_ShipmentOrder.Find(query.ShipmentOrderID).IsMerge;
                    if (IsMerge)
                    {
                        //合并订舱，同一个工厂、同一个HSCode报检时，同步更新。
                        var query_temp = context.Inspection_InspectionReceiptList.Where(d => d.ShipmentOrderID == query.ShipmentOrderID && d.HSCode == query.HSCode && d.PurchaseContractID != query.PurchaseContractID && d.Purchase_Contract.FactoryID == query.Purchase_Contract.FactoryID && !d.IsDelete);
                        if (tempReceiptIndex == null)
                        {
                            query_temp = query_temp.Where(d => d.ReceiptIndex == query.ReceiptIndex);
                        }
                        else
                        {
                            query_temp = query_temp.Where(d => !d.ReceiptIndex.HasValue);

                        }

                        #region 同步更新报检列表的创建人、ApproverIndex

                        foreach (var item in query_temp)
                        {
                            item.StatusID = query.StatusID;
                            if (tempReceiptIndex.HasValue)
                            {
                                item.ReceiptIndex = tempReceiptIndex;
                            }
                            item.ApproverIndex = query.ApproverIndex;
                            item.ST_MODIFYUSER = query.ST_MODIFYUSER;
                            item.DT_MODIFYDATE = query.DT_MODIFYDATE;
                        }
                        context.SaveChanges();

                        #endregion 同步更新报检列表的创建人、ApproverIndex
                    }
                }
            }
        }

        public string GetFactoryEmail(int ID)
        {
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                var query = context.Inspection_InspectionReceiptList.Find(ID);
                if (query != null)
                {
                    return query.Purchase_Contract.Factory.EmailAdress;
                }
                return "";
            }
        }

        public DBOperationStatus SendEmail(VMERPUser currentUser, int ID, VMSendEmail vm)
        {
            DBOperationStatus result = default(DBOperationStatus);
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Inspection_InspectionReceiptList.Find(ID);
                    if (query != null)
                    {
                        query.StatusID = (int)InspectionReceiptStatusEnum.Sended;
                        query.ST_MODIFYUSER = currentUser.UserID;
                        query.DT_MODIFYDATE = DateTime.Now;
                        query.IPAddress = CommonCode.GetIP();

                        foreach (var item in query.Inspection_InspectionReceipt)
                        {
                            item.InspectionReceiptStatus = (int)InspectionReceiptStatusEnum.Sended;
                            item.UpdateUserID = currentUser.UserID;
                            item.UpdateDate = DateTime.Now;
                            item.UpdateTerminal = CommonCode.GetIP();

                            Inspection_InspectionReceiptHis efIRHis = new Inspection_InspectionReceiptHis();
                            efIRHis.InspectionReceiptID = item.InspectionReceiptID;
                            efIRHis.AuditUserID = currentUser.UserID;
                            efIRHis.CreateDate = DateTime.Now;
                            efIRHis.InspectionReceiptStatus = (int)InspectionReceiptStatusEnum.Sended;

                            context.Inspection_InspectionReceiptHis.Add(efIRHis);

                            // 前台发送邮件的已上传附件，将附件URL替换为本地文件地址
                            vm.Attachs = ConstsMethod.ReplaceURLToLocalPath(vm.Attachs);

                            //查询“委托书”标签页已上传附件，把它们放在邮件附件中
                            var dbUpLoadFileList = context.UpLoadFiles.Where(p => p.LinkID == item.InspectionReceiptID && !p.IsDelete && p.ModuleType == (int)UploadFileType.InspectionReceiptCommission);

                            //把“委托书”标签页已上传附件放在邮件附件中
                            foreach (var item1 in dbUpLoadFileList)
                            {
                                if (string.IsNullOrEmpty(item1.ServerFileName))
                                {
                                    if (string.IsNullOrEmpty(vm.Attachs))
                                    {
                                        vm.Attachs += ConstsMethod.ReplaceURLToLocalPath(item1.ServerFileName);
                                    }
                                    else
                                    {
                                        vm.Attachs += ";" + ConstsMethod.ReplaceURLToLocalPath(item1.ServerFileName);
                                    }
                                }
                            }
                        }

                        int affectRows = context.SaveChanges();
                        //Save_Other(ShipmentOrderID);

                        SaveOther(ID);

                        #region 生成Excel文件

                        List<MakerTypeEnum> templeteList = new List<MakerTypeEnum>();
                        templeteList.Add(MakerTypeEnum.InspectionReceipt_PackingList);
                        templeteList.Add(MakerTypeEnum.InspectionReceipt_CommercialInvoice);
                        templeteList.Add(MakerTypeEnum.InspectionReceipt_SaleContract);
                        templeteList.Add(MakerTypeEnum.InspectionReceipt_Home);

                        List<string> list_InspectionReceiptID_InvoiceNo = new List<string>();
                        List<string> makeFileList = DownLoad(currentUser, ID, ref list_InspectionReceiptID_InvoiceNo);

                        #endregion 生成Excel文件

                        #region 发送邮件

                        //把标签页生成的数据文件放在邮件附件中
                        foreach (var itme1 in makeFileList)
                        {
                            if (string.IsNullOrEmpty(vm.Attachs))
                            {
                                vm.Attachs += Utils.GetMapPath("~" + itme1);
                            }
                            else
                            {
                                vm.Attachs += ";" + Utils.GetMapPath("~" + itme1);
                            }
                        }

                        Email.SendEmail(currentUser.UserName, vm);//发送电子邮件

                        #endregion 发送邮件

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

        //private void Save_Other(int ShipmentOrderID)
        //{
        //    #region 同步更新报检列表

        //    using (ERPEntitiesNew context2 = new ERPEntitiesNew())
        //    {
        //        var query_InspectionReceipt = context2.Inspection_InspectionReceipt.Where(d => d.ShipmentOrderID == ShipmentOrderID).OrderByDescending(d => d.UpdateDate).FirstOrDefault();
        //        var query_InspectionReceiptList = context2.Inspection_InspectionReceiptList.Where(d => d.ShipmentOrderID == ShipmentOrderID && !d.IsDelete);
        //        foreach (var item in query_InspectionReceiptList)
        //        {
        //            item.StatusID = query_InspectionReceipt.InspectionReceiptStatus;
        //            item.ST_MODIFYUSER = query_InspectionReceipt.UpdateUserID ?? 0;
        //            item.DT_MODIFYDATE = query_InspectionReceipt.UpdateDate;
        //        }
        //        context2.SaveChanges();
        //    }

        //    #endregion 同步更新报检列表
        //}

        public List<DTOInspectionReceipt> GetUploadList(VMERPUser currentUser, int ID)
        {
            List<DTOInspectionReceipt> list = new List<DTOInspectionReceipt>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query_InspectionReceiptList = context.Inspection_InspectionReceiptList.Find(ID);
                    if (query_InspectionReceiptList != null)
                    {
                        List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                        var query_InspectionReceipt = query_InspectionReceiptList.Inspection_InspectionReceipt;

                        bool IsMerge = context.Delivery_ShipmentOrder.Find(query_InspectionReceiptList.ShipmentOrderID).IsMerge;

                        if (query_InspectionReceipt.Count == 0 && IsMerge)
                        {
                            var query_temp = context.Inspection_InspectionReceiptList.Where(d => d.ShipmentOrderID == query_InspectionReceiptList.ShipmentOrderID && d.HSCode == query_InspectionReceiptList.HSCode && d.PurchaseContractID != query_InspectionReceiptList.PurchaseContractID && !d.IsDelete && d.ReceiptIndex == query_InspectionReceiptList.ReceiptIndex && d.Purchase_Contract.FactoryID == query_InspectionReceiptList.Purchase_Contract.FactoryID);
                            if (query_temp != null)
                            {
                                foreach (var item in query_temp)
                                {
                                    if (item.Inspection_InspectionReceipt.Count > 0)
                                    {
                                        query_InspectionReceipt = item.Inspection_InspectionReceipt;
                                    }
                                }
                            }
                        }

                        foreach (var item in query_InspectionReceipt)
                        {
                            List<int> list_ContractID = CommonCode.IdListToList(item.ContractIDList);
                            var query_Purchase_Contract = query_InspectionReceiptList.Purchase_Contract;

                            string CurrencyName2 = _dictionaryServices.GetDictionary_CurrencyName(query_Purchase_Contract.Factory.CurrencyType, list_Com_DataDictionary);
                            var IsNeedInspection = query_InspectionReceiptList.IsNeedInspection;
                            if (CurrencyName2 == Keys.USD && IsNeedInspection.HasValue && !IsNeedInspection.Value)//如果是美金工厂 并且IsNeedInspection=false 就不需要报检
                            {
                                continue;
                            }

                            DTOInspectionReceipt vm = new DTOInspectionReceipt();
                            vm.InspectionReceiptID = item.InspectionReceiptID;
                            vm.InspectionReceiptListID = ID;
                            vm.ShipmentOrderID = query_InspectionReceiptList.ShipmentOrderID ?? 0;
                            vm.InvNo = item.InvNO;
                            vm.InspectionReceiptStatusID = item.InspectionReceiptStatus;
                            vm.UploadReceipt = ConstsMethod.GetUploadFileList(item.InspectionReceiptID, UploadFileType.InspectionReceiptUploadReceipt);

                            list.Add(vm);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return list;
        }

        public DBOperationStatus Upload(VMERPUser currentUser, List<DTOInspectionReceipt> list_vm)
        {
            DBOperationStatus result = default(DBOperationStatus);
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    foreach (var item in list_vm)
                    {
                        var query = context.Inspection_InspectionReceiptList.Find(item.InspectionReceiptListID);
                        if (query != null)
                        {
                            query.StatusID = item.InspectionReceiptStatusID;
                            query.ST_MODIFYUSER = currentUser.UserID;
                            query.DT_MODIFYDATE = DateTime.Now;
                            query.IPAddress = CommonCode.GetIP();

                            foreach (var item2 in query.Inspection_InspectionReceipt)
                            {
                                item2.InspectionReceiptStatus = item.InspectionReceiptStatusID;
                                item2.UpdateDate = DateTime.Now;
                                item2.UpdateUserID = currentUser.UserID;
                                item2.UpdateTerminal = CommonCode.GetIP();

                                Inspection_InspectionReceiptHis history = new Inspection_InspectionReceiptHis();
                                history.AuditUserID = currentUser.UserID;
                                history.CreateDate = DateTime.Now;
                                history.InspectionReceiptStatus = item.InspectionReceiptStatusID;

                                item2.Inspection_InspectionReceiptHis.Add(history);

                            }

                            //保存上传附件列表数据
                            if (item.UploadReceipt != null)
                            {
                                ConstsMethod.SaveFileUpload(currentUser, item.InspectionReceiptID, item.UploadReceipt, context, UploadFileType.InspectionReceiptUploadReceipt);
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
                        result = DBOperationStatus.Success;

                        SaveOther(list_vm.FirstOrDefault().InspectionReceiptListID);
                    }

                    #region 报检上传凭条后，新建报关列表

                    int ShipmentOrderID = list_vm.FirstOrDefault().ShipmentOrderID;
                    var query_InspectionReceiptList = context.Inspection_InspectionReceiptList.Where(d => d.ShipmentOrderID == ShipmentOrderID && d.StatusID != (int)InspectionReceiptStatusEnum.Uploaded && !d.IsDelete && d.IsShowNeedInspection);
                    if (query_InspectionReceiptList.Count() == 0)
                    {
                        var query_InspectionCustoms = context.Inspection_InspectionCustoms.Where(d => d.ShipmentOrderID == ShipmentOrderID && !d.IsDelete);
                        if (query_InspectionCustoms.Count() == 0)
                        {
                            DAL.Inspection_InspectionCustoms query = new Inspection_InspectionCustoms()
                            {
                                ST_CREATEUSER = currentUser.UserID,
                                DT_CREATEDATE = DateTime.Now,
                                ST_MODIFYUSER = currentUser.UserID,
                                DT_MODIFYDATE = DateTime.Now,
                                IsDelete = false,
                                IPAddress = CommonCode.GetIP(),

                                ShipmentOrderID = ShipmentOrderID,
                                StatusID = (int)InspectionCustomsStatusEnum.PendingMaintenance,
                                IsShowNeedInspection = true,
                                IsNeedInspection = true,
                            };

                            context.Inspection_InspectionCustoms.Add(query);
                        }
                        context.SaveChanges();
                    }

                    #endregion 报检上传凭条后，新建报关列表
                }
                //Save_Other(list_vm.FirstOrDefault().ShipmentOrderID);
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
                result = DBOperationStatus.NoAffect;
            }
            return result;
        }

        public VMAjaxProcessResult CreateShippingMark(VMERPUser currentUser, int InspectionReceiptID)
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

                    var query = context.Inspection_InspectionReceipt.Find(InspectionReceiptID);
                    foreach (var item in query.Inspection_InspectionReceiptProduct.Where(d => !d.IsDelete))
                    {
                        if (item.ShipmentOrderProductID.HasValue && !list_ShipmentOrderProductID.Contains(item.ShipmentOrderProductID))
                        {
                            list_ShipmentOrderProductID.Add(item.ShipmentOrderProductID);

                            var item_product = context.Delivery_ShipmentOrderProduct.Find(item.ShipmentOrderProductID);
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

        public List<string> DownLoad(VMERPUser currentUser, int ID, ref List<string> list_InspectionReceiptID_InvoiceNo)
        {
            List<string> makeFileList = new List<string>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var vm_list = GetDetailByID(currentUser, ID);
                    if (vm_list != null)
                    {
                        foreach (var vm in vm_list)
                        {
                            list_InspectionReceiptID_InvoiceNo.Add(vm.InspectionReceiptID + "_" + vm.InvNo);
                            //vm.SelectCustomer = "S188";//TODO 待定
                            List<string> list_PdfFile = new List<string>();
                            MakerExcel_Documents.Maker(vm, MakerTypeEnum.InspectionReceipt_Home, vm.SelectCustomer, vm.InspectionReceiptID.ToString(), list_PdfFile);
                            MakerExcel_Documents.Maker(vm, MakerTypeEnum.InspectionReceipt_SaleContract, vm.SelectCustomer, vm.InspectionReceiptID.ToString(), list_PdfFile);
                            MakerExcel_Documents.Maker(vm, MakerTypeEnum.InspectionReceipt_CommercialInvoice, vm.SelectCustomer, vm.InspectionReceiptID.ToString(), list_PdfFile);
                            MakerExcel_Documents.Maker(vm, MakerTypeEnum.InspectionReceipt_PackingList, vm.SelectCustomer, vm.InspectionReceiptID.ToString(), list_PdfFile);

                            if (vm.ReceiptCommission != null && vm.ReceiptCommission.Count > 0)
                            {
                                foreach (var item in vm.ReceiptCommission)
                                {
                                    ConstsMethod.GetUploadToPdfList(list_PdfFile, item.ServerFileName);
                                }
                            }
                            makeFileList.Add(CommonCode.CreatePdfList(list_PdfFile, "InspectionReceipt", vm.InspectionReceiptID.ToString()));
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
            List<string> list_InspectionReceiptID_InvoiceNo = new List<string>();
            DownLoad(currentUser, ID, ref list_InspectionReceiptID_InvoiceNo);
            List<string> makeFileList = new List<string>();

            foreach (var item in list_InspectionReceiptID_InvoiceNo)
            {

                string path = "/data/Template/Out/InspectionReceipt/" + item.Split('_')[0] + "/PDFAndExcel";
                string zipPath = "/data/Template/Out/InspectionReceipt/" + item.Split('_')[0] + "/" + item.Split('_')[1] + "_" + CommonCode.GetTimeStamp() + ".zip";
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
        public DBOperationStatus IsNeedInspection(VMERPUser currentUser, int ID, int shipmentOrderID, bool IsNeedInspection)
        {
            DBOperationStatus result = default(DBOperationStatus);
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query2 = context.Inspection_InspectionReceiptList.Find(ID);
                    if (query2 != null)
                    {
                        query2.IsNeedInspection = IsNeedInspection;
                        query2.ST_MODIFYUSER = currentUser.UserID;
                        query2.DT_MODIFYDATE = DateTime.Now;

                        if (!IsNeedInspection)
                        {
                            query2.IsShowNeedInspection = false;
                        }
                        int affectRows = context.SaveChanges();
                    }

                    var query = context.Inspection_InspectionReceiptList.Where(d => d.ShipmentOrderID == shipmentOrderID && !d.IsDelete);
                    if (query != null && query.Count() > 0)
                    {
                        List<int> list_ShipmentOrderID = new List<int>();

                        List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());

                        var query_Delivery_ShipmentOrder = context.Delivery_ShipmentOrder.Where(d => d.ID == shipmentOrderID).FirstOrDefault();
                        if (query_Delivery_ShipmentOrder != null)
                        {
                            if (query_Delivery_ShipmentOrder.IsMerge)
                            {
                                list_ShipmentOrderID.AddRange(context.Delivery_ShipmentOrder.Where(d => !d.IsDelete && d.OrderIDList == query_Delivery_ShipmentOrder.OrderIDList).Select(d => d.ID).ToList());
                            }
                            else
                            {
                                list_ShipmentOrderID.Add(shipmentOrderID);
                            }
                        }

                        bool HasUSDFactory = false;
                        foreach (var item in query.Where(d => !d.IsNeedInspection.HasValue))//IsShowNeedInspection为空时
                        {
                            string CurrencyName = _dictionaryServices.GetDictionary_CurrencyName(item.Purchase_Contract.Factory.CurrencyType, list_Com_DataDictionary);
                            if (CurrencyName == Keys.USD)
                            {
                                HasUSDFactory = true;
                            }
                        }

                        foreach (var item in query.Where(d => !d.IsNeedInspection.HasValue))//IsShowNeedInspection为空时
                        {
                            string CurrencyName = _dictionaryServices.GetDictionary_CurrencyName(item.Purchase_Contract.Factory.CurrencyType, list_Com_DataDictionary);

                            if (CurrencyName == Keys.RMB && !HasUSDFactory)//如果只存在人民币工厂
                            {
                                item.IsNeedInspection = true;
                                item.ST_MODIFYUSER = currentUser.UserID;
                                item.DT_MODIFYDATE = DateTime.Now;
                            }
                        }



                        if (!IsNeedInspection)//不需要我司报检
                        {
                            bool HasUSDFactory2 = false;
                            bool HasRMBFactory2 = false;
                            foreach (var item in query)
                            {
                                string CurrencyName = _dictionaryServices.GetDictionary_CurrencyName(item.Purchase_Contract.Factory.CurrencyType, list_Com_DataDictionary);
                                if (CurrencyName == Keys.USD)
                                {
                                    HasUSDFactory2 = true;
                                }
                                if (CurrencyName == Keys.RMB)
                                {
                                    HasRMBFactory2 = true;
                                }
                            }


                            if (HasUSDFactory2 && !HasRMBFactory2)//只存在美金工厂，添加报关数据
                            {
                                var query_InspectionCustomss = context.Inspection_InspectionCustoms.Where(d => list_ShipmentOrderID.Contains(d.ShipmentOrderID) && !d.IsDelete);
                                if (query_InspectionCustomss.Count() == 0)
                                {
                                    DAL.Inspection_InspectionCustoms query_InspectionCustoms = new Inspection_InspectionCustoms()
                                    {
                                        ST_CREATEUSER = currentUser.UserID,
                                        DT_CREATEDATE = DateTime.Now,
                                        ST_MODIFYUSER = currentUser.UserID,
                                        DT_MODIFYDATE = DateTime.Now,
                                        IsDelete = false,
                                        IPAddress = CommonCode.GetIP(),

                                        ShipmentOrderID = shipmentOrderID,
                                        StatusID = (int)InspectionCustomsStatusEnum.PendingMaintenance,

                                        IsNeedInspection = null,
                                        IsShowNeedInspection = true,
                                    };

                                    context.Inspection_InspectionCustoms.Add(query_InspectionCustoms);
                                    context.SaveChanges();
                                }
                            }
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

        public List<VMShipmentOrder> GetDetailByID_Add(VMERPUser currentUser, int id)
        {
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Inspection_InspectionReceiptList.Find(id);
                    if (query != null)
                    {
                        var list_vm = _shipmentOrderService.GetDetailByID_ForEditPage(currentUser, query.ShipmentOrderID ?? 0, 0, query.Purchase_Contract.FactoryID, query.HSCode);

                        List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                        List<DAL.HarmonizedSystem> list_HarmonizedSystem = context.HarmonizedSystems.Where(p => !p.IsDelete).ToList();

                        foreach (var item in list_vm)
                        {
                            List<VMShipmentOrderCabinet> list_cabinet = new List<VMShipmentOrderCabinet>();
                            if (item.list_cabinet != null)
                            {
                                foreach (var item2 in item.list_cabinet)
                                {
                                    VMShipmentOrderCabinet cabinet = item2;

                                    List<VMShipmentOrderProduct> list_product = new List<VMShipmentOrderProduct>();

                                    foreach (var item3 in item2.list_product)
                                    {
                                        VMShipmentOrderProduct product = item3;
                                        var query2 = context.Inspection_InspectionReceiptProduct.Where(d => d.ShipmentOrderProductID == item3.ID && !d.IsDelete);
                                        int? SelectBoxQty = query2.Select(d => d.SelectBoxQty).Sum();
                                        if (SelectBoxQty.HasValue)
                                        {
                                            if (item3.SelectBoxQty > SelectBoxQty)
                                            {
                                                int? SumBoxQty = item3.SelectBoxQty - SelectBoxQty;

                                                int? Qty = Utils.StrToInt((SumBoxQty * item3.OuterBoxRate.Round(0)).ToString(), 0);

                                                product = _shipmentOrderService.GetProductInfo(context, query2.First().Delivery_ShipmentOrderProduct.OrderProduct, SumBoxQty, Qty, list_Com_DataDictionary, list_HarmonizedSystem);

                                                product.ID = query2.First().ShipmentOrderProductID ?? 0;
                                                product.SumBoxQty = SumBoxQty;
                                                product.SelectBoxQty = SumBoxQty;
                                                product.SCNo = query2.First().Delivery_ShipmentOrderProduct.SCNo;
                                                product.InvoiceNo = query2.First().Delivery_ShipmentOrderProduct.InvoiceNo;

                                                list_product.Add(product);
                                            }
                                            else if (item3.SelectBoxQty != SelectBoxQty)
                                            {
                                                list_product.Add(product);
                                            }
                                        }
                                        else
                                        {
                                            list_product.Add(product);
                                        }
                                    }
                                    item2.list_product = list_product;
                                    if (list_product != null && list_product.Count > 0)
                                    {
                                        list_cabinet.Add(cabinet);
                                    }
                                }
                                item.list_cabinet = list_cabinet;
                            }
                        }

                        return list_vm;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            throw new NotImplementedException();
        }

        public VMAjaxProcessResult Add(VMERPUser currentUser, VMShipmentOrder vm)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            try
            {
                List<int> list_ShipmentOrderID = new List<int>();
                int affectRows = 0;
                int tempID = 0;
                int? tempReceiptIndex = 0;
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    list_ShipmentOrderID = _shipmentOrderService.GetShipmentOrderIDlist(currentUser, vm.ID);
                    var query = context.Inspection_InspectionReceiptList.Where(d => list_ShipmentOrderID.Contains(d.ShipmentOrderID ?? 0) && !d.IsDelete);
                    int? ReceiptIndex = query.Select(d => d.ReceiptIndex).Max();
                    if (ReceiptIndex.HasValue)
                    {
                        ++ReceiptIndex;
                    }
                    else
                    {
                        ReceiptIndex = 1;
                    }
                    tempReceiptIndex = ReceiptIndex;

                    int Letter = 64;
                    if (query != null)
                    {
                        foreach (var item in query)
                        {
                            Letter += item.Inspection_InspectionReceipt.Count;
                        }
                    }

                    var query_InspectionReceiptList = context.Inspection_InspectionReceiptList.Find(vm.InspectionReceiptListID);

                    #region 新建列表

                    DAL.Inspection_InspectionReceiptList query_InspectionReceiptList2 = new Inspection_InspectionReceiptList()
                    {
                        ST_CREATEUSER = currentUser.UserID,
                        DT_CREATEDATE = DateTime.Now,
                        ST_MODIFYUSER = currentUser.UserID,
                        DT_MODIFYDATE = DateTime.Now,
                        IsDelete = false,
                        IPAddress = CommonCode.GetIP(),

                        ShipmentOrderID = query_InspectionReceiptList.ShipmentOrderID,
                        PurchaseContractID = query_InspectionReceiptList.PurchaseContractID,
                        HSCode = query_InspectionReceiptList.HSCode,
                        IsShowNeedInspection = true,

                        StatusID = (int)InspectionReceiptStatusEnum.OutLine,
                        ReceiptIndex = ReceiptIndex,
                        IsNeedInspection = query_InspectionReceiptList.IsNeedInspection,
                    };
                    context.Inspection_InspectionReceiptList.Add(query_InspectionReceiptList2);

                    #endregion 新建列表

                    foreach (var item in vm.list_cabinet)
                    {
                        ++Letter;

                        #region 新建详情

                        DAL.Inspection_InspectionReceipt query_InspectionReceipt = new Inspection_InspectionReceipt();
                        query_InspectionReceipt.OrderID = 0;
                        query_InspectionReceipt.ContractIDList = query_InspectionReceiptList.PurchaseContractID.ToString();
                        query_InspectionReceipt.HSID = query_InspectionReceiptList.HSCode ?? 0;
                        query_InspectionReceipt.InvNO = item.list_product.First().InvoiceNo.Substring(0, 7) + (char)Letter;
                        query_InspectionReceipt.SCNO = item.list_product.First().SCNo.Substring(0, 9) + (char)Letter;
                        query_InspectionReceipt.ShipmentOrderID = query_InspectionReceiptList.ShipmentOrderID;

                        query_InspectionReceipt.InspectionReceiptStatus = (int)InspectionReceiptStatusEnum.OutLine;
                        query_InspectionReceipt.CreateDate = DateTime.Now;
                        query_InspectionReceipt.CreateUserID = currentUser.UserID;
                        query_InspectionReceipt.CreateTerminal = CommonCode.GetIP();
                        query_InspectionReceipt.UpdateDate = DateTime.Now;

                        query_InspectionReceipt.ClaimFaxDate = DateTime.Now;
                        query_InspectionReceipt.PortID = 0;
                        query_InspectionReceipt.INProductPrice = 0;
                        query_InspectionReceipt.INRemark = null;
                        query_InspectionReceipt.SaleContractContent = null;
                        query_InspectionReceipt.ZLRRuleNumber = 0;
                        query_InspectionReceipt.TeamOfThePayment = null;

                        query_InspectionReceiptList2.Inspection_InspectionReceipt.Add(query_InspectionReceipt);

                        #endregion 新建详情

                        #region 新建产品

                        foreach (var item2 in item.list_product)
                        {
                            DAL.Inspection_InspectionReceiptProduct query_InspectionReceiptProduct = new Inspection_InspectionReceiptProduct();

                            query_InspectionReceiptProduct.ShipmentOrderProductID = item2.ID;
                            query_InspectionReceiptProduct.SelectBoxQty = item2.SelectBoxQty;
                            query_InspectionReceiptProduct.Qty = item2.SelectQty;
                            query_InspectionReceiptProduct.IsDelete = false;

                            query_InspectionReceiptProduct.DT_CREATEDATE = DateTime.Now;
                            query_InspectionReceiptProduct.ST_CREATEUSER = currentUser.UserID;
                            query_InspectionReceiptProduct.IPAddress = CommonCode.GetIP();
                            query_InspectionReceiptProduct.DT_MODIFYDATE = DateTime.Now;
                            query_InspectionReceiptProduct.ST_MODIFYUSER = currentUser.UserID;

                            query_InspectionReceipt.Inspection_InspectionReceiptProduct.Add(query_InspectionReceiptProduct);
                        }

                        #endregion 新建产品

                    }
                    affectRows += context.SaveChanges();

                    tempID = query_InspectionReceiptList2.ID;
                }

                if (affectRows > 0)
                {
                    result.IsSuccess = true;

                    SaveOther(tempID, tempReceiptIndex);

                    #region 如果都报检了，就删掉列表的数据

                    using (ERPEntitiesNew context = new ERPEntitiesNew())
                    {
                        var query = context.Inspection_InspectionReceiptList.Find(vm.InspectionReceiptListID);
                        if (query != null)
                        {
                            var list_vm = _shipmentOrderService.GetDetailByID_ForEditPage(currentUser, query.ShipmentOrderID ?? 0, 0, query.Purchase_Contract.FactoryID, query.HSCode);

                            List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                            List<DAL.HarmonizedSystem> list_HarmonizedSystem = context.HarmonizedSystems.Where(p => !p.IsDelete).ToList();

                            List<VMShipmentOrderProduct> list_product = new List<VMShipmentOrderProduct>();
                            foreach (var item1 in list_vm)
                            {
                                if (item1.list_cabinet != null)
                                {
                                    foreach (var item2 in item1.list_cabinet)
                                    {
                                        foreach (var item3 in item2.list_product)
                                        {
                                            VMShipmentOrderProduct product = item3;
                                            var query2 = context.Inspection_InspectionReceiptProduct.Where(d => d.ShipmentOrderProductID == item3.ID && !d.IsDelete);
                                            int? SelectBoxQty = query2.Select(d => d.SelectBoxQty).Sum();
                                            if (SelectBoxQty.HasValue)
                                            {
                                                if (item3.SelectBoxQty > SelectBoxQty)
                                                {
                                                    int? SumBoxQty = item3.SelectBoxQty - SelectBoxQty;

                                                    int? Qty = Utils.StrToInt((SumBoxQty * item3.OuterBoxRate.Round(0)).ToString(), 0);

                                                    product = _shipmentOrderService.GetProductInfo(context, query2.First().Delivery_ShipmentOrderProduct.OrderProduct, SumBoxQty, Qty, list_Com_DataDictionary, list_HarmonizedSystem);

                                                    product.ID = query2.First().ShipmentOrderProductID ?? 0;
                                                    product.SumBoxQty = SumBoxQty;
                                                    product.SelectBoxQty = SumBoxQty;

                                                    list_product.Add(product);
                                                }
                                            }
                                            else
                                            {
                                                list_product.Add(product);
                                            }
                                        }
                                    }
                                }
                            }

                            if (list_product.Count == 0)
                            {
                                query.IsDelete = true;
                                query.ST_MODIFYUSER = currentUser.UserID;
                                query.DT_MODIFYDATE = DateTime.Now;
                                query.IPAddress = CommonCode.GetIP();

                                context.SaveChanges();
                            }
                        }
                    }

                    #endregion 如果都报检了，就删掉列表的数据


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