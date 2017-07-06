using ERP.BLL.Consts;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.Encasement;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP.BLL.ERP.Encasement
{
    /// <summary>
    /// 出运明细
    /// </summary>
    public class EncasementServices
    {
        private Dictionary.DictionaryServices _DictionaryServices = new Dictionary.DictionaryServices();

        /// <summary>
        /// 用状态编号换取状态译文(出运明细)
        /// </summary>
        /// <param name="i">状态编号</param>
        /// <returns></returns>
        public static string GetEncasementStatus(int id)
        {
            Dictionary<int, string> dictionary = GetEncasementStatusList();
            string name = "";
            foreach (var item in dictionary)
            {
                if (item.Key == id)
                {
                    name = item.Value;
                    break;
                }
            }
            return name;
        }

        /// <summary>
        /// 获取出运明细列表数据状态枚举成员
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, string> GetEncasementStatusList()
        {
            Dictionary<int, string> di = EnumHelper.GetCustomEnums<int>(typeof(EncasementStatusEnum));
            return di;
        }

        public List<DTOEncasement> GetAll(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows, VMFilterEncasement vm_search)
        {
            List<DTOEncasement> vm_list = null;
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Purchase_Contract.Where(d => !d.IsDelete && d.PurchaseStatus >= (int)PurchaseStatusEnum.PassedCheck && d.ContractType == (int)ContractTypeEnum.Default);//已审核的采购合同

                    if (vm_search.PageType == PageTypeEnum.PendingCheckList)
                    {
                        query = query.Where(d => d.EncasementStatus != (int)EncasementStatusEnum.PassedCheck);
                    }
                    else
                    {
                        query = query.Where(d => d.EncasementStatus == (int)EncasementStatusEnum.PassedCheck);
                    }

                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForDelivery);

                    #region 筛选条件

                    if (!string.IsNullOrEmpty(vm_search.PurchaseNumber))
                    {
                        query = query.Where(p => p.PurchaseNumber.Contains(vm_search.PurchaseNumber));
                    }
                    if (!string.IsNullOrEmpty(vm_search.FactoryAbbreviation))
                    {
                        query = query.Where(p => p.Factory.Abbreviation.Contains(vm_search.FactoryAbbreviation));
                    }
                    if (!string.IsNullOrEmpty(vm_search.CustomerCode))
                    {
                        query = query.Where(p => p.Orders_Customers.CustomerCode == vm_search.CustomerCode);
                    }

                    if (!string.IsNullOrEmpty(vm_search.PurchaseDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.PurchaseDateStart);
                        query = query.Where(p => p.PurchaseDate >= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.PurchaseDateEnd))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.PurchaseDateEnd);
                        query = query.Where(p => p.PurchaseDate <= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.DateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.DateStart);
                        query = query.Where(p => p.DateStart >= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.DateEnd))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.DateEnd);
                        query = query.Where(p => p.DateEnd <= dt);
                    }
                    if (vm_search.EncasementStatus.HasValue)
                    {
                        query = query.Where(p => p.EncasementStatus == vm_search.EncasementStatus);
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
                        query = query.OrderByDescending(p => p.ID);
                    }

                    #endregion 排序

                    totalRows = query.Count();
                    var dataFromDB = query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

                    if (dataFromDB != null)
                    {
                        vm_list = new List<DTOEncasement>();
                        List<DAL.Com_DataDictionary> dtComDictionary = context.Com_DataDictionary.Where(p => p.IsDelete == 0).ToList();
                        List<DAL.HarmonizedSystem> dbHarmonizedSystem = context.HarmonizedSystems.Where(p => !p.IsDelete).ToList();

                        foreach (var item1 in dataFromDB)
                        {
                            DTOEncasement vm = new DTOEncasement();
                            vm.ContractID = item1.ID;
                            vm.PurchaseNumber = item1.PurchaseNumber;
                            vm.FactoryAbbreviation = item1.Factory.Abbreviation;
                            vm.CustomerCode = item1.Orders_Customers.CustomerCode;
                            vm.PurchaseDate = Utils.DateTimeToStr(item1.PurchaseDate);
                            vm.DateStart = Utils.DateTimeToStr(item1.DateStart);
                            vm.DateEnd = Utils.DateTimeToStr(item1.DateEnd);
                            vm.PortName = item1.Port;
                            vm.EncasementUpdateDateFormatter = Utils.DateTimeToStr2(item1.EncasementUpdateDate);

                            var dbOrderProduct = item1.Purchase_ContractBatch.First().Purchase_ContractProduct.Where(d => !d.IsDelete && !d.ParentProductMixedID.HasValue).First().OrderProduct;
                            int? iCurrencyType = dbOrderProduct.CurrencyType;
                            var dictionaryInfo = dtComDictionary.Where(p => p.Code == iCurrencyType).FirstOrDefault();
                            string CurrentSign = string.Empty;
                            if (dictionaryInfo != null)
                            {
                                CurrentSign = dictionaryInfo.Alias;//取得采购合同对应产品的货币符号
                            }
                            vm.ContractAmountSymbol = CurrentSign + item1.AllAmount.ToString();

                            var dbEncasementInfo = item1.Delivery_Encasements.FirstOrDefault();
                            string sCustomerPO = string.Empty, sCustomerSWStart = string.Empty, sCustomerSWEnd = string.Empty;
                            int iSumProductBoxNum = 0;
                            decimal dActualCUFT = 0;

                            DTOEncasementProducts vm_product = new DTOEncasementProducts();

                            if (dbEncasementInfo == null)
                            {
                                vm.EncasementStatusID = (int)EncasementStatusEnum.PendingEdit;

                                foreach (var item2 in item1.Purchase_ContractBatch)
                                {
                                    foreach (var item3 in item2.Purchase_ContractProduct.Where(d => !d.IsDelete && !d.ParentProductMixedID.HasValue))
                                    {
                                        vm_product = new DTOEncasementProducts();
                                        vm_product.Qty = item3.OrderProduct.Qty;
                                        vm_product.OuterBoxRate = item3.OrderProduct.OuterBoxRate ?? 0;
                                        vm_product.OuterLength = item3.OrderProduct.OuterLength ?? 0;
                                        vm_product.OuterWidth = item3.OrderProduct.OuterWidth ?? 0;
                                        vm_product.OuterHeight = item3.OrderProduct.OuterHeight ?? 0;

                                        iSumProductBoxNum += vm_product.BoxQty;
                                        dActualCUFT += vm_product.ActualCUFT;

                                        if (string.IsNullOrEmpty(sCustomerPO))
                                        {
                                            sCustomerPO = item3.OrderProduct.Order.POID;
                                            sCustomerSWStart = Utils.DateTimeToStr(item3.OrderProduct.Order.OrderDateStart);
                                            sCustomerSWEnd = Utils.DateTimeToStr(item3.OrderProduct.Order.OrderDateEnd);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                vm.EncasementID = dbEncasementInfo.EncasementsID;
                                vm.CreateUserID = dbEncasementInfo.CreateUserID;
                                vm.ApproverIndex = dbEncasementInfo.ApproverIndex;
                                vm.EncasementStatusID = dbEncasementInfo.EncasementsStatus;

                                var dbEncasementProduct = dbEncasementInfo.Delivery_EncasementsProducts;

                                foreach (var item2 in dbEncasementProduct)
                                {
                                    vm_product = new DTOEncasementProducts();
                                    vm_product.OuterBoxRate = item2.OrderProduct.OuterBoxRate ?? 0;
                                    vm_product.OuterLength = item2.OuterLength ?? 0;
                                    vm_product.OuterWidth = item2.OuterWidth ?? 0;
                                    vm_product.OuterHeight = item2.OuterHeight ?? 0;
                                    vm_product.Qty = item2.OrderProduct.Qty;

                                    iSumProductBoxNum += vm_product.BoxQty;
                                    dActualCUFT += vm_product.ActualCUFT;

                                    if (string.IsNullOrEmpty(sCustomerPO))
                                    {
                                        sCustomerPO = item2.OrderProduct.Order.POID;
                                        sCustomerSWStart = Utils.DateTimeToStr(item2.OrderProduct.Order.OrderDateStart);
                                        sCustomerSWEnd = Utils.DateTimeToStr(item2.OrderProduct.Order.OrderDateEnd);
                                    }
                                }
                            }

                            vm.CustomerPO = sCustomerPO;
                            vm.CustomerSWStart = sCustomerSWStart;
                            vm.CustomerSWEnd = sCustomerSWEnd;
                            vm.SumProductBoxNum = iSumProductBoxNum;
                            vm.ActualCUFT = dActualCUFT;
                            vm.EncasementStatus = GetEncasementStatus(vm.EncasementStatusID);

                            //当加载待审核页面数据列表时，获取审批流权限值，以便前台判断用户是否具有审批数据行的按钮权限
                            if (vm_search.PageType == PageTypeEnum.PendingCheckList)
                            {
                                vm.IsCanAudit = Workflow.ApprovalService.HasApprovalPermission(WorkflowTypes.ApprovalShipping, currentUser, vm.CreateUserID, vm.ApproverIndex, item1.CustomerID);

                                if (vm.EncasementStatusID == (int)EncasementStatusEnum.PendingCheck)
                                {
                                    vm.NextApproverInfos = Workflow.ApprovalService.GetNextApproverInfos(WorkflowTypes.ApprovalShipping, vm.CreateUserID, vm.ApproverIndex, item1.CustomerID);
                                }
                            }

                            vm_list.Add(vm);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return vm_list;
        }

        public DTOEncasement GetDetailByID(VMERPUser currentUser, DTOEncasement vm)
        {
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Purchase_Contract.Where(d => d.ID == vm.ContractID && d.ContractType == (int)ContractTypeEnum.Default).FirstOrDefault();

                    vm.ContractID = query.ID;
                    vm.PurchaseNumber = query.PurchaseNumber;
                    vm.FactoryAbbreviation = query.Factory.Abbreviation;
                    vm.CustomerCode = query.Orders_Customers.CustomerCode;
                    vm.AllAmount = query.AllAmount;
                    vm.PurchaseDate = Utils.DateTimeToStr(query.PurchaseDate);
                    vm.DateStart = Utils.DateTimeToStr(query.DateStart);
                    vm.DateEnd = Utils.DateTimeToStr(query.DateEnd);
                    vm.PortName = query.Port;
                    vm.SelectCustomer = query.Orders_Customers.SelectCustomer;

                    List<DTOAuditHis> list_history = new List<DTOAuditHis>();

                    if (vm.EncasementID == 0)
                    {
                        vm.EncasementStatusID = (int)EncasementStatusEnum.PendingEdit;
                    }
                    else
                    {
                        var dbEncasementInfo = query.Delivery_Encasements.FirstOrDefault();

                        vm.EncasementStatusID = dbEncasementInfo.EncasementsStatus;

                        var dtEncasementHisHis = dbEncasementInfo.Delivery_EncasementsHis;
                        DTOAuditHis vmAuditPacksHis = null;

                        foreach (var item1 in dtEncasementHisHis)
                        {
                            vmAuditPacksHis = new DTOAuditHis();
                            vmAuditPacksHis.AuditUserName = item1.SystemUser.UserName;
                            vmAuditPacksHis.AuditIdea = item1.AuditIdea;
                            vmAuditPacksHis.EncasementStatus = GetEncasementStatus(item1.EncasementsStatus);

                            if (item1.CreateDate.HasValue)
                            {
                                vmAuditPacksHis.AuditCreateDate = Utils.DateTimeToStr2((DateTime)item1.CreateDate);
                            }
                            list_history.Add(vmAuditPacksHis);
                        }
                    }

                    vm.AuditHisList = list_history;

                    string customerPO = string.Empty, EHIPC = string.Empty;
                    List<DTOEncasementProducts> productList = new List<DTOEncasementProducts>();
                    var dtEncasementInfo = query.Delivery_Encasements.FirstOrDefault();
                    productList = GetEncasementProducts(query, productList, out customerPO, out EHIPC, 0);

                    vm.CustomerPO = customerPO;
                    vm.EHIPC = EHIPC;
                    vm.EncasementProducts = productList;

                    productList = new List<DTOEncasementProducts>();
                    vm.listProducts_Mixed = GetEncasementProducts(query, productList, out customerPO, out EHIPC, 2);

                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return vm;
        }

        /// <summary>
        /// 根据采购合同编号查询产品出运明细数据列表
        /// </summary>
        /// <param name="pc"></param>
        /// <param name="productList"></param>
        /// <returns></returns>
        public List<DTOEncasementProducts> GetEncasementProducts(Purchase_Contract pc, List<DTOEncasementProducts> productList, out string customerPO, out string EHIPC, int ProductMixed_Type = 0)
        {
            customerPO = string.Empty;
            EHIPC = string.Empty;
            try
            {

                DTOEncasementProducts vm = null;

                foreach (var item_batch in pc.Purchase_ContractBatch)
                {
                    var query_product = item_batch.Purchase_ContractProduct.Where(d => !d.IsDelete);
                    if (ProductMixed_Type == 0)
                    {
                        //普通的
                        query_product = query_product.Where(d => !d.ParentProductMixedID.HasValue);
                    }
                    else if (ProductMixed_Type == 1)
                    {
                        //如果是混装产品，显示混装产品的详情里面的。
                        query_product = query_product.Where(d => !d.IsProductMixed || (d.IsProductMixed && d.ParentProductMixedID.HasValue));
                    }
                    else if (ProductMixed_Type == 2)
                    {
                        //只显示混装产品的详情里面的。
                        query_product = query_product.Where(d => d.IsProductMixed && d.ParentProductMixedID.HasValue);
                    }

                    foreach (var item2 in query_product)
                    {
                        var OrderProduct = item2.OrderProduct;

                        vm = new DTOEncasementProducts();

                        vm.OrderProductID = item2.OrderProductID;
                        vm.ProductID = OrderProduct.ProductID;
                        vm.ProductImage = OrderProduct.Image;
                        vm.No = OrderProduct.No;
                        vm.SkuNumber = OrderProduct.SkuNumber;
                        vm.Desc = OrderProduct.Desc;
                        vm.FactoryName = OrderProduct.Factory.Abbreviation;
                        vm.InnerBoxRate = OrderProduct.InnerBoxRate ?? 0;
                        vm.OuterVolume = OrderProduct.OuterVolume ?? 0;
                        vm.OuterBoxRate = OrderProduct.OuterBoxRate ?? 0;
                        vm.Qty = OrderProduct.Qty;

                        vm.BeforeProductOuterLength = OrderProduct.OuterLength ?? 0;
                        vm.BeforeProductOuterWidth = OrderProduct.OuterWidth ?? 0;
                        vm.BeforeProductOuterHeight = OrderProduct.OuterHeight ?? 0;
                        vm.OuterLength = OrderProduct.OuterLength ?? 0;
                        vm.OuterWidth = OrderProduct.OuterWidth ?? 0;
                        vm.OuterHeight = OrderProduct.OuterHeight ?? 0;
                        vm.OuterWeightGross = OrderProduct.OuterWeightGross ?? 0;
                        vm.OuterWeightNet = OrderProduct.OuterWeightNet ?? 0;
                        vm.CUFT = vm.ActualCUFT;
                        vm.IsProductMixed = OrderProduct.IsProductMixed;
                        vm.ParentProductMixedID = OrderProduct.ParentProductMixedID;
                        vm.Qty2 = OrderProduct.Qty2 ?? 0;

                        var dbEncasementProductInfo = OrderProduct.Delivery_EncasementsProducts.FirstOrDefault();
                        if (dbEncasementProductInfo != null)
                        {
                            vm.EncasementID = dbEncasementProductInfo.EncasementsID;
                            vm.EncasementProductID = dbEncasementProductInfo.EncasementsProductID;

                            vm.OuterLength = dbEncasementProductInfo.OuterLength ?? 0;
                            vm.OuterWidth = dbEncasementProductInfo.OuterWidth ?? 0;
                            vm.OuterHeight = dbEncasementProductInfo.OuterHeight ?? 0;
                            vm.OuterWeightGross = dbEncasementProductInfo.WeightGross ?? 0;
                            vm.OuterWeightNet = dbEncasementProductInfo.WeightNet ?? 0;
                        }


                        customerPO = OrderProduct.Order.POID;
                        EHIPC = OrderProduct.Order.EHIPO;

                        productList.Add(vm);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return productList;
        }

        /// <summary>
        /// 根据表主键自编号查询数据
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="vmInfo"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public DTOEncasement GetEncasementInfo(VMERPUser currentUser, DTOEncasement vmInfo, int id)
        {
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Delivery_Encasements.Where(p => p.ContractsID == id);
                    var dbinfo = query.FirstOrDefault();

                    if (dbinfo != null)
                    {
                        vmInfo.EncasementID = dbinfo.EncasementsID;
                        vmInfo.ApproverIndex = dbinfo.ApproverIndex;
                        vmInfo.CreateUserID = dbinfo.CreateUserID;
                    }
                }
            }
            catch (Exception e) { LogHelper.WriteError(e); }

            return vmInfo;
        }

        /// <summary>
        /// 待审核代印合同修改后保存至DB
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="ocStatus"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public VMAjaxProcessResult Save(VMERPUser currentUser, DTOEncasement vm)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();

            try
            {
                int affectRows = 0;
                int iEncasementHisID = 0;

                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    string sAuditIdea = string.Empty;

                    var query_Purchase_Contract = context.Purchase_Contract.Find(vm.ContractID);
                    if (query_Purchase_Contract == null)
                    {
                        result.IsSuccess = false;
                        return result;
                    }

                    Delivery_EncasementsHis query_history = new Delivery_EncasementsHis();
                    query_history.CreateDate = DateTime.Now;
                    query_history.AuditUserID = currentUser.UserID;
                    query_history.EncasementsStatus = vm.EncasementStatusID;

                    if (!string.IsNullOrEmpty(vm.AuditIdea))
                    {
                        query_history.AuditIdea = vm.AuditIdea;
                    }

                    var query_Delivery_Encasements = query_Purchase_Contract.Delivery_Encasements.FirstOrDefault();

                    //审核时，不需要插入或更新采购合同、出运明细表数据
                    if (vm.EncasementStatusID == (int)EncasementStatusEnum.PendingEdit || vm.EncasementStatusID == (int)EncasementStatusEnum.Draft || vm.EncasementStatusID == (int)EncasementStatusEnum.PendingCheck)
                    {
                        //首次编辑采购合同出运明细时，需要把对应产品的主键值插入
                        if (query_Delivery_Encasements == null)
                        {
                            Delivery_Encasements query = new Delivery_Encasements();
                            query.UpdateDate = DateTime.Now;
                            query.EncasementsStatus = vm.EncasementStatusID;
                            query.ContractsID = vm.ContractID;
                            query.CreateDate = DateTime.Now;
                            query.CreateUserID = currentUser.UserID;
                            query.CreateTerminal = CommonCode.GetIP();

                            Delivery_EncasementsProducts query_product = new Delivery_EncasementsProducts();

                            foreach (var item in vm.EncasementProducts)
                            {
                                query_product = new Delivery_EncasementsProducts();

                                query_product.OrderProductID = item.OrderProductID;
                                query_product.OuterLength = item.OuterLength;
                                query_product.OuterWidth = item.OuterWidth;
                                query_product.OuterHeight = item.OuterHeight;
                                query_product.WeightGross = item.OuterWeightGross;
                                query_product.WeightNet = item.OuterWeightNet;

                                query_product.IsProductMixed = item.IsProductMixed;

                                query.Delivery_EncasementsProducts.Add(query_product);
                            }
                            query.Delivery_EncasementsHis.Add(query_history);
                            context.Delivery_Encasements.Add(query);
                        }
                        else
                        {
                            if (query_Delivery_Encasements.EncasementsStatus == (int)EncasementStatusEnum.PendingEdit)
                            {
                                query_Delivery_Encasements.CreateDate = DateTime.Now;
                                query_Delivery_Encasements.CreateUserID = currentUser.UserID;
                                query_Delivery_Encasements.CreateTerminal = CommonCode.GetIP();
                            }

                            query_Delivery_Encasements.EncasementsStatus = vm.EncasementStatusID;
                            query_Delivery_Encasements.UpdateUserID = currentUser.UserID;
                            query_Delivery_Encasements.UpdateTerminal = CommonCode.GetIP();
                            query_Delivery_Encasements.UpdateDate = DateTime.Now;

                            foreach (var item1 in vm.EncasementProducts)
                            {
                                var query_product = query_Delivery_Encasements.Delivery_EncasementsProducts.Where(p => p.EncasementsProductID == item1.EncasementProductID).FirstOrDefault();
                                if (query_product != null)
                                {
                                    query_product.IsProductMixed = item1.IsProductMixed;

                                    query_product.OuterLength = item1.OuterLength;
                                    query_product.OuterWidth = item1.OuterWidth;
                                    query_product.OuterHeight = item1.OuterHeight;
                                    query_product.WeightGross = item1.OuterWeightGross;
                                    query_product.WeightNet = item1.OuterWeightNet;
                                }
                            }

                            query_Delivery_Encasements.Delivery_EncasementsHis.Add(query_history);
                        }
                    }
                    else
                    {
                        //审核数据时
                        query_Delivery_Encasements.UpdateUserID = currentUser.UserID;
                        query_Delivery_Encasements.UpdateTerminal = CommonCode.GetIP();
                        query_Delivery_Encasements.UpdateDate = DateTime.Now;
                        query_Delivery_Encasements.Delivery_EncasementsHis.Add(query_history);
                    }

                    affectRows = context.SaveChanges();

                    Save_ProductMixed(currentUser.UserID, query_Purchase_Contract.ID, vm.listProducts_Mixed);

                    iEncasementHisID = query_history.AuditHisID;
                    vm.EncasementID = query_history.EncasementsID;

                    if (affectRows == 0)
                    {
                        result.IsSuccess = false;
                        return result;
                    }
                    else
                    {
                        bool bIsPass = true;
                        if (vm.EncasementStatusID == (int)EncasementStatusEnum.NotPassCheck)
                        {
                            bIsPass = false;
                        }

                        List<int> listStatus = new List<int>();
                        listStatus.Add((int)EncasementStatusEnum.PendingCheck);
                        listStatus.Add((int)EncasementStatusEnum.NotPassCheck);
                        int[] iArrLogicStatus = { (int)EncasementStatusEnum.PendingCheck, (int)EncasementStatusEnum.NotPassCheck, (int)EncasementStatusEnum.PassedCheck };

                        ConstsMethod.InsertAuditStream(vm.EncasementID, bIsPass, WorkflowTypes.ApprovalShipping, currentUser.UserID, listStatus, iArrLogicStatus, vm.AuditIdea);

                        result.IsSuccess = true;

                        #region 如果外箱长、外箱宽、外箱高的上下浮动比例不超出，不需要业务经理审批

                        if (vm.EncasementStatusID == (int)EncasementStatusEnum.PendingCheck || vm.EncasementStatusID == (int)EncasementStatusEnum.PassedCheck)
                        {
                            var query_purchase = context.Purchase_Contract.Where(d => d.ID == vm.ContractID && d.ContractType == (int)ContractTypeEnum.Default).FirstOrDefault();
                            if (query_purchase != null)
                            {
                                var query_Delivery_Encasements2 = query_purchase.Delivery_Encasements.FirstOrDefault();
                                if (currentUser.RoleNames.Contains("业务员"))
                                {
                                    string SelectCustomer = query_purchase.Orders_Customers.SelectCustomer;
                                    decimal Proportion = 0;
                                    bool IsExceed = false;
                                    switch (SelectCustomer)
                                    {
                                        case "F20":
                                        case "S13":
                                        case "S235":
                                        case "S200":
                                        case "S60":
                                            Proportion = 0.05m;
                                            break;

                                        case "S188":
                                        case "DGHW":
                                        case "DGS":
                                            Proportion = 0.03m;
                                            break;

                                        default:
                                            break;
                                    }

                                    bool isApproval = false;
                                    if (Proportion > 0)
                                    {
                                        foreach (var item_product in query_Delivery_Encasements2.Delivery_EncasementsProducts)
                                        {
                                            var query_OrderProduct = context.OrderProducts.Find(item_product.OrderProductID);
                                            if (query_OrderProduct != null)
                                            {
                                                if (IsExceedScale(item_product.OuterLength, query_OrderProduct.OuterLength, Proportion))
                                                {
                                                    IsExceed = true;
                                                }
                                                if (IsExceedScale(item_product.OuterWidth, query_OrderProduct.OuterWidth, Proportion))
                                                {
                                                    IsExceed = true;
                                                }
                                                if (IsExceedScale(item_product.OuterHeight, query_OrderProduct.OuterHeight, Proportion))
                                                {
                                                    IsExceed = true;
                                                }
                                            }
                                        }
                                        isApproval = IsExceed;
                                    }
                                    else
                                    {
                                        isApproval = false;
                                    }
                                    if (!isApproval)
                                    {
                                        //不需要业务经理审批
                                        query_Delivery_Encasements2.ApproverIndex = int.MinValue;
                                        query_Delivery_Encasements2.EncasementsStatus = (int)EncasementStatusEnum.PassedCheck;

                                        query_purchase.ApproverIndexEncasement = int.MinValue;
                                        query_purchase.EncasementStatus = (int)EncasementStatusEnum.PassedCheck;

                                        context.SaveChanges();
                                    }
                                }
                            }
                        }

                        #endregion 如果外箱长、外箱宽、外箱高的上下浮动比例不超出，不需要业务经理审批
                    }
                }

                //审核数据时，执行以下代码
                if (vm.EncasementStatusID >= (int)EncasementStatusEnum.PendingCheck)
                {
                    SaveData(vm.ContractID, iEncasementHisID, currentUser);
                }

                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query_Purchase_Contract = context.Purchase_Contract.Find(vm.ContractID);
                    if (query_Purchase_Contract != null)
                    {
                        var query_Delivery_Encasements = query_Purchase_Contract.Delivery_Encasements.FirstOrDefault();

                        query_Purchase_Contract.EncasementStatus = query_Delivery_Encasements.EncasementsStatus;
                        query_Purchase_Contract.EncasementUpdateDate = DateTime.Now;
                        context.SaveChanges();
                    }

                }
            }
            catch (Exception e)
            {
                LogHelper.WriteError(e);
                result.IsSuccess = false;
            }

            return result;
        }

        /// <summary>
        /// 判断是否超过浮动比例
        /// </summary>
        /// <param name="afterVal">之后的值</param>
        /// <param name="beforeVal">之前的值</param>
        /// <returns></returns>
        public bool IsExceedScale(decimal? afterVal, decimal? beforeVal, decimal Proportion)
        {
            bool IsExceed = false;
            if (beforeVal.HasValue && afterVal.HasValue)
            {
                if (beforeVal != 0 && afterVal != 0 && beforeVal != afterVal)
                {
                    if (Math.Abs((decimal)afterVal - (decimal)beforeVal) / beforeVal > Proportion)
                    {
                        IsExceed = true;
                    }
                }
            }
            return IsExceed;
        }

        /// <summary>
        /// 更新采购合同主表出运明细状态为审核已通过，生成报关管理所用的发票编号
        /// </summary>
        /// <param name="iContractID"></param>
        /// <param name="iEncasementStatusID"></param>
        /// <returns></returns>
        private int SaveData(int iContractID, int iEncasementHisID, VMERPUser currentUser)
        {
            //当数据达到终审时，把结果值赋值于采购合同主表
            using (ERPEntitiesNew dbContext = new ERPEntitiesNew())
            {
                var query_purchase = dbContext.Purchase_Contract.Where(d => d.ID == iContractID && d.ContractType == (int)ContractTypeEnum.Default).FirstOrDefault();
                if (query_purchase != null)
                {
                    var query_Encasements = query_purchase.Delivery_Encasements.FirstOrDefault();

                    if (query_Encasements != null)
                    {
                        int EncasementsStatus = query_Encasements.EncasementsStatus;

                        //审核数据时，执行以下代码
                        if (EncasementsStatus >= (int)EncasementStatusEnum.NotPassCheck)
                        {
                            var dbEncasementHisInfo = query_Encasements.Delivery_EncasementsHis.Where(p => p.AuditHisID == iEncasementHisID).FirstOrDefault();

                            if (dbEncasementHisInfo != null)
                            {
                                dbEncasementHisInfo.EncasementsStatus = EncasementsStatus;
                            }

                            dbContext.SaveChanges();
                        }


                        //出运资料审核通过后，添加订舱
                        if (query_Encasements.EncasementsStatus == (int)EncasementStatusEnum.PassedCheck)
                        {
                            query_purchase.EncasementStatus = (int)EncasementStatusEnum.PassedCheck;
                            query_purchase.ApproverIndexEncasement = int.MinValue;
                            query_purchase.EncasementUpdateDate = DateTime.Now;

                            dbContext.SaveChanges();

                            SaveOther(currentUser, query_purchase.OrderID ?? 0);
                        }
                    }
                }
            }

            return 1;
        }

        public void SaveOther(VMERPUser currentUser, int OrderID)
        {
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    #region 订舱时，出运明细中关于此销售合同生成的采购合同必须都要为已审核通过才能进行订舱

                    int count = context.Purchase_Contract.Where(d => d.OrderID == OrderID && d.EncasementStatus != (int)EncasementStatusEnum.PassedCheck && !d.IsDelete && d.ContractType == (int)ContractTypeEnum.Default).Count();
                    if (count == 0)//如果该采购合同的销售订单里面的采购合同都出运通过后，添加订舱的数据。
                    {
                        var query_ShipmentOrder = context.Delivery_ShipmentOrder.Where(d => d.OrderID == OrderID);
                        if (query_ShipmentOrder.Count() == 0)
                        {
                            DAL.Delivery_ShipmentOrder query = new DAL.Delivery_ShipmentOrder()
                            {
                                OrderID = (int)OrderID,

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

                            context.Delivery_ShipmentOrder.Add(query);

                            context.SaveChanges();
                        }
                    }

                    #endregion 订舱时，出运明细中关于此销售合同生成的采购合同必须都要为已审核通过才能进行订舱
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
        }

        /// <summary>
        /// 删除本订单的其他数据
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public DBOperationStatus ClearData(VMERPUser currentUser, int id)
        {
            DBOperationStatus result = default(DBOperationStatus);
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Delivery_Encasements.Find(id);
                    if (query != null)
                    {
                        query.ApproverIndex = null;
                        query.EncasementsStatus = (int)EncasementStatusEnum.PendingEdit;
                        query.UpdateDate = DateTime.Now;
                        query.UpdateTerminal = CommonCode.GetIP();
                        query.UpdateUserID = currentUser.UserID;

                        query.Delivery_EncasementsHis.Add(new Delivery_EncasementsHis()
                        {
                            AuditIdea = "删除本订单的其他数据",
                            AuditUserID = currentUser.UserID,
                            CreateDate = DateTime.Now,
                            EncasementsStatus = (int)EncasementStatusEnum.PendingEdit,
                        });

                        int OrderID = query.Purchase_Contract.OrderID ?? 0;

                        var purchase = query.Purchase_Contract;
                        purchase.ApproverIndexEncasement = null;
                        purchase.EncasementStatus = (int)EncasementStatusEnum.PendingEdit;
                        purchase.EncasementUpdateDate = DateTime.Now;

                        var query_shipmentOrder = context.Delivery_ShipmentOrder.Where(d => d.OrderID == OrderID && !d.IsDelete);
                        if (query_shipmentOrder.Count() > 0)
                        {
                            List<int> list_OrderID = new List<int>();

                            #region 删除订舱和出运通知的相关数据

                            if (query_shipmentOrder.FirstOrDefault().IsMerge)
                            {
                                var OrderIDList = query_shipmentOrder.FirstOrDefault().OrderIDList;
                                query_shipmentOrder = context.Delivery_ShipmentOrder.Where(d => d.OrderIDList == OrderIDList && !d.IsDelete);

                                list_OrderID = CommonCode.IdListToList(OrderIDList);
                                foreach (var item in list_OrderID)
                                {
                                    if (item != OrderID)
                                    {
                                        DAL.Delivery_ShipmentOrder query_ShipmentOrder = new DAL.Delivery_ShipmentOrder()
                                        {
                                            OrderID = item,

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

                                        context.Delivery_ShipmentOrder.Add(query_ShipmentOrder);
                                    }

                                }
                            }
                            else
                            {
                                list_OrderID.Add(OrderID);
                            }

                            foreach (var item in list_OrderID)
                            {
                                //清空销售订单的港杂费发票信息
                                var query_order = context.Orders.Find(item);
                                query_order.DesignatedAgencyAmount = null;
                                query_order.OurAgencyAmount = null;
                                query_order.PortChargesInvoice_StatusID = null;
                                query_order.PortChargesInvoice_CreateUserID = null;
                                query_order.PortChargesInvoice_UpdateDate = null;
                            }

                            var list_ShipmentOrderID = query_shipmentOrder.Select(d => d.ID).ToList();

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
                            context.Delivery_ShipmentOrder.RemoveRange(query_shipmentOrder);

                            #endregion 删除订舱和出运通知的相关数据

                            #region 删除报检的数据

                            var InspectionReceipt = from d in context.Inspection_InspectionReceipt
                                                    where list_ShipmentOrderID.Contains(d.ShipmentOrderID ?? 0)
                                                    select d;

                            var list_InspectionReceiptID = InspectionReceipt.Select(d => d.InspectionReceiptID).ToList();

                            var InspectionReceiptList = from d in context.Inspection_InspectionReceiptList
                                                        where list_ShipmentOrderID.Contains(d.ShipmentOrderID ?? 0)
                                                        select d;

                            var InspectionReceiptProduct = from d in context.Inspection_InspectionReceiptProduct
                                                           where list_InspectionReceiptID.Contains(d.InspectionReceiptID)
                                                           select d;

                            var InspectionReceiptHis = from d in context.Inspection_InspectionReceiptHis
                                                       where list_InspectionReceiptID.Contains(d.InspectionReceiptID)
                                                       select d;

                            context.Inspection_InspectionReceiptHis.RemoveRange(InspectionReceiptHis);
                            context.Inspection_InspectionReceiptProduct.RemoveRange(InspectionReceiptProduct);
                            context.Inspection_InspectionReceipt.RemoveRange(InspectionReceipt);
                            context.Inspection_InspectionReceiptList.RemoveRange(InspectionReceiptList);

                            #endregion 删除报检的数据

                            #region 删除报关的数据

                            var InspectionCustoms = from d in context.Inspection_InspectionCustoms
                                                    where list_ShipmentOrderID.Contains(d.ShipmentOrderID)
                                                    select d;

                            var list_InspectionCustomsID = InspectionCustoms.Select(d => d.InspectionCustomsID).ToList();

                            var InspectionCustomsDetail = from d in context.Inspection_InspectionCustomsDetail
                                                          where list_InspectionCustomsID.Contains(d.InspectionCustomsID)
                                                          select d;

                            var list_InspectionCustomsDetailID = InspectionCustomsDetail.Select(d => d.ID).ToList();

                            var InspectionCustomsProduct = from d in context.Inspection_InspectionCustomsProduct
                                                           where list_InspectionCustomsDetailID.Contains(d.InspectionCustomsDetailID)
                                                           select d;

                            var list_InspectionCustomsProductID = InspectionCustomsProduct.Select(d => d.ID).ToList();

                            var InspectionCustomsProduct2 = from d in context.Inspection_InspectionCustomsProduct2
                                                            where list_InspectionCustomsProductID.Contains(d.InspectionCustomsProductID ?? 0)
                                                            select d;

                            var InspectionCustomsHis = from d in context.Inspection_InspectionCustomsHis
                                                       where list_InspectionCustomsID.Contains(d.InspectionCustomsID)
                                                       select d;

                            context.Inspection_InspectionCustomsHis.RemoveRange(InspectionCustomsHis);
                            context.Inspection_InspectionCustomsProduct2.RemoveRange(InspectionCustomsProduct2);
                            context.Inspection_InspectionCustomsProduct.RemoveRange(InspectionCustomsProduct);
                            context.Inspection_InspectionCustomsDetail.RemoveRange(InspectionCustomsDetail);
                            context.Inspection_InspectionCustoms.RemoveRange(InspectionCustoms);

                            #endregion 删除报关的数据

                            #region 删除清关的数据

                            var Inspection_InspectionClearance = from d in context.Inspection_InspectionClearance
                                                                 where list_ShipmentOrderID.Contains(d.ShipmentOrderID)
                                                                 select d;

                            var list_InspectionClearanceID = Inspection_InspectionClearance.Select(d => d.InspectionClearanceID).ToList();

                            var Inspection_InspectionClearanceHis = from d in context.Inspection_InspectionClearanceHis
                                                                    where list_InspectionClearanceID.Contains(d.InspectionClearanceID)
                                                                    select d;

                            context.Inspection_InspectionClearanceHis.RemoveRange(Inspection_InspectionClearanceHis);
                            context.Inspection_InspectionClearance.RemoveRange(Inspection_InspectionClearance);

                            #endregion 删除清关的数据

                            #region 删除结汇的数据

                            var Inspection_InspectionExchange = from d in context.Inspection_InspectionExchange
                                                                where list_ShipmentOrderID.Contains(d.ShipmentOrderID)
                                                                select d;

                            var list_InspectionExchangeID = Inspection_InspectionExchange.Select(d => d.InspectionExchangeID).ToList();

                            var Inspection_InspectionExchangeHis = from d in context.Inspection_InspectionExchangeHis
                                                                   where list_InspectionExchangeID.Contains(d.InspectionExchangeID)
                                                                   select d;

                            context.Inspection_InspectionExchangeHis.RemoveRange(Inspection_InspectionExchangeHis);
                            context.Inspection_InspectionExchange.RemoveRange(Inspection_InspectionExchange);

                            #endregion 删除结汇的数据

                            #region 删除单证索引的数据

                            var DocumentsIndexings = from d in context.DocumentsIndexings
                                                     where list_ShipmentOrderID.Contains(d.ShipmentOrderID ?? 0)
                                                     select d;

                            var list_DocumentsIndexings = DocumentsIndexings.Select(d => d.ID).ToList();

                            var DocumentsIndexingHistories = from d in context.DocumentsIndexingHistories
                                                             where list_DocumentsIndexings.Contains(d.DocumentsIndexingID)
                                                             select d;

                            context.DocumentsIndexingHistories.RemoveRange(DocumentsIndexingHistories);
                            context.DocumentsIndexings.RemoveRange(DocumentsIndexings);

                            #endregion 删除单证索引的数据

                            #region 删除上传的文件

                            List<int> list_Delete_UploadIDList = new List<int>();

                            list_Delete_UploadIDList.AddRange(context.UpLoadFiles.Where(d => list_ShipmentOrderCabinetID.Contains(d.LinkID) && d.ModuleType == (int)UploadFileType.ShipmentNotification_DeliveryNotification && !d.IsDelete).Select(d => d.ID).ToList());//配仓单

                            list_Delete_UploadIDList.AddRange(context.UpLoadFiles.Where(d => list_InspectionReceiptID.Contains(d.LinkID) && d.ModuleType == (int)UploadFileType.InspectionReceiptCommission && !d.IsDelete).Select(d => d.ID).ToList());//报检委托书

                            list_Delete_UploadIDList.AddRange(context.UpLoadFiles.Where(d => list_InspectionReceiptID.Contains(d.LinkID) && d.ModuleType == (int)UploadFileType.InspectionReceiptUploadReceipt && !d.IsDelete).Select(d => d.ID).ToList());//报检凭条

                            list_Delete_UploadIDList.AddRange(context.UpLoadFiles.Where(d => list_InspectionCustomsDetailID.Contains(d.LinkID) && d.ModuleType == (int)UploadFileType.InspectionCustomsUploadReceipt && !d.IsDelete).Select(d => d.ID).ToList());//报关选择的报检凭条

                            list_Delete_UploadIDList.AddRange(context.UpLoadFiles.Where(d => list_InspectionCustomsDetailID.Contains(d.LinkID) && d.ModuleType == (int)UploadFileType.InspectionCustomsCommission && !d.IsDelete).Select(d => d.ID).ToList());//报关委托书

                            list_Delete_UploadIDList.AddRange(context.UpLoadFiles.Where(d => list_InspectionCustomsDetailID.Contains(d.LinkID) && d.ModuleType == (int)UploadFileType.InspectionCustomsDeclareElements && !d.IsDelete).Select(d => d.ID).ToList());//报关申报要素

                            list_Delete_UploadIDList.AddRange(context.UpLoadFiles.Where(d => list_InspectionClearanceID.Contains(d.LinkID) && d.ModuleType == (int)UploadFileType.InspectionClearance && !d.IsDelete).Select(d => d.ID).ToList());//清关其他

                            list_Delete_UploadIDList.AddRange(context.UpLoadFiles.Where(d => list_InspectionClearanceID.Contains(d.LinkID) && d.ModuleType == (int)UploadFileType.InspectionClearance_FCR && !d.IsDelete).Select(d => d.ID).ToList());//FCR

                            list_Delete_UploadIDList.AddRange(context.UpLoadFiles.Where(d => list_InspectionExchangeID.Contains(d.LinkID) && d.ModuleType == (int)UploadFileType.InspectionExchange && !d.IsDelete).Select(d => d.ID).ToList());//结汇其他

                            var UpLoadFiles = from d in context.UpLoadFiles
                                              where list_Delete_UploadIDList.Contains(d.ID)
                                              select d;

                            context.UpLoadFiles.RemoveRange(UpLoadFiles);

                            #endregion 删除上传的文件
                        }

                        int i = context.SaveChanges();
                        if (i > 0)
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
            }
            return result;
        }


        /// <summary>
        /// 保存混装产品
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="context"></param>
        /// <param name="quotID"></param>
        private void Save_ProductMixed(int UserID, int ID, List<DTOEncasementProducts> list)
        {
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query_Purchase_Contract = context.Purchase_Contract.Find(ID);
                    var query_Delivery_Encasements = query_Purchase_Contract.Delivery_Encasements.FirstOrDefault();

                    var list_product = new List<Delivery_EncasementsProducts>();

                    //删除混装产品
                    var listProductsMixed_Delete = query_Delivery_Encasements.Delivery_EncasementsProducts.Where(d => d.IsProductMixed && d.ParentProductMixedID.HasValue && d.EncasementsID == query_Delivery_Encasements.EncasementsID);
                    if (listProductsMixed_Delete.Count() > 0)
                    {
                        context.Delivery_EncasementsProducts.RemoveRange(listProductsMixed_Delete);
                    }

                    foreach (var item in query_Delivery_Encasements.Delivery_EncasementsProducts)
                    {
                        if (item.OrderProduct.IsProductMixed && !item.OrderProduct.ParentProductMixedID.HasValue)
                        {
                            var thisProduct2 = list.Where(d => d.ParentProductMixedID == item.OrderProductID);
                            foreach (var item2 in thisProduct2)
                            {
                                var query_product = new Delivery_EncasementsProducts();

                                query_product.EncasementsID = item.EncasementsID;
                                query_product.IsProductMixed = true;
                                query_product.ParentProductMixedID = item.EncasementsProductID;

                                query_product.OrderProductID = item2.OrderProductID;

                                query_product.OuterLength = item2.OuterLength;
                                query_product.OuterWidth = item2.OuterWidth;
                                query_product.OuterHeight = item2.OuterHeight;

                                query_product.WeightGross = item2.OuterWeightGross;
                                query_product.WeightNet = item2.OuterWeightNet;

                                list_product.Add(query_product);
                            }
                        }
                    }
                    context.Delivery_EncasementsProducts.AddRange(list_product);
                    int i = context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
        }

    }
}