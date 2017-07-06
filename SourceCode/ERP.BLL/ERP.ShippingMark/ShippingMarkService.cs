using ERP.BLL.Consts;
using ERP.BLL.ERP.Consts;
using ERP.BLL.Workflow;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.Purchase;
using ERP.Models.ShippingMark;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace ERP.BLL.ERP.ShippingMark
{
    public class ShippingMarkService
    {
        private ERP.Dictionary.DictionaryServices _dictionaryServices = new ERP.Dictionary.DictionaryServices();

        #region HelperMethod

        /// <summary>
        /// 获取唛头资料状态的值
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public static string GetShippingMark(int id)
        {
            Dictionary<int, string> dictionary = GetShippingMarkEnum();
            string name = "";
            foreach (var item in dictionary)
            {
                if (item.Key == id)
                {
                    name = item.Value;
                }
            }
            return name;
        }

        /// <summary>
        /// 获取唛头资料状态的字典
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, string> GetShippingMarkEnum()
        {
            Dictionary<int, string> di = EnumHelper.GetCustomEnums<int>(typeof(ShippingMarkStatusEnum));
            return di;
        }

        #endregion HelperMethod

        #region UserMethod

        /// <summary>
        /// 获取列表数据
        /// </summary>
        public List<DTOPurchaseContract> GetAll(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows,
            VMPurchaseSearch vm_search)
        {
            List<DTOPurchaseContract> listModel = null;
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Purchase_Contract.Where(d => !d.IsDelete && d.PurchaseStatus >= (short)PurchaseStatusEnum.PassedCheck && d.ContractType == (int)ContractTypeEnum.Default);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForShippingMark);

                    if (currentUser.HierachyType == HierachyType.Agency)
                    {
                        var userHierachy = new List<int?>()
                        {
                            currentUser.HierachyID,
                            context.SystemUsers.FirstOrDefault(p => p.HierarchyID == currentUser.HierachyID).Hierarchy.Hierarchy2.HierarchyID
                        };

                        query = query.Where(p => userHierachy.Contains(p.Factory.Hierarchy));
                    }

                    switch (vm_search.PageType)
                    {
                        case PageTypeEnum.PendingCheckList:
                            query = query.Where(d => d.ShippingMark_StatusID < (int)ShippingMarkStatusEnum.PassedCheck);
                            break;

                        case PageTypeEnum.PassedCheckList:
                            query = query.Where(d => d.ShippingMark_StatusID >= (int)ShippingMarkStatusEnum.PassedCheck);
                            break;

                        default:
                            break;
                    }

                    #region 筛选条件

                    if (!string.IsNullOrEmpty(vm_search.PurchaseNumber))
                    {
                        query = query.Where(d => d.PurchaseNumber.Contains(vm_search.PurchaseNumber));
                    }
                    if (!string.IsNullOrEmpty(vm_search.FactoryAbbreviation))
                    {
                        query = query.Where(d => d.Factory.Abbreviation.Contains(vm_search.FactoryAbbreviation));
                    }
                    if (!string.IsNullOrEmpty(vm_search.PurchaseDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.PurchaseDateStart);
                        query = query.Where(d => d.PurchaseDate >= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.PurchaseDateEnd))
                    {
                        DateTime dt = CommonCode.GetDateEnd(vm_search.PurchaseDateEnd);
                        query = query.Where(d => d.PurchaseDate <= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.DateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.DateStart);
                        query = query.Where(d => d.DateStart >= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.DateEnd))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.DateEnd);
                        query = query.Where(d => d.DateStart <= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.CustomerCode))
                    {
                        query = query.Where(d => d.Orders_Customers.CustomerCode == vm_search.CustomerCode);
                    }
                    if (!string.IsNullOrEmpty(vm_search.ShippingMark_StatusID))
                    {
                        int i = Utils.StrToInt(vm_search.ShippingMark_StatusID, (short)ShippingMarkStatusEnum.OutLine);
                        query = query.Where(d => d.ShippingMark_StatusID == i);
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
                        query = query.OrderByDescending(d => d.ShippingMark_ModifyDate);
                    }

                    #endregion 排序

                    totalRows = query.Count();//获取总条数

                    var dataFromDB = query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();//分页

                    #region 给Model赋值

                    if (dataFromDB != null && dataFromDB.Count > 0)
                    {
                        List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                        listModel = new List<DTOPurchaseContract>();
                        foreach (var entity in dataFromDB)
                        {
                            bool IsHasApprovalPermission = true;

                            #region 判断是否有审批流的权限

                            if (entity.ShippingMark_StatusID == (int)ShippingMarkStatusEnum.PendingCheck)
                            {
                                IsHasApprovalPermission = ApprovalService.HasApprovalPermission(WorkflowTypes.ApprovalShippingMark, currentUser, (int)entity.ShippingMark_CreateUser, entity.ApproverIndexShippingMark, entity.CustomerID);
                            }

                            #endregion 判断是否有审批流的权限

                            string OrderNumber = null;
                            var query_Order = context.Orders.Find(entity.OrderID);
                            if (query_Order != null)
                            {
                                OrderNumber = query_Order.OrderNumber;
                            }
                            listModel.Add(new DTOPurchaseContract()
                            {
                                ID = entity.ID,
                                PurchaseNumber = entity.PurchaseNumber,
                                OrderNumber = OrderNumber,
                                FactoryAbbreviation = entity.Factory.Abbreviation,
                                CustomerCode = entity.Orders_Customers.CustomerCode,
                                CustomerID = entity.Orders_Customers.OCID,
                                PurchaseDate = Utils.DateTimeToStr(entity.PurchaseDate),
                                PaymentType = entity.PaymentType,
                                AllAmount = entity.AllAmount,
                                Port = entity.Port,
                                DT_MODIFYDATE = Utils.DateTimeToStr2(entity.DT_MODIFYDATE),
                                DateStart = Utils.DateTimeToStr(entity.DateStart),
                                DateEnd = Utils.DateTimeToStr(entity.DateEnd),
                                CurrentSign = _dictionaryServices.GetDictionary_CurrencySign(entity.Purchase_ContractBatch.First().Purchase_ContractProduct.First().OrderProduct.CurrencyType, list_Com_DataDictionary),
                                IsHasApprovalPermission = IsHasApprovalPermission,
                                ShippingMark_StatusName = GetShippingMark(entity.ShippingMark_StatusID),
                                ShippingMark_StatusID = entity.ShippingMark_StatusID,
                                ShippingMark_CreateUser = entity.ShippingMark_CreateUser,
                                ShippingMark_ModifyDate = entity.ShippingMark_ModifyDate,
                                ShippingMark_ModifyDateFormatter = Utils.DateTimeToStr2(entity.ShippingMark_ModifyDate),
                                ShippingMark_CustomerID = entity.ShippingMark_CustomerID,
                                ShippingMark_Comment = entity.ShippingMark_Comment,
                                ApproverIndexShippingMark = entity.ApproverIndexShippingMark,
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
        /// <param name="id"></param>
        /// <returns></returns>
        public VMPurchase GetDetailByID(VMERPUser currentUser, int id, int? ShippingMark_AcceptInformationID = null)
        {
            VMPurchase vm = new VMPurchase();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Purchase_Contract.Where(p => p.ID == id && !p.IsDelete);

                    var dataFromDB = query.FirstOrDefault();
                    if (dataFromDB != null)
                    {
                        string AcceptInformation_CompanyName = "";
                        string AcceptInformation_StreetAddress = "";
                        string AcceptInformation_CustomerReg = "";
                        string AcceptInformation_Comment = "";

                        vm.ShippingMark_AcceptInformationID = dataFromDB.ShippingMark_AcceptInformationID;
                        if (ShippingMark_AcceptInformationID.HasValue)
                        {
                            vm.ShippingMark_AcceptInformationID = ShippingMark_AcceptInformationID.Value;

                        }
                        var query_Orders_AcceptInformation = context.Orders_AcceptInformation.Find(vm.ShippingMark_AcceptInformationID);
                        if (query_Orders_AcceptInformation != null)
                        {
                            AcceptInformation_CompanyName = query_Orders_AcceptInformation.CompanyName;
                            AcceptInformation_StreetAddress = query_Orders_AcceptInformation.StreetAddress;

                            AcceptInformation_CustomerReg = query_Orders_AcceptInformation.City + ","
                                + context.Reg_Area.Where(p => p.ARID == query_Orders_AcceptInformation.Province).FirstOrDefault().AreaName + ","
                                + query_Orders_AcceptInformation.PostalCode + ","
                                + context.Reg_Country.Where(p => p.COID == query_Orders_AcceptInformation.Country).FirstOrDefault().CountryName;

                            AcceptInformation_Comment = query_Orders_AcceptInformation.Comment;
                        }

                        var query_batch = dataFromDB.Purchase_ContractBatch;
                        List<VMPurchaseBatch> list_batch = new List<VMPurchaseBatch>();
                        List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                        List<DAL.SystemUser> list_SystemUser = context.SystemUsers.ToList();
                        string CurrentSign = "";
                        foreach (var item_batch in query_batch)
                        {
                            var query_product = item_batch.Purchase_ContractProduct.Where(d => !d.IsDelete && !d.ParentProductMixedID.HasValue);
                            List<VMPurchaseProduct> list_product = new List<VMPurchaseProduct>();
                            foreach (var item_product in query_product)
                            {
                                CurrentSign = _dictionaryServices.GetDictionary_CurrencySign(item_product.OrderProduct.CurrencyType, list_Com_DataDictionary);
                                decimal PriceFactory = item_product.OrderProduct.PriceFactory;
                                decimal ProductAmount = PriceFactory * item_product.OrderProduct.Qty;
                                string PackageName = item_product.PackageName;
                                if (string.IsNullOrEmpty(PackageName))//TODO 暂时加上
                                {
                                    PackageName = _dictionaryServices.GetDictionary_PackingMannerZhName(item_product.OrderProduct.PackingMannerZhID, list_Com_DataDictionary);
                                }
                                string CustomerCode = context.Orders_Customers.Find(item_product.OrderProduct.CustomerID) == null ? null : context.Orders_Customers.Find(item_product.OrderProduct.CustomerID).CustomerCode;

                                string IsFragile3 = "";
                                if (item_product.IsFragile.HasValue)
                                {
                                    IsFragile3 = "否";
                                    if (item_product.IsFragile.Value)
                                    {
                                        IsFragile3 = "是";
                                    }
                                }

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

                                var CarbinetType = (int)PurchaseProduct_CarbinetTypeEnum.StandardContainer;
                                if (item_product.CarbinetType.HasValue)
                                {
                                    CarbinetType = item_product.CarbinetType.Value;
                                }

                                string ProductUPC = "";
                                string InnerUPC = "";
                                string OuterUPC = "";

                                var query_PackProductsUPC = item_product.OrderProduct.Purchase_PackProductsUPC;
                                if (query_PackProductsUPC != null && query_PackProductsUPC.Count() > 0)
                                {
                                    ProductUPC = query_PackProductsUPC.First().ProductUPC;
                                    InnerUPC = query_PackProductsUPC.First().InnerUPC;
                                    OuterUPC = query_PackProductsUPC.First().OuterUPC;
                                }

                                var BoxQty = CalculateHelper.GetBoxQty(item_product.OrderProduct.Qty, item_product.OrderProduct.OuterBoxRate);

                                list_product.Add(new VMPurchaseProduct()
                                {
                                    ID = item_product.ID,
                                    ProductID = item_product.OrderProduct.ProductID,
                                    Image = item_product.OrderProduct.Image,
                                    PriceFactory = PriceFactory,
                                    PriceFactoryFormatter = CurrentSign + PriceFactory,
                                    CurrentSign = CurrentSign,
                                    Name = item_product.OrderProduct.Name,
                                    No = item_product.OrderProduct.No,
                                    Qty = item_product.OrderProduct.Qty,
                                    PackageName = PackageName,
                                    UnitName = _dictionaryServices.GetDictionary_UnitName(item_product.OrderProduct.UnitID, list_Com_DataDictionary),
                                    ProductAmount = ProductAmount,
                                    ProductAmountFormatter = CurrentSign + ProductAmount,
                                    InnerBoxRate = item_product.OrderProduct.InnerBoxRate,
                                    OuterBoxRate = item_product.OrderProduct.OuterBoxRate,
                                    PDQPackRate = item_product.OrderProduct.PDQPackRate,
                                    StyleName = _dictionaryServices.GetDictionary_StyleName(item_product.OrderProduct.StyleID, list_Com_DataDictionary),
                                    MixedMode = item_product.MixedMode,
                                    OtherComment = item_product.OtherComment,
                                    SkuNumber = item_product.OrderProduct.SkuNumber,
                                    CustomerID = item_product.OrderProduct.CustomerID,
                                    CustomerCode = CustomerCode,
                                    POID = item_product.OrderProduct.Order.POID,
                                    OuterWeightGross = item_product.OrderProduct.OuterWeightGross,
                                    OuterWeightNet = item_product.OrderProduct.OuterWeightNet,
                                    OuterVolume = item_product.OrderProduct.OuterVolume,
                                    Desc = item_product.OrderProduct.Desc,
                                    LengthIN = item_product.OrderProduct.LengthIN,
                                    WidthIN = item_product.OrderProduct.WidthIN,
                                    HeightIN = item_product.OrderProduct.HeightIN,

                                    InnerLengthIN = item_product.OrderProduct.InnerLengthIN,
                                    InnerWidthIN = item_product.OrderProduct.InnerWidthIN,
                                    InnerHeightIN = item_product.OrderProduct.InnerHeightIN,

                                    UPC = item_product.OrderProduct.UPC,
                                    WeightLBS = item_product.OrderProduct.WeightLBS,
                                    DestinationPortID = item_product.OrderProduct.Order.DestinationPortID,
                                    DestinationPortName = _dictionaryServices.GetDictionary_DestinationPortName(item_product.OrderProduct.Order.DestinationPortID, list_Com_DataDictionary),

                                    Season = item_product.OrderProduct.Season,
                                    SeasonName = SeasonName,
                                    SeasonPrefix = SeasonPrefix,
                                    SeasonSuffix = SeasonSuffix,
                                    SeasonDepartmentNumber = SeasonDepartmentNumber,

                                    IsFragile2 = item_product.IsFragile,
                                    IsFragile3 = IsFragile3,
                                    OuterLengthIN = item_product.OrderProduct.OuterLengthIN,
                                    OuterWidthIN = item_product.OrderProduct.OuterWidthIN,
                                    OuterHeightIN = item_product.OrderProduct.OuterHeightIN,
                                    OuterWeightGrossLBS = item_product.OrderProduct.OuterWeightGrossLBS,
                                    OuterWeightNetLBS = item_product.OrderProduct.OuterWeightNetLBS,
                                    DepartmentName = _dictionaryServices.GetDictionaryByAlias(item_product.OrderProduct.Department, list_Com_DataDictionary),
                                    CarbinetType = CarbinetType,
                                    ProductUPC = ProductUPC,
                                    InnerPKUPC = InnerUPC,
                                    OuterPKUPC = OuterUPC,
                                    CaseOrderQty = BoxQty,
                                    InnerGrossWeightLBS = item_product.OrderProduct.InnerWeightLBS,
                                    InnerNetWeightLBS = item_product.OrderProduct.InnerWeightGrossLBS,
                                    ColorName = item_product.OrderProduct.ColorName,

                                    OuterLength = item_product.OrderProduct.OuterLength,
                                    OuterWidth = item_product.OrderProduct.OuterWidth,
                                    OuterHeight = item_product.OrderProduct.OuterHeight,
                                    SkuCode = item_product.OrderProduct.SkuCode,
                                    UnitEngName = _dictionaryServices.GetDictionaryByAlias(item_product.OrderProduct.UnitID, list_Com_DataDictionary),
                                    AcceptInformation_StreetAddress = AcceptInformation_StreetAddress,
                                    AcceptInformation_CustomerReg = AcceptInformation_CustomerReg,
                                    AcceptInformation_CompanyName = AcceptInformation_CompanyName,
                                    AcceptInformation_Comment = AcceptInformation_Comment,

                                    PreTicketed = item_product.OrderProduct.PreTicketed,
                                    OfStoreReadyInnersEnclosed = item_product.OrderProduct.OfStoreReadyInnersEnclosed,
                                    OrderProductID = item_product.OrderProductID,

                                    IsProductMixed=item_product.IsProductMixed,
                                });
                            }

                            list_batch.Add(new VMPurchaseBatch()
                            {
                                DateStart = item_batch.DateStart,
                                DateEnd = item_batch.DateEnd,
                                Times = item_batch.Times,
                                BatchAmount = item_batch.BatchAmount,
                                listProduct = list_product,
                                DateStartForamt = Utils.DateTimeToStr(item_batch.DateStart),
                            });
                        }

                        #region 获取ShippingMark历史记录列表

                        var query_history = dataFromDB.Purchase_ShippingMarkHistory;
                        List<VMPurchaseHistory> list_history = new List<VMPurchaseHistory>();
                        foreach (var item_history in query_history)
                        {
                            list_history.Add(new VMPurchaseHistory()
                            {
                                ST_CREATEUSER = list_SystemUser.Find(d => d.UserID == item_history.ST_CREATEUSER).UserName,
                                DT_CREATEDATE = item_history.DT_CREATEDATE,
                                Comment = item_history.Comment,
                                CheckSuggest = item_history.CheckSuggest,
                            });
                        }

                        #endregion 获取ShippingMark历史记录列表

                        var query_order = context.Orders.Find(dataFromDB.OrderID);

                        var uploadPDF = context.UpLoadFiles.Where(d => d.LinkID == dataFromDB.ID && !d.IsDelete && d.ModuleType == (int)UploadFileType.ShippingMark_CreatePDF).FirstOrDefault();
                        string ShippingMark_PDF = null;
                        if (uploadPDF != null)
                        {
                            ShippingMark_PDF = uploadPDF.ServerFileName;

                            if (!Utils.FileExists(ShippingMark_PDF))
                            {
                                ShippingMark_PDF = null;
                            }
                        }

                        vm = new VMPurchase()
                        {
                            ID = dataFromDB.ID,
                            OrderNumber = query_order == null ? null : query_order.OrderNumber,
                            CallPeople = dataFromDB.Factory.CallPeople,
                            FactoryAbbreviation = dataFromDB.Factory.Abbreviation,
                            FactoryName = dataFromDB.Factory.Name,
                            FactoryID = dataFromDB.FactoryID,
                            FactoryNo = dataFromDB.Factory.No,
                            Fax = dataFromDB.Factory.Fax,
                            FactoryEmail = dataFromDB.Factory.EmailAdress,
                            Telephone = dataFromDB.Factory.Telephone,
                            OtherFee = dataFromDB.OtherFee ?? 0,
                            AllAmount = dataFromDB.AllAmount,
                            PurchaseNumber = dataFromDB.PurchaseNumber,
                            PurchaseDate = Utils.DateTimeToStr(dataFromDB.PurchaseDate),
                            PortName = _dictionaryServices.GetDictionary_PortName(dataFromDB.PortID, list_Com_DataDictionary),
                            Port = dataFromDB.Port == null ? null : dataFromDB.Port.Trim(),
                            PaymentType = dataFromDB.PaymentType == null ? null : dataFromDB.PaymentType.Trim(),
                            CustomerID = dataFromDB.CustomerID,
                            CustomerCode = dataFromDB.Orders_Customers.CustomerCode,
                            IsImmediatelySend = dataFromDB.IsImmediatelySend,
                            IsThirdVerification = dataFromDB.IsThirdVerification,
                            IsThirdAudits = dataFromDB.IsThirdAudits,
                            IsThirdTest = dataFromDB.IsThirdTest,
                            Comment = dataFromDB.Comment,
                            ContractTerms = dataFromDB.ContractTerms,
                            list_batch = list_batch,
                            list_history = list_history,
                            list_UpLoadFile = ConstsMethod.GetUploadFileList(id, UploadFileType.ShippingMark),
                            CurrentSign = CurrentSign,
                            AllQty = dataFromDB.AllQty ?? 0,
                            ST_CREATEUSER = dataFromDB.ST_CREATEUSER,
                            ApproverIndexPurchaseContract = dataFromDB.ApproverIndexPurchaseContract,
                            DateStartFormatter = Utils.DateTimeToStr(dataFromDB.DateStart),
                            DateEndFormatter = Utils.DateTimeToStr(dataFromDB.DateEnd),
                            AfterDate = dataFromDB.AfterDate,
                            POID = query_order == null ? null : query_order.POID,
                            DestinationPortID = query_order == null ? null : query_order.DestinationPortID,
                            DestinationPortName = query_order == null ? null : _dictionaryServices.GetDictionary_DestinationPortName(query_order.DestinationPortID, list_Com_DataDictionary),
                            ShippingMark_StatusID = dataFromDB.ShippingMark_StatusID,
                            ShippingMark_CreateUser = dataFromDB.ShippingMark_CreateUser,
                            ShippingMark_ModifyDate = dataFromDB.ShippingMark_ModifyDate,
                            ShippingMark_ModifyDateFormatter = Utils.DateTimeToStr2(dataFromDB.ShippingMark_ModifyDate),
                            ShippingMark_CustomerID = dataFromDB.ShippingMark_CustomerID,
                            ShippingMark_Comment = dataFromDB.ShippingMark_Comment,
                            ApproverIndexShippingMark = dataFromDB.ApproverIndexShippingMark,
                            ShippingMark_PDF = ShippingMark_PDF,
                            ShippingMark_UpLoadFileList = ConstsMethod.GetUploadFileList(dataFromDB.ID, UploadFileType.ShippingMark_For188),
                            SelectCustomer = dataFromDB.Orders_Customers.SelectCustomer,
                            ShippingMark_AcceptInformationID = dataFromDB.ShippingMark_AcceptInformationID,
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
        /// 保存
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public VMAjaxProcessResult Save(VMERPUser currentUser, VMPurchase vm)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Purchase_Contract.Where(p => p.ID == vm.ID);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForShippingMark);

                    var dataFromDB = query.FirstOrDefault();
                    if (dataFromDB != null)
                    {
                        if (dataFromDB.ShippingMark_StatusID == (int)ShippingMarkStatusEnum.PendingMaintenance)
                        {
                            dataFromDB.ShippingMark_CreateUser = currentUser.UserID;
                        }
                        dataFromDB.ShippingMark_CustomerID = vm.ShippingMark_CustomerID;
                        dataFromDB.ShippingMark_ModifyDate = DateTime.Now;

                        if (vm.ShippingMark_StatusID != (int)ShippingMarkStatusEnum.NotPassCheck && vm.ShippingMark_StatusID != (int)ShippingMarkStatusEnum.PassedCheck)
                        {
                            dataFromDB.ShippingMark_StatusID = vm.ShippingMark_StatusID;
                        }

                        string ShippingMark_Comment = "";
                        if (vm.ShippingMark_StatusID == (int)ShippingMarkStatusEnum.PassedCheck || vm.ShippingMark_StatusID == (int)ShippingMarkStatusEnum.NotPassCheck)
                        {
                            ShippingMark_Comment = vm.ShippingMark_Comment;
                        }
                        dataFromDB.ShippingMark_Comment = vm.ShippingMark_Comment;
                        dataFromDB.ShippingMark_AcceptInformationID = vm.ShippingMark_AcceptInformationID;

                        context.Purchase_ShippingMarkHistory.Add(new Purchase_ShippingMarkHistory()
                        {
                            DT_CREATEDATE = DateTime.Now,
                            ST_CREATEUSER = currentUser.UserID,
                            IPAddress = CommonCode.GetIP(),
                            PurchaseContractID = vm.ID,
                            Comment = GetShippingMark(vm.ShippingMark_StatusID),
                            CheckSuggest = vm.ShippingMark_Comment,
                        });

                        ConstsMethod.SaveFileUpload(currentUser, vm.ID, vm.ShippingMark_UpLoadFileList, context, UploadFileType.ShippingMark_For188);

                        foreach (var item in vm.list_batch)
                        {
                            foreach (var item2 in item.listProduct)
                            {
                                var query_Purchase_ContractProduct = context.Purchase_ContractProduct.Find(item2.ID);
                                if (query_Purchase_ContractProduct != null)
                                {
                                    query_Purchase_ContractProduct.CarbinetType = item2.CarbinetType;
                                }
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

                            ExecuteApproval((int)dataFromDB.ShippingMark_CreateUser, dataFromDB.ID, "", vm.ShippingMark_StatusID, currentUser.UserID, false);//执行审批流
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
            if (StatusID == (int)ShippingMarkStatusEnum.NotPassCheck)
            {
                isPass = false;
            }

            List<int> validWaitingApproveStatus = new List<int>(){
                            (int)ShippingMarkStatusEnum.PendingCheck,
                            (int)ShippingMarkStatusEnum.NotPassCheck
                        };
            ApprovalService.ExcuteApproval(new ApprovalInfo
            {
                WorkflowType = WorkflowTypes.ApprovalShippingMark,
                IsPass = isPass,
                IdentityID = identityID,
                ValidWaitingApproveStatus = validWaitingApproveStatus,
                StatusApproving = (int)ShippingMarkStatusEnum.PendingCheck,
                StatusNextTo = (int)ShippingMarkStatusEnum.PassedCheck,
                StatusRejected = (int)ShippingMarkStatusEnum.NotPassCheck,
                ApproveOpinion = CheckSuggest,
                ApproveUserID = UserID,
                LogMethod = () =>
                {
                    return null;
                }
            });
        }

        #endregion UserMethod

        /// <summary>
        /// 保存 修改状态
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public DBOperationStatus Save_ChangeStatus(VMERPUser currentUser, int id)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Purchase_Contract.Find(id);
                    if (query != null)
                    {
                        query.ShippingMark_StatusID += 1;
                        query.ShippingMark_ModifyDate = DateTime.Now;

                        context.Purchase_ShippingMarkHistory.Add(new Purchase_ShippingMarkHistory()
                        {
                            DT_CREATEDATE = DateTime.Now,
                            ST_CREATEUSER = currentUser.UserID,
                            IPAddress = CommonCode.GetIP(),
                            PurchaseContractID = id,
                            Comment = GetShippingMark(query.ShippingMark_StatusID),
                        });

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
        public VMUpLoad GetUploadDetial(int id)
        {
            VMUpLoad vm = null;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Purchase_Contract.Find(id);
                    vm = new VMUpLoad()
                    {
                        ID = query.ID,
                        No_Description = "采购合同编号",
                        No = query.PurchaseNumber,
                        Field1_Description = "工厂",
                        Field1 = query.Factory.Abbreviation,
                        Field2_Description = "客户",
                        Field2 = query.Orders_Customers.CustomerCode,
                        FileName_Description = "附件",
                        UpLoadFileList = ConstsMethod.GetUploadFileList(id, UploadFileType.ShippingMark),
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
        public DBOperationStatus SaveUpLoad(VMERPUser currentUser, VMUpLoad vm)
        {
            DBOperationStatus result = default(DBOperationStatus);
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Purchase_Contract.Where(p => p.ID == vm.ID);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForShippingMark);
                    var dataFromDB = query.FirstOrDefault();
                    if (dataFromDB != null)
                    {
                        dataFromDB.ShippingMark_StatusID = (int)ShippingMarkStatusEnum.UpLoaded;
                        dataFromDB.ST_MODIFYUSER = currentUser.UserID;
                        dataFromDB.DT_MODIFYDATE = DateTime.Now;

                        context.Purchase_ShippingMarkHistory.Add(new Purchase_ShippingMarkHistory()
                        {
                            DT_CREATEDATE = DateTime.Now,
                            ST_CREATEUSER = currentUser.UserID,
                            IPAddress = CommonCode.GetIP(),
                            PurchaseContractID = vm.ID,
                            Comment = GetShippingMark((int)ShippingMarkStatusEnum.UpLoaded),
                        });

                        ConstsMethod.SaveFileUpload(currentUser, vm.ID, vm.UpLoadFileList, context, UploadFileType.ShippingMark);

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

        public VMAjaxProcessResult CreateShippingMark(VMERPUser currentUser, VMPurchase vm2)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    if (!vm2.ShippingMark_AcceptInformationID.HasValue)
                    {
                        vm2.ShippingMark_AcceptInformationID = -1;
                    }
                    var vm = GetDetailByID(currentUser, vm2.ID, vm2.ShippingMark_AcceptInformationID);
                    ShippingMarkEnum shippingMarkEnum = (ShippingMarkEnum)EnumHelper.GetCustomEnumByDesc(typeof(ShippingMarkEnum), vm2.ShippingMark_CustomerID.ToString());
                    List<string> filesAsolutePathList = new List<string>();
                    List<string> filesPhysicalPathList = new List<string>();
                    List<string> list_ShippingMark = new List<string>();

                    var list_ImageList_ServerFileName = CommonCode.StrToList(vm2.ImageList_ServerFileName, ';');
                    var list_ImageList_DisplayFileName = CommonCode.StrToList(vm2.ImageList_DisplayFileName, ';');

                    foreach (var item_batch in vm.list_batch)
                    {
                        foreach (var item_product in item_batch.listProduct)
                        {
                            if (list_ImageList_ServerFileName != null && list_ImageList_ServerFileName.Count > 0)
                            {
                                foreach (var item in list_ImageList_DisplayFileName)
                                {
                                    if (Utils.GetFilePrefix(item) == item_product.No)
                                    {
                                        int index = list_ImageList_DisplayFileName.IndexOf(item);
                                        item_product.CartonBarcodeLabel_Image = list_ImageList_ServerFileName[index];
                                    }

                                }
                            }

                            item_product.CarbinetType = vm2.list_batch[0].listProduct.Where(d => d.ID == item_product.ID).First().CarbinetType;


                            result = MakerExcel_ShippingMark.CreateShippingMark(shippingMarkEnum, item_product);

                            List<string> list_ImageList = new List<string>();
                            List<VMShippingMark> list_ImageList2 = new List<VMShippingMark>();
                            if (result.IsSuccess)
                            {
                                list_ImageList2 = result.Data as List<VMShippingMark>;
                                foreach (var item in list_ImageList2)
                                {
                                    list_ImageList.Add(item.ImagePath);
                                    filesAsolutePathList.Add(Utils.GetMapPath(item.ImagePath));
                                    filesPhysicalPathList.Add(ConstsMethod.ReplaceURLToLocalPath(Utils.GetMapPath(item.ImagePath)));
                                    list_ShippingMark.Add(item.Description);
                                }
                            }
                            else
                            {
                                return result;
                            }

                            #region 保存生成的唛头资料——JPG

                            var vm_upload = context.UpLoadFiles.Where(d => d.LinkID == item_product.ID && !d.IsDelete && d.ModuleType == (int)UploadFileType.ShippingMark_CreateJPG);
                            if (vm_upload != null)
                            {
                                foreach (var item_upload in vm_upload)
                                {
                                    item_upload.IsDelete = true;
                                }
                            }

                            List<VMUpLoadFile> UpLoadFileList = new List<VMUpLoadFile>();
                            UpLoadFileList.Add(new VMUpLoadFile()
                            {
                                DT_CREATEDATE = DateTime.Now,
                                DisplayFileName = item_product.No,
                                ServerFileName = CommonCode.ListToString(list_ImageList),
                                ST_CREATEUSER = currentUser.UserID,
                                IsDelete = false,
                            });

                            ConstsMethod.SaveFileUpload(currentUser, item_product.ID, UpLoadFileList, context, UploadFileType.ShippingMark_CreateJPG);

                            #endregion 保存生成的唛头资料——JPG
                        }
                    }

                    string pdfFileName = CommonCode.CreatePdfList(filesAsolutePathList, filesPhysicalPathList, list_ShippingMark);

                    #region 保存生成的唛头资料——PDF

                    var vm_upload2 = context.UpLoadFiles.Where(d => d.LinkID == vm.ID && !d.IsDelete && d.ModuleType == (int)UploadFileType.ShippingMark_CreatePDF);
                    if (vm_upload2 != null)
                    {
                        foreach (var item_upload in vm_upload2)
                        {
                            item_upload.IsDelete = true;
                        }
                    }

                    List<VMUpLoadFile> UpLoadFileList2 = new List<VMUpLoadFile>();
                    UpLoadFileList2.Add(new VMUpLoadFile()
                    {
                        DT_CREATEDATE = DateTime.Now,
                        DisplayFileName = vm.PurchaseNumber,
                        ServerFileName = pdfFileName,
                        ST_CREATEUSER = currentUser.UserID,
                        IsDelete = false,
                    });
                    ConstsMethod.SaveFileUpload(currentUser, vm.ID, UpLoadFileList2, context, UploadFileType.ShippingMark_CreatePDF);

                    #endregion 保存生成的唛头资料——PDF

                    context.SaveChanges();

                    result.IsSuccess = true;
                    result.Msg = pdfFileName;
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