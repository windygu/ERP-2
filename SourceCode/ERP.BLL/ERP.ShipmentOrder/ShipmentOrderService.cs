using ERP.BLL.Consts;
using ERP.BLL.Workflow;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.Order;
using ERP.Models.ShipmentOrder;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP.BLL.ERP.ShipmentOrder
{
    /// <summary>
    /// 订舱信息
    /// </summary>
    public class ShipmentOrderService
    {
        private ERP.Dictionary.DictionaryServices _dictionaryServices = new Dictionary.DictionaryServices();

        #region HelperMethod

        /// <summary>
        /// 获取状态描述的内容
        /// </summary>
        /// <param name="StatusID"></param>
        /// <returns></returns>
        private static string GetShipmentOrderStatusEnum_Description(int StatusID)
        {
            return EnumHelper.GetCustomEnumDesc(typeof(ShipmentOrderStatusEnum), (ShipmentOrderStatusEnum)StatusID);
        }

        #endregion HelperMethod

        #region HelperMethod

        /// <summary>
        /// 获取状态描述的内容
        /// </summary>
        /// <param name="NotificationStatusID"></param>
        /// <returns></returns>
        private static string GetShipmentNotificationStatusEnum_Description(int StatusID)
        {
            return EnumHelper.GetCustomEnumDesc(typeof(ShipmentNotificationStatusEnum), (ShipmentNotificationStatusEnum)StatusID);
        }

        /// <summary>
        /// 获取拉柜费用状态描述的内容
        /// </summary>
        /// <param name="NotificationStatusID"></param>
        /// <returns></returns>
        private static string GetShipmentRegisterFeesStatusEnum_Description(int StatusID)
        {
            return EnumHelper.GetCustomEnumDesc(typeof(ShipmentRegisterFeesStatusEnum), (ShipmentRegisterFeesStatusEnum)StatusID);
        }

        #endregion HelperMethod

        #region UserMethod

        /// <summary>
        /// 获取列表数据
        /// </summary>
        public List<VMShipmentOrder> GetAll(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows,
          VMShipmentOrderSearch vm_search)
        {
            List<VMShipmentOrder> listModel = null;
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Delivery_ShipmentOrder.Where(p => !p.IsDelete);//查询审批已通过的销售订单
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForDelivery);

                    switch (vm_search.PageType)
                    {
                        case PageTypeEnum.PendingApproval:
                            query = query.Where(d => d.StatusID != (short)ShipmentOrderStatusEnum.PassedCheck);
                            break;

                        case PageTypeEnum.PassedApproval:
                            query = query.Where(d => d.StatusID == (short)ShipmentOrderStatusEnum.PassedCheck);
                            break;

                        default:
                            break;
                    }

                    #region 筛选条件

                    if (!string.IsNullOrEmpty(vm_search.OrderNumber))
                    {
                        query = query.Where(d => d.Order.OrderNumber.Contains(vm_search.OrderNumber));
                    }
                    if (!string.IsNullOrEmpty(vm_search.CustomerCode))
                    {
                        query = query.Where(d => d.Order.Orders_Customers.CustomerCode == vm_search.CustomerCode);
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
                        listModel = new List<VMShipmentOrder>();
                        List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());

                        foreach (var item in dataFromDB)
                        {
                            bool IsHasApprovalPermission = true;

                            #region 判断是否有审批流的权限

                            if (vm_search.PageType == PageTypeEnum.PendingApproval)//待审核
                            {
                                IsHasApprovalPermission = ApprovalService.HasApprovalPermission(WorkflowTypes.ApprovalShipmentOrder, currentUser, item.ST_CREATEUSER, item.ApproverIndex, item.Order.CustomerID);
                            }

                            #endregion 判断是否有审批流的权限

                            string OrderNumberList = "";
                            List<string> listOrderNumberList = new List<string>();
                            if (item.IsMerge)
                            {
                                foreach (var OrderID in CommonCode.IdListToList(item.OrderIDList))
                                {
                                    if (OrderID != item.OrderID)
                                    {
                                        listOrderNumberList.Add(context.Orders.Find(OrderID).OrderNumber);
                                    }
                                }
                            }
                            if (listOrderNumberList.Count > 0)
                            {
                                OrderNumberList = string.Join(",", listOrderNumberList.ToArray());
                            }

                            //string PortName = _dictionaryServices.GetDictionary_PortName(item.PortID, list_Com_DataDictionary);
                            //List<string> list_ShipToPortName = new List<string>();

                            //if (item.Delivery_ShipmentOrderCabinet != null)
                            //{
                            //    var list_ShipToPortID = item.Delivery_ShipmentOrderCabinet.Select(d => d.ShipToPortID).Distinct().ToList();
                            //    foreach (var ShipToPortID in list_ShipToPortID)
                            //    {
                            //        string ShipToPortName = _dictionaryServices.GetDictionary_DestinationPortName(ShipToPortID, list_Com_DataDictionary);
                            //        list_ShipToPortName.Add(ShipToPortName);
                            //    }
                            //}

                            string PortName = _dictionaryServices.GetDictionary_PortName(item.Order.PortID, list_Com_DataDictionary);

                            string DestinationPortName = _dictionaryServices.GetDictionary_DestinationPortName(item.Order.DestinationPortID, list_Com_DataDictionary);

                            listModel.Add(new VMShipmentOrder()
                            {
                                ID = item.ID,
                                OrderID = item.OrderID,
                                OrderNumber = item.Order.OrderNumber,
                                POID = item.Order.POID,
                                EHIPO = item.Order.EHIPO,
                                CustomerCode = item.Order.Orders_Customers.CustomerCode,
                                PortID = item.PortID,
                                PortName = PortName,
                                DestinationPortID = item.Order.DestinationPortID ?? 0,
                                DestinationPortName = DestinationPortName,
                                CustomerDate = Utils.DateTimeToStr(item.Order.CustomerDate),
                                OrderAmount = item.Order.OrderAmount,
                                OrderDateStartFormatter = Utils.DateTimeToStr(item.Order.OrderDateStart),
                                OrderDateEndFormatter = Utils.DateTimeToStr(item.Order.OrderDateEnd),
                                StatusID = item.StatusID,
                                StatusName = GetShipmentOrderStatusEnum_Description(item.StatusID),
                                DT_MODIFYDATEFormatter = Utils.DateTimeToStr2(item.DT_MODIFYDATE),
                                IsHasApprovalPermission = IsHasApprovalPermission,
                                ST_CREATEUSER = item.ST_CREATEUSER,
                                ApproverIndex = item.ApproverIndex,
                                ApproverIndexNotification = item.ApproverIndexNotification,
                                CustomerID = item.Order.CustomerID,
                                IsMerge = item.IsMerge,
                                IsBatchShipped = item.IsBatchShipped,
                                Merge = item.IsMerge ? "合并订舱" : (item.IsBatchShipped ? "分批订舱" : ""),
                                OrderNumberList = OrderNumberList,
                                OrderIDList = item.OrderIDList,
                                SelectCustomer = item.Order.Orders_Customers.SelectCustomer,
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
        /// 获取订舱的详细信息。新建订舱页面用到了
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="idList"></param>
        /// <returns></returns>
        public List<VMShipmentOrder> GetDetailByID(VMERPUser currentUser, List<int> idList, int ProductMixed_Type = 0, int? FactoryID = null, int? HSCode = null)
        {
            List<VMShipmentOrder> list_vm = new List<VMShipmentOrder>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Delivery_ShipmentOrder.Where(p => idList.Contains(p.ID) && !p.IsDelete);
                    List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                    List<DAL.HarmonizedSystem> list_HarmonizedSystem = context.HarmonizedSystems.Where(p => !p.IsDelete).ToList();

                    List<int?> list_ShipmentOrderProductID = new List<int?>();//不需要报关的产品

                    if (HSCode.HasValue && HSCode == -1)//新建报关的
                    {
                        var query2 = context.Inspection_InspectionReceiptList.Where(d => idList.Contains(d.ShipmentOrderID ?? 0) && !d.IsDelete);
                        if (query2.Count() > 0)
                        {
                            foreach (var item in query2)
                            {
                                if (!item.IsShowNeedInspection)//在报检时，不需要我司报关的产品，在新建报关里面就不显示出来。
                                {
                                    var query_Purchase = context.Purchase_Contract.Find(item.PurchaseContractID);
                                    if (query_Purchase != null)
                                    {
                                        foreach (var item2 in query_Purchase.Purchase_ContractBatch)
                                        {
                                            foreach (var item3 in item2.Purchase_ContractProduct.Where(d => !d.IsDelete && !d.ParentProductMixedID.HasValue))
                                            {
                                                if (item3.OrderProduct.HSCode == item.HSCode)
                                                {
                                                    list_ShipmentOrderProductID.Add(item3.OrderProduct.Delivery_ShipmentOrderProduct.Where(d => !d.IsDelete && !d.ParentProductMixedID.HasValue).First().ID);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    List<int> listShipmentOrderID = new List<int>();
                    foreach (var item in query)
                    {
                        listShipmentOrderID.Add(item.ID);

                        List<VMShipmentOrderHistory> list_history = new List<VMShipmentOrderHistory>();
                        foreach (var item_history in item.Delivery_ShipmentOrderHistory)
                        {
                            VMShipmentOrderHistory vm_history = new VMShipmentOrderHistory()
                            {
                                DT_CREATEDATE = item_history.DT_CREATEDATE,
                                ST_CREATEUSER = item_history.SystemUser.UserName,
                                StatusName = GetShipmentOrderStatusEnum_Description(item_history.StatusID),
                                CheckSuggest = item_history.CheckSuggest,
                            };
                            list_history.Add(vm_history);
                        }

                        List<VMShipmentOrderHistory> list_NotificationHistory = new List<VMShipmentOrderHistory>();
                        foreach (var item_NotificationHistory in item.Delivery_ShipmentNotificationHistory)
                        {
                            VMShipmentOrderHistory vm_history = new VMShipmentOrderHistory()
                            {
                                DT_CREATEDATE = item_NotificationHistory.DT_CREATEDATE,
                                ST_CREATEUSER = item_NotificationHistory.SystemUser.UserName,
                                StatusName = GetShipmentNotificationStatusEnum_Description(item_NotificationHistory.StatusID),
                                CheckSuggest = item_NotificationHistory.CheckSuggest,
                            };
                            list_NotificationHistory.Add(vm_history);
                        }

                        List<VMShipmentOrderHistory> list_historyRegister = new List<VMShipmentOrderHistory>();
                        foreach (var item_history in item.Delivery_ShipmentRegisterFeesHistory)
                        {
                            VMShipmentOrderHistory vm_history = new VMShipmentOrderHistory()
                            {
                                DT_CREATEDATE = item_history.DT_CREATEDATE,
                                ST_CREATEUSER = item_history.SystemUser.UserName,
                                StatusName = GetShipmentRegisterFeesStatusEnum_Description(item_history.StatusID),
                                CheckSuggest = item_history.CheckSuggest,
                            };
                            list_historyRegister.Add(vm_history);
                        }

                        list_vm.Add(new VMShipmentOrder()
                        {
                            ID = item.ID,
                            OrderID = item.OrderID,
                            OrderNumber = item.Order.OrderNumber,
                            OrderDateStartFormatter = Utils.DateTimeToStr(item.Order.OrderDateStart),
                            OrderDateEndFormatter = Utils.DateTimeToStr(item.Order.OrderDateEnd),
                            POID = item.Order.POID,
                            EHIPO = item.Order.EHIPO,
                            DestinationPortID = item.Order.DestinationPortID ?? 0,
                            Shipment_AgencyID = item.Shipment_AgencyID,
                            Shipment_AgencyName = item.Shipment_AgencyID == null ? null : item.Shipment_Agencies.ShippingAgencyName,
                            DestinationPortName = _dictionaryServices.GetDictionary_DestinationPortName(item.Order.DestinationPortID, list_Com_DataDictionary),
                            NotificationStatusID = item.NotificationStatusID,
                            IsMerge = item.IsMerge,

                            StatusID = item.StatusID,
                            CheckSuggest = item.CheckSuggest,
                            ST_CREATEUSER = item.ST_CREATEUSER,
                            ApproverIndex = item.ApproverIndex,
                            ApproverIndexNotification = item.ApproverIndexNotification,
                            NotificationCheckSuggest = item.NotificationCheckSuggest,
                            ST_CREATEUSERNotification = item.ST_CREATEUSERNotification,

                            PortID = item.PortID,
                            PortName = _dictionaryServices.GetDictionary_PortName(item.PortID, list_Com_DataDictionary),
                            OrderIDList = item.OrderIDList,

                            SelectCustomer = item.Order.Orders_Customers.SelectCustomer,

                            list_history = list_history,
                            list_NotificationHistory = list_NotificationHistory,
                            list_historyRegister = list_historyRegister,
                        });
                    }

                    List<int?> list_CabinetIndex = new List<int?>();
                    var query_carbinet = context.Delivery_ShipmentOrderCabinet.Where(d => listShipmentOrderID.Contains((int)d.ShipmentOrderID) && !d.IsDelete);
                    foreach (var item in query_carbinet)
                    {
                        if (!list_CabinetIndex.Contains(item.CabinetIndex))
                        {
                            list_CabinetIndex.Add(item.CabinetIndex);
                        }
                    }

                    List<VMShipmentOrderCabinet> list_cabinet = new List<VMShipmentOrderCabinet>();
                    foreach (var CabinetIndex in list_CabinetIndex.OrderBy(d => d.Value))
                    {
                        var cabinets = query_carbinet.Where(d => d.CabinetIndex == CabinetIndex);

                        List<VMShipmentOrderProduct> list_product = new List<VMShipmentOrderProduct>();
                        foreach (var item_cabinet in cabinets)
                        {
                            var query_product = item_cabinet.Delivery_ShipmentOrderProduct.Where(d => !d.IsDelete);
                            if (ProductMixed_Type == 0)
                            {
                                //普通的
                                query_product = query_product.Where(d => !d.ParentProductMixedID.HasValue);
                            }
                            else if (ProductMixed_Type == 1)
                            {
                                //如果是混装产品，显示混装产品的详情里面的。不显示主产品。
                                query_product = query_product.Where(d => !d.IsProductMixed || (d.IsProductMixed && d.ParentProductMixedID.HasValue));
                            }
                            else if (ProductMixed_Type == 2)
                            {
                                //只显示混装产品的详情里面的。
                                query_product = query_product.Where(d => d.IsProductMixed && d.ParentProductMixedID.HasValue);
                            }

                            foreach (var item_product in query_product)
                            {
                                VMShipmentOrderProduct vm_product = GetProductInfo(context, item_product.OrderProduct, item_product.BoxQty, item_product.Qty, list_Com_DataDictionary, list_HarmonizedSystem);
                                vm_product.ShipmentOrderCabinetID = item_product.ShipmentOrderCabinetID;
                                vm_product.ID = item_product.ID;
                                vm_product.SCNo = item_product.SCNo;
                                vm_product.InvoiceNo = item_product.InvoiceNo;

                                if (HSCode.HasValue)
                                {
                                    if (HSCode.Value >= 0)
                                    {
                                        if (HSCode == item_product.OrderProduct.HSCode && FactoryID == item_product.OrderProduct.FactoryID)
                                        {
                                            list_product.Add(vm_product);
                                        }
                                    }
                                    else if (HSCode.Value == -1)
                                    {
                                        if (!list_ShipmentOrderProductID.Contains(item_product.ID))
                                        {
                                            list_product.Add(vm_product);
                                        }
                                    }
                                }
                                else
                                {
                                    list_product.Add(vm_product);
                                }
                            }
                        }

                        var thisCarbinet = cabinets.FirstOrDefault();
                        string BoxNumber = "";
                        string SealingNumber = "";
                        if (thisCarbinet.SealingNumber != null)
                        {
                            BoxNumber = thisCarbinet.SealingNumber;
                            if (thisCarbinet.SealingNumber.Contains("/"))
                            {
                                BoxNumber = thisCarbinet.SealingNumber.Split('/')[0];
                                SealingNumber = thisCarbinet.SealingNumber.Split('/')[1];
                            }
                        }
                        list_cabinet.Add(new VMShipmentOrderCabinet()
                        {
                            ID = thisCarbinet.ID,
                            CabinetID = thisCarbinet.Shipment_CabinetID ?? 0,
                            CabinetName = thisCarbinet.Shipment_Cabinet.Name,
                            CabinetSize = thisCarbinet.Shipment_Cabinet.Size,
                            CarbinetIndex = thisCarbinet.CabinetIndex,

                            ShipToPortID = thisCarbinet.ShipToPortID,
                            ShipToPortName = _dictionaryServices.GetDictionary_DestinationPortName(thisCarbinet.ShipToPortID, list_Com_DataDictionary),
                            CabinetType = thisCarbinet.CabinetType,
                            CabinetNumber = thisCarbinet.CabinetNumber,
                            BoxNumber = BoxNumber,
                            SealingNumber = SealingNumber,

                            GatherType = thisCarbinet.GatherType,
                            GatherDateFormatter = Utils.DateTimeToStr(thisCarbinet.GatherDate),
                            GatherEndDateFormatter = Utils.DateTimeToStr(thisCarbinet.GatherEndDate),
                            ShippingDateStartFormatter = Utils.DateTimeToStr(thisCarbinet.ShippingDateStart),
                            ShippingDateEndFormatter = Utils.DateTimeToStr(thisCarbinet.ShippingDateEnd),
                            GatherAddress = thisCarbinet.GatherAddress,
                            GatherDescription = thisCarbinet.GatherDescription,
                            IsSendEmailToFactory = thisCarbinet.IsSendEmailToFactory,
                            RegisterFees = thisCarbinet.RegisterFees,
                            OceanVessel = thisCarbinet.OceanVessel,

                            UpLoadFileList = ConstsMethod.GetUploadFileList(thisCarbinet.ID, UploadFileType.ShipmentNotification_DeliveryNotification),
                            list_product = list_product,
                        });
                    }

                    if (list_vm.Count > 0)
                    {
                        list_vm.FirstOrDefault().list_cabinet = list_cabinet;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return list_vm;
        }

        /// <summary>
        /// 获取产品的信息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="tempProduct"></param>
        /// <param name="SelectBoxQty">当前订舱箱数</param>
        /// <returns></returns>
        public VMShipmentOrderProduct GetProductInfo(ERPEntitiesNew context, DAL.OrderProduct tempProduct, int? SelectBoxQty, int? SelectQty, List<DAL.Com_DataDictionary> list_Com_DataDictionary, List<DAL.HarmonizedSystem> list_HarmonizedSystem)
        {
            decimal? OuterLength = 0;
            decimal? OuterWidth = 0;
            decimal? OuterHeight = 0;
            decimal? OuterWeightGross = 0;
            decimal? OuterWeightNet = 0;

            #region 从出运明细的产品里面获取外箱长宽高、毛重、净重

            var Encasement_Product = context.Delivery_EncasementsProducts.Where(d => d.OrderProductID == tempProduct.ID).FirstOrDefault();
            if (Encasement_Product != null)
            {
                OuterLength = Encasement_Product.OuterLength;
                OuterWidth = Encasement_Product.OuterWidth;
                OuterHeight = Encasement_Product.OuterHeight;
                OuterWeightGross = Encasement_Product.WeightGross;
                OuterWeightNet = Encasement_Product.WeightNet;
            }

            #endregion 从出运明细的产品里面获取外箱长宽高、毛重、净重

            int? SumBoxQty = CalculateHelper.GetBoxQty(tempProduct.Qty, tempProduct.OuterBoxRate);
            if (SelectBoxQty == null)//如果为空，当前订舱箱数=总出运订舱箱数
            {
                SelectBoxQty = SumBoxQty;
            }

            bool IsNeedInspection = false;
            string IsNeedInspectionName = "";
            int HSCode = tempProduct.HSCode ?? 0;
            if (HSCode > 0)
            {
                IsNeedInspection = ConstsMethod.IsNeedInspection(list_HarmonizedSystem, HSCode);
                IsNeedInspectionName = IsNeedInspection ? "是" : "否";
            }

            int decimals = 0;
            string SelectCustomer = tempProduct.Order.Orders_Customers.SelectCustomer;
            if (SelectCustomer == SelectCustomerEnum.S135.ToString())
            {
                decimals = 2;
            }

            return (new VMShipmentOrderProduct()
            {
                OrderID = tempProduct.OrderID,
                OrderNumber = tempProduct.Order.OrderNumber,

                ProductID = tempProduct.ProductID,
                OrderProductID = tempProduct.ID,
                Image = tempProduct.Image,
                No = tempProduct.No,
                SkuNumber = tempProduct.SkuNumber,
                FactoryAbbreviation = tempProduct.Factory.Abbreviation,
                Desc = tempProduct.Desc,

                InnerBoxRate = tempProduct.InnerBoxRate,
                OuterBoxRate = tempProduct.OuterBoxRate,
                OuterLength = OuterLength,
                OuterWidth = OuterWidth,
                OuterHeight = OuterHeight,

                OuterVolume = tempProduct.OuterVolume,
                SumOuterVolume = CalculateHelper.GetSumOuterVolume(OuterLength, OuterWidth, OuterHeight, SumBoxQty),
                SumBoxQty = SumBoxQty,
                Qty = tempProduct.Qty,
                SelectBoxQty = SelectBoxQty,
                SelectVolume = CalculateHelper.GetSumOuterVolume(OuterLength, OuterWidth, OuterHeight, SelectBoxQty),
                RemainedBoxQty = SelectBoxQty,
                SelectQty = SelectQty,

                OuterWeightGross = OuterWeightGross,
                SumOuterWeightGross = (OuterWeightGross * SumBoxQty).Round(decimals),
                OuterWeightNet = OuterWeightNet,
                SumOuterWeightNet = (OuterWeightNet * SumBoxQty).Round(decimals),

                HTS = tempProduct.HTS == null ? "" : context.HarmonizedSystems.Find(tempProduct.HTS) == null ? "" : context.HarmonizedSystems.Find(tempProduct.HTS).HSCode,
                HSCode = tempProduct.HSCode == null ? "" : context.HarmonizedSystems.Find(tempProduct.HSCode) == null ? "" : context.HarmonizedSystems.Find(tempProduct.HSCode).HSCode,

                IsNeedInspection = IsNeedInspection,
                IsNeedInspectionName = IsNeedInspectionName,
                CurrencyName = _dictionaryServices.GetDictionary_CurrencyName(tempProduct.Factory.CurrencyType, list_Com_DataDictionary),

                SelectSumOuterWeightGross = (OuterWeightGross * SelectBoxQty).Round(decimals),
                SelectSumOuterWeightNet = (OuterWeightNet * SelectBoxQty).Round(decimals),
                HSID = tempProduct.HSCode,

                HSCodeName = tempProduct.HSCode == null ? "" : context.HarmonizedSystems.Find(tempProduct.HSCode) == null ? "" : context.HarmonizedSystems.Find(tempProduct.HSCode).CodeName,
                HSCodeEngName = tempProduct.HSCode == null ? "" : context.HarmonizedSystems.Find(tempProduct.HSCode) == null ? "" : context.HarmonizedSystems.Find(tempProduct.HSCode).CodeEngName,

                IsProductMixed = tempProduct.IsProductMixed,
            });
        }

        /// <summary>
        /// 获取订舱的详细信息。编辑页面用到了
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="ShipmentOrderID"></param>
        /// <returns></returns>
        public List<VMShipmentOrder> GetDetailByID_ForEditPage(VMERPUser currentUser, int ShipmentOrderID, int ProductMixed_Type = 0, int? FactoryID = null, int? HSCode = null)
        {
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Delivery_ShipmentOrder.Where(p => p.ID == ShipmentOrderID && !p.IsDelete);

                    var dataFromDB = query.FirstOrDefault();
                    if (dataFromDB != null)
                    {
                        List<int> OrderIDList = CommonCode.IdListToList(dataFromDB.OrderIDList);
                        if (string.IsNullOrEmpty(dataFromDB.OrderIDList))
                        {
                            OrderIDList = new List<int>();
                            OrderIDList.Add(dataFromDB.OrderID);
                        }
                        List<int> idList = new List<int>();
                        if (dataFromDB.IsMerge)
                        {
                            foreach (var OrderID in OrderIDList)
                            {
                                idList.Add(context.Delivery_ShipmentOrder.Where(d => d.OrderID == OrderID && !d.IsDelete).FirstOrDefault().ID);
                            }
                        }
                        else
                        {
                            idList.Add(dataFromDB.ID);
                        }
                        return GetDetailByID(currentUser, idList, ProductMixed_Type, FactoryID, HSCode);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return null;
        }

        /// <summary>
        /// 获取销售订单未订舱的产品，或者已订舱的产品：当前订舱箱数小于总出运箱数
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public List<VMShipmentOrderProduct> GetOrderProducts(List<int> OrderIDList, int ID)
        {
            List<VMShipmentOrderProduct> list_product = new List<VMShipmentOrderProduct>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Orders.Where(p => OrderIDList.Contains(p.OrderID) && !p.IsDelete);
                    var query_ShipmentOrder = context.Delivery_ShipmentOrder.Where(p => OrderIDList.Contains(p.OrderID) && !p.IsDelete);

                    List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                    List<DAL.HarmonizedSystem> list_HarmonizedSystem = context.HarmonizedSystems.Where(p => !p.IsDelete).ToList();

                    Dictionary<int, int> di_OrderProductID_BoxQty = new Dictionary<int, int>();
                    Dictionary<int, int> di_OrderProductID_Qty = new Dictionary<int, int>();

                    var temp_ShipmentOrder = context.Delivery_ShipmentOrder.Find(ID);

                    if (OrderIDList.Count == 1 && temp_ShipmentOrder.StatusID != (int)ShipmentOrderStatusEnum.PendingMaintenance)
                    {
                        return null;
                    }

                    foreach (var item in query_ShipmentOrder)
                    {
                        foreach (var item_cabinet in item.Delivery_ShipmentOrderCabinet.Where(d => !d.IsDelete))
                        {
                            foreach (var item_product in item_cabinet.Delivery_ShipmentOrderProduct.Where(d => !d.IsDelete && !d.ParentProductMixedID.HasValue))
                            {
                                if (di_OrderProductID_BoxQty.Keys.Contains(item_product.OrderProductID))
                                {
                                    di_OrderProductID_BoxQty[item_product.OrderProductID] += item_product.BoxQty ?? 0;
                                }
                                else
                                {
                                    di_OrderProductID_BoxQty.Add(item_product.OrderProductID, item_product.BoxQty ?? 0);
                                }

                                if (di_OrderProductID_Qty.Keys.Contains(item_product.OrderProductID))
                                {
                                    di_OrderProductID_Qty[item_product.OrderProductID] += item_product.Qty;
                                }
                                else
                                {
                                    di_OrderProductID_Qty.Add(item_product.OrderProductID, item_product.Qty);
                                }
                            }
                        }
                    }

                    foreach (var item in query)
                    {
                        foreach (var item_product in item.OrderProducts.Where(d => !d.IsDelete && !d.ParentProductMixedID.HasValue))
                        {
                            int? SumBoxQty = CalculateHelper.GetBoxQty(item_product.Qty, item_product.OuterBoxRate);//总出运箱数
                            int? Qty = item_product.Qty;
                            if (di_OrderProductID_BoxQty.Keys.Contains(item_product.ID))
                            {
                                SumBoxQty -= di_OrderProductID_BoxQty[item_product.ID];
                                if (SumBoxQty <= 0)
                                {
                                    continue;
                                }
                            }

                            if (di_OrderProductID_Qty.Keys.Contains(item_product.ID))
                            {
                                Qty -= di_OrderProductID_Qty[item_product.ID];
                                if (SumBoxQty <= 0)
                                {
                                    continue;
                                }
                            }
                            list_product.Add(GetProductInfo(context, item_product, SumBoxQty, Qty, list_Com_DataDictionary, list_HarmonizedSystem));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return list_product;
        }

        /// <summary>
        /// 保存订舱
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public VMAjaxProcessResult Save(VMERPUser currentUser, VMShipmentOrder vm)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            try
            {
                List<int> list_id = new List<int>();
                var OrderIDList = CommonCode.IdListToList(vm.OrderIDList);
                if (string.IsNullOrEmpty(vm.OrderIDList))
                {
                    OrderIDList = new List<int>();
                    OrderIDList.Add(vm.OrderID);
                }

                foreach (var OrderID in OrderIDList)
                {
                    using (ERPEntitiesNew context = new ERPEntitiesNew())
                    {
                        var query = context.Delivery_ShipmentOrder.Where(d => d.OrderID == OrderID && !d.IsDelete).FirstOrDefault();

                        if (OrderIDList.Count == 1)
                        {
                            query = context.Delivery_ShipmentOrder.Find(vm.ID);
                            query.IsDelete = true;
                        }

                        query.PortID = vm.PortID;
                        query.Shipment_AgencyID = vm.Shipment_AgencyID;
                        query.CheckSuggest = vm.CheckSuggest;

                        if (string.IsNullOrEmpty(vm.OrderIDList) || OrderIDList.Count() == 1)
                        {
                            query.OrderIDList = null;
                            query.IsMerge = false;
                        }
                        else
                        {
                            query.OrderIDList = vm.OrderIDList;
                            query.IsMerge = true;
                        }

                        bool IsMerge = query.IsMerge;

                        if (query.StatusID == (int)ShipmentOrderStatusEnum.PendingMaintenance)
                        {
                            query.DT_CREATEDATE = DateTime.Now;
                            query.ST_CREATEUSER = currentUser.UserID;
                        }

                        query.ST_MODIFYUSER = currentUser.UserID;
                        query.DT_MODIFYDATE = DateTime.Now;
                        query.IPAddress = CommonCode.GetIP();
                        if (vm.StatusID != (int)ShipmentOrderStatusEnum.NotPassCheck && vm.StatusID != (int)ShipmentOrderStatusEnum.PassedCheck)
                        {
                            query.StatusID = vm.StatusID;
                        }

                        if (vm.StatusID == (int)ShipmentOrderStatusEnum.PassedCheck)
                        {
                            query.ST_CREATEUSERNotification = currentUser.UserID;
                        }

                        if (!IsMerge)//不是合并订舱
                        {
                            int StatusID = (int)ShipmentOrderStatusEnum.PendingCheck;
                            if (vm.StatusID != (int)ShipmentOrderStatusEnum.NotPassCheck && vm.StatusID != (int)ShipmentOrderStatusEnum.PassedCheck)
                            {
                                StatusID = vm.StatusID;
                            }

                            //新建一个新的订舱
                            DAL.Delivery_ShipmentOrder query_new = new DAL.Delivery_ShipmentOrder()
                            {
                                OrderID = query.OrderID,
                                StatusID = StatusID,
                                IsDelete = false,
                                DT_CREATEDATE = DateTime.Now,
                                ST_CREATEUSER = query.ST_CREATEUSER,
                                ST_MODIFYUSER = currentUser.UserID,
                                DT_MODIFYDATE = DateTime.Now,
                                IPAddress = CommonCode.GetIP(),

                                ApproverIndex = query.ApproverIndex,
                                IsMerge = false,
                                IsBatchShipped = false,
                                PortID = vm.PortID,
                                Shipment_AgencyID = vm.Shipment_AgencyID,
                                NotificationStatusID = (short)ShipmentNotificationStatusEnum.PendingMaintenance,
                                RegisterFeesStatusID = (short)ShipmentRegisterFeesStatusEnum.PendingMaintenance,
                            };

                            context.Delivery_ShipmentOrder.Add(query_new);

                            var list_history = query.Delivery_ShipmentOrderHistory.ToList();
                            foreach (var item in list_history)
                            {
                                var history = item;
                                history.ShipmentOrderID = query_new.ID;
                                context.Delivery_ShipmentOrderHistory.Add(history);
                            }

                            context.SaveChanges();

                            query = query_new;
                        }
                        list_id.Add(query.ID);

                        DAL.Delivery_ShipmentOrderHistory query_history = new Delivery_ShipmentOrderHistory()
                        {
                            DT_CREATEDATE = DateTime.Now,
                            ST_CREATEUSER = currentUser.UserID,
                            IPAddress = CommonCode.GetIP(),
                            StatusID = vm.StatusID,
                            CheckSuggest = vm.CheckSuggest,
                        };
                        query.Delivery_ShipmentOrderHistory.Add(query_history);

                        foreach (var item in query.Delivery_ShipmentOrderCabinet)
                        {
                            item.IsDelete = true;
                            foreach (var item2 in item.Delivery_ShipmentOrderProduct)
                            {
                                item2.IsDelete = true;
                            }
                        }

                        if (vm.list_cabinet != null)
                        {
                            foreach (var item_cabinet in vm.list_cabinet.Where(d => !d.IsDelete))//遍历箱柜
                            {
                                List<DAL.Delivery_ShipmentOrderProduct> list_products = new List<Delivery_ShipmentOrderProduct>();
                                foreach (var item_product in item_cabinet.list_product.Where(d => d.OrderProductID >= 0))//遍历产品
                                {
                                    if (item_product.OrderID == OrderID)
                                    {
                                        list_products.Add(new Delivery_ShipmentOrderProduct()
                                        {
                                            IsProductMixed = item_product.IsProductMixed,
                                            OrderProductID = item_product.OrderProductID,
                                            BoxQty = item_product.SelectBoxQty ?? 0,
                                            Qty = item_product.SelectQty ?? 0,

                                            IsDelete = false,
                                            DT_CREATEDATE = DateTime.Now,
                                            ST_CREATEUSER = currentUser.UserID,
                                            ST_MODIFYUSER = currentUser.UserID,
                                            DT_MODIFYDATE = DateTime.Now,
                                            IPAddress = CommonCode.GetIP(),
                                        });
                                    }
                                }

                                if (list_products.Count > 0)
                                {
                                    DAL.Delivery_ShipmentOrderCabinet query_cabinet = new Delivery_ShipmentOrderCabinet()
                                    {
                                        Shipment_CabinetID = item_cabinet.CabinetID,
                                        ShipToPortID = item_cabinet.ShipToPortID,

                                        CabinetType = item_cabinet.CabinetType,
                                        CabinetNumber = item_cabinet.CabinetNumber,
                                        SealingNumber = item_cabinet.BoxNumber + "/" + item_cabinet.SealingNumber,

                                        CabinetIndex = item_cabinet.CarbinetIndex,
                                        Delivery_ShipmentOrderProduct = list_products,

                                        IsDelete = false,
                                        DT_CREATEDATE = DateTime.Now,
                                        ST_CREATEUSER = currentUser.UserID,
                                        ST_MODIFYUSER = currentUser.UserID,
                                        DT_MODIFYDATE = DateTime.Now,
                                        IPAddress = CommonCode.GetIP(),
                                    };

                                    query.Delivery_ShipmentOrderCabinet.Add(query_cabinet);
                                }
                            }
                        }

                        //foreach (var item_product in context.Delivery_ShipmentOrderProduct.Where(d => !d.IsDelete))//TODO 待定
                        //{
                        //    item_product.Qty = item_product.BoxQty * item_product.OrderProduct.OuterBoxRate ?? 0;
                        //}

                        int affectRows = context.SaveChanges();
                        if (affectRows == 0)
                        {
                            result.IsSuccess = false;
                            result.Msg = Keys.ErrorMsg;
                        }
                        else
                        {
                            result.IsSuccess = true;
                            ExecuteApproval(query.ST_CREATEUSER, query.ID, "", vm.StatusID, currentUser.UserID, false);//执行审批流

                            if (!IsMerge)
                            {
                                #region 判断是否订舱完全，如果产品都订舱了，删除待订舱的那条数据

                                var query_OrderProduct = context.OrderProducts.Where(d => !d.IsDelete && d.OrderID == OrderID && !d.ParentProductMixedID.HasValue);
                                int count_OrderProduct = 0;
                                foreach (var item in query_OrderProduct)
                                {
                                    count_OrderProduct += CalculateHelper.GetBoxQty(item.Qty, item.OuterBoxRate) ?? 0;
                                }

                                var ShipmentOrder = context.Delivery_ShipmentOrder.Where(d => !d.IsDelete && d.OrderID == OrderID);

                                int count = 0;
                                foreach (var item in ShipmentOrder)
                                {
                                    foreach (var item2 in item.Delivery_ShipmentOrderCabinet.Where(d => !d.IsDelete))
                                    {
                                        foreach (var item3 in item2.Delivery_ShipmentOrderProduct.Where(d => !d.IsDelete && !d.ParentProductMixedID.HasValue))
                                        {
                                            count += item3.BoxQty ?? 0;
                                        }
                                    }
                                }

                                var temp = ShipmentOrder.Where(d => d.StatusID == (int)ShipmentOrderStatusEnum.PendingMaintenance);

                                if (count_OrderProduct == count)
                                {
                                    foreach (var item in temp)
                                    {
                                        item.IsDelete = true;
                                    }
                                }
                                else
                                {
                                    if (temp.Count() == 0)
                                    {
                                        //新建一个新的订舱
                                        DAL.Delivery_ShipmentOrder query_new = new DAL.Delivery_ShipmentOrder()
                                        {
                                            OrderID = query.OrderID,

                                            IsDelete = false,
                                            DT_CREATEDATE = DateTime.Now,
                                            ST_CREATEUSER = currentUser.UserID,
                                            ST_MODIFYUSER = currentUser.UserID,
                                            DT_MODIFYDATE = DateTime.Now,
                                            IPAddress = CommonCode.GetIP(),
                                            StatusID = (short)ShipmentOrderStatusEnum.PendingMaintenance,
                                            NotificationStatusID = (short)ShipmentNotificationStatusEnum.PendingMaintenance,
                                            RegisterFeesStatusID = (short)ShipmentRegisterFeesStatusEnum.PendingMaintenance,
                                        };
                                        context.Delivery_ShipmentOrder.Add(query_new);
                                    }
                                }

                                #endregion 判断是否订舱完全，如果产品都订舱了，删除待订舱的那条数据

                                if (OrderIDList.Count == 1)
                                {
                                    var query_temp = context.Delivery_ShipmentOrder.Where(d => d.OrderID == OrderID && !d.IsDelete);
                                    if (query_temp.Count() > 1)
                                    {
                                        foreach (var item in query_temp)
                                        {
                                            item.IsBatchShipped = true;//设置为是分批出运
                                        }
                                    }
                                }
                            }

                            #region 删除标记为删除的订舱信息

                            var shipmentOrder = from d in context.Delivery_ShipmentOrder
                                                where d.IsDelete
                                                select d;

                            var list_ShipmentOrderID = shipmentOrder.Select(d => d.ID).ToList();

                            var cabinet = from d in context.Delivery_ShipmentOrderCabinet
                                          where d.IsDelete || list_ShipmentOrderID.Contains(d.ShipmentOrderID ?? 0)
                                          select d;

                            var list_ShipmentOrderCabinetID = cabinet.Select(d => d.ID).ToList();

                            var product = from d in context.Delivery_ShipmentOrderProduct
                                          where d.IsDelete || list_ShipmentOrderCabinetID.Contains(d.ShipmentOrderCabinetID)
                                          select d;

                            var history = from d in context.Delivery_ShipmentOrderHistory
                                          where list_ShipmentOrderID.Contains(d.ShipmentOrderID)
                                          select d;

                            var history2 = from d in context.Delivery_ShipmentNotificationHistory
                                           where list_ShipmentOrderID.Contains(d.ShipmentOrderID)
                                           select d;

                            var history3 = from d in context.Delivery_ShipmentRegisterFeesHistory
                                           where list_ShipmentOrderID.Contains(d.ShipmentOrderID)
                                           select d;

                            context.Delivery_ShipmentOrderProduct.RemoveRange(product);
                            context.Delivery_ShipmentOrderCabinet.RemoveRange(cabinet);
                            context.Delivery_ShipmentOrderHistory.RemoveRange(history);
                            context.Delivery_ShipmentNotificationHistory.RemoveRange(history2);
                            context.Delivery_ShipmentRegisterFeesHistory.RemoveRange(history3);
                            context.Delivery_ShipmentOrder.RemoveRange(shipmentOrder);

                            #endregion 删除标记为删除的订舱信息

                            context.SaveChanges();
                        }
                    }
                }

                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query_ShipmentOrder = context.Delivery_ShipmentOrder.Where(d => list_id.Contains(d.ID));

                    if (query_ShipmentOrder.FirstOrDefault().StatusID == (int)ShipmentOrderStatusEnum.PassedCheck)
                    {
                        #region 审核通过后，设置订舱的产品的发票编号、SCNo

                        List<string> listHSCode_FactoryID = new List<string>();
                        Dictionary<string, List<int>> di = new Dictionary<string, List<int>>();

                        List<string> listHSCode_FactoryID_NotInspection = new List<string>();//不需要报检的
                        Dictionary<string, List<int>> di_NotInspection = new Dictionary<string, List<int>>();//不需要报检的

                        List<DAL.HarmonizedSystem> list_HarmonizedSystem = context.HarmonizedSystems.Where(p => !p.IsDelete).ToList();
                        foreach (var item in query_ShipmentOrder)
                        {
                            foreach (var item2 in item.Delivery_ShipmentOrderCabinet)
                            {
                                foreach (var item_product in item2.Delivery_ShipmentOrderProduct.Where(d => !d.IsDelete && !d.ParentProductMixedID.HasValue))
                                {
                                    int HSCode = item_product.OrderProduct.HSCode ?? 0;
                                    if (HSCode > 0)
                                    {
                                        bool IsNeedInspection = ConstsMethod.IsNeedInspection(list_HarmonizedSystem, HSCode);

                                        string temp = HSCode + "_" + item_product.OrderProduct.FactoryID;
                                        if (IsNeedInspection)//需要报检
                                        {
                                            if (!listHSCode_FactoryID.Contains(temp))
                                            {
                                                listHSCode_FactoryID.Add(temp);

                                                List<int> list = new List<int>();
                                                list.Add(item_product.ID);
                                                di.Add(temp, list);
                                            }
                                            else
                                            {
                                                di[temp].Add(item_product.ID);
                                            }
                                        }
                                        else
                                        {
                                            if (!listHSCode_FactoryID_NotInspection.Contains(temp))
                                            {
                                                listHSCode_FactoryID_NotInspection.Add(temp);

                                                List<int> list = new List<int>();
                                                list.Add(item_product.ID);
                                                di_NotInspection.Add(temp, list);
                                            }
                                            else
                                            {
                                                di_NotInspection[temp].Add(item_product.ID);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        var list_InvoiceNo = context.Delivery_ShipmentOrderProduct.Where(d => d.InvoiceNo != null).Select(d => d.InvoiceNo);
                        int index = 201;
                        if (list_InvoiceNo.Count() > 0)
                        {
                            index = Utils.StrToInt(list_InvoiceNo.Max().Substring(4, 3), 0) + 1;
                        }

                        string InvoicePrefix = "ZL" + DateTime.Now.ToString("yy") + index;
                        int InvoiceIndex = 65;//大写字母A开始
                        foreach (var HSCode_FactoryID in listHSCode_FactoryID)
                        {
                            List<int> list_ID = di[HSCode_FactoryID];
                            var query_product = context.Delivery_ShipmentOrderProduct.Where(d => list_ID.Contains(d.ID) && !d.IsDelete && !d.ParentProductMixedID.HasValue);
                            int OrderID = 0;
                            List<int> ContractIDList = new List<int>();
                            string InvoiceNo = "";
                            string SCNo = "";
                            foreach (var item in query_product)
                            {
                                string temp = ((char)InvoiceIndex).ToString();
                                if (listHSCode_FactoryID.Count == 1)//如果只有一个发票编号时，没有后缀
                                {
                                    temp = "";
                                }
                                item.InvoiceNo = InvoicePrefix + temp;
                                item.SCNo = "SC " + item.OrderProduct.Order.OrderNumber + temp;

                                OrderID = item.OrderProduct.OrderID;

                                int ContractID = item.OrderProduct.Purchase_ContractProduct.Where(d => !d.IsDelete && !d.ParentProductMixedID.HasValue).FirstOrDefault().Purchase_ContractBatch.PurchaseContractID;
                                if (!ContractIDList.Contains(ContractID))
                                {
                                    ContractIDList.Add(ContractID);
                                }
                                InvoiceNo = item.InvoiceNo;
                                SCNo = item.SCNo;
                            }

                            ++InvoiceIndex;
                        }

                        foreach (var HSCode_FactoryID in listHSCode_FactoryID_NotInspection)
                        {
                            List<int> list_ID = di_NotInspection[HSCode_FactoryID];
                            var query_product = context.Delivery_ShipmentOrderProduct.Where(d => list_ID.Contains(d.ID) && !d.IsDelete && !d.ParentProductMixedID.HasValue);
                            foreach (var item in query_product)
                            {
                                item.InvoiceNo = InvoicePrefix + "!";//表示不需要报检的
                                item.SCNo = "SC " + item.OrderProduct.Order.OrderNumber + "!";
                            }
                            ++InvoiceIndex;
                        }

                        context.SaveChanges();

                        #endregion 审核通过后，设置订舱的产品的发票编号、SCNo

                        SaveOther(currentUser, list_id.FirstOrDefault());

                        SaveOther_AddClearance(currentUser, list_id.FirstOrDefault(), InvoicePrefix);

                        Save_ProductMixed(currentUser.UserID, list_id);
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

        /// <summary>
        /// 保存订舱的柜号、箱号、封号
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public VMAjaxProcessResult Save2(VMERPUser currentUser, VMShipmentOrder vm)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    foreach (var item in vm.list_cabinet)
                    {
                        var query = context.Delivery_ShipmentOrderCabinet.Find(item.ID);
                        if (query != null)
                        {
                            List<int> list_ShipmentOrderID = GetShipmentOrderIDlist(currentUser, query.ShipmentOrderID ?? 0);
                            var query2 = context.Delivery_ShipmentOrderCabinet.Where(d => list_ShipmentOrderID.Contains(d.ShipmentOrderID ?? 0) && !d.IsDelete && d.CabinetIndex == item.CarbinetIndex);
                            foreach (var item2 in query2)
                            {
                                item2.SealingNumber = item.BoxNumber + "/" + item.SealingNumber;
                                item2.CabinetNumber = item.CabinetNumber;
                            }
                        }
                    }
                    int i = context.SaveChanges();
                    if (i > 0)
                    {
                        result.IsSuccess = true;
                    }
                    else
                    {
                        result.IsSuccess = false;
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

        public DBOperationStatus SaveOther(VMERPUser currentUser, int id)
        {
            DBOperationStatus result = default(DBOperationStatus);
            try
            {
                int affectRows = 0;
                bool IsMerge = false;
                List<string> list_PurchaseID_HSCode = new List<string>();//报检：采购合同 + HSCode

                bool HasUSDFactory = false;
                bool HasRMBFactory = false;

                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    List<int?> list_HSCode = new List<int?>();
                    var query_ShipmentOrder = context.Delivery_ShipmentOrder.Find(id);
                    if (query_ShipmentOrder != null)
                    {
                        List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                        List<DAL.HarmonizedSystem> list_HarmonizedSystem = context.HarmonizedSystems.Where(p => !p.IsDelete).ToList();
                        IsMerge = query_ShipmentOrder.IsMerge;
                        if (IsMerge)//合并订舱
                        {
                            #region 合并订舱 (采购合同 + HSCode)

                            var OrderIDList = CommonCode.IdListToList(query_ShipmentOrder.OrderIDList);
                            var query_ShipmentOrder2 = context.Delivery_ShipmentOrder.Where(d => !d.IsDelete && OrderIDList.Contains(d.OrderID));

                            foreach (var item in query_ShipmentOrder2)
                            {
                                foreach (var item2 in item.Delivery_ShipmentOrderCabinet.Where(d => !d.IsDelete))
                                {
                                    foreach (var item3 in item2.Delivery_ShipmentOrderProduct.Where(d => !d.IsDelete && !d.ParentProductMixedID.HasValue))
                                    {
                                        var HSCodeID = item3.OrderProduct.HSCode;
                                        if (!list_HSCode.Contains(HSCodeID))
                                        {
                                            list_HSCode.Add(HSCodeID);
                                        }
                                    }
                                }
                            }

                            var query_PurchaseContract = context.Purchase_Contract.Where(d => OrderIDList.Contains(d.OrderID ?? 0) && !d.IsDelete && d.ContractType == (int)ContractTypeEnum.Default);

                            foreach (var item in query_PurchaseContract)
                            {
                                foreach (var item2 in item.Purchase_ContractBatch)
                                {
                                    string CurrencyName2 = _dictionaryServices.GetDictionary_CurrencyName(item.Factory.CurrencyType, list_Com_DataDictionary);
                                    foreach (var item3 in item2.Purchase_ContractProduct.Where(d => !d.IsDelete && !d.ParentProductMixedID.HasValue))
                                    {
                                        int HSCodeID = item3.OrderProduct.HSCode ?? 0;
                                        if (list_HSCode.Contains(HSCodeID))
                                        {
                                            string temp = item.ID + "_" + HSCodeID;
                                            bool IsNeedInspection = ConstsMethod.IsNeedInspection(list_HarmonizedSystem, HSCodeID);
                                            if (IsNeedInspection)
                                            {
                                                if (!list_PurchaseID_HSCode.Contains(temp))
                                                {
                                                    list_PurchaseID_HSCode.Add(temp);
                                                }

                                                if (CurrencyName2 == Keys.USD)
                                                {
                                                    HasUSDFactory = true;
                                                }
                                                if (CurrencyName2 == Keys.RMB)
                                                {
                                                    HasRMBFactory = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            #endregion 合并订舱 (采购合同 + HSCode)
                        }
                        else
                        {
                            #region 不是合并订舱 (采购合同 + HS CODE)

                            foreach (var item2 in query_ShipmentOrder.Delivery_ShipmentOrderCabinet.Where(d => !d.IsDelete))
                            {
                                foreach (var item3 in item2.Delivery_ShipmentOrderProduct.Where(d => !d.IsDelete && !d.ParentProductMixedID.HasValue))
                                {
                                    var HSCodeID = item3.OrderProduct.HSCode;
                                    if (!list_HSCode.Contains(HSCodeID))
                                    {
                                        list_HSCode.Add(HSCodeID);
                                    }
                                }
                            }

                            foreach (var item2 in query_ShipmentOrder.Delivery_ShipmentOrderCabinet.Where(d => !d.IsDelete))
                            {
                                foreach (var item3 in item2.Delivery_ShipmentOrderProduct.Where(d => !d.IsDelete && !d.ParentProductMixedID.HasValue))
                                {
                                    var purchase = item3.OrderProduct.Purchase_ContractProduct.Where(d => !d.IsDelete && !d.ParentProductMixedID.HasValue).FirstOrDefault().Purchase_ContractBatch.Purchase_Contract;
                                    int PurchaseContractID = purchase.ID;

                                    int HSCodeID = item3.OrderProduct.HSCode ?? 0;
                                    string CurrencyName2 = _dictionaryServices.GetDictionary_CurrencyName(purchase.Factory.CurrencyType, list_Com_DataDictionary);

                                    if (list_HSCode.Contains(HSCodeID))
                                    {
                                        string temp = PurchaseContractID + "_" + HSCodeID;

                                        bool IsNeedInspection = ConstsMethod.IsNeedInspection(list_HarmonizedSystem, HSCodeID);
                                        if (IsNeedInspection)
                                        {
                                            if (!list_PurchaseID_HSCode.Contains(temp))
                                            {
                                                list_PurchaseID_HSCode.Add(temp);
                                            }

                                            if (CurrencyName2 == Keys.USD)
                                            {
                                                HasUSDFactory = true;
                                            }
                                            if (CurrencyName2 == Keys.RMB)
                                            {
                                                HasRMBFactory = true;
                                            }
                                        }
                                    }
                                }
                            }

                            #endregion 不是合并订舱 (采购合同 + HS CODE)
                        }
                    }
                }

                if (list_PurchaseID_HSCode.Count > 0)
                {
                    #region 新建报检列表 (采购合同 + HSCode)

                    foreach (var PurchaseID_HSCode in list_PurchaseID_HSCode)
                    {
                        using (ERPEntitiesNew context = new ERPEntitiesNew())
                        {
                            int PurchaseID = Utils.StrToInt(PurchaseID_HSCode.Split('_')[0], 0);
                            int HSCode = Utils.StrToInt(PurchaseID_HSCode.Split('_')[1], 0);

                            DAL.Inspection_InspectionReceiptList query = new Inspection_InspectionReceiptList()
                            {
                                ST_CREATEUSER = currentUser.UserID,
                                DT_CREATEDATE = DateTime.Now,
                                ST_MODIFYUSER = currentUser.UserID,
                                DT_MODIFYDATE = DateTime.Now,
                                IsDelete = false,
                                IPAddress = CommonCode.GetIP(),

                                ShipmentOrderID = id,
                                PurchaseContractID = PurchaseID,
                                HSCode = HSCode,
                                IsShowNeedInspection = true,
                            };

                            if (HasRMBFactory && !HasUSDFactory)//只有人民币工厂
                            {
                                query.IsNeedInspection = true;
                            }

                            context.Inspection_InspectionReceiptList.Add(query);

                            affectRows = context.SaveChanges();
                        }
                    }

                    #endregion 新建报检列表 (采购合同 + HSCode)
                }
                else
                {
                    #region 不需要报检时，新建报关列表

                    using (ERPEntitiesNew context = new ERPEntitiesNew())
                    {
                        var list = GetShipmentOrderIDlist(currentUser, id);
                        var query_InspectionCustoms = context.Inspection_InspectionCustoms.Where(d => list.Contains(d.ShipmentOrderID) && !d.IsDelete);
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

                                ShipmentOrderID = id,
                                StatusID = (int)InspectionCustomsStatusEnum.PendingMaintenance,

                                IsNeedInspection = null,
                                IsShowNeedInspection = true,
                            };

                            context.Inspection_InspectionCustoms.Add(query);

                            affectRows = context.SaveChanges();
                        }
                    }

                    #endregion 不需要报检时，新建报关列表
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return result;
        }

        private void SaveOther_AddClearance(VMERPUser currentUser, int ShipmentOrderID, string InvoicePrefix)
        {
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                #region 添加清关列表的数据

                var query_ShipmentOrder = context.Delivery_ShipmentOrder.Where(d => d.ID == ShipmentOrderID && !d.IsDelete);
                if (query_ShipmentOrder.First().IsMerge)
                {
                    var OrderIDList = query_ShipmentOrder.First().OrderIDList;
                    query_ShipmentOrder = context.Delivery_ShipmentOrder.Where(d => d.OrderIDList == OrderIDList && !d.IsDelete);
                }
                var list_ShipmentOrderID = query_ShipmentOrder.Select(d => d.ID);

                var query_InspectionClearance = context.Inspection_InspectionClearance.Where(d => list_ShipmentOrderID.Contains(d.ShipmentOrderID) && !d.IsDelete);
                if (query_InspectionClearance.Count() == 0)
                {
                    DAL.Inspection_InspectionClearance query = new Inspection_InspectionClearance()
                    {
                        ST_CREATEUSER = currentUser.UserID,
                        DT_CREATEDATE = DateTime.Now,
                        ST_MODIFYUSER = currentUser.UserID,
                        DT_MODIFYDATE = DateTime.Now,
                        IsDelete = false,
                        IPAddress = CommonCode.GetIP(),

                        ShipmentOrderID = ShipmentOrderID,
                        StatusID = (int)InspectionClearanceStatusEnum.PendingMaintenance,
                        InvoiceNo = InvoicePrefix,
                    };

                    context.Inspection_InspectionClearance.Add(query);

                    context.SaveChanges();
                }

                #endregion 添加清关列表的数据
            }
        }

        /// <summary>
        /// 获取订舱的详细信息。编辑页面用到了
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="ShipmentOrderID"></param>
        /// <returns></returns>
        public List<int> GetShipmentOrderIDlist(VMERPUser currentUser, int ShipmentOrderID)
        {
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Delivery_ShipmentOrder.Where(p => p.ID == ShipmentOrderID && !p.IsDelete);

                    var dataFromDB = query.FirstOrDefault();
                    if (dataFromDB != null)
                    {
                        List<int> OrderIDList = CommonCode.IdListToList(dataFromDB.OrderIDList);
                        if (string.IsNullOrEmpty(dataFromDB.OrderIDList))
                        {
                            OrderIDList = new List<int>();
                            OrderIDList.Add(dataFromDB.OrderID);
                        }
                        List<int> idList = new List<int>();
                        if (dataFromDB.IsMerge)
                        {
                            foreach (var OrderID in OrderIDList)
                            {
                                idList.Add(context.Delivery_ShipmentOrder.Where(d => d.OrderID == OrderID && !d.IsDelete).FirstOrDefault().ID);
                            }
                        }
                        else
                        {
                            idList.Add(dataFromDB.ID);
                        }
                        return idList;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return null;
        }

        /// <summary>
        /// 保存混装产品
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="context"></param>
        /// <param name="quotID"></param>
        private void Save_ProductMixed(int UserID, List<int> list_id)
        {
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query_ShipmentOrder = context.Delivery_ShipmentOrder.Where(d => list_id.Contains(d.ID));

                    var list_products = new List<Delivery_ShipmentOrderProduct>();

                    foreach (var item in query_ShipmentOrder)
                    {
                        foreach (var item2 in item.Delivery_ShipmentOrderCabinet.Where(d => !d.IsDelete))
                        {
                            foreach (var item3 in item2.Delivery_ShipmentOrderProduct.Where(d => !d.IsDelete && d.IsProductMixed && !d.ParentProductMixedID.HasValue))
                            {
                                var product = context.OrderProducts.Where(d => !d.IsDelete && d.ParentProductMixedID == item3.OrderProduct.ID);

                                foreach (var item4 in product)
                                {

                                    list_products.Add(new Delivery_ShipmentOrderProduct()
                                    {
                                        OrderProductID = item4.ID,
                                        BoxQty = item3.BoxQty,
                                        Qty = item3.Qty,

                                        InvoiceNo = item3.InvoiceNo,
                                        SCNo = item3.SCNo,
                                        IsProductMixed = item3.IsProductMixed,
                                        ParentProductMixedID = item3.ID,
                                        ShipmentOrderCabinetID = item3.ShipmentOrderCabinetID,

                                        IsDelete = false,
                                        DT_CREATEDATE = DateTime.Now,
                                        ST_CREATEUSER = UserID,
                                        ST_MODIFYUSER = UserID,
                                        DT_MODIFYDATE = DateTime.Now,
                                        IPAddress = CommonCode.GetIP(),
                                    });
                                }
                            }
                        }
                    }

                    if (list_products.Count > 0)
                    {
                        context.Delivery_ShipmentOrderProduct.AddRange(list_products);
                        int i = context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
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
            if (StatusID == (int)ShipmentOrderStatusEnum.NotPassCheck)
            {
                isPass = false;
            }

            List<int> validWaitingApproveStatus = new List<int>(){
                            (int)ShipmentOrderStatusEnum.PendingCheck,
                            (int)ShipmentOrderStatusEnum.NotPassCheck
                        };
            ApprovalService.ExcuteApproval(new ApprovalInfo
            {
                WorkflowType = WorkflowTypes.ApprovalShipmentOrder,
                IsPass = isPass,
                IdentityID = identityID,
                ValidWaitingApproveStatus = validWaitingApproveStatus,
                StatusApproving = (int)ShipmentOrderStatusEnum.PendingCheck,
                StatusNextTo = (int)ShipmentOrderStatusEnum.PassedCheck,
                StatusRejected = (int)ShipmentOrderStatusEnum.NotPassCheck,
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