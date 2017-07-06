using ERP.BLL.Consts;
using ERP.BLL.ERP.Consts;
using ERP.BLL.ERP.ProductFitting;
using ERP.BLL.Workflow;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.Order;
using ERP.Models.Product;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.Tools.Logs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ERP.BLL.ERP.Order
{
    public class OrderService
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
        private static string GetOrderStatusEnum_Description(int StatusID)
        {
            return EnumHelper.GetCustomEnumDesc(typeof(OrderStatusEnum), (OrderStatusEnum)StatusID);
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
                    var query = context.Orders.Where(p => !p.IsDelete);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForOrder);

                    switch (vm_search.PageType)
                    {
                        case PageTypeEnum.PendingCheckList:
                            query = query.Where(d => d.OrderStatusID != (short)OrderStatusEnum.PassedApproval);
                            break;

                        case PageTypeEnum.PassedApproval:
                            query = query.Where(d => d.OrderStatusID == (short)OrderStatusEnum.PassedApproval);
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
                        query = query.OrderByDescending(d => d.DT_MODIFYDATE);
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
                            bool IsHasApprovalPermission = true;

                            #region 判断是否有审批流的权限

                            if (vm_search.PageType == PageTypeEnum.PendingCheckList)//待审核
                            {
                                IsHasApprovalPermission = ApprovalService.HasApprovalPermission(WorkflowTypes.ApprovalOrder, currentUser, item.ST_CREATEUSER, item.ApproverIndex, item.CustomerID);
                            }

                            #endregion 判断是否有审批流的权限

                            listModel.Add(new DTOOrder()
                            {
                                OrderID = item.OrderID,
                                OrderNumber = item.OrderNumber,
                                CustomerID = item.CustomerID,
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
                                OrderStatusID = item.OrderStatusID,
                                OrderStatusName = GetOrderStatusEnum_Description(item.OrderStatusID),
                                DT_MODIFYDATE = Utils.DateTimeToStr2(item.DT_MODIFYDATE),
                                ST_CREATEUSER = item.ST_CREATEUSER,
                                ApproverIndex = item.ApproverIndex,
                                IsHasApprovalPermission = IsHasApprovalPermission,
                                SelectCustomer = item.Orders_Customers.SelectCustomer,
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
        /// 批量删除、删除
        /// </summary>
        public DBOperationStatus Delete(VMERPUser currentUser, List<int> IDs)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Orders.Where(p => IDs.Contains(p.OrderID));
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForOrder);
                    if (query != null)
                    {
                        foreach (var item in query)
                        {
                            item.IsDelete = true;
                            item.DT_MODIFYDATE = DateTime.Now;
                            item.ST_MODIFYUSER = currentUser.UserID;
                            item.IPAddress = CommonCode.GetIP();
                        }

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
            }

            return result;
        }


        /// <summary>
        /// 获取销售订单详情
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="id"></param>
        /// <param name="ProductMixed_Type">0：普通的。1：如果是混装产品，显示混装产品的详情里面的。2：只显示混装产品的详情里面的。</param>
        /// <returns></returns>
        public VMOrderEdit GetDetailByID(VMERPUser currentUser, int id, int ProductMixed_Type = 0)
        {
            VMOrderEdit vm = null;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Orders.Where(p => p.OrderID == id && !p.IsDelete);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForOrder);

                    var dataFromDB = query.FirstOrDefault();
                    if (dataFromDB != null)
                    {
                        List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());

                        List<VMOrderProduct> list_product = new List<VMOrderProduct>();
                        List<VMOrderProduct> listProducts_Mixed = new List<VMOrderProduct>();
                        if (ProductMixed_Type == 1)
                        {
                            list_product = GetProducts(context, dataFromDB, list_Com_DataDictionary, 1);
                        }
                        else
                        {
                            list_product = GetProducts(context, dataFromDB, list_Com_DataDictionary, 0);
                            listProducts_Mixed = GetProducts(context, dataFromDB, list_Com_DataDictionary, 2);
                        }

                        #region 获取历史记录列表

                        List<VMOrderHistory> list_history = new List<VMOrderHistory>();
                        foreach (var item_history in dataFromDB.OrderHistories)
                        {
                            list_history.Add(new VMOrderHistory()
                            {
                                ST_CREATEUSER = context.SystemUsers.Where(d => d.UserID == item_history.ST_CREATEUSER).FirstOrDefault().UserName,
                                DT_CREATEDATE = Utils.DateTimeToStr2(item_history.DT_CREATEDATE),
                                Comment = item_history.Comment,
                                CheckSuggest = item_history.CheckSuggest,
                            });
                        }

                        #endregion 获取历史记录列表

                        vm = new VMOrderEdit()
                        {
                            OrderID = dataFromDB.OrderID,
                            OrderNumber = dataFromDB.OrderNumber,
                            POID = dataFromDB.POID,
                            EHIPO = dataFromDB.EHIPO,
                            PortID = dataFromDB.PortID,
                            PortName = _dictionaryServices.GetDictionary_PortName(dataFromDB.PortID, list_Com_DataDictionary),
                            DestinationPortID = dataFromDB.DestinationPortID,
                            DestinationPortName = _dictionaryServices.GetDictionary_DestinationPortName(dataFromDB.DestinationPortID, list_Com_DataDictionary),
                            CustomerID = dataFromDB.CustomerID,
                            CustomerNo = context.Orders_Customers.Where(d => !d.IsDelete && d.OCID == dataFromDB.CustomerID).FirstOrDefault() == null ? "" : context.Orders_Customers.Where(d => !d.IsDelete && d.OCID == dataFromDB.CustomerID).FirstOrDefault().CustomerCode,
                            CustomerDate = Utils.DateTimeToStr(dataFromDB.CustomerDate),
                            OrderStatusID = dataFromDB.OrderStatusID,
                            OrderAmount = dataFromDB.OrderAmount,
                            OrderDateStart = Utils.DateTimeToStr(dataFromDB.OrderDateStart),
                            OrderDateEnd = Utils.DateTimeToStr(dataFromDB.OrderDateEnd),
                            OrderOrigin = dataFromDB.OrderOrigin,
                            OrderStatusName = GetOrderStatusEnum_Description(dataFromDB.OrderStatusID),
                            Comment = dataFromDB.Comment,
                            CheckSuggest = dataFromDB.CheckSuggest,
                            IsThirdAudits = dataFromDB.IsThirdAudits,
                            IsThirdVerification = dataFromDB.IsThirdVerification,
                            IsThirdTest = dataFromDB.IsThirdTest,
                            IsThirdSampling = dataFromDB.IsThirdSampling,
                            ShippingType = dataFromDB.ShippingType,
                            CabinetRemark = dataFromDB.CabinetRemark,
                            QuotID = dataFromDB.QuotID,
                            CurrencyExchange = dataFromDB.CurrencyExchange,
                            Additional = dataFromDB.Additional,

                            DesignatedAgencyAmount = dataFromDB.DesignatedAgencyAmount,
                            OurAgencyAmount = dataFromDB.OurAgencyAmount,
                            PortChargesInvoice_StatusID = dataFromDB.PortChargesInvoice_StatusID,
                            CategoryManager = dataFromDB.CategoryManager,
                            //PaymentTerms = dataFromDB.PaymentTerms,
                            TransportType = dataFromDB.TransportType,
                            TestingStandardsFilename = dataFromDB.TestingStandardsFilename,

                            list_OrderProduct = list_product,
                            listProducts_Mixed = listProducts_Mixed,
                            list_OrderHistory = list_history,

                            SelectCustomer = dataFromDB.Orders_Customers.SelectCustomer,
                        };

                        vm.ST_CREATEUSER = dataFromDB.ST_CREATEUSER;
                        vm.ApproverIndex = dataFromDB.ApproverIndex;
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
        /// 获取产品信息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dataFromDB"></param>
        /// <param name="list_Com_DataDictionary"></param>
        /// <param name="ProductMixed_Type">0：普通的。1：如果是混装产品，显示混装产品的详情里面的。2：只显示混装产品的详情里面的。</param>
        /// <returns></returns>
        private List<VMOrderProduct> GetProducts(ERPEntitiesNew context, DAL.Order dataFromDB, List<Com_DataDictionary> list_Com_DataDictionary, int ProductMixed_Type = 0)
        {
            var query = dataFromDB.OrderProducts.Where(d => !d.IsDelete);
            if (ProductMixed_Type == 0)
            {
                //普通的
                query = query.Where(d => !d.ParentProductMixedID.HasValue);
            }
            else if (ProductMixed_Type == 1)
            {
                //如果是混装产品，显示混装产品的详情里面的。
                query = query.Where(d => !d.IsProductMixed || (d.IsProductMixed && d.ParentProductMixedID.HasValue));
            }
            else if (ProductMixed_Type == 2)
            {
                //只显示混装产品的详情里面的。
                query = query.Where(d => d.IsProductMixed && d.ParentProductMixedID.HasValue);
            }

            List<VMOrderProduct> list_product = new List<VMOrderProduct>();

            foreach (var item_product in query)
            {
                var product = GetProduct(item_product, context, list_Com_DataDictionary);

                if (ProductMixed_Type == 2)
                {
                    var tempProduct = context.OrderProducts.Find(item_product.ParentProductMixedID);
                    if (tempProduct != null)
                    {
                        if (tempProduct.OuterBoxRate.HasValue && tempProduct.OuterBoxRate.Value != 0)
                        {
                            product.Ctns = tempProduct.Qty / (decimal)tempProduct.OuterBoxRate;
                            product.Ctns = product.Ctns.Round();
                        }

                    }
                    product.SumSalePrice = product.Qty * product.SalePrice;
                }
                list_product.Add(product);
            }

            return list_product;
        }

        private VMOrderProduct GetProduct(OrderProduct item_product, ERPEntitiesNew context, List<Com_DataDictionary> list_Com_DataDictionary)
        {
            int id = item_product.ID;
            DAL.Order dataFromDB = item_product.Order;

            if (item_product.IsProductFitting)
            {
                var query_ProductFitting = context.ProductFittings.Where(d => d.ParentID == item_product.ID && !d.Deleted && d.ModuleType == (int)ModuleTypeEnum.Order);
                decimal? PriceFactory = 0;
                foreach (var item2 in query_ProductFitting)
                {
                    PriceFactory += item2.Qty * item2.PriceFactory * (1 + item2.FeesRate / 100);
                }
                //FTY PRICE=PriceFactory+配件单价*配件数量*（1+配件跟单费用%）
                item_product.PriceFactory += PriceFactory ?? 0;
            }

            #region 计算公式

            decimal NetPrice = item_product.NetPrice ?? 0;
            decimal SumNetPrice = (NetPrice * item_product.Qty).Round();//总净价

            decimal SumSalePrice = item_product.SalePrice ?? 0 * item_product.Qty;
            decimal SumPriceFactory = item_product.PriceFactory * item_product.Qty;
            string CurrencySign = _dictionaryServices.GetDictionary_CurrencySign(item_product.CurrencyType, list_Com_DataDictionary);//货币符号

            if (item_product.Commission == null)
            {
                item_product.Commission = 0;
            }

            #endregion 计算公式

            string SeasonPrefix = "";
            string SeasonSuffix = "";
            string SeasonName = "";
            string SeasonAlias = "";
            string SeasonDepartmentNumber = "";

            var query_TempSeason = list_Com_DataDictionary.Where(d => d.TableKind == (int)DictionaryTableKind.Season && d.Code == item_product.Season);
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

            #region 获取客户的联系人信息

            string AssistantName = "";
            string AssistantTel = "";
            string AssistantEmail = "";
            string BuyerName = "";
            string BuyerTel = "";
            string BuyerEmail = "";
            string CustomerSeasonPrefix = "";

            var query_Orders_Contacts = dataFromDB.Orders_Customers.Orders_Contacts.Where(d => !d.IsDelete);
            if (query_Orders_Contacts != null && query_Orders_Contacts.Count() > 0)
            {
                foreach (var item in query_Orders_Contacts)
                {
                    if (item.Duty == "Buyer")
                    {
                        BuyerName = item.FirstName + item.LastName;
                        BuyerTel = item.TelPhone;
                        BuyerEmail = item.Email;
                    }
                    else if (item.Duty == "Assistant")
                    {
                        AssistantName = item.FirstName + item.LastName;
                        AssistantTel = item.TelPhone;
                        AssistantEmail = item.Email;
                    }

                    if (!string.IsNullOrEmpty(item.SeasonIDList))
                    {
                        var list_SeasonID = CommonCode.StrToList(item.SeasonIDList);
                        if (list_SeasonID.Count > 0)
                        {
                            int tempSeasonID = Utils.StrToInt(list_SeasonID.First(), 0);
                            var query_TempSeason2 = list_Com_DataDictionary.Where(d => d.TableKind == (int)DictionaryTableKind.Season && d.Code == tempSeasonID);
                            if (query_TempSeason2 != null && query_TempSeason2.Count() > 0)
                            {
                                CustomerSeasonPrefix = query_TempSeason2.First().Name;
                            }
                        }
                    }
                }
            }

            #endregion 获取客户的联系人信息

            var BoxQty = CalculateHelper.GetBoxQty(item_product.Qty, item_product.OuterBoxRate);

            string PaymentType = "";

            var query_PaymentType = list_Com_DataDictionary.Where(d => d.ID == dataFromDB.Orders_Customers.PaymentType);
            if (query_PaymentType != null && query_PaymentType.Count() > 0)
            {
                PaymentType = query_PaymentType.First().Name;
            }

            VMOrderProduct vm = new VMOrderProduct()
            {
                ID = item_product.ID,
                ProductID = item_product.ProductID,
                Image = item_product.Image,
                No = item_product.No,
                Name = item_product.Name,
                SkuNumber = item_product.SkuNumber,
                SkuCode = item_product.SkuCode,
                UPC = item_product.UPC,
                QualityAssuranceTesting = item_product.QualityAssuranceTesting.HasValue ? item_product.QualityAssuranceTesting : false,
                FactoryName = item_product.Factory.Abbreviation,
                Desc = item_product.Desc,
                StyleName = _dictionaryServices.GetDictionary_StyleName(item_product.StyleID, list_Com_DataDictionary),
                StyleNumber = _dictionaryServices.GetDictionary_StyleNumber(item_product.StyleID, list_Com_DataDictionary),
                UnitName = _dictionaryServices.GetDictionary_UnitName(item_product.UnitID, list_Com_DataDictionary),
                InnerBoxRate = item_product.InnerBoxRate,

                OuterBoxRate = item_product.OuterBoxRate,
                OuterLengthIN = item_product.OuterLengthIN,
                OuterWidthIN = item_product.OuterWidthIN,
                OuterHeightIN = item_product.OuterHeightIN,
                OuterVolume = item_product.OuterVolume,
                Qty = item_product.Qty,
                Qty2 = item_product.Qty2 ?? 0,
                RetailPrice = item_product.RetailPrice,

                SalePrice = item_product.SalePrice ?? 0,
                SumSalePrice = SumSalePrice,

                PriceFactory = item_product.PriceFactory,
                SumPriceFactory = SumPriceFactory,
                ELC = item_product.Elc,

                Commission = item_product.Commission,
                CurrencySign = CurrencySign,
                CommissionAmount = item_product.CommissionAmount,
                Allowance = item_product.Allowance,
                Agent = item_product.Agent,
                MUPercent = item_product.Mu,
                Rate = item_product.Rate,
                RateFormatter = item_product.Rate + (CurrencySign == Keys.USD_Sign ? "%" : ""),

                NetPrice = NetPrice,
                SumNetPrice = SumNetPrice,

                PriceFactoryFormatter = CurrencySign + item_product.PriceFactory,
                SumPriceFactoryFormatter = CurrencySign + SumPriceFactory,

                OrderID = id,
                SortIndex = item_product.SortIndex,
                MiscImportLoad = item_product.MiscImportLoad,
                QuoteTemplateFileName = item_product.QuoteTemplateFileName,
                PalletPc = item_product.PalletPc,
                Duty = item_product.Duty,
                Freight = item_product.Freight,
                FOBFTY = item_product.Fobfty,
                InspectionVerificationFee = item_product.InspectionVerificationFee,
                InspectionVerificationFee_ForFactory = item_product.InspectionVerificationFee_ForFactory,

                POID = dataFromDB.POID,
                CategoryManager = dataFromDB.CategoryManager,
                PortName = _dictionaryServices.GetDictionary_PortName(dataFromDB.PortID, list_Com_DataDictionary),
                PortEngName = _dictionaryServices.GetDictionary_PortEnName(dataFromDB.PortID, list_Com_DataDictionary),
                PaymentType = PaymentType,
                OrderDateStart = dataFromDB.OrderDateStart,
                OrderDateEnd = dataFromDB.OrderDateEnd,
                Department = item_product.Department,
                SeasonFullName = _dictionaryServices.GetDictionary_AllSeasonName(item_product.Season, list_Com_DataDictionary),

                ColorName = item_product.ColorName,
                INR = item_product.INR,
                Remarks = item_product.Remarks,

                Season = item_product.Season,
                SeasonName = SeasonName,
                SeasonPrefix = SeasonPrefix,
                SeasonSuffix = SeasonSuffix,

                BuyerName = BuyerName,
                BuyerTel = BuyerTel,
                BuyerEmail = BuyerEmail,
                AssistantName = AssistantName,
                AssistantTel = AssistantTel,
                AssistantEmail = AssistantEmail,
                OuterWeightGrossLBS = item_product.OuterWeightGrossLBS,
                OuterWeightNetLBS = item_product.OuterWeightNetLBS,
                Factory_EnglishName = item_product.Factory.EnglishName,
                Factory_EnglishAddress = item_product.Factory.EnglishAddress,
                FactoryID = item_product.FactoryID,
                CustomerSeasonPrefix = CustomerSeasonPrefix,
                TransportTypeName = dataFromDB.TransportType.HasValue ? CommonCode.GetStatusEnumName(dataFromDB.TransportType ?? 0, typeof(TransportTypeEnum)) : "",
                IngredientZh = item_product.IngredientZh,
                IngredientEn = item_product.IngredientEn,
                TestingStandardsFilename = dataFromDB.TestingStandardsFilename,
                BoxQty = BoxQty,

                LengthIN = item_product.LengthIN,
                WidthIN = item_product.WidthIN,
                HeightIN = item_product.HeightIN,
                OfStoreReadyInnersEnclosed = item_product.OfStoreReadyInnersEnclosed,
                PreTicketed = item_product.PreTicketed,
                SumOuterVolume = CalculateHelper.GetSumOuterVolume(item_product.OuterLength, item_product.OuterWidth, item_product.OuterHeight, BoxQty),
                IsProductFitting = item_product.IsProductFitting,

                list_ProductIngredient = context.ProductIngredients.Where(d => d.ModuleType == (int)ModuleTypeEnum.Product && d.ProductID == item_product.ProductID).Select(p => new VMProductIngredients
                {
                    IngredientName = p.IngredientName,
                    IngredientPercent = p.IngredientPercent,
                    ProductID = p.ProductID,
                }).ToList(),

                IsProductMixed = item_product.IsProductMixed,
                ParentProductMixedID = item_product.ParentProductMixedID,



                Length = item_product.Length,
                Height = item_product.Height,
                Width = item_product.Width,
                Weight = item_product.Weight,

                OuterLength = item_product.OuterLength,
                OuterWidth = item_product.OuterWidth,
                OuterHeight = item_product.OuterHeight,

                OuterWeightGross = item_product.OuterWeightGross ?? 0,
                OuterWeightNet = item_product.OuterWeightNet ?? 0,

                MOQZh = item_product.MOQZh,
                MOQEn = item_product.MOQEn,
                PDQPackRate = item_product.PDQPackRate,
                PDQLength = item_product.PDQLength,
                PDQWidth = item_product.PDQWidth,
                PDQHeight = item_product.PDQHeight,

                InnerLength = item_product.InnerLength,
                InnerWidth = item_product.InnerWidth,
                InnerHeight = item_product.InnerHeight,
                InnerWeight = item_product.InnerWeight,

                SRP = item_product.SRP,
                CtnsPallet = item_product.CtnsPallet,
                DutyPercent = item_product.DutyPercent,
                Comment = item_product.Comment,

                WeightLBS = item_product.WeightLBS,
                PDQLengthIN = item_product.PDQLengthIN,
                PDQWidthIN = item_product.PDQWidthIN,
                PDQHeightIN = item_product.PDQHeightIN,
                InnerLengthIN = item_product.InnerLengthIN,
                InnerWidthIN = item_product.InnerWidthIN,
                InnerHeightIN = item_product.InnerHeightIN,
                InnerVolume = item_product.InnerVolume,
                InnerWeightLBS = item_product.InnerWeightLBS,
                InnerWeightGrossLBS = item_product.InnerWeightGrossLBS,

                FOBNET = item_product.Fobnet,
                FinalFOB = item_product.FinalFOB,
                MiscImportLoadAmount = item_product.MiscImportLoadAmount,
                PcsPallet = item_product.PcsPallet,

                FOBChinaLCL = item_product.FobChinaLCL,
                Cost = item_product.Cost,

                CustomerID = item_product.CustomerID,

                FreightRate = item_product.FreightRate,

                StyleID = item_product.StyleID,
                PortID = item_product.PortID,
                PortEnName = _dictionaryServices.GetDictionary_PortEnName(item_product.PortID, list_Com_DataDictionary),
                PackingMannerZhID = item_product.PackingMannerZhID,
                PackingMannerZhName = _dictionaryServices.GetDictionary_PackingMannerZhName(item_product.PackingMannerZhID, list_Com_DataDictionary),
                PackingMannerEnID = item_product.PackingMannerEnID,
                PackingMannerEnName = item_product.PackingMannerEnID == null ? "" : _dictionaryServices.GetDictionary_PackingMannerEnName(item_product.PackingMannerZhID, list_Com_DataDictionary),
                UnitID = item_product.UnitID,
                CurrencyType = item_product.CurrencyType,
                CurrencyName = _dictionaryServices.GetDictionary_CurrencyName(item_product.CurrencyType, list_Com_DataDictionary),

                HTS = item_product.HTS,
                HSCode = item_product.HSCode.ToString(),
                FOBChinaPort = item_product.FOBChinaPort ?? 0,

                ColorID = item_product.ColorID,

            };
            return vm;
        }

        /// <summary>
        /// 新建销售订单
        /// </summary>
        public VMAjaxProcessResult Add(VMERPUser currentUser, VMOrderEdit vm)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    bool isRepeat = IsRepeat(vm, context);
                    if (isRepeat)//PO1和PO2是唯一的
                    {
                        result.IsSuccess = false;
                        result.Msg = "PO1或PO2与其他的订单重复了！";
                        return result;
                    }

                    string maxOrderNumber = context.Orders.Where(d => !d.OrderNumber.Contains("_") && !d.IsDelete).Max(d => d.OrderNumber);

                    DAL.Order query = new DAL.Order()
                    {
                        OrderNumber = CommonCode.GetRandom_OrderNumber(maxOrderNumber),
                        OrderOrigin = vm.OrderOrigin,
                        OrderDateStart = Utils.StrToDateTime(vm.OrderDateStart),
                        OrderDateEnd = Utils.StrToDateTime(vm.OrderDateEnd),
                        PortID = vm.PortID,
                        DestinationPortID = vm.DestinationPortID,
                        POID = vm.POID,
                        EHIPO = vm.EHIPO,
                        CustomerID = vm.CustomerID,
                        CustomerDate = Utils.StrToDateTime(vm.CustomerDate),
                        IsThirdVerification = vm.IsThirdVerification,
                        IsThirdAudits = vm.IsThirdAudits,
                        IsThirdTest = vm.IsThirdTest,
                        IsThirdSampling = vm.IsThirdSampling,
                        OrderRate = vm.OrderRate,
                        OrderRate_En = vm.OrderRate_En,
                        ShippingType = vm.ShippingType,
                        CabinetRemark = vm.CabinetRemark,
                        QuotID = vm.QuotID,
                        CurrencyExchange = vm.CurrencyExchange,
                        CategoryManager = vm.CategoryManager,

                        IsDelete = false,
                        OrderAmount = vm.OrderAmount,
                        Additional = vm.Additional,
                        TransportType = vm.TransportType,
                        TestingStandardsFilename = vm.TestingStandardsFilename,

                        DT_CREATEDATE = DateTime.Now,
                        ST_CREATEUSER = currentUser.UserID,
                        ST_MODIFYUSER = currentUser.UserID,
                        DT_MODIFYDATE = DateTime.Now,
                        IPAddress = CommonCode.GetIP(),
                    };

                    if (vm.QuotID > 0)
                    {
                        var query_quote = context.Quot_Quot.Find(vm.QuotID);
                        if (query_quote != null)
                        {
                            query_quote.OrderID = vm.OrderID.ToString();
                        }
                    }

                    query.OrderStatusID = vm.OrderStatusID;

                    if (vm.OrderStatusID == (int)OrderStatusEnum.OutLine)//草稿时
                    {
                        DAL.OrderHistory history = new OrderHistory()
                        {
                            DT_CREATEDATE = DateTime.Now,
                            ST_CREATEUSER = currentUser.UserID,
                            IPAddress = CommonCode.GetIP(),
                            Comment = GetOrderStatusEnum_Description(vm.OrderStatusID),
                        };
                        query.OrderHistories.Add(history);
                    }

                    bool isVaild_HSCode = true;
                    List<string> listValid_ProductNO = new List<string>();

                    foreach (var item in vm.list_OrderProduct.Where(d => d.ProductFrom == (short)OrderProductSource.FromProduct))//添加 来源于产品
                    {
                        var queryProduct = context.Products.Find(item.ProductID);
                        if (queryProduct != null)
                        {
                            DAL.OrderProduct query_product = SetDBEntityFromViewModel(queryProduct, currentUser.UserID, item.FreightRate);

                            if (!query_product.HSCode.HasValue)
                            {
                                isVaild_HSCode = false;
                                listValid_ProductNO.Add(query_product.No);
                            }

                            query_product = SetEditProduct(item, query_product, currentUser.UserID);
                            query.OrderProducts.Add(query_product);
                        }
                    }

                    foreach (var item in vm.list_OrderProduct.Where(d => d.ProductFrom == (short)OrderProductSource.FromQuoteProduct))//添加 来源于报价单
                    {
                        var queryProudct = context.Quot_QuotProduct.Where(d => !d.IsDelete && d.ID == item.QuotProductID).FirstOrDefault();
                        if (queryProudct != null)
                        {
                            DAL.OrderProduct query_product = SetDBEntityFromViewModel_QuoteProduct(queryProudct, currentUser.UserID);

                            if (!query_product.HSCode.HasValue)
                            {
                                isVaild_HSCode = false;
                                listValid_ProductNO.Add(query_product.No);
                            }

                            query_product = SetEditProduct(item, query_product, currentUser.UserID);
                            query.OrderProducts.Add(query_product);
                        }
                    }

                    if (!isVaild_HSCode && vm.OrderStatusID != (int)OrderStatusEnum.OutLine)//当不是草稿时，需要判断报关编码是否为空。
                    {
                        result.IsSuccess = false;
                        result.Msg = "产品的报关编码不能为空！产品编号如下：" + string.Join(",", listValid_ProductNO.ToArray()) + "。";
                        return result;
                    }

                    context.Orders.Add(query);

                    int affectRows = context.SaveChanges();
                    if (affectRows == 0)
                    {
                        result.IsSuccess = false;
                        result.Msg = "出错了！";
                        return result;
                    }
                    else
                    {
                        result.IsSuccess = true;
                        result.identity = maxOrderNumber;

                        ExecuteApproval(query.ST_CREATEUSER, query.OrderID, "", vm.OrderStatusID, currentUser.UserID, false);//执行审批流

                        SaveOther(currentUser, query.OrderID);

                        RemoveOneCachedOrder(currentUser.UserID, vm.OrderID);

                        Save_ProductFitting(currentUser.UserID, query.OrderID);

                        Save_ProductMixed(currentUser.UserID, query.OrderID, vm.listProducts_Mixed);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return result;
        }

        /// <summary>
        /// 保存
        /// </summary>
        public VMAjaxProcessResult Save(VMERPUser currentUser, VMOrderEdit vm)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    bool isRepeat = IsRepeat(vm, context);
                    if (isRepeat)//PO1和PO2是唯一的
                    {
                        result.IsSuccess = false;
                        result.Msg = "PO1或PO2与其他的订单重复了！";
                        return result;
                    }

                    bool historyAdded = false;

                    var query = context.Orders.Find(vm.OrderID);

                    query.OrderOrigin = vm.OrderOrigin;
                    query.OrderDateStart = Utils.StrToDateTime(vm.OrderDateStart);
                    query.OrderDateEnd = Utils.StrToDateTime(vm.OrderDateEnd);
                    query.PortID = vm.PortID;
                    query.POID = vm.POID;
                    query.DestinationPortID = vm.DestinationPortID;
                    query.EHIPO = vm.EHIPO;
                    query.CustomerID = vm.CustomerID;
                    query.CustomerDate = Utils.StrToDateTime(vm.CustomerDate);
                    query.IsThirdVerification = vm.IsThirdVerification;
                    query.IsThirdAudits = vm.IsThirdAudits;
                    query.IsThirdTest = vm.IsThirdTest;
                    query.IsThirdSampling = vm.IsThirdSampling;

                    query.CheckSuggest = vm.CheckSuggest;
                    query.Comment = vm.Comment;

                    query.ShippingType = vm.ShippingType;
                    query.CabinetRemark = vm.CabinetRemark;

                    query.OrderRate = vm.OrderRate;
                    query.OrderRate_En = vm.OrderRate_En;

                    if (vm.QuotID > 0)
                    {
                        var query_quote = context.Quot_Quot.Find(vm.QuotID);
                        if (query_quote != null)
                        {
                            query_quote.OrderID = vm.OrderID.ToString();
                        }
                    }
                    query.QuotID = vm.QuotID;
                    query.CurrencyExchange = vm.CurrencyExchange;
                    query.CategoryManager = vm.CategoryManager;
                    query.OrderAmount = vm.OrderAmount;
                    query.Additional = vm.Additional;
                    //query.PaymentTerms = vm.PaymentTerms;
                    query.TransportType = vm.TransportType;
                    query.TestingStandardsFilename = vm.TestingStandardsFilename;

                    if (vm.OrderStatusID != (int)OrderStatusEnum.NotPassApproval && vm.OrderStatusID != (int)OrderStatusEnum.PassedApproval)
                    {
                        query.OrderStatusID = vm.OrderStatusID;
                    }

                    query.ST_MODIFYUSER = currentUser.UserID;
                    query.DT_MODIFYDATE = DateTime.Now;
                    query.IPAddress = CommonCode.GetIP();

                    string CheckSuggest = "";
                    if (vm.OrderStatusID == (short)OrderStatusEnum.NotPassApproval || vm.OrderStatusID == (short)OrderStatusEnum.PassedApproval)
                    {
                        CheckSuggest = vm.CheckSuggest;
                    }

                    if (vm.OrderStatusID != (int)OrderStatusEnum.NotPassApproval && vm.OrderStatusID != (int)OrderStatusEnum.PassedApproval)
                    {
                        historyAdded = true;
                        DAL.OrderHistory history = new OrderHistory()
                        {
                            DT_CREATEDATE = DateTime.Now,
                            ST_CREATEUSER = currentUser.UserID,
                            IPAddress = CommonCode.GetIP(),
                            Comment = GetOrderStatusEnum_Description(vm.OrderStatusID),
                            CheckSuggest = CheckSuggest,
                        };
                        query.OrderHistories.Add(history);
                    }

                    var OrderProducts_IDList = context.OrderProducts.Where(d => d.OrderID == vm.OrderID && !d.ParentProductMixedID.HasValue).Select(d => d.ID).ToList();
                    var vmProducts_IDList = vm.list_OrderProduct.Select(d => d.ID).ToList();

                    var idlist_update = OrderProducts_IDList.Intersect(vmProducts_IDList);
                    var idlist_delete = OrderProducts_IDList.Except(vmProducts_IDList);

                    //删除
                    var OrderProducts_Delete = context.OrderProducts.Where(d => idlist_delete.Contains(d.ID) && d.OrderID == vm.OrderID && !d.ParentProductMixedID.HasValue);
                    foreach (var item in OrderProducts_Delete)
                    {
                        context.OrderProducts.Remove(item);
                    }

                    //删除混装产品
                    var listProductsMixed_Delete = query.OrderProducts.Where(d => d.IsProductMixed && d.ParentProductMixedID.HasValue && d.OrderID == vm.OrderID);
                    if (listProductsMixed_Delete.Count() > 0)
                    {
                        context.OrderProducts.RemoveRange(listProductsMixed_Delete);
                    }

                    //更新
                    foreach (var item in idlist_update)
                    {
                        var queryProudct = query.OrderProducts.Where(d => d.ID == item).FirstOrDefault();
                        if (queryProudct != null)
                        {
                            var OrderProducts_Update = vm.list_OrderProduct.Find(d => d.ID == item);
                            queryProudct = SetEditProduct(OrderProducts_Update, queryProudct, currentUser.UserID);
                        }
                    }

                    bool isVaild_HSCode = true;
                    List<string> listValid_ProductNO = new List<string>();


                    foreach (var item in vm.list_OrderProduct.Where(d => d.ProductFrom == (short)OrderProductSource.Default))//添加 来源于销售订单的产品
                    {
                        var queryProudct = context.OrderProducts.Find(item.ID);
                        if (queryProudct != null)
                        {
                            if (!queryProudct.HSCode.HasValue)
                            {
                                isVaild_HSCode = false;
                                listValid_ProductNO.Add(queryProudct.No);
                            }
                            
                        }
                        
                    }

                    foreach (var item in vm.list_OrderProduct.Where(d => d.ProductFrom == (short)OrderProductSource.FromProduct))//添加 来源于产品
                    {
                        var queryProudct = context.Products.Where(d => !d.Deleted && d.ID == item.ProductID).FirstOrDefault();
                        if (queryProudct != null)
                        {
                            DAL.OrderProduct query_product = SetDBEntityFromViewModel(queryProudct, currentUser.UserID, item.FreightRate);

                            if (!query_product.HSCode.HasValue)
                            {
                                isVaild_HSCode = false;
                                listValid_ProductNO.Add(query_product.No);
                            }

                            query_product = SetEditProduct(item, query_product, currentUser.UserID);
                            query.OrderProducts.Add(query_product);
                        }
                    }

                    foreach (var item in vm.list_OrderProduct.Where(d => d.ProductFrom == (short)OrderProductSource.FromQuoteProduct))//添加 来源于报价单
                    {
                        var queryProudct = context.Quot_QuotProduct.Where(d => !d.IsDelete && d.ID == item.QuotProductID).FirstOrDefault();
                        if (queryProudct != null)
                        {
                            DAL.OrderProduct query_product = SetDBEntityFromViewModel_QuoteProduct(queryProudct, currentUser.UserID);

                            if (!query_product.HSCode.HasValue)
                            {
                                isVaild_HSCode = false;
                                listValid_ProductNO.Add(query_product.No);
                            }

                            query_product = SetEditProduct(item, query_product, currentUser.UserID);
                            query.OrderProducts.Add(query_product);
                        }
                    }

                    if (!isVaild_HSCode && vm.OrderStatusID != (int)OrderStatusEnum.OutLine)//当不是草稿时，需要判断报关编码是否为空。
                    {
                        result.IsSuccess = false;
                        result.Msg = "产品的报关编码不能为空！产品编号如下：" + string.Join(",", listValid_ProductNO.ToArray()) + "。";
                        return result;
                    }

                    int affectRows = context.SaveChanges();
                    if (affectRows == 0)
                    {
                        result.IsSuccess = false;
                        result.Msg = "出错了！";
                        return result;
                    }
                    else
                    {
                        result.IsSuccess = true;

                        ExecuteApproval(query.ST_CREATEUSER, query.OrderID, CheckSuggest, vm.OrderStatusID, currentUser.UserID, historyAdded);//执行审批流

                        SaveOther(currentUser, query.OrderID);

                        RemoveOneCachedOrder(currentUser.UserID, vm.OrderID);

                        Save_ProductFitting(currentUser.UserID, vm.OrderID);

                        Save_ProductMixed(currentUser.UserID, vm.OrderID, vm.listProducts_Mixed);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return result;
        }

        /// <summary>
        /// 添加产品配件
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="OrderID"></param>
        private void Save_ProductFitting(int UserID, int OrderID)
        {
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Orders.Find(OrderID);
                    if (query != null)
                    {
                        var ProductFittings = new List<DAL.ProductFitting>();
                        foreach (var item in query.OrderProducts.Where(d => !d.IsDelete && d.IsProductFitting && !d.ParentProductMixedID.HasValue))
                        {
                            var temp2 = context.ProductFittings.Where(d => d.ParentID == item.ID && !d.Deleted && d.ModuleType == (int)ModuleTypeEnum.Order);
                            context.ProductFittings.RemoveRange(temp2);

                            var list = context.ProductFittings.Where(d => d.ParentID == item.ProductID && !d.Deleted && d.ModuleType == (int)ModuleTypeEnum.Product);

                            foreach (var item2 in list)
                            {
                                DAL.ProductFitting query_ProductFitting = item2;
                                query_ProductFitting.ParentID = item.ID;
                                query_ProductFitting.ModuleType = (int)ModuleTypeEnum.Order;

                                query_ProductFitting.DT_CREATEDATE = DateTime.Now;
                                query_ProductFitting.ST_CREATEUSER = UserID;
                                query_ProductFitting.DT_MODIFYDATE = DateTime.Now;
                                query_ProductFitting.ST_MODIFYUSER = UserID;
                                query_ProductFitting.Deleted = false;
                                context.ProductFittings.Add(query_ProductFitting);
                            }
                        }

                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
        }

        /// <summary>
        /// 保存混装产品
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="context"></param>
        /// <param name="quotID"></param>
        private void Save_ProductMixed(int UserID, int OrderID, List<VMOrderProduct> list)
        {
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {

                    #region 添加混装产品

                    var query2 = context.OrderProducts.Where(d => d.OrderID == OrderID && !d.IsDelete && d.IsProductMixed && !d.ParentProductMixedID.HasValue);
                    if (query2 != null && query2.Count() > 0)
                    {
                        foreach (var item in query2)
                        {
                            var thisProduct2 = list.Where(d => (d.ParentProductMixedID < 0 && d.ParentProductMixedID == -item.ProductID) || (d.ParentProductMixedID > 0 && d.ParentProductMixedID == item.ID));
                            if (thisProduct2.Count() > 0)
                            {
                                foreach (var item2 in thisProduct2)
                                {
                                    var queryProductMixed = context.Products.Where(d => d.IsProductMixed && !d.Deleted && d.ParentProductMixedID == item.ProductID && d.ID == item2.ProductID);
                                    if (queryProductMixed.Count() > 0)
                                    {
                                        var thisProduct = queryProductMixed.First();
                                        DAL.OrderProduct product = SetDBEntityFromViewModel(thisProduct, UserID, item2.FreightRate);

                                        product = SetEditProduct(item2, product, UserID);

                                        product.ParentProductMixedID = item.ID;
                                        product.OrderID = item.OrderID;

                                        context.OrderProducts.Add(product);
                                    }
                                }

                            }

                        }
                        int i = context.SaveChanges();

                    }
                    #endregion 添加混装产品



                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
        }

        /// <summary>
        /// 销售订单——审核已通过时，添加第三方验货
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="id"></param>
        public void SaveOther(VMERPUser currentUser, int OrderID)
        {
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Orders.Find(OrderID);

                    if (query.OrderStatusID == (int)OrderStatusEnum.PassedApproval)
                    {
                        #region 审核已通过并勾选了第三方验货复选框，添加第三方验货

                        if (query.IsThirdVerification)
                        {
                            DAL.Purchase_ThirdPartyVerification DAL_ThirdPartyVerification = new Purchase_ThirdPartyVerification()
                            {
                                OrderID = query.OrderID,
                                StatusID = (short)ThirdPartyVerificationStatusEnum.PendingUpload,

                                DT_CREATEDATE = DateTime.Now,
                                ST_CREATEUSER = query.ST_CREATEUSER,
                                ST_MODIFYUSER = currentUser.UserID,
                                DT_MODIFYDATE = DateTime.Now,
                                IPAddress = CommonCode.GetIP(),
                            };
                            context.Purchase_ThirdPartyVerification.Add(DAL_ThirdPartyVerification);

                            context.SaveChanges();
                        }

                        #endregion 审核已通过并勾选了第三方验货复选框，添加第三方验货

                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
        }

        /// <summary>
        /// POID和EHIPO是否重复
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static bool IsRepeat(VMOrderEdit vm, ERPEntitiesNew context)
        {
            if (vm.EHIPO != null)
            {
                vm.EHIPO = vm.EHIPO.Trim();
            }

            bool isRepeat = false;
            var queryOrder = context.Orders.Where(d => !d.IsDelete && d.OrderID != vm.OrderID);
            int POID_Count = queryOrder.Where(d => d.POID == vm.POID).Count();
            int EHIPO_Count = queryOrder.Where(d => d.EHIPO == vm.EHIPO).Count();

            if (POID_Count > 0)
            {
                isRepeat = true;
            }
            if (!string.IsNullOrEmpty(vm.EHIPO) && EHIPO_Count > 0)
            {
                isRepeat = true;
            }
            return isRepeat;
        }

        public List<VMViewProductList> GetViewProductList(VMERPUser currentUser, List<int> idList)
        {
            List<VMViewProductList> list_vm = new List<VMViewProductList>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Orders.Where(d => idList.Contains(d.OrderID));
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForOrder);
                    foreach (var item in query)
                    {
                        var query_product = item.OrderProducts.Where(d => !d.IsDelete && !d.ParentProductMixedID.HasValue);
                        foreach (var item_product in query_product)
                        {
                            list_vm.Add(new VMViewProductList()
                            {
                                No = item_product.No,
                                Image = item_product.Image,
                            });
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

        /// <summary>
        /// 修改销售订单编辑的产品信息
        /// </summary>
        /// <param name="item"></param>
        /// <param name="query_product"></param>
        /// <returns></returns>
        private static DAL.OrderProduct SetEditProduct(VMOrderProduct item, DAL.OrderProduct query_product, int userID)
        {
            #region 保存从客户里面取数据

            query_product.MiscImportLoad = item.MiscImportLoad;
            query_product.FreightRate = item.FreightRate;
            query_product.Commission = item.Commission;
            query_product.Agent = item.Agent;
            query_product.Allowance = item.Allowance;
            query_product.Mu = item.MUPercent;
            //query_product.Fobnet = item.FOBNET;
            query_product.FinalFOB = item.FinalFOB;
            query_product.CtnsPallet = item.CtnsPallet;
            query_product.PcsPallet = item.PcsPallet;
            query_product.PalletPc = item.PalletPc;

            #endregion 保存从客户里面取数据

            query_product.CommissionAmount = item.CommissionAmount;
            query_product.SkuNumber = item.SkuNumber;
            query_product.SkuCode = item.SkuCode;
            query_product.UPC = item.UPC;
            query_product.QualityAssuranceTesting = item.QualityAssuranceTesting;
            query_product.Desc = item.Desc;
            query_product.Qty = item.Qty;
            query_product.Qty2 = item.Qty2;
            //query_product.PriceFactory = item.PriceFactory;
            query_product.SalePrice = item.SalePrice;
            query_product.NetPrice = item.NetPrice;
            query_product.Elc = item.ELC;
            query_product.RetailPrice = item.RetailPrice;

            query_product.ST_MODIFYUSER = userID;
            query_product.DT_MODIFYDATE = DateTime.Now;

            query_product.SortIndex = item.SortIndex;
            query_product.QuoteTemplateFileName = item.QuoteTemplateFileName;
            query_product.ELCFill = item.ELCFill;
            query_product.Rate = item.Rate;
            query_product.Department = item.Department;
            query_product.ColorName = item.ColorName;
            query_product.INR = item.INR;
            query_product.Remarks = item.Remarks;
            query_product.OfStoreReadyInnersEnclosed = item.OfStoreReadyInnersEnclosed;
            query_product.PreTicketed = item.PreTicketed;
            return query_product;
        }

        private DAL.OrderProduct SetDBEntityFromViewModel(DAL.Product entity, int userID, decimal? FreightRate)
        {
            return new DAL.OrderProduct()
            {
                #region 赋值公共的

                ID = entity.ID,
                No = entity.No,
                NoFactory = entity.NoFactory,
                Name = entity.Name,
                Desc = entity.Desc,
                Image = entity.Image,
                Length = entity.Length,
                Height = entity.Height,
                Width = entity.Width,
                Weight = entity.Weight,
                IngredientZh = entity.IngredientZh,
                IngredientEn = entity.IngredientEn,
                MOQZh = entity.MOQZh,
                MOQEn = entity.MOQEn,
                PDQPackRate = entity.PDQPackRate,
                PDQLength = entity.PDQLength,
                PDQWidth = entity.PDQWidth,
                PDQHeight = entity.PDQHeight,
                InnerBoxRate = entity.InnerBoxRate,
                InnerLength = entity.InnerLength,
                InnerWidth = entity.InnerWidth,
                InnerHeight = entity.InnerHeight,
                InnerWeight = entity.InnerWeight,
                InnerWeightGross = entity.InnerWeightGross,
                OuterBoxRate = entity.OuterBoxRate,
                OuterLength = entity.OuterLength,
                OuterWidth = entity.OuterWidth,
                OuterHeight = entity.OuterHeight,
                PriceFactory = entity.PriceFactory,
                MiscImportLoad = entity.MiscImportLoad,
                SRP = entity.SRP,
                CtnsPallet = entity.CtnsPallet,
                DutyPercent = entity.DutyPercent,
                Remarks = entity.Remarks,
                Comment = entity.Comment,
                UPC = entity.UPC,

                LengthIN = entity.LengthIN,
                HeightIN = entity.HeightIN,
                WidthIN = entity.WidthIN,
                WeightLBS = entity.WeightLBS,
                PDQLengthIN = entity.PDQLengthIN,
                PDQWidthIN = entity.PDQWidthIN,
                PDQHeightIN = entity.PDQHeightIN,
                InnerLengthIN = entity.InnerLengthIN,
                InnerWidthIN = entity.InnerWidthIN,
                InnerHeightIN = entity.InnerHeightIN,
                OuterVolume = entity.OuterVolume,
                InnerVolume = entity.InnerVolume,
                InnerWeightLBS = entity.InnerWeightLBS,
                InnerWeightGrossLBS = entity.InnerWeightGrossLBS,
                OuterLengthIN = entity.OuterLengthIN,
                OuterWidthIN = entity.OuterWidthIN,
                OuterHeightIN = entity.OuterHeightIN,
                Fobfty = entity.FOBFTY,
                Fobnet = entity.FOBNET,
                Rate = entity.Rate,
                FinalFOB = entity.FinalFOB,
                MiscImportLoadAmount = entity.MiscImportLoadAmount,
                PcsPallet = entity.PcsPallet,
                PalletPc = entity.PalletPc,
                Duty = entity.Duty,
                FobChinaLCL = entity.FOBChinaLCL,
                Cost = entity.Cost,
                Commission = entity.Commission,
                CommissionAmount = entity.CommissionAmount,

                Allowance = entity.Allowance,
                CustomerID = entity.CustomerID,
                FreightRate = entity.FreightRate,
                Agent = entity.Agent,
                FactoryID = entity.FactoryID,

                StyleID = entity.StyleID,
                PortID = entity.PortID,
                PackingMannerZhID = entity.PackingMannerZhID,
                PackingMannerEnID = entity.PackingMannerEnID,
                UnitID = entity.UnitID,
                CurrencyType = entity.CurrencyType,

                HTS = entity.HTS,
                HSCode = entity.HSCode,

                #endregion 赋值公共的

                ProductID = entity.ID,
                Fobus = _productServices.CalculateFOBUS(entity, FreightRate ?? 0),
                AgentAmount = _productServices.CalculateAgentAmount(entity),
                Freight = _productServices.CalculateFreight(entity, FreightRate ?? 0),

                CheckSuggest = "",
                PortEnID = entity.PortID,
                TermsID = 0,

                DT_CREATEDATE = DateTime.Now,
                DT_MODIFYDATE = DateTime.Now,
                ST_CREATEUSER = userID,
                ST_MODIFYUSER = userID,
                IPAddress = CommonCode.GetIP(),
                IsDelete = false,
                FOBChinaPort = entity.FOBChinaPort,
                OuterWeightGross = entity.OuterWeightGross,
                OuterWeightNet = entity.OuterWeightNet,
                OuterWeightGrossLBS = entity.OuterWeightGrossLBS,
                OuterWeightNetLBS = entity.OuterWeightNetLBS,
                ValidDate = entity.ValidDate,
                PriceInputDate = entity.PriceInputDate,
                Season = entity.Season,
                ProductCopyRight = entity.ProductCopyRight,
                ColorID = entity.ColorID,
                IsProductFitting = entity.IsProductFitting,
                IsProductMixed = entity.IsProductMixed,
                ParentProductMixedID = entity.ParentProductMixedID,
            };
        }

        private static DAL.OrderProduct SetDBEntityFromViewModel_QuoteProduct(DAL.Quot_QuotProduct entity, int userID)
        {
            return new DAL.OrderProduct()
            {
                #region 赋值公共的

                ID = entity.ID,
                No = entity.No,
                NoFactory = entity.NoFactory,
                Name = entity.Name,
                Desc = entity.Desc,
                Image = entity.Image,
                Length = entity.Length,
                Height = entity.Height,
                Width = entity.Width,
                Weight = entity.Weight,
                IngredientZh = entity.IngredientZh,
                IngredientEn = entity.IngredientEn,
                MOQZh = entity.MOQZh,
                MOQEn = entity.MOQEn,
                PDQPackRate = entity.PDQPackRate,
                PDQLength = entity.PDQLength,
                PDQWidth = entity.PDQWidth,
                PDQHeight = entity.PDQHeight,
                InnerBoxRate = entity.InnerBoxRate,
                InnerLength = entity.InnerLength,
                InnerWidth = entity.InnerWidth,
                InnerHeight = entity.InnerHeight,
                InnerWeight = entity.InnerWeight,
                InnerWeightGross = entity.InnerWeightGross,
                OuterBoxRate = entity.OuterBoxRate,
                OuterLength = entity.OuterLength,
                OuterWidth = entity.OuterWidth,
                OuterHeight = entity.OuterHeight,
                PriceFactory = entity.PriceFactory ?? 0,
                MiscImportLoad = entity.MiscImportLoad,
                SRP = entity.SRP,
                CtnsPallet = entity.CtnsPallet,
                DutyPercent = entity.DutyPercent,
                Remarks = entity.Remarks,
                Comment = entity.Comment,
                UPC = entity.UPC,

                LengthIN = entity.LengthIN,
                HeightIN = entity.HeightIN,
                WidthIN = entity.WidthIN,
                WeightLBS = entity.WeightLBS,
                PDQLengthIN = entity.PDQLengthIN,
                PDQWidthIN = entity.PDQWidthIN,
                PDQHeightIN = entity.PDQHeightIN,
                InnerLengthIN = entity.InnerLengthIN,
                InnerWidthIN = entity.InnerWidthIN,
                InnerHeightIN = entity.InnerHeightIN,
                OuterVolume = entity.OuterVolume,
                InnerVolume = entity.InnerVolume,
                InnerWeightLBS = entity.InnerWeightLBS,
                InnerWeightGrossLBS = entity.InnerWeightGrossLBS,
                OuterLengthIN = entity.OuterLengthIN,
                OuterWidthIN = entity.OuterWidthIN,
                OuterHeightIN = entity.OuterHeightIN,
                Fobfty = entity.FOBFTY,
                Fobnet = entity.FOBNET,
                Rate = entity.Rate,
                FinalFOB = entity.FinalFOB,
                MiscImportLoadAmount = entity.MiscImportLoadAmount,
                PcsPallet = entity.PcsPallet,
                PalletPc = entity.PalletPc,
                Duty = entity.Duty,
                FobChinaLCL = entity.FOBChinaLCL,
                Cost = entity.Cost,
                Commission = entity.Commission,
                CommissionAmount = entity.CommissionAmount,

                Allowance = entity.Allowance,
                CustomerID = entity.CustomerID,
                FreightRate = entity.FreightRate,
                Agent = entity.Agent,
                FactoryID = entity.FactoryID,

                StyleID = entity.StyleID,
                PortID = entity.PortID,
                PackingMannerZhID = entity.PackingMannerZhID,
                PackingMannerEnID = entity.PackingMannerEnID,
                UnitID = entity.UnitID,
                CurrencyType = entity.CurrencyType,

                HTS = entity.HTS,
                HSCode = entity.HSCode,

                #endregion 赋值公共的

                ProductID = entity.ProductID,
                Fobus = entity.FOBUS,
                Ddp = entity.DDP,
                Poe = entity.POE,
                Mu = entity.MU,
                AgentAmount = entity.AgentAmount,
                Elc = entity.ELC,
                Retail = entity.Retail,
                Freight = entity.Freight,
                CheckSuggest = "",
                PortEnID = entity.PortEnID,
                TermsID = entity.TermsID,

                DT_CREATEDATE = DateTime.Now,
                DT_MODIFYDATE = DateTime.Now,
                ST_CREATEUSER = userID,
                ST_MODIFYUSER = userID,
                IPAddress = CommonCode.GetIP(),
                IsDelete = false,
                FOBChinaPort = entity.FOBChinaPort,
                OuterWeightGross = entity.OuterWeightGross,
                OuterWeightNet = entity.OuterWeightNet,
                OuterWeightGrossLBS = entity.OuterWeightGrossLBS,
                OuterWeightNetLBS = entity.OuterWeightNetLBS,
                ValidDate = entity.ValidDate,
                PriceInputDate = entity.PriceInputDate,
                Season = entity.Season,
                ProductCopyRight = entity.ProductCopyRight,
                ColorID = entity.ColorID,
                IsProductFitting = entity.IsProductFitting,
                IsProductMixed = entity.IsProductMixed,
                ParentProductMixedID = entity.ParentProductMixedID,
            };
        }

        #region Buying Confirmation

        /// <summary>
        /// 生成Buying Confirmation
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="idList"></param>
        /// <returns></returns>
        public List<string> Create_BuyingConfirmation(VMERPUser currentUser, string idList)
        {
            List<string> makeFileList = new List<string>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    string GeneratedFileList = "";
                    string UploadFileList = "";

                    var query = context.BuyingConfirmations.Where(d => d.OrderIDList == idList && !d.IsDelete);
                    if (query.Count() > 0)
                    {
                        query.First().DT_MODIFYDATE = DateTime.Now;
                        query.First().ST_MODIFYUSER = currentUser.UserID;
                        query.First().GeneratedFileList = GeneratedFileList;
                    }
                    else
                    {
                        DAL.BuyingConfirmation query_BuyingConfirmation = new BuyingConfirmation()
                        {
                            GeneratedFileList = GeneratedFileList,
                            OrderIDList = idList,
                            UploadFileList = UploadFileList,

                            DT_CREATEDATE = DateTime.Now,
                            DT_MODIFYDATE = DateTime.Now,
                            IPAddress = CommonCode.GetIP(),
                            ST_CREATEUSER = currentUser.UserID,
                            ST_MODIFYUSER = currentUser.UserID,
                            IsDelete = false,
                        };
                        context.BuyingConfirmations.Add(query_BuyingConfirmation);
                    }
                    int i = context.SaveChanges();
                    if (i > 0)
                    {
                        var query2 = context.BuyingConfirmations.Where(d => d.OrderIDList == idList && !d.IsDelete);
                        if (query2.Count() > 0)
                        {
                            List<VMOrderProduct> list_product = new List<VMOrderProduct>();
                            foreach (var id in CommonCode.IdListToList(idList))
                            {
                                var vm = GetDetailByID(currentUser, id, 1);
                                if (vm != null)
                                {
                                    list_product.AddRange(vm.list_OrderProduct);
                                }
                            }

                            if (list_product != null && list_product.Count > 0)
                            {
                                DateTime OrderDateStart = list_product.Min(d => d.OrderDateStart);
                                DateTime OrderDateEnd = list_product.Max(d => d.OrderDateEnd);

                                foreach (var item in list_product)
                                {
                                    item.OrderDateStart = OrderDateStart;
                                    item.OrderDateEnd = OrderDateEnd;
                                }

                                var temp = list_product.GroupBy(d => d.No);
                                foreach (var item in temp)
                                {
                                    List<string> filesAsolutePathList = new List<string>();
                                    List<string> filesPhysicalPathList = new List<string>();
                                    string No = item.First().No;

                                    string filePath = MakerExcel_BuyingConfirmation.Maker(item, MakerTypeEnum.Order_BuyingConfirmation, query.First().ID, No, SelectCustomerEnum.S188.ToString());
                                    makeFileList.Add(filePath);
                                }
                            }
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

        /// <summary>
        /// 获取上传附件的详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public VMUpLoad GetUploadDetial(int id)
        {
            VMUpLoad vm = null;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.BuyingConfirmations.Where(d => d.OrderIDList.Contains(id.ToString()) && !d.IsDelete);
                    if (query != null && query.Count() > 0)
                    {
                        var list = query.OrderByDescending(d => d.DT_MODIFYDATE).Select(d => d.OrderIDList);
                        foreach (var item in list)
                        {
                            var OrderIDList = CommonCode.IdListToList(item);
                            if (OrderIDList.Contains(id))
                            {
                                var query2 = context.BuyingConfirmations.Where(d => d.OrderIDList == item && !d.IsDelete);
                                if (query2 != null && query2.Count() > 0)
                                {
                                    vm = new VMUpLoad()
                                    {
                                        ID = query2.First().ID,
                                        UpLoadFileList = ConstsMethod.GetUploadFileList(query2.First().ID, UploadFileType.BuyingConfirmation),
                                    };

                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        vm = new VMUpLoad()
                        {
                            ID = -1,
                        };
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
        /// 保存上传附件
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public DBOperationStatus UpLoad(VMERPUser currentUser, int id, VMUpLoad vm)
        {
            DBOperationStatus result = default(DBOperationStatus);
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.BuyingConfirmations.Where(p => p.ID == id);
                    var dataFromDB = query.FirstOrDefault();
                    if (dataFromDB != null)
                    {
                        dataFromDB.ST_MODIFYUSER = currentUser.UserID;
                        dataFromDB.DT_MODIFYDATE = DateTime.Now;

                        ConstsMethod.SaveFileUpload(currentUser, id, vm.UpLoadFileList, context, UploadFileType.BuyingConfirmation);

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
            }

            return result;
        }

        /// <summary>
        /// 获取上传附件的详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public VMUpLoad GetUploadDetial_Order(int id)
        {
            VMUpLoad vm = null;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    vm = new VMUpLoad()
                    {
                        ID = id,
                        UpLoadFileList = ConstsMethod.GetUploadFileList(id, UploadFileType.Order),
                    };
                }

            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return vm;
        }

        /// <summary>
        /// 保存上传附件
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public DBOperationStatus UpLoad_Order(VMERPUser currentUser, int id, VMUpLoad vm)
        {
            DBOperationStatus result = default(DBOperationStatus);
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    ConstsMethod.SaveFileUpload(currentUser, id, vm.UpLoadFileList, context, UploadFileType.Order);

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
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return result;
        }
        /// <summary>
        /// 下载BC
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<string> DownLoadBC(VMERPUser currentUser, int id)
        {
            List<string> makeFileList = new List<string>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    List<VMOrderProduct> list_product = new List<VMOrderProduct>();

                    List<int> idList = new List<int>();

                    var query = context.BuyingConfirmations.Where(d => d.OrderIDList.Contains(id.ToString()) && !d.IsDelete);
                    if (query != null && query.Count() > 0)
                    {
                        var list = query.OrderByDescending(d => d.DT_MODIFYDATE).Select(d => d.OrderIDList);
                        foreach (var item in list)
                        {
                            var query2 = context.BuyingConfirmations.Where(d => d.OrderIDList == item && !d.IsDelete).First();

                            var ID = query2.ID;
                            var UpLoadFileList = ConstsMethod.GetUploadFileList(query2.ID, UploadFileType.BuyingConfirmation);

                            string path = "/data/Template/Out/Order_BuyingConfirmation/" + query2.ID + "/PDFAndExcel";

                            if (UpLoadFileList != null && UpLoadFileList.Count > 0)
                            {
                                foreach (var item2 in UpLoadFileList)
                                {
                                    string sourceFileName = ConstsMethod.ReplaceURLToLocalPath(item2.ServerFileName);
                                    string destFileName = Utils.GetMapPath(path + "/" + item2.DisplayFileName);
                                    if (File.Exists(sourceFileName))
                                    {
                                        if (!Directory.Exists(Utils.GetMapPath(path)))
                                        {
                                            Directory.CreateDirectory(Utils.GetMapPath(path));
                                        }

                                        File.Copy(sourceFileName, destFileName, true);
                                    }
                                }
                            }

                            string path2 = Utils.GetMapPath(path);
                            if (System.IO.Directory.Exists(path2))
                            {
                                string tempPath = "/data/Template/Out/Order_BuyingConfirmation/" + query2.ID + "/" + CommonCode.GetTimeStamp() + ".zip";

                                ExcelHelper.Zip(path2, Utils.GetMapPath(tempPath));

                                makeFileList.Add(tempPath);
                            }

                            break;
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

        #endregion Buying Confirmation

        /// <summary>
        /// 下载SC
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<string> DownLoadSC(VMERPUser currentUser, int id)
        {
            List<string> makeFileList = new List<string>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    List<VMOrderProduct> list_product = new List<VMOrderProduct>();

                    List<int> idList = new List<int>();

                    string filePath = "";
                    var vm = GetDetailByID(currentUser, id, 1);
                    if (vm.SelectCustomer == SelectCustomerEnum.S13.ToString())
                    {
                        filePath = MakerExcel_BuyingConfirmation.Maker(vm.list_OrderProduct, MakerTypeEnum.Order_BuyingConfirmation, id, CommonCode.GetTimeStamp(), SelectCustomerEnum.S13.ToString());
                    }
                    else if (vm.SelectCustomer == SelectCustomerEnum.S56.ToString())
                    {
                        filePath = MakerExcel_BuyingConfirmation.Maker(vm.list_OrderProduct, MakerTypeEnum.Order_BuyingConfirmation, id, CommonCode.GetTimeStamp(), SelectCustomerEnum.S56.ToString());
                    }

                    makeFileList.Add(filePath);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return makeFileList;
        }

        #endregion UserMethod

        #region PublicMethod

        /// <summary>
        /// 选择销售订单 新建采购合同页面使用到了
        /// </summary>
        public List<DTOOrder> SelectOrder(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows,
            VMSelectOrder vm)
        {
            List<DTOOrder> listModel = null;
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Orders.Include("Orders_Customers").Where(p => !p.IsDelete).Where(p => p.OrderStatusID == (int)OrderStatusEnum.PassedApproval);

                    #region 筛选条件

                    if (vm.CustomerID != 0)
                    {
                        query = query.Where(p => p.CustomerID == vm.CustomerID);
                    }
                    if (!string.IsNullOrEmpty(vm.POID))
                    {
                        query = query.Where(p => p.POID.Contains(vm.POID));
                    }
                    if (vm.DateStart != new DateTime())
                    {
                        query = query.Where(p => p.OrderDateStart >= vm.DateStart);
                    }
                    if (vm.DateEnd != new DateTime())
                    {
                        query = query.Where(p => p.OrderDateEnd <= vm.DateEnd);
                    }
                    if (!string.IsNullOrEmpty(vm.OrderNumber))
                    {
                        query = query.Where(p => p.OrderNumber.Contains(vm.OrderNumber));
                    }

                    if (vm.usingOccasion == (int)SelectOrdersOccasion.NewPurContract)//一个销售订单只能生成一次采购合同
                    {
                        List<int?> list_OrderID = context.Purchase_Contract.Where(d => !d.IsDelete).Select(d => d.OrderID).Distinct().ToList();

                        query = query.Where(d => !list_OrderID.Contains(d.OrderID));
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
                        listModel = new List<DTOOrder>();
                        foreach (var entity in dataFromDB)
                        {
                            listModel.Add(new DTOOrder()
                            {
                                OrderID = entity.OrderID,
                                OrderNumber = entity.OrderNumber,
                                POID = entity.POID,
                                CustomerNo = entity.Orders_Customers.CustomerCode,
                                CustomerDate = Utils.DateTimeToStr(entity.CustomerDate),
                                OrderAmount = entity.OrderAmount,
                                OrderDateStart = Utils.DateTimeToStr(entity.OrderDateStart),
                                OrderDateEnd = Utils.DateTimeToStr(entity.OrderDateEnd),
                                OrderOrigin = entity.OrderOrigin,
                                OrderStatusName = GetOrderStatusEnum_Description(entity.OrderStatusID),
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

        #endregion PublicMethod

        #region 定时缓存销售订单

        /// <summary>
        /// 缓存一个销售订单
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public bool CacheOneOrder(int userID, VMOrderEdit vm)
        {
            bool result = false;
            try
            {
                var dto = new DTOMongo<VMOrderEdit>() { LastUpdateDate = DateTime.Now, UserID = userID, Type = MongoCachedTypes.OrderEditing, Data = vm };
                var dtoJSON = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
                var document = BsonDocument.Parse(dtoJSON);

                var builder = Builders<BsonDocument>.Filter;
                var filter = builder.Eq("UserID", userID);
                filter &= builder.Eq("Type", MongoCachedTypes.OrderEditing);
                filter &= builder.Eq("Data.OrderID", vm.OrderID);

                new MongoDBHelper().UpsetOne(document, MongoDBHelper.CollectionName_UserCache, filter);
                result = true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return result;
        }

        /// <summary>
        /// 获取缓存的销售订单
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public VMOrderEdit GetOneCachedOrder(int userID, int id)
        {
            VMOrderEdit vm = null;
            try
            {
                var builder = Builders<BsonDocument>.Filter;
                var filter = builder.Eq("UserID", userID);
                filter &= builder.Eq("Type", MongoCachedTypes.OrderEditing);
                filter &= builder.Eq("Data.OrderID", id);

                BsonDocument document = new MongoDBHelper().GetOneRecord(filter, MongoDBHelper.CollectionName_UserCache);
                if (document != null)
                {
                    DTOMongo<VMOrderEdit> cachedData = BsonSerializer.Deserialize<DTOMongo<VMOrderEdit>>(document);
                    if (cachedData != null)
                    {
                        vm = cachedData.Data;
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
        /// 移除缓存的销售订单
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="productID"></param>
        /// <returns></returns>
        public bool RemoveOneCachedOrder(int userID, int id)
        {
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("UserID", userID);
            filter &= builder.Eq("Type", MongoCachedTypes.OrderEditing);
            filter &= builder.Eq("Data.OrderID", id);

            return new MongoDBHelper().RemoveOneRecord(filter, MongoDBHelper.CollectionName_UserCache);
        }

        #endregion 定时缓存销售订单

        #region 审批流

        /// <summary>
        /// 首页——获取待审核的销售订单的数量
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public int GetPendingApproveCountByUser(VMERPUser currentUser)
        {
            int count = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Orders.Where(p => !p.IsDelete);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForOrder);
                    query = query.Where(d => d.OrderStatusID == (short)OrderStatusEnum.PendingApproval);

                    var result = (from q in query
                                  select new
                                  {
                                      CreateUserID = q.ST_CREATEUSER,
                                      ApproverIndex = q.ApproverIndex,
                                  }).ToList();

                    // GetApproverIndexAndUsers 已被删除，需要通过部门来区别是否属于自己的审核范围内
                    count = result.Count;
                    //ApprovalServices approvalServices = new ApprovalServices();
                    //Dictionary<int, List<int>> dictIndexesAndUsers = approvalServices.GetApproverIndexAndUsers(WorkflowTypes.ApprovalOrder, currentUser);
                    //foreach (var q in result)
                    //{
                    //    foreach (var kvp in dictIndexesAndUsers)
                    //    {
                    //        if (q.ApproverIndex.HasValue && q.ApproverIndex == kvp.Key && kvp.Value.Contains(q.CreateUserID))
                    //        {
                    //            count++;
                    //            break;
                    //        }
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return count;
        }

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
            if (StatusID == (int)OrderStatusEnum.NotPassApproval)
            {
                isPass = false;
            }

            List<int> validWaitingApproveStatus = new List<int>(){
                            (int)OrderStatusEnum.PendingApproval,
                            (int)OrderStatusEnum.NotPassApproval
                        };
            ApprovalService.ExcuteApproval(new ApprovalInfo
            {
                WorkflowType = WorkflowTypes.ApprovalOrder,
                IsPass = isPass,
                IdentityID = identityID,
                ValidWaitingApproveStatus = validWaitingApproveStatus,
                StatusApproving = (int)OrderStatusEnum.PendingApproval,
                StatusNextTo = (int)OrderStatusEnum.PassedApproval,
                StatusRejected = (int)OrderStatusEnum.NotPassApproval,
                ApproveUserID = UserID,
                ApproveOpinion = CheckSuggest,
                LogMethod = () =>
                {
                    if (!historyAdded)
                    {
                        var history = new DAL.OrderHistory
                        {
                            Comment = GetOrderStatusEnum_Description(StatusID),
                            CheckSuggest = CheckSuggest,
                            DT_CREATEDATE = DateTime.Now,
                            ST_CREATEUSER = UserID,
                            IPAddress = CommonCode.GetIP(),
                        };
                        return history;
                    }
                    return null;
                }
            });
        }

        #endregion 审批流


        public List<VMOrderProduct> GetProducts_Mixed(VMERPUser currentUser, int ID)
        {
            List<VMOrderProduct> listModel = new List<VMOrderProduct>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {

                    var query = context.OrderProducts.Where(p => p.IsProductMixed && p.ParentProductMixedID == ID && !p.IsDelete);

                    // 先查出字典表数据
                    List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                    List<DAL.HarmonizedSystem> list_HarmonizedSystem = context.HarmonizedSystems.Where(d => !d.IsDelete).ToList();


                    foreach (var item in query)
                    {

                        VMOrderProduct vm_product = GetProduct(item, context, list_Com_DataDictionary);

                        #region 计算Model

                        var HarmonizedSystem = list_HarmonizedSystem.Where(d => d.ID == vm_product.HTS);
                        if (HarmonizedSystem.Count() > 0)
                        {
                            vm_product.HTS_Name = HarmonizedSystem.FirstOrDefault().HSCode;
                        }

                        HarmonizedSystem = list_HarmonizedSystem.Where(d => d.ID == Utils.StrToInt(vm_product.HSCode, 0));
                        if (HarmonizedSystem.Count() > 0)
                        {
                            vm_product.HSCode_Name = HarmonizedSystem.FirstOrDefault().HSCode;
                        }

                        vm_product.MOQEn = _productServices.CalculateMOQ(vm_product.MOQEn, vm_product.Cost);

                        #endregion 计算Model

                        vm_product.DutyPercentFormatter = vm_product.DutyPercent.HasValue ? vm_product.DutyPercent + "%" : "";
                        vm_product.CommissionFormatter = vm_product.Commission + "%";
                        vm_product.MiscImportLoadFormatter = vm_product.MiscImportLoad.HasValue ? vm_product.MiscImportLoad + "%" : "";
                        vm_product.FactoryID_ForQuote = vm_product.FactoryID;

                        vm_product.PriceFactoryFormatter = vm_product.CurrencySign + vm_product.PriceFactory;

                        vm_product.ProductAmountFormatter = vm_product.CurrencySign + (vm_product.PriceFactory * vm_product.Qty);

                        int HSCode = Utils.StrToInt(vm_product.HSCode, 0);
                        if (HSCode > 0)
                        {
                            vm_product.IsNeedInspection = ConstsMethod.IsNeedInspection(list_HarmonizedSystem, HSCode);
                            vm_product.IsNeedInspectionName = vm_product.IsNeedInspection ? "是" : "否";
                        }

                        vm_product.SumSalePrice = vm_product.Qty * vm_product.SalePrice;

                        listModel.Add(vm_product);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return listModel;
        }
    }
}