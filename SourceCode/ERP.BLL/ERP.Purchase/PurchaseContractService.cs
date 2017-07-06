using ERP.BLL.Consts;
using ERP.BLL.ERP.Consts;
using ERP.BLL.ERP.ProductFitting;
using ERP.BLL.Workflow;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.ProductFitting;
using ERP.Models.Purchase;
using ERP.Tools;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ERP.BLL.ERP.Purchase
{
    public class PurchaseContractService
    {
        private ERP.Dictionary.DictionaryServices _dictionaryServices = new Dictionary.DictionaryServices();
        private ProductFittingServices _productFittingServices = new ProductFittingServices();

        #region HelperMethod

        /// <summary>
        /// 获取采购合同状态的值
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public static string GetPurchaseContract(int id)
        {
            return CommonCode.GetStatusEnumName(id, typeof(PurchaseStatusEnum));
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
                    var query = context.Purchase_Contract.Where(p => !p.IsDelete);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForPurchase);

                    if (currentUser.HierachyType == HierachyType.Agency)
                    {
                        var userHierachy = new List<int?>()
                        {
                            currentUser.HierachyID,
                            context.SystemUsers.FirstOrDefault(p => p.HierarchyID == currentUser.HierachyID).Hierarchy.Hierarchy2.HierarchyID
                        };

                        query = query.Where(p => userHierachy.Contains(p.Factory.Hierarchy));
                    }

                    if (vm_search.PageType == PageTypeEnum.PassedCheckList)//已审核
                    {
                        query = query.Where(d => d.PurchaseStatus >= (short)PurchaseStatusEnum.PassedCheck);
                    }
                    else
                    {
                        query = query.Where(d => d.PurchaseStatus < (short)PurchaseStatusEnum.PassedCheck);
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
                    if (!string.IsNullOrEmpty(vm_search.PurchaseStatus))
                    {
                        int i = Utils.StrToInt(vm_search.PurchaseStatus, (short)PurchaseStatusEnum.OutLine);
                        query = query.Where(d => d.PurchaseStatus == i);
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
                        List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                        listModel = new List<DTOPurchaseContract>();
                        foreach (var entity in dataFromDB)
                        {
                            bool IsHasApprovalPermission = true;

                            #region 判断是否有审批流的权限

                            if (vm_search.PageType == PageTypeEnum.PendingCheckList)
                            {
                                IsHasApprovalPermission = ApprovalService.HasApprovalPermission(WorkflowTypes.ApprovalPurchaseContract, currentUser, entity.ST_CREATEUSER, entity.ApproverIndexPurchaseContract, entity.CustomerID);
                            }

                            #endregion 判断是否有审批流的权限

                            string CurrentSign = "";
                            if (entity.ContractType == (int)ContractTypeEnum.Default)
                            {
                                CurrentSign = _dictionaryServices.GetDictionary_CurrencySign(entity.Purchase_ContractBatch.First().Purchase_ContractProduct.Where(d => !d.IsDelete && !d.ParentProductMixedID.HasValue).First().OrderProduct.CurrencyType, list_Com_DataDictionary);
                            }
                            else
                            {
                                var ParentID = entity.Purchase_ContractBatch.First().ID;
                                var CurrencyType = context.ProductFittings.Where(d => d.ParentID == ParentID && !d.Deleted && d.ModuleType == (int)ModuleTypeEnum.PurchaseContract).First().CurrencyType;
                                CurrentSign = _dictionaryServices.GetDictionary_CurrencySign(CurrencyType, list_Com_DataDictionary);
                            }

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
                                IsImmediatelySend = entity.IsImmediatelySend ? "是" : "否",
                                IsThirdVerification = entity.IsThirdVerification ? "是" : "否",
                                IsThirdAudits = entity.IsThirdAudits ? "是" : "否",
                                IsThirdTest = entity.IsThirdTest ? "是" : "否",
                                IsThirdSampling = entity.IsThirdSampling ? "是" : "否",
                                DT_MODIFYDATE = Utils.DateTimeToStr2(entity.DT_MODIFYDATE),
                                DateStart = Utils.DateTimeToStr(entity.DateStart),
                                DateEnd = Utils.DateTimeToStr(entity.DateEnd),
                                PurchaseStatus = GetPurchaseContract(entity.PurchaseStatus),
                                PurchaseStatusID = entity.PurchaseStatus,
                                CurrentSign = CurrentSign,
                                ST_CREATEUSER = entity.ST_CREATEUSER,
                                ApproverIndexPurchaseContract = entity.ApproverIndexPurchaseContract,
                                IsHasApprovalPermission = IsHasApprovalPermission,
                                SelectCustomer = entity.Orders_Customers.SelectCustomer,
                                ContractTypeFormatter = entity.ContractType == 0 ? "" : "配件合同",
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
        /// <param name="currentUser"></param>
        /// <param name="IDs"></param>
        /// <returns></returns>
        public DBOperationStatus Delete(VMERPUser currentUser, List<int> IDs)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Purchase_Contract.Where(p => IDs.Contains(p.ID));
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForPurchase);
                    if (query != null)
                    {
                        foreach (var item in query)
                        {
                            item.IsDelete = true;
                            item.DT_MODIFYDATE = DateTime.Now;
                            item.ST_MODIFYUSER = currentUser.UserID;
                            item.IPAddress = CommonCode.GetIP();

                            foreach (var item2 in item.Purchase_ContractBatch)
                            {
                                foreach (var item3 in item2.Purchase_ContractProduct)
                                {
                                    item3.IsDelete = true;
                                    item3.DT_MODIFYDATE = DateTime.Now;
                                    item3.ST_MODIFYUSER = currentUser.UserID;
                                    item3.IPAddress = CommonCode.GetIP();
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
        /// 新建采购合同——获取选中订单的数据
        /// </summary>
        /// <param name="IDs">销售订单编号列表</param>
        /// <param name="totalRows"></param>
        /// <returns></returns>
        public List<VMPurchase> GetALL_Add(VMERPUser currentUser, int? OrderID)
        {
            List<VMPurchase> list_vm = null;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query_order = context.Orders.Find(OrderID);
                    var query = query_order.OrderProducts.Where(d => !d.IsDelete && !d.ParentProductMixedID.HasValue);

                    List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                    var list_HarmonizedSystems = context.HarmonizedSystems.Where(p => !p.IsDelete).ToList();

                    var query_Orders_Customers = context.Orders_Customers.Find(query_order.CustomerID);
                    string CustomerCode = "";
                    string SelectCustomer = "";
                    if (query_Orders_Customers != null)
                    {
                        CustomerCode = query_Orders_Customers.CustomerCode;
                        SelectCustomer = query_Orders_Customers.SelectCustomer;
                    }

                    #region 给Model赋值

                    if (query != null && query.Count() > 0)
                    {
                        string CurrentSign = "";

                        #region 获取集合

                        List<int> listFactoryID = new List<int>();//工厂编号集合
                        foreach (var item in query)
                        {
                            var data = item;
                            var factoryID = data.FactoryID ?? 0;

                            if (!listFactoryID.Contains(factoryID))
                            {
                                listFactoryID.Add(factoryID);
                            }
                        }

                        #endregion 获取集合

                        list_vm = new List<VMPurchase>();
                        List<VMProductFittingInfo> list_ProductFitting = new List<VMProductFittingInfo>();

                        #region 产品的

                        int PurchaseNumberTemp = 65;
                        foreach (int factoryID in listFactoryID)//遍历工厂ID
                        {
                            var dataFromDB = query.Where(d => d.FactoryID == factoryID);
                            decimal AllAmount = 0;
                            decimal AllVolume = 0;
                            int AllQty = 0;

                            List<VMPurchaseBatch> list_batch = new List<VMPurchaseBatch>();
                            List<string> list_ProductCopyRight_OurCompany_No = new List<string>();
                            List<string> list_ProductCopyRight_Factory_No = new List<string>();
                            List<string> list_OuterVolume = new List<string>();
                            List<string> list_OuterVolume_S60 = new List<string>();
                            List<string> list_OuterVolume_S164 = new List<string>();
                            List<string> list_Inner_S135 = new List<string>();
                            List<string> list_Outer_S135 = new List<string>();
                            List<int?> list_ParentID = new List<int?>();

                            decimal? SumOuterVolume = 0;
                            List<VMContractTerms> list_ContractTerms = new List<VMContractTerms>();
                            bool IsInnerBoxRate = false;
                            List<string> list_PackageName = new List<string>();

                            #region 遍历批次

                            List<VMPurchaseProduct> list_product = new List<VMPurchaseProduct>();
                            decimal BatchAmount = 0;

                            #region 遍历订单的产品

                            string Unit = "CUFT";
                            if (SelectCustomer == SelectCustomerEnum.S60.ToString())
                            {
                                Unit = "CUFT";
                            }
                            if (SelectCustomer == SelectCustomerEnum.S288.ToString() || SelectCustomer == SelectCustomerEnum.S135.ToString() || SelectCustomer == SelectCustomerEnum.S52.ToString() || SelectCustomer == SelectCustomerEnum.S56.ToString())
                            {
                                Unit = "M3";
                            }

                            foreach (var item in dataFromDB)//遍历订单的产品
                            {
                                var query_product = context.Products.Find(item.ProductID);
                                if (query_product != null)
                                {
                                    int? ParentProductID = query_product.ParentProductID;
                                    if (!list_ParentID.Contains(ParentProductID))
                                    {
                                        list_ParentID.Add(ParentProductID);

                                        list_Inner_S135.Add(item.InnerBoxRate + "只/邮购内盒/" + item.InnerLength + "X" + item.InnerWidth + "X" + item.InnerHeight + "CM");
                                        list_Outer_S135.Add(item.OuterBoxRate + "只/外箱/" + item.OuterLength + "X" + item.OuterWidth + "X" + item.OuterHeight + "CM");
                                    }
                                }

                                decimal ProductAmount = item.PriceFactory * item.Qty;
                                CurrentSign = _dictionaryServices.GetDictionary_CurrencySign(item.CurrencyType, list_Com_DataDictionary);
                                string StyleNumber = item.StyleID == null ? null : _dictionaryServices.GetDictionary_StyleNumber(item.StyleID, list_Com_DataDictionary);
                                string MixedMode = null;
                                if (StyleNumber != null)
                                {
                                    if (StyleNumber.StrTrim() != "1")
                                    {
                                        MixedMode = StyleNumber + "款平均混装";
                                    }
                                }
                                string PackageName = item.PackingMannerZhID == null ? null : _dictionaryServices.GetDictionary_PackingMannerZhName(item.PackingMannerZhID, list_Com_DataDictionary);

                                #region 设置条款里面的内容

                                if (item.ProductCopyRight.HasValue && item.ProductCopyRight.Value == (int)ProductCopyRightTypeEnum.OurCompany && !list_ProductCopyRight_OurCompany_No.Contains(item.No))
                                {
                                    list_ProductCopyRight_OurCompany_No.Add(item.No);
                                }

                                if (item.ProductCopyRight.HasValue && item.ProductCopyRight.Value == (int)ProductCopyRightTypeEnum.Factory && !list_ProductCopyRight_Factory_No.Contains(item.No))
                                {
                                    list_ProductCopyRight_Factory_No.Add(item.No);
                                }
                                decimal? OuterVolume = CalculateHelper.GetOuterVolume(item.OuterLength, item.OuterWidth, item.OuterHeight);
                                decimal? OuterVolume_CUFT = CalculateHelper.GetOuterVolume_CUFT(item.OuterLength, item.OuterWidth, item.OuterHeight);
                                list_OuterVolume.Add(item.No + "的单个外箱箱规不得超过" + OuterVolume + Unit);

                                list_OuterVolume_S60.Add(item.No + "单个外箱箱规不得超过或小于:" + item.OuterLength + "CM * " + item.OuterWidth + "CM * " + item.OuterHeight + "CM / " + OuterVolume_CUFT.Round(2) + Unit);

                                list_OuterVolume_S164.Add(item.No + "的单个外箱箱柜不得超过" + item.OuterLength + "X" + item.OuterWidth + "X" + item.OuterHeight + "CM/" + OuterVolume_CUFT.Round(2) + "'/" + item.OuterWeightNetLBS + "LBS");

                                var BoxQty = CalculateHelper.GetBoxQty(item.Qty, item.OuterBoxRate);//箱数
                                SumOuterVolume += CalculateHelper.GetSumOuterVolume(item.OuterLength, item.OuterWidth, item.OuterHeight, BoxQty);//该采购合同的总体积

                                if (item.HSCode.HasValue)//报关
                                {
                                    int HSCode = item.HSCode.Value;
                                    var dbHS = context.HarmonizedSystems.Find(HSCode);
                                    string ProjectName = ConstsMethod.GetHSCodeName(list_HarmonizedSystems, list_Com_DataDictionary, dbHS, HSCode);

                                    if (list_ContractTerms.Select(d => d.HSCodeID).Contains(HSCode))
                                    {
                                        var temp = list_ContractTerms.Where(d => d.HSCodeID == HSCode);
                                        if (temp.Count() > 0)
                                        {
                                            List<string> list_No = temp.First().list_No;
                                            list_No.Add(item.No);

                                            temp.First().list_No = list_No;
                                        }
                                    }
                                    else
                                    {
                                        List<string> list_No = new List<string>();
                                        list_No.Add(item.No);

                                        list_ContractTerms.Add(new VMContractTerms()
                                        {
                                            HSCodeID = HSCode,
                                            HSCode = dbHS.HSCode,
                                            HSCodeName = dbHS.CodeName,
                                            list_No = list_No,
                                            ProjectName = ProjectName,
                                        });
                                    }
                                }

                                if (item.InnerBoxRate.HasValue && item.InnerBoxRate.Value > 0)
                                {
                                    IsInnerBoxRate = true;
                                }

                                if (!list_PackageName.Contains(PackageName) && !string.IsNullOrEmpty(PackageName))
                                {
                                    list_PackageName.Add(PackageName);
                                }

                                #endregion 设置条款里面的内容

                                list_product.Add(new VMPurchaseProduct()
                                {
                                    ID = item.ID,
                                    ProductID = item.ProductID,
                                    Image = item.Image,
                                    Name = item.Name,
                                    No = item.No,
                                    PackageName = PackageName,
                                    PriceFactory = item.PriceFactory,
                                    PriceFactoryFormatter = CurrentSign + item.PriceFactory,
                                    Qty = item.Qty,
                                    UnitID = item.UnitID,
                                    UnitName = _dictionaryServices.GetDictionary_UnitName(item.UnitID, list_Com_DataDictionary),
                                    ProductAmount = ProductAmount,
                                    ProductAmountFormatter = CurrentSign + ProductAmount,
                                    CurrentSign = CurrentSign,
                                    InnerBoxRate = item.InnerBoxRate,
                                    OuterBoxRate = item.OuterBoxRate,
                                    PDQPackRate = item.PDQPackRate,
                                    PackingMannerZhID = item.PackingMannerZhID,
                                    StyleID = item.StyleID,
                                    StyleName = item.StyleID == null ? null : _dictionaryServices.GetDictionary_StyleName(item.StyleID, list_Com_DataDictionary),
                                    MixedMode = MixedMode,
                                    IsFragile = 2,
                                    IsProductMixed = item.IsProductMixed,
                                });
                                BatchAmount += ProductAmount;
                                AllVolume += item.OuterVolume ?? 0;
                                AllQty += item.Qty;

                                if (item.IsProductFitting)
                                {
                                    var list_ProductFittingInfo = _productFittingServices.GetProductFittingInfo(currentUser, item.ID, (int)ModuleTypeEnum.Order);
                                    for (int k = 0; k < list_ProductFittingInfo.Count; k++)
                                    {
                                        list_ProductFittingInfo[k].Qty *= item.Qty;//配件产品的数量=销售订单的产品数量*配件产品的数量
                                    }
                                    list_ProductFitting.AddRange(list_ProductFittingInfo);
                                }
                            }

                            #endregion 遍历订单的产品

                            if (list_product.Count > 0)
                            {
                                list_batch.Add(new VMPurchaseBatch()
                                {
                                    DateStart = query_order.OrderDateStart,
                                    DateEnd = query_order.OrderDateEnd,
                                    Times = 1,
                                    BatchAmount = BatchAmount,
                                    CurrentSign = CurrentSign,
                                    listProduct = list_product,
                                });
                                AllAmount += BatchAmount;
                            }

                            #endregion 遍历批次

                            var query_factory = context.Factories.Find(factoryID);
                            decimal RegisterFees = 0;//拖柜费
                            if (query_factory != null)
                            {
                                RegisterFees = query_factory.RegisterFees ?? 0;
                            }

                            string str_ProductCopyRight_Factory_No = CommonCode.ListToString(list_ProductCopyRight_Factory_No);
                            string str_ProductCopyRight_OurCompany_No = CommonCode.ListToString(list_ProductCopyRight_OurCompany_No);
                            List<string> list_Customs = new List<string>();
                            List<string> list_Customs_S10 = new List<string>();
                            foreach (var item in list_ContractTerms)
                            {
                                if (!string.IsNullOrEmpty(item.ProjectName))
                                {
                                    list_Customs.Add("本合同" + CommonCode.ListToString(item.list_No) + "产品报关品名" + item.HSCodeName + "，出运前须办理" + item.ProjectName);

                                    list_Customs_S10.Add("本合同产品出货前请办理" + item.ProjectName + "，报检品名为" + item.HSCodeName + "，HS编码为" + item.HSCode + "。");
                                }
                            }
                            string str_Customs = CommonCode.ListToString(list_Customs, "；");
                            string str_Customs_S10 = CommonCode.ListToString(list_Customs_S10, "；");

                            if (SelectCustomer == SelectCustomerEnum.S13.ToString() || SelectCustomer == SelectCustomerEnum.S259.ToString())
                            {
                                Unit = "CBM";
                            }
                            string str_SumOuterVolume = SumOuterVolume + Unit;//总体积
                            string PortName = context.Com_DataDictionary.Find(query_order.PortID).Name;//销售订单的出运港

                            string str_ProductPackingMethod = CommonCode.ListToString(list_PackageName);
                            StringBuilder sb = PurchaseContractHelper.Bind_PurchaseContract(CurrentSign, AllAmount, list_OuterVolume, SelectCustomer, RegisterFees, str_ProductCopyRight_Factory_No, str_ProductCopyRight_OurCompany_No, str_Customs, str_SumOuterVolume, PortName, IsInnerBoxRate, str_ProductPackingMethod, list_OuterVolume_S60, query_order.Orders_Customers.CustomerCode, list_Inner_S135, list_Outer_S135, str_Customs_S10, list_OuterVolume_S164);

                            if (list_batch.Count > 0)
                            {
                                list_vm.Add(new VMPurchase()
                                {
                                    OrderID = query_order.OrderID,
                                    OrderNumber = query_order.OrderNumber,
                                    PurchaseNumber = "PC" + query_order.OrderNumber + (char)PurchaseNumberTemp,
                                    AllAmount = AllAmount,
                                    AllVolume = AllVolume,
                                    AllQty = AllQty,
                                    FactoryID = factoryID,
                                    CallPeople = query_factory.CallPeople,
                                    Fax = query_factory.Fax,
                                    FactoryNo = query_factory.No,
                                    FactoryAbbreviation = query_factory.Abbreviation,
                                    FactoryName = query_factory.Name,
                                    Telephone = query_factory.Telephone,
                                    //PortID = data.PortID ?? 0,
                                    IsThirdAudits = query_order.IsThirdAudits,
                                    IsThirdVerification = query_order.IsThirdVerification,
                                    IsThirdTest = query_order.IsThirdTest,
                                    IsThirdSampling = query_order.IsThirdSampling,
                                    CustomerCode = CustomerCode,
                                    CurrentSign = CurrentSign,
                                    DateStartFormatter = Utils.DateTimeToStr(query_order.OrderDateStart.AddDays(-15)),
                                    DateEndFormatter = Utils.DateTimeToStr(query_order.OrderDateEnd.AddDays(-15)),
                                    list_batch = list_batch,
                                    ContractTerms = sb.ToString(),
                                    ContractType = (int)ContractTypeEnum.Default,
                                });

                                ++PurchaseNumberTemp;
                            }
                        }

                        #endregion 产品的

                        #region 配件产品的

                        decimal AllAmount2 = 0;
                        var temp2 = list_ProductFitting.GroupBy(d => d.FactoryID);
                        foreach (var item in temp2)
                        {
                            List<VMPurchaseBatch> list_batch2 = new List<VMPurchaseBatch>();
                            int FactoryID = item.First().FactoryID;

                            var query_factory = context.Factories.Find(FactoryID);

                            decimal BatchAmount = 0;
                            int AllQty = 0;
                            List<VMProductFittingInfo> listProductFitting = new List<VMProductFittingInfo>();
                            foreach (var item2 in item)
                            {
                                AllQty += item2.Qty ?? 0;
                                BatchAmount += item2.Qty * item2.PriceFactory ?? 0;
                                listProductFitting.Add(item2);
                            }

                            CurrentSign = _dictionaryServices.GetDictionary_CurrencySign(item.First().CurrencyType, list_Com_DataDictionary);

                            if (listProductFitting.Count > 0)
                            {
                                list_batch2.Add(new VMPurchaseBatch()
                                {
                                    DateStart = query_order.OrderDateStart,
                                    DateEnd = query_order.OrderDateEnd,
                                    Times = 1,
                                    BatchAmount = BatchAmount,
                                    CurrentSign = CurrentSign,
                                    listProductFitting = listProductFitting,
                                });
                                AllAmount2 += BatchAmount;
                            }

                            list_vm.Add(new VMPurchase()
                            {
                                OrderID = query_order.OrderID,
                                OrderNumber = query_order.OrderNumber,
                                PurchaseNumber = "PC" + query_order.OrderNumber + (char)PurchaseNumberTemp,
                                AllAmount = AllAmount2,
                                //AllVolume = AllVolume,
                                AllQty = AllQty,
                                FactoryID = FactoryID,
                                CallPeople = query_factory.CallPeople,
                                Fax = query_factory.Fax,
                                FactoryNo = query_factory.No,
                                FactoryAbbreviation = query_factory.Abbreviation + "(配件)",
                                FactoryName = query_factory.Name,
                                Telephone = query_factory.Telephone,

                                CustomerCode = CustomerCode,
                                CurrentSign = CurrentSign,
                                list_batch = list_batch2,
                                ContractTerms = PurchaseContractHelper.Bind_PurchaseContact_ProductFitting().ToString(),
                                ContractType = (int)ContractTypeEnum.ProductFitting,
                            });

                            ++PurchaseNumberTemp;
                        }

                        #endregion 配件产品的
                    }

                    #endregion 给Model赋值
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return list_vm;
        }

        /// <summary>
        /// 新建采购合同——新建
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public VMAjaxProcessResult Add(VMERPUser currentUser, List<VMPurchase> list)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            int affectRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    foreach (VMPurchase item in list)
                    {
                        DAL.Purchase_Contract query_purchase = new DAL.Purchase_Contract();
                        if (item.ContractType == (int)ContractTypeEnum.ProductFitting)
                        {
                            item.DateStart = DateTime.Now;
                            item.DateEnd = DateTime.Now;
                        }

                        query_purchase.PurchaseNumber = item.PurchaseNumber;
                        query_purchase.OrderID = item.OrderID;
                        query_purchase.FactoryID = item.FactoryID;
                        query_purchase.PurchaseDate = DateTime.Now;
                        query_purchase.IsImmediatelySend = item.IsImmediatelySend;
                        query_purchase.IsThirdAudits = item.IsThirdAudits;
                        query_purchase.IsThirdVerification = item.IsThirdVerification;
                        query_purchase.IsThirdTest = item.IsThirdTest;
                        query_purchase.IsThirdSampling = item.IsThirdSampling;
                        query_purchase.IsDelete = false;
                        query_purchase.ST_CREATEUSER = currentUser.UserID;
                        query_purchase.DT_CREATEDATE = DateTime.Now;
                        query_purchase.ST_MODIFYUSER = currentUser.UserID;
                        query_purchase.DT_MODIFYDATE = DateTime.Now;
                        query_purchase.IPAddress = CommonCode.GetIP();
                        query_purchase.Port = item.Port.StrTrim();
                        query_purchase.PaymentType = item.PaymentType;
                        query_purchase.AfterDate = item.AfterDate ?? 0;
                        query_purchase.DateStart = item.DateStart;
                        query_purchase.DateEnd = item.DateEnd;
                        query_purchase.OtherFee = item.OtherFee;
                        query_purchase.AllAmount = item.AllAmount;
                        query_purchase.AllQty = item.AllQty;
                        query_purchase.CustomerID = item.CustomerID;
                        query_purchase.ContractTerms = item.ContractTerms;
                        query_purchase.PurchaseStatus = item.PurchaseStatus;
                        query_purchase.PacksStatus = 0;
                        query_purchase.ContractType = item.ContractType;

                        DAL.Purchase_ContractHistory model_history = new DAL.Purchase_ContractHistory()
                        {
                            DT_CREATEDATE = DateTime.Now,
                            ST_CREATEUSER = currentUser.UserID,
                            IPAddress = CommonCode.GetIP(),
                            Comment = GetPurchaseContract(item.PurchaseStatus),
                        };
                        query_purchase.Purchase_ContractHistory.Add(model_history);

                        foreach (var item_batch in item.list_batch)
                        {
                            if (item.ContractType == (int)ContractTypeEnum.ProductFitting)
                            {
                                item_batch.DateStart = DateTime.Now;
                                item_batch.DateEnd = DateTime.Now;
                            }

                            DAL.Purchase_ContractBatch model_batch = new DAL.Purchase_ContractBatch()
                            {
                                DateStart = item_batch.DateStart,
                                DateEnd = item_batch.DateEnd,
                                Times = item_batch.Times,
                                BatchAmount = item_batch.BatchAmount,
                            };
                            if (item.ContractType == (int)ContractTypeEnum.Default)
                            {
                                if (item_batch.listProduct != null)
                                {
                                    foreach (var item_product in item_batch.listProduct)
                                    {
                                        DAL.Purchase_ContractProduct model_product = new DAL.Purchase_ContractProduct()
                                        {
                                            OrderProductID = item_product.ID,
                                            MixedMode = item_product.MixedMode,
                                            OtherComment = item_product.OtherComment,
                                            IsFragile = GetIsFragile(item_product.IsFragile),
                                            PackageName = item_product.PackageName,
                                            IsProductMixed = item_product.IsProductMixed,

                                            IsDelete = false,
                                            DT_CREATEDATE = DateTime.Now,
                                            DT_MODIFYDATE = DateTime.Now,
                                            ST_CREATEUSER = currentUser.UserID,
                                            ST_MODIFYUSER = currentUser.UserID,
                                            IPAddress = CommonCode.GetIP(),
                                        };
                                        model_batch.Purchase_ContractProduct.Add(model_product);
                                    }
                                }
                            }
                            query_purchase.Purchase_ContractBatch.Add(model_batch);
                        }
                        context.Purchase_Contract.Add(query_purchase);

                        affectRows = context.SaveChanges();

                        if (affectRows > 0)
                        {
                            ExecuteApproval(query_purchase.ST_CREATEUSER, query_purchase.ID, "", item.PurchaseStatus, currentUser.UserID, false);//执行审批流

                            SaveOther(currentUser, query_purchase.ID);
                            if (item.ContractType == (int)ContractTypeEnum.ProductFitting)
                            {
                                Save_ProductFitting(currentUser.UserID, query_purchase.ID, item);
                            }

                            Save_ProductMixed(currentUser.UserID, query_purchase.ID);
                        }
                    }
                }

                if (affectRows == 0)
                {
                    result.IsSuccess = false;
                }
                else
                {
                    result.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
                result.IsSuccess = false;
            }
            return result;
        }

        private static bool? GetIsFragile(int? val)
        {
            bool? IsFragile = null;
            switch (val)
            {
                case (int)CommonStatusEnum.Empty:
                    IsFragile = null;
                    break;

                case (int)CommonStatusEnum.Yes:
                    IsFragile = true;
                    break;

                case (int)CommonStatusEnum.No:
                    IsFragile = false;
                    break;

                default:
                    break;
            }

            return IsFragile;
        }

        private static int? GetIsFragile(bool? val)
        {
            if (val.HasValue)
            {
                if (val.Value)
                {
                    return 1;
                }
                else
                {
                    return 2;
                }
            }
            else
            {
                return 2;
            }
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
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForPurchase);

                    var dataFromDB = query.FirstOrDefault();
                    if (dataFromDB != null)
                    {
                        dataFromDB.Port = vm.Port.StrTrim();
                        dataFromDB.PaymentType = vm.PaymentType;
                        dataFromDB.IsImmediatelySend = vm.IsImmediatelySend;
                        dataFromDB.IsThirdAudits = vm.IsThirdAudits;
                        dataFromDB.IsThirdVerification = vm.IsThirdVerification;
                        dataFromDB.IsThirdTest = vm.IsThirdTest;
                        dataFromDB.IsThirdSampling = vm.IsThirdSampling;
                        dataFromDB.AfterDate = vm.AfterDate ?? 0;
                        dataFromDB.OtherFee = vm.OtherFee;
                        dataFromDB.AllAmount = vm.AllAmount;
                        dataFromDB.ST_MODIFYUSER = currentUser.UserID;
                        dataFromDB.DT_MODIFYDATE = DateTime.Now;

                        dataFromDB.DateStart = vm.DateStart;

                        if (dataFromDB.ContractType == (int)ContractTypeEnum.ProductFitting)
                        {
                            dataFromDB.AllQty = vm.AllQty;
                        }

                        if (vm.PurchaseStatus != (int)PurchaseStatusEnum.NotPassCheck && vm.PurchaseStatus != (int)PurchaseStatusEnum.PassedCheck)
                        {
                            dataFromDB.PurchaseStatus = vm.PurchaseStatus;
                        }
                        dataFromDB.ContractTerms = vm.ContractTerms;

                        string Comment = "";
                        if (vm.PurchaseStatus == (int)PurchaseStatusEnum.PassedCheck || vm.PurchaseStatus == (int)PurchaseStatusEnum.NotPassCheck)
                        {
                            Comment = vm.Comment;
                        }
                        dataFromDB.Comment = Comment;

                        context.Purchase_ContractHistory.Add(new Purchase_ContractHistory()
                        {
                            DT_CREATEDATE = DateTime.Now,
                            ST_CREATEUSER = currentUser.UserID,
                            IPAddress = CommonCode.GetIP(),
                            PurchaseContractID = vm.ID,
                            Comment = GetPurchaseContract(vm.PurchaseStatus),
                            CheckSuggest = vm.Comment,
                        });

                        foreach (var item in vm.list_batch)
                        {
                            var query_batch = dataFromDB.Purchase_ContractBatch.First();
                            query_batch.DateStart = item.DateStart;
                            query_batch.DateEnd = item.DateEnd;

                            if (dataFromDB.ContractType == (int)ContractTypeEnum.Default)
                            {
                                if (item.listProduct != null)
                                {
                                    foreach (var item_product in item.listProduct)
                                    {
                                        var query_product = context.Purchase_ContractProduct.Find(item_product.ID);
                                        query_product.MixedMode = item_product.MixedMode;
                                        query_product.OtherComment = item_product.OtherComment;
                                        query_product.PackageName = item_product.PackageName;
                                        query_product.IsFragile = GetIsFragile(item_product.IsFragile);

                                        query_product.ST_MODIFYUSER = currentUser.UserID;
                                        query_product.DT_MODIFYDATE = DateTime.Now;
                                    }
                                }
                            }
                            else
                            {
                                if (item.listProductFitting != null)
                                {
                                    foreach (var item_product in item.listProductFitting)
                                    {
                                        var query_product = context.ProductFittings.Find(item_product.ID);
                                        query_product.Qty = item_product.Qty;
                                        query_product.PackageName = item_product.PackageName;
                                        query_product.Comment = item_product.Comment;

                                        query_product.ST_MODIFYUSER = currentUser.UserID;
                                        query_product.DT_MODIFYDATE = DateTime.Now;
                                    }
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

                            ExecuteApproval(dataFromDB.ST_CREATEUSER, dataFromDB.ID, "", vm.PurchaseStatus, currentUser.UserID, false);//执行审批流

                            SaveOther(currentUser, dataFromDB.ID);

                            Save_ProductMixed(currentUser.UserID, dataFromDB.ID);
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
        /// 采购合同——审核已通过时，添加第三方验厂、第三方检测、生产计划、三期QC
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="id"></param>
        public void SaveOther(VMERPUser currentUser, int id)
        {
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var dataFromDB = context.Purchase_Contract.Find(id);
                    if (dataFromDB.PurchaseStatus == (int)PurchaseStatusEnum.PassedCheck)
                    {
                        if (dataFromDB.ContractType == (int)ContractTypeEnum.Default)
                        {
                            dataFromDB.PacksStatus = 1;//包装资料的状态
                            dataFromDB.EncasementStatus = 1;//出运的状态
                            dataFromDB.IsOutsourcing = false;//代购合同

                            #region 审核通过，并且需要第三方验厂

                            if (dataFromDB.IsThirdAudits)
                            {
                                DAL.ThirdParty_Inspection query_Inspection = new DAL.ThirdParty_Inspection()
                                {
                                    PurchaseID = dataFromDB.ID,
                                    StatusID = (int)InspectionStatusEnum.PendingInput,
                                    TypeID = (int)InspectionTypeEnum.AuditNotice,
                                    InspectionAuditFee = 0,
                                    InspectionDetectFee = 0,
                                    InspectionAuditFee_ForFactory = 0,
                                    InspectionDetectFee_ForFactory = 0,

                                    DT_CREATEDATE = DateTime.Now,
                                    ST_CREATEUSER = currentUser.UserID,
                                    DT_MODIFYDATE = DateTime.Now,
                                    ST_MODIFYUSER = currentUser.UserID,
                                    IsDelete = false,
                                    IPAddress = CommonCode.GetIP(),
                                };
                                context.ThirdParty_Inspection.Add(query_Inspection);
                            }

                            #endregion 审核通过，并且需要第三方验厂

                            #region 审核通过，并且需要第三方检测

                            if (dataFromDB.IsThirdTest)
                            {
                                DAL.ThirdParty_Inspection query_Inspection = new DAL.ThirdParty_Inspection()
                                {
                                    PurchaseID = dataFromDB.ID,
                                    StatusID = (int)InspectionStatusEnum.PendingInput,
                                    TypeID = (int)InspectionTypeEnum.DetectNotice,
                                    InspectionAuditFee = 0,
                                    InspectionDetectFee = 0,
                                    InspectionAuditFee_ForFactory = 0,
                                    InspectionDetectFee_ForFactory = 0,

                                    DT_CREATEDATE = DateTime.Now,
                                    ST_CREATEUSER = currentUser.UserID,
                                    DT_MODIFYDATE = DateTime.Now,
                                    ST_MODIFYUSER = currentUser.UserID,
                                    IsDelete = false,
                                    IPAddress = CommonCode.GetIP(),
                                };
                                context.ThirdParty_Inspection.Add(query_Inspection);
                            }

                            #endregion 审核通过，并且需要第三方检测

                            #region 审核通过，并且需要第三方抽检

                            if (dataFromDB.IsThirdSampling)
                            {
                                DAL.ThirdParty_Inspection query_Inspection = new DAL.ThirdParty_Inspection()
                                {
                                    PurchaseID = dataFromDB.ID,
                                    StatusID = (int)InspectionStatusEnum.PendingInput,
                                    TypeID = (int)InspectionTypeEnum.SamplingNotice,
                                    InspectionAuditFee = 0,
                                    InspectionDetectFee = 0,
                                    InspectionAuditFee_ForFactory = 0,
                                    InspectionDetectFee_ForFactory = 0,

                                    DT_CREATEDATE = DateTime.Now,
                                    ST_CREATEUSER = currentUser.UserID,
                                    DT_MODIFYDATE = DateTime.Now,
                                    ST_MODIFYUSER = currentUser.UserID,
                                    IsDelete = false,
                                    IPAddress = CommonCode.GetIP(),
                                };
                                context.ThirdParty_Inspection.Add(query_Inspection);
                            }

                            #endregion 审核通过，并且需要第三方抽检

                            #region 生产计划

                            DAL.Plan_ProducePlan query_ProductPlan = new DAL.Plan_ProducePlan()
                            {
                                PurchaseID = dataFromDB.ID,
                                OrderList = context.Orders.Find(dataFromDB.OrderID).OrderNumber,
                                Status = (int)ProducePlanStatusEnum.PendingUpload,

                                DT_CREATEDATE = DateTime.Now,
                                DT_MODIFYDATE = DateTime.Now,
                                ST_CREATEUSER = currentUser.UserID,
                                ST_MODIFYUSER = currentUser.UserID,
                                IPAdress = CommonCode.GetIP(),
                            };
                            context.Plan_ProducePlan.Add(query_ProductPlan);

                            #endregion 生产计划

                            #region 三期QC

                            DAL.Purchase_ThreeTimesQC query_ThreeTimesQC = new DAL.Purchase_ThreeTimesQC()
                            {
                                PurchaseContractID = dataFromDB.ID,
                                StatusID = (int)ThreeTimesQCStatusEnum.PendingMaintenance,

                                DT_CREATEDATE = DateTime.Now,
                                ST_CREATEUSER = currentUser.UserID,
                                DT_MODIFYDATE = DateTime.Now,
                                ST_MODIFYUSER = currentUser.UserID,
                                IPAddress = CommonCode.GetIP(),
                            };
                            context.Purchase_ThreeTimesQC.Add(query_ThreeTimesQC);

                            #endregion 三期QC

                            context.SaveChanges();
                        }
                        else
                        {
                            #region 添加配件产品的单证索引

                            var query_Purchase_Contract = context.Purchase_Contract.Where(d => d.OrderID == dataFromDB.OrderID && !d.IsDelete && d.ContractType == (int)ContractTypeEnum.ProductFitting && d.PurchaseStatus < (int)PurchaseStatusEnum.PassedCheck);
                            if (query_Purchase_Contract.Count() == 0)
                            {
                                var query_temp = context.DocumentsIndexings.Where(d => d.OrderID == dataFromDB.OrderID && !d.IsDelete && d.DocumentsIndexingType == 1);
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

                                        OrderID = dataFromDB.OrderID ?? 0,
                                        StatusID = (int)DocumentsIndexingStatusEnum.PendingMaintenance,
                                        DocumentsIndexingType = (int)DocumentsIndexingTypeEnum.ProductFitting,
                                    };
                                    context.DocumentsIndexings.Add(query_DocumentsIndexing);

                                    context.SaveChanges();
                                }
                            }

                            #endregion 添加配件产品的单证索引
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
        }

        /// <summary>
        /// 添加产品配件
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="OrderID"></param>
        private void Save_ProductFitting(int UserID, int ContractID, VMPurchase vm)
        {
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Purchase_Contract.Find(ContractID);
                    if (query != null)
                    {
                        var ContractBatchID = query.Purchase_ContractBatch.First().ID;
                        foreach (var item in vm.list_batch)
                        {
                            foreach (var item2 in item.listProductFitting)
                            {
                                var ProductFittings = new List<DAL.ProductFitting>();

                                DAL.ProductFitting query_ProductFitting = context.ProductFittings.Find(item2.ID);

                                query_ProductFitting.ParentID = ContractBatchID;
                                query_ProductFitting.ModuleType = (int)ModuleTypeEnum.PurchaseContract;

                                query_ProductFitting.Qty = item2.Qty;
                                query_ProductFitting.PackageName = item2.PackageName;
                                query_ProductFitting.Comment = item2.Comment;

                                query_ProductFitting.DT_CREATEDATE = DateTime.Now;
                                query_ProductFitting.ST_CREATEUSER = UserID;
                                query_ProductFitting.DT_MODIFYDATE = DateTime.Now;
                                query_ProductFitting.ST_MODIFYUSER = UserID;
                                query_ProductFitting.Deleted = false;
                                context.ProductFittings.Add(query_ProductFitting);
                            }
                        }
                    }

                    context.SaveChanges();
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
        private void Save_ProductMixed(int UserID, int PurchaseContractID)
        {
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    #region 添加混装产品

                    var query2 = context.Purchase_ContractBatch.Where(d => d.PurchaseContractID == PurchaseContractID);
                    if (query2 != null && query2.Count() > 0)
                    {
                        List<DAL.Purchase_ContractProduct> list_Product = new List<Purchase_ContractProduct>();
                        foreach (var item in query2)
                        {
                            foreach (var item2 in item.Purchase_ContractProduct.Where(d => !d.IsDelete && d.IsProductMixed && !d.ParentProductMixedID.HasValue))
                            {
                                var queryProductMixed = context.OrderProducts.Where(d => !d.IsDelete && d.IsProductMixed && d.ParentProductMixedID == item2.OrderProductID);
                                if (queryProductMixed != null)
                                {
                                    foreach (var item3 in queryProductMixed)
                                    {
                                        DAL.Purchase_ContractProduct product = new Purchase_ContractProduct();
                                        product.OrderProductID = item3.ID;
                                        product.PurchaseContractBatchID = item2.PurchaseContractBatchID;
                                        product.IsDelete = false;
                                        product.DT_CREATEDATE = DateTime.Now;
                                        product.ST_CREATEUSER = UserID;
                                        product.DT_MODIFYDATE = DateTime.Now;
                                        product.ST_MODIFYUSER = UserID;
                                        product.IPAddress = CommonCode.GetIP();
                                        product.Comment = item2.Comment;
                                        product.PackageName = item2.PackageName;
                                        product.MixedMode = item2.MixedMode;
                                        product.OtherComment = item2.OtherComment;
                                        product.IsFragile = item2.IsFragile;
                                        product.CarbinetType = item2.CarbinetType;
                                        product.IsProductMixed = item2.IsProductMixed;
                                        product.ParentProductMixedID = item2.ID;

                                        list_Product.Add(product);
                                    }
                                }
                            }

                            //删除混装产品
                            var listProductsMixed_Delete = context.Purchase_ContractProduct.Where(d => d.IsProductMixed && d.ParentProductMixedID.HasValue && d.PurchaseContractBatchID == item.ID);
                            if (listProductsMixed_Delete.Count() > 0)
                            {
                                context.Purchase_ContractProduct.RemoveRange(listProductsMixed_Delete);
                            }
                        }
                        if (list_Product.Count > 0)
                        {
                            context.Purchase_ContractProduct.AddRange(list_Product);
                            context.SaveChanges();
                        }
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
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public VMPurchase GetDetailByID(VMERPUser currentUser, int id, int ProductMixed_Type = 0)
        {
            VMPurchase vm = null;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Purchase_Contract.Where(p => p.ID == id && !p.IsDelete);
                    //query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForPurchase);

                    var dataFromDB = query.FirstOrDefault();
                    if (dataFromDB != null)
                    {
                        var query_batch = dataFromDB.Purchase_ContractBatch;
                        List<VMPurchaseBatch> list_batch = new List<VMPurchaseBatch>();
                        List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                        List<DAL.SystemUser> list_SystemUser = context.SystemUsers.ToList();
                        string CurrentSign = "";
                        foreach (var item_batch in query_batch)
                        {
                            List<VMPurchaseProduct> list_product = new List<VMPurchaseProduct>();
                            List<VMProductFittingInfo> listProductFitting = new List<VMProductFittingInfo>();
                            if (dataFromDB.ContractType == (int)ContractTypeEnum.Default)
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

                                    string SeasonZhName = "";
                                    if (item_product.OrderProduct.Season.HasValue)
                                    {
                                        SeasonZhName = _dictionaryServices.GetDictionary_SeasonZhName(item_product.OrderProduct.Season, list_Com_DataDictionary);
                                    }
                                    list_product.Add(new VMPurchaseProduct()
                                    {
                                        ID = item_product.ID,
                                        OrderProductID = item_product.OrderProductID,
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
                                        IsFragile = GetIsFragile(item_product.IsFragile),
                                        Department = item_product.OrderProduct.Department,
                                        DepartmentName = _dictionaryServices.GetDictionaryByName(item_product.OrderProduct.Department, list_Com_DataDictionary),
                                        Season = item_product.OrderProduct.Season,
                                        SeasonZhName = SeasonZhName,
                                        SkuCode = item_product.OrderProduct.SkuCode,
                                        IsProductMixed = item_product.OrderProduct.IsProductMixed,
                                    });
                                }
                            }
                            else
                            {
                                var ParentProductID = item_batch.ID;
                                var CurrencyType = context.ProductFittings.Where(d => d.ParentID == ParentProductID && !d.Deleted && d.ModuleType == (int)ModuleTypeEnum.PurchaseContract).First().CurrencyType;
                                CurrentSign = _dictionaryServices.GetDictionary_CurrencySign(CurrencyType, list_Com_DataDictionary);

                                listProductFitting = _productFittingServices.GetProductFittingInfo(currentUser, item_batch.ID, (int)ModuleTypeEnum.PurchaseContract);
                            }

                            list_batch.Add(new VMPurchaseBatch()
                            {
                                DateStart = item_batch.DateStart,
                                DateEnd = item_batch.DateEnd,
                                Times = item_batch.Times,
                                BatchAmount = item_batch.BatchAmount,
                                listProduct = list_product,
                                listProductFitting = listProductFitting,
                                DateStartForamt = Utils.DateTimeToStr(item_batch.DateStart),
                            });
                        }

                        #region 获取历史记录列表

                        var query_history = dataFromDB.Purchase_ContractHistory;
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

                        #endregion 获取历史记录列表

                        var query_order = context.Orders.Find(dataFromDB.OrderID);
                        List<string> list_ToEmailAddress, list_CallName;
                        Get_ToEmailAddress_CallName(context, dataFromDB, out list_ToEmailAddress, out list_CallName);

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
                            PurchaseStatus = dataFromDB.PurchaseStatus,
                            PortName = _dictionaryServices.GetDictionary_PortName(dataFromDB.PortID, list_Com_DataDictionary),
                            Port = dataFromDB.Port == null ? null : dataFromDB.Port.StrTrim(),
                            PaymentType = dataFromDB.PaymentType == null ? null : dataFromDB.PaymentType.StrTrim(),
                            CustomerCode = dataFromDB.Orders_Customers.CustomerCode,
                            IsImmediatelySend = dataFromDB.IsImmediatelySend,
                            IsThirdVerification = dataFromDB.IsThirdVerification,
                            IsThirdAudits = dataFromDB.IsThirdAudits,
                            IsThirdTest = dataFromDB.IsThirdTest,
                            IsThirdSampling = dataFromDB.IsThirdSampling,
                            Comment = dataFromDB.Comment,
                            ContractTerms = dataFromDB.ContractTerms,

                            list_batch = list_batch,
                            list_history = list_history,
                            list_UpLoadFile = ConstsMethod.GetUploadFileList(id, UploadFileType.PurchaseContract),
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
                            CustomerID = dataFromDB.CustomerID,
                            list_ToEmailAddress = CommonCode.ListToString(list_ToEmailAddress, ";"),
                            list_CallName = CommonCode.ListToString(list_CallName),
                            EmailSign = context.SystemUsers.Find(currentUser.UserID).EmailSign,
                            SelectCustomer = dataFromDB.Orders_Customers.SelectCustomer,
                            ContractType = dataFromDB.ContractType,
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
        /// 获取收件人、收件人姓名
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dataFromDB"></param>
        /// <param name="list_ToEmailAddress"></param>
        /// <param name="list_CallName"></param>
        public static void Get_ToEmailAddress_CallName(ERPEntitiesNew context, Purchase_Contract dataFromDB, out List<string> list_ToEmailAddress, out List<string> list_CallName)
        {
            //如果发给办事处，就显示办事处内勤人名即可。黄岩办要发给生产部内勤，不是开发部内勤。办事处都有这个角色账号。
            //如果发给工厂，就显示工厂的联系人，如果联系人是厂长，需要在人名后增加厂长的称呼。
            list_ToEmailAddress = new List<string>();
            list_CallName = new List<string>();
            if (dataFromDB.Factory.Hierarchy.HasValue)//有跟单人
            {
                var query_SystemUsers_temp = dataFromDB.Factory.Hierarchy1.SystemUsers;

                if (dataFromDB.Factory.Hierarchy1.Name.Contains("黄岩办"))
                {
                    query_SystemUsers_temp = context.SystemUsers.Where(d => d.Hierarchy.Name.Contains("黄岩办")).ToList();
                }

                foreach (var item in query_SystemUsers_temp)
                {
                    string rowName = "办事处内勤";
                    if (item.Hierarchy.Name.Contains("黄岩办"))
                    {
                        rowName = "生产部内勤";
                    }
                    if (item.UserRoles.Where(d => d.ERPRole.Name == rowName).Count() > 0)
                    {
                        list_ToEmailAddress.Add(item.Email);
                        list_CallName.Add(item.UserName);
                    }
                }
            }

            if (list_ToEmailAddress == null || list_ToEmailAddress.Count == 0)
            {
                if (dataFromDB.Factory.Province == "上海" || dataFromDB.Factory.Province == "江苏" || dataFromDB.Factory.Province == "福建" || dataFromDB.Factory.Province == "广东")
                {
                    list_ToEmailAddress.Add(dataFromDB.Factory.EmailAdress);
                    string CallPeople = dataFromDB.Factory.CallPeople;
                    if (dataFromDB.Factory.Duty == "厂长")
                    {
                        CallPeople += " 厂长";
                    }
                    list_CallName.Add(CallPeople);
                }
            }
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
                        FileName_Description = "合同附件",
                        UpLoadFileList = ConstsMethod.GetUploadFileList(id, UploadFileType.PurchaseContract),
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
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForPurchase);
                    var dataFromDB = query.FirstOrDefault();
                    if (dataFromDB != null)
                    {
                        dataFromDB.PurchaseStatus = (int)PurchaseStatusEnum.ContractUploaded;
                        dataFromDB.ST_MODIFYUSER = currentUser.UserID;
                        dataFromDB.DT_MODIFYDATE = DateTime.Now;

                        context.Purchase_ContractHistory.Add(new Purchase_ContractHistory()
                        {
                            DT_CREATEDATE = DateTime.Now,
                            ST_CREATEUSER = currentUser.UserID,
                            IPAddress = CommonCode.GetIP(),
                            PurchaseContractID = vm.ID,
                            Comment = GetPurchaseContract((int)PurchaseStatusEnum.ContractUploaded),
                        });

                        ConstsMethod.SaveFileUpload(currentUser, vm.ID, vm.UpLoadFileList, context, UploadFileType.PurchaseContract);

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
        /// 发送合同
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public VMAjaxProcessResult SendContract(VMERPUser currentUser, int id, VMSendEmail vm)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var dataFromDB = context.Purchase_Contract.Find(id);
                    if (dataFromDB != null)
                    {
                        dataFromDB.PurchaseStatus = (int)PurchaseStatusEnum.ContractSent;
                        dataFromDB.ST_MODIFYUSER = currentUser.UserID;
                        dataFromDB.DT_MODIFYDATE = DateTime.Now;
                        dataFromDB.IPAddress = CommonCode.GetIP();

                        context.Purchase_ContractHistory.Add(new Purchase_ContractHistory()
                        {
                            DT_CREATEDATE = DateTime.Now,
                            ST_CREATEUSER = currentUser.UserID,
                            IPAddress = CommonCode.GetIP(),
                            PurchaseContractID = id,
                            Comment = GetPurchaseContract((int)PurchaseStatusEnum.ContractSent),
                        });

                        int affectRows = context.SaveChanges();
                        if (affectRows == 0)
                        {
                            result.IsSuccess = false;
                            result.Msg = "发送合同失败了！";
                            return result;
                        }
                        else
                        {
                            result.IsSuccess = true;

                            if (vm.StatusID == 2)
                            {
                                List<string> list_Attachs = new List<string>();
                                var vm2 = GetDetailByID(currentUser, id, 1);
                                int i = 0;

                                foreach (var item in vm2.list_batch)
                                {
                                    foreach (var item2 in item.listProduct)
                                    {
                                        i++;
                                    }
                                }

                                if (vm.UpLoadFileList != null)
                                {
                                    foreach (var item in vm.UpLoadFileList)
                                    {
                                        if (!item.IsDelete)
                                        {
                                            string ServerFileName = ConstsMethod.ReplaceURLToLocalPath(item.ServerFileName);
                                            if (File.Exists(ServerFileName))
                                            {
                                                string di_path = Utils.GetMapPath("/data/Template/Out/PurchaseContract/" + vm2.ID);
                                                DirectoryInfo di = new DirectoryInfo(di_path);
                                                if (!di.Exists)
                                                {
                                                    di.Create();
                                                }
                                                string new_ServerFileName = di_path + "\\" + item.DisplayFileName;
                                                if (!File.Exists(new_ServerFileName))
                                                {
                                                    File.Copy(ServerFileName, new_ServerFileName);
                                                }
                                                list_Attachs.Add(new_ServerFileName);
                                            }
                                        }
                                    }
                                }

                                if (vm.IsContainMakerExcel)
                                {
                                    string filePath = MakeExcel(currentUser, id, "xls", vm2);//生成采购合同的xls
                                    list_Attachs.Add(Utils.GetMapPath(filePath));
                                }

                                if (vm.IsContainMakerExcel_pdf)
                                {
                                    string filePath = MakeExcel(currentUser, id, "pdf", vm2);//生成采购合同的PDF
                                    list_Attachs.Add(Utils.GetMapPath(filePath));
                                }

                                result = Email.SendEmail(vm.ToAddress, vm.Subject, vm.BodyContent, list_Attachs, MailType.PurchaseContract, currentUser, vm.CcAddress, vm.BccAddress);

                                //Email.SendMail(currentUser.UserName, vm);//发送电子邮件
                            }
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
        /// 生成采购合同的PDF
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public string MakeExcel(VMERPUser currentUser, int id, string extension)
        {
            var vm = GetDetailByID(currentUser, id, 1);
            return MakeExcel(currentUser, id, extension, vm);
        }

        public string MakeExcel(VMERPUser currentUser, int id, string extension, VMPurchase vm)
        {
            MakerTypeEnum MakerTypeEnum = MakerTypeEnum.PurchaseContract;
            if (vm.ContractType == (int)ContractTypeEnum.ProductFitting)
            {
                MakerTypeEnum = MakerTypeEnum.PurchaseContract_ProductFitting;
            }
            string xlsPath = MakerExcel.Maker(vm, MakerTypeEnum, id.ToString());
            if (extension == "xls")
            {
                return xlsPath;
            }
            else if (extension == "pdf")
            {
                string pdfPath = xlsPath.Replace(".xls", ".pdf");

                AsposeX asposeX = new AsposeX();
                string errMsg;

                //生成pdf文件
                asposeX.ExcelToPdf(Utils.GetMapPath(xlsPath), Utils.GetMapPath(pdfPath), out errMsg);

                return pdfPath;
            }
            return "";
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
            if (StatusID == (int)PurchaseStatusEnum.NotPassCheck)
            {
                isPass = false;
            }

            List<int> validWaitingApproveStatus = new List<int>(){
                            (int)PurchaseStatusEnum.PendingCheck,
                            (int)PurchaseStatusEnum.NotPassCheck
                        };
            ApprovalService.ExcuteApproval(new ApprovalInfo
            {
                WorkflowType = WorkflowTypes.ApprovalPurchaseContract,
                IsPass = isPass,
                IdentityID = identityID,
                ValidWaitingApproveStatus = validWaitingApproveStatus,
                StatusApproving = (int)PurchaseStatusEnum.PendingCheck,
                StatusNextTo = (int)PurchaseStatusEnum.PassedCheck,
                StatusRejected = (int)PurchaseStatusEnum.NotPassCheck,
                ApproveOpinion = CheckSuggest,
                ApproveUserID = UserID,
                LogMethod = () =>
                {
                    return null;
                }
            });
        }

        /// <summary>
        /// 获取上传附件的详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public VMUpLoad GetUploadDetial_MakeMoney(int id)
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
                        FileName_Description = "合同附件",
                        UpLoadFileList = ConstsMethod.GetUploadFileList(id, UploadFileType.PurchaseContract_MakeMoney),
                        SelectCustomer = query.Orders_Customers.SelectCustomer,
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
        public DBOperationStatus SaveUpLoad_MakeMoney(VMERPUser currentUser, VMUpLoad vm)
        {
            DBOperationStatus result = default(DBOperationStatus);
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Purchase_Contract.Where(p => p.ID == vm.ID);
                    var dataFromDB = query.FirstOrDefault();
                    if (dataFromDB != null)
                    {
                        dataFromDB.ST_MODIFYUSER = currentUser.UserID;
                        dataFromDB.DT_MODIFYDATE = DateTime.Now;

                        context.Purchase_ContractHistory.Add(new Purchase_ContractHistory()
                        {
                            DT_CREATEDATE = DateTime.Now,
                            ST_CREATEUSER = currentUser.UserID,
                            IPAddress = CommonCode.GetIP(),
                            PurchaseContractID = vm.ID,
                            Comment = "上传请款合同",
                        });

                        ConstsMethod.SaveFileUpload(currentUser, vm.ID, vm.UpLoadFileList, context, UploadFileType.PurchaseContract_MakeMoney);

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

        #endregion UserMethod
    }
}