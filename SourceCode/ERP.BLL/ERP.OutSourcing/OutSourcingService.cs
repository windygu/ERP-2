using ERP.BLL.Consts;
using ERP.BLL.ERP.Consts;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.Contacts;
using ERP.Models.CustomEnums;
using ERP.Models.OutSourcing;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ERP.BLL.ERP.OutSourcing
{
    public class OutSourcingService
    {
        private ERP.Dictionary.DictionaryServices _dictionaryServices = new Dictionary.DictionaryServices();

        /// <summary>
        /// 获取代印合同的值
        /// </summary>
        /// <param name="i">编号</param>
        /// <returns></returns>
        public static string GetOutContract(int id)
        {
            return CommonCode.GetStatusEnumName(id, typeof(OutContractStatusEnum));
        }

        /// <summary>
        /// 用筛选条件查询待审核代印合同
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="sortColumnsNames"></param>
        /// <param name="sortColumnOrders"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRows"></param>
        /// <param name="vmFilter">筛选条件对象</param>
        /// <returns></returns>
        public List<DTOOutsourcing> GetAll(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows, VMFilterOC vmFilter)
        {
            List<DTOOutsourcing> objList = new List<DTOOutsourcing>();
            totalRows = 0;

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    #region 筛选条件

                    var query = context.Purchase_OutContracts.Where(p => (bool)!p.IsDelete);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForOutsourcing);

                    if (currentUser.HierachyType == HierachyType.Agency)
                    {
                        var userHierachy = new List<int?>()
                        {
                            currentUser.HierachyID,
                            context.SystemUsers.FirstOrDefault(p => p.HierarchyID == currentUser.HierachyID).Hierarchy.Hierarchy2.HierarchyID
                        };

                        query = query.Where(p => userHierachy.Contains(p.Factory.Hierarchy));
                    }

                    if (vmFilter.PageType == PageTypeEnum.PendingCheckList)
                    {
                        if (vmFilter.OutContractStatusID == 0)
                        {
                            query = query.Where(p => p.OutContractStatus <= (int)OutContractStatusEnum.NotPassCheck);
                        }
                        else
                        {
                            query = query.Where(p => p.OutContractStatus == vmFilter.OutContractStatusID);
                        }
                    }
                    else
                    {
                        if (vmFilter.OutContractStatusID == 0)
                        {
                            query = query.Where(p => p.OutContractStatus > (int)OutContractStatusEnum.NotPassCheck);
                        }
                        else
                        {
                            query = query.Where(p => p.OutContractStatus == vmFilter.OutContractStatusID);
                        }
                    }

                    if (!string.IsNullOrEmpty(vmFilter.PurchaseNumber))
                    {
                        query = query.Where(p => p.Purchase_Contract.PurchaseNumber.Contains(vmFilter.PurchaseNumber));
                    }
                    if (!string.IsNullOrEmpty(vmFilter.CustomerCode))
                    {
                        query = query.Where(p => p.Purchase_Contract.Orders_Customers.CustomerCode == vmFilter.CustomerCode);
                    }

                    if (!string.IsNullOrEmpty(vmFilter.FactoryAbbreviation))
                    {
                        query = query.Where(p => p.Factory.Abbreviation.Contains(vmFilter.FactoryAbbreviation) && p.Factory.DataFlag == 1);
                    }
                    if (!string.IsNullOrEmpty(vmFilter.PurchaseDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(vmFilter.PurchaseDateStart);
                        query = query.Where(p => p.Purchase_Contract.PurchaseDate >= dt);
                    }
                    if (!string.IsNullOrEmpty(vmFilter.PurchaseDateEnd))
                    {
                        DateTime dt = CommonCode.GetDateEnd(vmFilter.PurchaseDateEnd);
                        query = query.Where(p => p.Purchase_Contract.PurchaseDate <= dt);
                    }
                    if (!string.IsNullOrEmpty(vmFilter.DeliveryDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(vmFilter.DeliveryDateStart);
                        query = query.Where(p => p.DeliveryDateStart >= dt);
                    }
                    if (!string.IsNullOrEmpty(vmFilter.DeliveryDateEnd))
                    {
                        DateTime dt = Utils.StrToDateTime(vmFilter.DeliveryDateEnd);
                        query = query.Where(p => p.DeliveryDateEnd <= dt);
                    }

                    if (!string.IsNullOrEmpty(vmFilter.OutCompany))
                    {
                        query = query.Where(p => p.Factory.Abbreviation.Contains(vmFilter.OutCompany) && p.Factory.DataFlag == 2);
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
                        query = query.OrderByDescending(p => p.UpdateDate);
                    }

                    #endregion 排序

                    var dataFromDB = query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                    if (dataFromDB != null)
                    {
                        DTOOutsourcing vmOutContractInfo = new DTOOutsourcing();

                        foreach (var entity in dataFromDB)
                        {
                            vmOutContractInfo = new DTOOutsourcing();
                            vmOutContractInfo = GetDTOOutsourcingFromDB(entity, vmOutContractInfo);

                            //当加载待审核页面数据列表时，获取审批流权限值，以便前台判断用户是否具有审批数据行的按钮权限
                            if (vmFilter.PageType == PageTypeEnum.PendingCheckList)
                            {
                                vmOutContractInfo.IsCanAudit = Workflow.ApprovalService.HasApprovalPermission(WorkflowTypes.ApprovalOutsourcingContract, currentUser, entity.CreateUserID, entity.ApproverIndex, entity.Purchase_Contract.CustomerID);

                                if (vmOutContractInfo.OutContractStatusID == (int)OutContractStatusEnum.PendingCheck)
                                {
                                    vmOutContractInfo.NextApproverInfos = Workflow.ApprovalService.GetNextApproverInfos(WorkflowTypes.ApprovalOutsourcingContract, entity.CreateUserID, entity.ApproverIndex, entity.Purchase_Contract.CustomerID);
                                }
                            }

                            objList.Add(vmOutContractInfo);
                        }

                        totalRows = objList.Count();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return objList;
        }

        /// <summary>
        /// 装载代印合同至VM中，供查询列表数据显示用
        /// </summary>
        /// <param name="dbOutContractsInfo">采购合同->包装资料VM</param>
        /// <returns></returns>
        private DTOOutsourcing GetDTOOutsourcingFromDB(Purchase_OutContracts dbOutContractsInfo, DTOOutsourcing vmOutContractInfo)
        {
            vmOutContractInfo.ID = dbOutContractsInfo.ID;
            vmOutContractInfo.OutContracNo = dbOutContractsInfo.OutContracNo;
            vmOutContractInfo.PurchaseNumber = dbOutContractsInfo.Purchase_Contract.PurchaseNumber;
            vmOutContractInfo.CustomerCode = dbOutContractsInfo.Purchase_Contract.Orders_Customers.CustomerCode;
            vmOutContractInfo.PurchaseAmount = dbOutContractsInfo.Purchase_Contract.AllAmount;
            vmOutContractInfo.PurchaseDate = Utils.DateTimeToStr(dbOutContractsInfo.Purchase_Contract.PurchaseDate);
            vmOutContractInfo.PurchaseDateStart = Utils.DateTimeToStr(dbOutContractsInfo.Purchase_Contract.DateStart);
            vmOutContractInfo.PurchaseDateEnd = Utils.DateTimeToStr(dbOutContractsInfo.Purchase_Contract.DateEnd);
            vmOutContractInfo.FactoryAbbreviation = dbOutContractsInfo.Purchase_Contract.Factory.Abbreviation;
            if (dbOutContractsInfo.DeliveryDateStart.HasValue)
            {
                vmOutContractInfo.DeliveryDateStart = Utils.DateTimeToStr((DateTime)dbOutContractsInfo.DeliveryDateStart);
            }
            if (dbOutContractsInfo.DeliveryDateEnd.HasValue)
            {
                vmOutContractInfo.DeliveryDateEnd = Utils.DateTimeToStr((DateTime)dbOutContractsInfo.DeliveryDateEnd);
            }
            vmOutContractInfo.OutContractSum = dbOutContractsInfo.OutContractSum ?? 0;
            vmOutContractInfo.OutContractSum += dbOutContractsInfo.OthersFee;
            vmOutContractInfo.OutCompany = dbOutContractsInfo.Factory.Abbreviation;
            vmOutContractInfo.OutContractStatusID = dbOutContractsInfo.OutContractStatus ?? 0;
            vmOutContractInfo.OutContractStatus = GetOutContract(dbOutContractsInfo.OutContractStatus ?? 0);
            vmOutContractInfo.DT_MODIFYDATEFormatter = Utils.DateTimeToStr2(dbOutContractsInfo.UpdateDate);

            return vmOutContractInfo;
        }

        /// <summary>
        /// 根据筛选条件查询审核通过的采购合同，用于生成代印合同
        /// </summary>
        /// <param name="vmOuts"></param>
        /// <returns></returns>
        public List<VMPCDataList> GetAll(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows, DTOOutsourcing vmOuts)
        {
            List<VMPCDataList> vmpcdtList = new List<VMPCDataList>();
            VMPCDataList vmpcdt = new VMPCDataList();
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    //默认查询条件：采购合同：1=没有删除；2=包装资料状态为审核已通过；3=包装资料中的标签需要代购
                    var query = context.Purchase_Contract.Where(d => d.IsDelete == false && d.PacksStatus >= (int)PurchasePacksStatusEnum.PassedCheck && (bool)d.IsOutsourcing && d.ContractType == (int)ContractTypeEnum.Default);

                    query = query.Where(d => d.Purchase_OutContracts.Where(dd => (bool)!dd.IsDelete).Count() == 0);

                    #region 筛选条件

                    if (!string.IsNullOrEmpty(vmOuts.PurchaseNumber))
                    {
                        query = query.Where(p => p.PurchaseNumber.Contains(vmOuts.PurchaseNumber));
                    }
                    if (vmOuts.OCID > 0)
                    {
                        query = query.Where(p => p.Orders_Customers.OCID == vmOuts.OCID);
                    }

                    if (!string.IsNullOrEmpty(vmOuts.FactoryAbbreviation))
                    {
                        query = query.Where(p => p.Factory.Abbreviation.Contains(vmOuts.FactoryAbbreviation) && p.Factory.DataFlag == 1);
                    }
                    if (!string.IsNullOrEmpty(vmOuts.PurchaseDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(vmOuts.PurchaseDateStart);
                        query = query.Where(p => p.PurchaseDate >= dt);
                    }
                    if (!string.IsNullOrEmpty(vmOuts.PurchaseDateEnd))
                    {
                        DateTime dt = Utils.StrToDateTime(vmOuts.PurchaseDateEnd);
                        query = query.Where(p => p.PurchaseDate <= dt);
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

                    totalRows = query.Count();

                    var dataFromDB = query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

                    if (dataFromDB != null)
                    {
                        foreach (var p in dataFromDB)
                        {
                            vmpcdt = new VMPCDataList() { };
                            vmpcdt.PCID = p.ID;
                            vmpcdt.PurchaseNumber = p.PurchaseNumber;
                            vmpcdt.FactoryAbbreviation = p.Factory.Abbreviation;
                            vmpcdt.CustomerCode = p.Orders_Customers.CustomerCode;
                            vmpcdt.PurchaseDate = Utils.DateTimeToStr((DateTime)p.PurchaseDate);
                            vmpcdt.PurchaseAmount = p.AllAmount;
                            //vmpcdt.TagName=p.Purchase_Packs
                            vmpcdtList.Add(vmpcdt);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteError(e);
            }

            return vmpcdtList;
        }

        /// <summary>
        /// 从DB中取得采购、代印合同相关数据
        /// </summary>
        /// <param name="currentUser">登录信息</param>
        /// <param name="vmOuts">代印合同视图模型</param>
        /// <returns></returns>
        public DTOOutsourcing GetDetailByID(VMERPUser currentUser, DTOOutsourcing vmOuts)
        {
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var dtDataDictionary = context.Com_DataDictionary.Where(p => p.IsDelete == 0).ToList();//取数据字典表的全部数据，供下面关联使用

                    //新建代印合同：
                    if (vmOuts.PageTypeID == 1)
                    {
                        var query = context.Purchase_Contract.Where(d => d.ID == vmOuts.ID && d.ContractType == (int)ContractTypeEnum.Default);
                        var dbContract = query.FirstOrDefault();

                        if (dbContract != null)
                        {
                            //上部静态显示项
                            vmOuts.ContractsID = dbContract.ID;
                            vmOuts.PurchaseNumber = dbContract.PurchaseNumber;
                            vmOuts.PurchaseDate = Utils.DateTimeToStr(dbContract.PurchaseDate);
                            vmOuts.PurchaseDateStart = Utils.DateTimeToStr(dbContract.DateStart);
                            vmOuts.PurchaseDateEnd = Utils.DateTimeToStr(dbContract.DateEnd);
                            vmOuts.CustomerCode = dbContract.Orders_Customers.CustomerCode;
                            vmOuts.FactoryAbbreviation = dbContract.Factory.Abbreviation;

                            //查询采购合同对应的所有标签及其产品信息，然后以标签记录分组
                            List<DTOOCPacksData> vmocpList = new List<DTOOCPacksData>();
                            List<DTOOCPacksProductData> vmocpproList = new List<DTOOCPacksProductData>();
                            DTOOCPacksData vmocp = new DTOOCPacksData();
                            DTOOCPacksProductData vmocppro = new DTOOCPacksProductData();

                            decimal dAmount = 0, dOutContractSum = 0;

                            foreach (var item1 in dbContract.Purchase_Packs)
                            {
                                if (item1.IsOutsourcing && !item1.isDelete)
                                {
                                    dAmount = 0;
                                    vmocp = new DTOOCPacksData();

                                    //vmocp.ID = item1.ID;
                                    vmocp.PacksID = item1.ID;
                                    vmocp.TagID = item1.TagID;

                                    var dictinaryInfo = dtDataDictionary.Where(p => p.Code == item1.TagID).FirstOrDefault();
                                    if (dictinaryInfo != null)
                                    {
                                        vmocp.TagName = dictinaryInfo.Name;
                                    }
                                    vmocp.ContractsID = item1.ContractsID;

                                    vmocpproList = new List<DTOOCPacksProductData>();

                                    foreach (var item2 in item1.Purchase_PackProducts)
                                    {
                                        vmocppro = new DTOOCPacksProductData();

                                        vmocppro.PackProductsID = item2.ID;
                                        vmocppro.OrderProductID = item2.OrderProductID;
                                        vmocppro.PacksID = item2.PacksID;
                                        vmocppro.ProductID = item2.OrderProduct.ProductID;
                                        vmocppro.ProductImage = item2.OrderProduct.Image;
                                        vmocppro.OrderProductNO = item2.OrderProduct.No;
                                        vmocppro.OrderProductFPrice = item2.OrderProduct.SalePrice ?? 0;
                                        vmocppro.ProductTagsNumber = item2.OrderProduct.Qty;
                                        vmocppro.ProductTagsAmount = vmocppro.OrderProductFPrice * vmocppro.ProductTagsNumber;
                                        dAmount += vmocppro.ProductTagsAmount;
                                        vmocppro.TagDescribe = item1.TagDescribe;

                                        vmocpproList.Add(vmocppro);
                                    }
                                    dOutContractSum += dAmount;
                                    vmocp.TagProductsAmount = dAmount;
                                    vmocp.OCPacksProducts = vmocpproList;

                                    vmocp.OCClasue = CommonCode.GetOutsourcingDefaultContract();
                                    vmocpList.Add(vmocp);
                                }
                                vmOuts.OCPacksData = vmocpList;
                                vmOuts.OutContractSum = dOutContractSum;
                                vmOuts.OutContractSumText = dOutContractSum;
                            }
                        }
                    }
                    else //查看、编辑、审核代印合同
                    {
                        var query = context.Purchase_OutContracts.Where(p => p.ID == vmOuts.ID);
                        var dbOutContract = query.FirstOrDefault();

                        vmOuts.OutContracNo = dbOutContract.OutContracNo;
                        vmOuts.ID = dbOutContract.ID;
                        vmOuts.ContractsID = dbOutContract.ContractsID;
                        vmOuts.PurchaseNumber = dbOutContract.Purchase_Contract.PurchaseNumber;
                        vmOuts.PurchaseDate = Utils.DateTimeToStr(dbOutContract.Purchase_Contract.PurchaseDate);
                        vmOuts.PurchaseDateStart = Utils.DateTimeToStr(dbOutContract.Purchase_Contract.DateStart);
                        vmOuts.PurchaseDateEnd = Utils.DateTimeToStr(dbOutContract.Purchase_Contract.DateEnd);
                        vmOuts.CustomerCode = dbOutContract.Purchase_Contract.Orders_Customers.CustomerCode;
                        vmOuts.FactoryAbbreviation = dbOutContract.Purchase_Contract.Factory.Abbreviation;

                        if (dbOutContract.CreateDate.HasValue)
                        {
                            vmOuts.CreateDate = Utils.DateTimeToStr((DateTime)dbOutContract.CreateDate);
                        }
                        if (dbOutContract.DeliveryDateStart.HasValue)
                        {
                            vmOuts.DeliveryDateStart = Utils.DateTimeToStr((DateTime)dbOutContract.DeliveryDateStart);
                        }
                        if (dbOutContract.DeliveryDateEnd.HasValue)
                        {
                            vmOuts.DeliveryDateEnd = Utils.DateTimeToStr((DateTime)dbOutContract.DeliveryDateEnd);
                        }

                        vmOuts.OutCompanyID = dbOutContract.FactoryID;
                        vmOuts.OutCompany = dbOutContract.Factory.Abbreviation;
                        vmOuts.CallPeoPle = dbOutContract.Factory.CallPeople;
                        vmOuts.TelePhone = dbOutContract.Factory.Telephone;
                        vmOuts.Remark = dbOutContract.Remark;
                        vmOuts.Clasue = dbOutContract.Clasue;
                        vmOuts.FactoryEmail = dbOutContract.Factory.EmailAdress;
                        vmOuts.DeliveryName = dbOutContract.DeliveryName;

                        decimal amount = 0;

                        vmOuts.OCPacksData = SetDTOOCPacksData(dbOutContract.Purchase_OutContractsPacks, dtDataDictionary, out amount);
                        vmOuts.OthersFee = dbOutContract.OthersFee;
                        vmOuts.OutContractSum = dbOutContract.OthersFee + amount;//sum
                        vmOuts.OutContractSumText = dbOutContract.OutContractSum ?? 0;//sum

                        List<DTOOutContractHis> listOutContractHis = new List<DTOOutContractHis>();

                        DTOOutContractHis vmochis = new DTOOutContractHis();

                        foreach (var item in dbOutContract.Purchase_OutContractHis)
                        {
                            vmochis = new DTOOutContractHis();

                            if (item.CreateDate.HasValue)
                            {
                                vmochis.AuditCreateDate = ((DateTime)item.CreateDate).ToString();
                            }
                            vmochis.AuditPacksIdea = item.AuditIdea;
                            vmochis.AuditUserName = item.SystemUser.UserName;
                            vmochis.OutContractStatus = GetOutContract(item.OutContractStatus);
                            listOutContractHis.Add(vmochis);
                        }
                        vmOuts.OCOutContractHis = listOutContractHis;

                        //获取币种
                        //List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                        //string CurrentSign = "";
                        //foreach (var item_batch in dbOutContract.Purchase_Contract.Purchase_ContractBatch)
                        //{
                        //    var query_product = item_batch.Purchase_ContractProduct;
                        //    foreach (var item_product in query_product)
                        //    {
                        //        CurrentSign = _dictionaryServices.GetDictionary_CurrencySign(item_product.OrderProduct.CurrencyType, list_Com_DataDictionary);
                        //    }

                        //}
                        //vmOuts.CurrentSign = CurrentSign;

                        List<string> list_ToEmailAddress, list_CallName;
                        Purchase.PurchaseContractService.Get_ToEmailAddress_CallName(context, dbOutContract.Purchase_Contract, out list_ToEmailAddress, out list_CallName);
                        vmOuts.list_ToEmailAddress = CommonCode.ListToString(list_ToEmailAddress, ";");
                        vmOuts.list_CallName = CommonCode.ListToString(list_CallName);

                        vmOuts.EmailSign = context.SystemUsers.Find(currentUser.UserID).EmailSign;

                        var list_id = dbOutContract.Purchase_Contract.Purchase_Packs.Where(d => !d.isDelete && d.IsOutsourcing).Select(d => d.ID).ToList();
                        List<VMUpLoadFile> list_PacksUpload = new List<VMUpLoadFile>();
                        foreach (var item in list_id)
                        {
                            list_PacksUpload.AddRange(ConstsMethod.GetUploadFileList(item, UploadFileType.Packs));
                        }
                        vmOuts.list_PacksUpload = list_PacksUpload;
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteError(e);
            }
            return vmOuts;
        }

        /// <summary>
        /// 根据表主键自编号查询数据
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="vmInfo"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public DTOOutsourcing GetOutContractInfo(VMERPUser currentUser, DTOOutsourcing vmInfo, int id)
        {
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Purchase_OutContracts.Where(p => p.ID == id);
                    var dbInfo = query.FirstOrDefault();

                    if (dbInfo != null)
                    {
                        vmInfo.ID = dbInfo.ID;
                        vmInfo.ApproverIndex = dbInfo.ApproverIndex;
                        vmInfo.CreateUserID = dbInfo.CreateUserID;
                    }
                }
            }
            catch (Exception e) { LogHelper.WriteError(e); }

            return vmInfo;
        }

        /// <summary>
        /// 把DB中的代印合同对应的标签资料数据装载至VM中
        /// </summary>
        /// <param name="efpList">标签资料数据DB</param>
        /// <param name="amount">返回的代印合同金额计算结果</param>
        /// <returns>标签资料的List VM</returns>
        private List<DTOOCPacksData> SetDTOOCPacksData(ICollection<Purchase_OutContractsPacks> efpList, List<Com_DataDictionary> listDictinary, out decimal amount)
        {
            List<DTOOCPacksData> vmocpList = new List<DTOOCPacksData>();
            using (ERPEntitiesNew erp = new ERPEntitiesNew())
            {
                DTOOCPacksData vmocp = new DTOOCPacksData();
                amount = 0;
                decimal pta = 0;

                foreach (var efp in efpList)
                {
                    vmocp = new DTOOCPacksData();

                    vmocp.ID = efp.ID;
                    vmocp.PacksID = efp.PacksID;
                    vmocp.TagID = efp.Purchase_Packs.TagID;
                    vmocp.ContractsID = efp.Purchase_OutContracts.ContractsID;
                    vmocp.OutCompanyID = efp.Purchase_OutContracts.FactoryID;
                    //vmocp.DeliveryID = efp.Purchase_OutContracts.DeliveryID;
                    vmocp.DeliveryName = efp.Purchase_OutContracts.DeliveryName;
                    vmocp.PacksRemark = efp.PacksRemark.StrTrim();

                    var dictinaryInfo = listDictinary.Where(p => p.Code == vmocp.TagID).FirstOrDefault();
                    if (dictinaryInfo != null)
                    {
                        vmocp.TagName = dictinaryInfo.Name;
                    }
                    vmocp.OCPacksProducts = SetPacksProductData(efp.Purchase_OutContractProduct, out pta);
                    vmocp.TagProductsAmount = pta;//sum
                    amount += pta;//sum

                    vmocpList.Add(vmocp);
                }
            }
            return vmocpList;
        }

        /// <summary>
        /// 把DB中的代印合同对应的标签资料的产品数据装载至VM中
        /// </summary>
        /// <param name="efppList">标签资料的产品数据DB</param>
        /// <param name="pta">返回的标签资料金额计算结果</param>
        /// <returns>标签资料的产品List VM</returns>
        private List<DTOOCPacksProductData> SetPacksProductData(ICollection<Purchase_OutContractProduct> efppList, out decimal pta)
        {
            pta = 0;
            List<DTOOCPacksProductData> vmocpproList = new List<DTOOCPacksProductData>();
            DTOOCPacksProductData vmocppro = new DTOOCPacksProductData();

            foreach (var efpp in efppList)
            {
                vmocppro = new DTOOCPacksProductData();

                vmocppro.ID = efpp.ID;
                vmocppro.OrderProductID = efpp.OrderProductID;
                vmocppro.PacksID = efpp.OCPacksID;
                vmocppro.OrderProductNO = efpp.OrderProduct.No;
                vmocppro.OrderProductFPrice = efpp.Price ?? 0;
                if (efpp.Qty.HasValue)
                {
                    vmocppro.ProductTagsNumber = efpp.Qty ?? 0;
                }
                else
                {
                    vmocppro.ProductTagsNumber = efpp.OrderProduct.Qty;
                }
                vmocppro.ProductTagsAmount = vmocppro.OrderProductFPrice * vmocppro.ProductTagsNumber;
                vmocppro.TagDescribe = efpp.TagDescribe;
                vmocppro.ProductID = efpp.OrderProduct.ProductID;
                vmocppro.ProductImage = efpp.OrderProduct.Image;

                pta += vmocppro.ProductTagsAmount;

                if (efpp.OrderProduct.Purchase_PackProductsUPC != null && efpp.OrderProduct.Purchase_PackProductsUPC.Count > 0)
                {
                    vmocppro.ProductUPC = efpp.OrderProduct.Purchase_PackProductsUPC.First().ProductUPC;
                }

                vmocpproList.Add(vmocppro);
            }

            return vmocpproList;
        }

        /// <summary>
        /// 保存新建代印合同至DB
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="vmOC"></param>
        /// <returns></returns>
        public VMAjaxProcessResult Add(VMERPUser currentUser, DTOOutsourcing vmOC)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    DAL.Purchase_OutContracts efOutContract = new DAL.Purchase_OutContracts();
                    DAL.Purchase_OutContractHis efocHis = new Purchase_OutContractHis();
                    DAL.Purchase_OutContractsPacks efOCsPacks = new Purchase_OutContractsPacks();
                    DAL.Purchase_OutContractProduct efOCsProduct = new Purchase_OutContractProduct();
                    int affectRows = 0;

                    if (vmOC.OCPacksData != null)
                    {
                        DateTime dtNow = DateTime.Now;

                        vmOC.OCPacksData.Sort(
                            delegate (DTOOCPacksData p1, DTOOCPacksData p2)
                            {
                                int compareDate = p1.OutCompanyID.CompareTo(p2.OutCompanyID);
                                if (compareDate == 0)
                                {
                                    return p2.OutCompanyID.CompareTo(p1.OutCompanyID);
                                }
                                return compareDate;
                            }
                            );//排序

                        List<int> listOutCompanyID = new List<int>();

                        decimal dOutContract = 0;//新建代购合同金额
                        decimal dSumOthersFee = 0;//新建代购合同金额

                        foreach (var item1 in vmOC.OCPacksData)
                        {
                            if (!listOutCompanyID.Contains(item1.OutCompanyID))
                            {
                                listOutCompanyID.Add(item1.OutCompanyID);
                            }
                        }

                        List<DTOOCPacksData> ocPacksList = new List<DTOOCPacksData>();
                        string sClasue = string.Empty;

                        foreach (int iOutCompanyID in listOutCompanyID)
                        {
                            dOutContract = 0;
                            dSumOthersFee = 0;

                            efOutContract = new DAL.Purchase_OutContracts();
                            ocPacksList = new List<DTOOCPacksData>();
                            ocPacksList = vmOC.OCPacksData.Where(p => p.OutCompanyID == iOutCompanyID).ToList();

                            //同一代购公司对应的标签
                            foreach (var item1 in ocPacksList)
                            {
                                dOutContract += item1.TagProductsAmount;//产品总金额合计+其他费用
                                dSumOthersFee += item1.OthersFee;

                                //添加标签及其产品信息数据至实体对象中
                                efOCsPacks = new Purchase_OutContractsPacks();
                                efOCsPacks.PacksID = item1.PacksID;
                                efOCsPacks.PacksRemark = item1.PacksRemark.StrTrim();
                                efOCsPacks.IsDelete = false;
                                sClasue = item1.OCClasue;

                                foreach (var item2 in item1.OCPacksProducts)
                                {
                                    efOCsProduct = new Purchase_OutContractProduct();
                                    //efOCsProduct.OCPacksID = item2.PacksID;
                                    efOCsProduct.OrderProductID = item2.OrderProductID;

                                    if (!string.IsNullOrEmpty(item2.TagDescribe))
                                    {
                                        efOCsProduct.TagDescribe = item2.TagDescribe;
                                    }
                                    efOCsProduct.Price = item2.OrderProductFPrice;
                                    efOCsProduct.Qty = item2.ProductTagsNumber;
                                    efOCsProduct.IsDelete = false;

                                    efOCsPacks.Purchase_OutContractProduct.Add(efOCsProduct);
                                }

                                efOutContract.Purchase_OutContractsPacks.Add(efOCsPacks);
                            }
                            var query_purchase = context.Purchase_Contract.Find(vmOC.ContractsID);

                            string OrderNumber = null;
                            var query_Order = context.Orders.Find(query_purchase.OrderID);
                            if (query_Order != null)
                            {
                                OrderNumber = query_Order.OrderNumber;
                            }

                            string PurchaseNumber_Max = context.Purchase_Contract.Where(d => d.OrderID == query_purchase.OrderID && !d.IsDelete).Select(d => d.PurchaseNumber).Max();
                            int PurchaseNumberTemp = PurchaseNumber_Max.ToCharArray(PurchaseNumber_Max.Length - 1, 1)[0] + 1;
                            efOutContract.OutContracNo = "PC" + OrderNumber + (char)PurchaseNumberTemp;//代购合同编号取采购合同最大的后缀字母

                            efOutContract.ContractsID = vmOC.ContractsID;
                            efOutContract.FactoryID = iOutCompanyID;
                            //efOCs.DeliveryID = item1.DeliveryID;
                            efOutContract.DeliveryName = ocPacksList.FirstOrDefault().DeliveryName;
                            efOutContract.OthersFee = dSumOthersFee;
                            efOutContract.OutContractSum = dOutContract - dSumOthersFee;//保存时不包含其他费用

                            //if (!string.IsNullOrEmpty(ocPacksList.FirstOrDefault().PacksRemark))
                            //{
                            //    efOutContract.Remark = ocPacksList.FirstOrDefault().PacksRemark;
                            //}
                            if (!string.IsNullOrEmpty(ocPacksList.FirstOrDefault().OCClasue))
                            {
                                efOutContract.Clasue = ocPacksList.FirstOrDefault().OCClasue;//代印合同条款
                            }
                            //efOutContract.Clasue = sClasue;//代印合同条款

                            if (!string.IsNullOrEmpty(vmOC.DeliveryDateStart))
                            {
                                efOutContract.DeliveryDateStart = Utils.StrToDateTime(vmOC.DeliveryDateStart);
                            }
                            efOutContract.IsDelete = false;
                            efOutContract.OutContractStatus = vmOC.OutContractStatusID;
                            efOutContract.CreateUserID = currentUser.UserID;
                            efOutContract.CreateTerminal = CommonCode.GetIP();
                            efOutContract.CreateDate = dtNow;
                            efOutContract.UpdateDate = dtNow;//用于把最新操作的数据显示在第一页的第一行
                            efOutContract.UpdateUserID = currentUser.UserID;

                            //插入每个代印合同的状态变更记录
                            efocHis = new Purchase_OutContractHis();
                            efocHis.AuditUserID = currentUser.UserID;
                            efocHis.CreateDate = dtNow;
                            efocHis.OutContractStatus = vmOC.OutContractStatusID;

                            efOutContract.Purchase_OutContractHis.Add(efocHis);
                            context.Purchase_OutContracts.Add(efOutContract);

                            affectRows = context.SaveChanges();

                            if (affectRows == 0)
                            {
                                result.IsSuccess = false;
                            }
                            else
                            {
                                result.IsSuccess = true;

                                bool bIsPass = true;
                                if (vmOC.OutContractStatusID == (int)OutContractStatusEnum.NotPassCheck)
                                {
                                    bIsPass = false;
                                }

                                List<int> listStatus = new List<int>();
                                listStatus.Add((int)OutContractStatusEnum.PendingCheck);
                                listStatus.Add((int)OutContractStatusEnum.NotPassCheck);
                                int[] iArrLogicStatus = { (int)OutContractStatusEnum.PendingCheck, (int)OutContractStatusEnum.NotPassCheck, (int)OutContractStatusEnum.PassedCheck };

                                ConstsMethod.InsertAuditStream(efOutContract.ID, bIsPass, WorkflowTypes.ApprovalOutsourcingContract, currentUser.UserID, listStatus, iArrLogicStatus, vmOC.AuditIdea);
                            }
                        }

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
        /// 保存编辑后的代印合同至DB
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="vmOC"></param>
        /// <returns></returns>
        public VMAjaxProcessResult Save(VMERPUser currentUser, DTOOutsourcing vmOC)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    DateTime dtNow = DateTime.Now;
                    string sAuditIdea = string.Empty;
                    var dbOutContract = context.Purchase_OutContracts.Where(o => o.ID == vmOC.ID).FirstOrDefault();

                    //dbOutContract.Remark = vmOC.Remark;
                    //dbOutContract.AuditIdea = vmOC.AuditIdea;

                    dbOutContract.UpdateUserID = currentUser.UserID;
                    dbOutContract.UpdateTerminal = CommonCode.GetIP();
                    dbOutContract.UpdateDate = dtNow;

                    if (vmOC.OutContractStatusID != (int)OutContractStatusEnum.NotPassCheck && vmOC.OutContractStatusID != (int)OutContractStatusEnum.PassedCheck)
                    {
                        dbOutContract.OutContractStatus = vmOC.OutContractStatusID;
                    }
                    if (vmOC.PageTypeID != 4)
                    {
                        if (!string.IsNullOrEmpty(vmOC.DeliveryDateStart))
                        {
                            dbOutContract.DeliveryDateStart = Utils.StrToDateTime(vmOC.DeliveryDateStart);
                        }

                        dbOutContract.OthersFee = vmOC.OthersFee;
                        dbOutContract.Clasue = vmOC.Clasue;
                    }

                    decimal OutContractSum = 0;
                    foreach (var item1 in vmOC.OCPacksData)
                    {
                        var efOCsPacks = dbOutContract.Purchase_OutContractsPacks.Where(p => p.ID == item1.ID).FirstOrDefault();
                        if (efOCsPacks != null)
                        {
                            if (vmOC.PageTypeID != 4)
                            {
                                //已在代购合同主表中存储
                                if (!string.IsNullOrEmpty(item1.PacksRemark))
                                {
                                    efOCsPacks.PacksRemark = item1.PacksRemark.StrTrim();
                                }
                            }

                            foreach (var item2 in item1.OCPacksProducts)
                            {
                                var efOCsProduct = efOCsPacks.Purchase_OutContractProduct.Where(p => p.ID == item2.ID).FirstOrDefault();
                                if (efOCsProduct != null)
                                {
                                    if (!string.IsNullOrEmpty(item2.TagDescribe))
                                    {
                                        efOCsProduct.TagDescribe = item2.TagDescribe;
                                    }

                                    efOCsProduct.Price = item2.OrderProductFPrice;
                                    efOCsProduct.Qty = item2.ProductTagsNumber;
                                }
                            }
                        }
                        OutContractSum += item1.TagProductsAmount;
                    }
                    dbOutContract.OutContractSum = OutContractSum;

                    //插入更新履历
                    DAL.Purchase_OutContractHis efOChis = new Purchase_OutContractHis();
                    efOChis.CreateDate = dtNow;
                    efOChis.AuditIdea = vmOC.AuditIdea;
                    efOChis.AuditUserID = currentUser.UserID;
                    efOChis.OutContractStatus = vmOC.OutContractStatusID;
                    dbOutContract.Purchase_OutContractHis.Add(efOChis);

                    int affectRows = context.SaveChanges();

                    if (affectRows == 0)
                    {
                        result.IsSuccess = false;
                    }
                    else
                    {
                        bool bIsPass = true;
                        if (vmOC.OutContractStatusID == (int)OutContractStatusEnum.NotPassCheck)
                        {
                            bIsPass = false;
                        }

                        List<int> listStatus = new List<int>();
                        listStatus.Add((int)OutContractStatusEnum.PendingCheck);
                        listStatus.Add((int)OutContractStatusEnum.NotPassCheck);
                        int[] iArrLogicStatus = { (int)OutContractStatusEnum.PendingCheck, (int)OutContractStatusEnum.NotPassCheck, (int)OutContractStatusEnum.PassedCheck };

                        ConstsMethod.InsertAuditStream(dbOutContract.ID, bIsPass, WorkflowTypes.ApprovalOutsourcingContract, currentUser.UserID, listStatus, iArrLogicStatus, vmOC.AuditIdea);

                        result.IsSuccess = true;
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
        /// 删除代印合同
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public DBOperationStatus Delete(VMERPUser currentUser, List<int> ids)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    DateTime dtNow = DateTime.Now;

                    var efOCs = context.Purchase_OutContracts.Where(o => ids.Contains(o.ID));
                    var dtOCs = efOCs.ToList();
                    foreach (var item1 in dtOCs)
                    {
                        if (item1 != null)
                        {
                            item1.UpdateUserID = currentUser.UserID;
                            item1.UpdateTerminal = CommonCode.GetIP();
                            item1.UpdateDate = dtNow;
                            item1.IsDelete = true;

                            if (item1.Purchase_OutContractsPacks != null)
                            {
                                foreach (var item2 in item1.Purchase_OutContractsPacks)
                                {
                                    item2.IsDelete = true;
                                    foreach (var item3 in item2.Purchase_OutContractProduct)
                                    {
                                        item3.IsDelete = true;
                                    }
                                }
                            }
                        }
                    }

                    //result = DBOperationStatus.Success;
                    //return result;

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
            catch (Exception e)
            {
                LogHelper.WriteError(e);
            }

            return result;
        }

        /// <summary>
        /// 获取上传附件的详情(根据id获取上传文件表中相应信息，注意linkid是代印合同id)
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
                    #region 获取文件列表

                    List<VMUpLoadFile> list_UpLoadFile = new List<VMUpLoadFile>();
                    var query_UpLoadFile = context.UpLoadFiles.Where(d => d.LinkID == id && !d.IsDelete && d.ModuleType == 1);
                    foreach (var item in query_UpLoadFile)
                    {
                        list_UpLoadFile.Add(new VMUpLoadFile()
                        {
                            ID = item.ID,
                            DisplayFileName = item.DisplayFileName,
                            ServerFileName = item.ServerFileName,
                            ModuleType = item.ModuleType,
                            LinkID = item.LinkID,
                            IsDelete = item.IsDelete,
                            DT_CREATEDATE = item.DT_CREATEDATE,
                            ST_CREATEUSER = item.ST_CREATEUSER,
                        });
                    }

                    #endregion 获取文件列表

                    //根据上传文件id查找到代印表
                    var query = context.Purchase_OutContracts.Find(id);

                    //var query = context.Purchase_Contract.Find(id);
                    vm = new VMUpLoad()
                    {
                        ID = query.ID,
                        No_Description = "采购合同编号",
                        No = query.Purchase_Contract.PurchaseNumber,
                        Field1_Description = "工厂",
                        Field1 = query.Factory.Abbreviation,
                        Field2_Description = "客户",
                        Field2 = context.Orders_Customers.Find(context.Purchase_Contract.Where(d => d.ID == query.Purchase_Contract.ID && d.ContractType == (int)ContractTypeEnum.Default).FirstOrDefault().CustomerID).CustomerCode,
                        Field3_Description = "代印合同编号",
                        Field3 = query.OutContracNo,
                        FileName_Description = "合同附件",
                        UpLoadFileList = list_UpLoadFile,
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
                    var query = context.Purchase_OutContracts.Where(p => p.ID == vm.ID);
                    var dataFromDB = query.FirstOrDefault();

                    if (dataFromDB != null)
                    {
                        dataFromDB.OutContractStatus = (int)OutContractStatusEnum.ContractUploaded;
                        dataFromDB.UpdateUserID = currentUser.UserID;
                        dataFromDB.UpdateDate = DateTime.Now;

                        int newID = 0;
                        foreach (var item in vm.UpLoadFileList)
                        {
                            if (item.ID != 0)
                            {
                                //更新
                                var query_UpLoadFiles = context.UpLoadFiles.Find(item.ID);
                                query_UpLoadFiles.IsDelete = item.IsDelete;
                                query_UpLoadFiles.DT_MODIFYDATE = DateTime.Now;
                                query_UpLoadFiles.ST_MODIFYUSER = currentUser.UserID;
                            }
                            else
                            {
                                //添加
                                context.UpLoadFiles.Add(new UpLoadFile()
                                {
                                    ID = --newID,
                                    DisplayFileName = item.DisplayFileName,
                                    ServerFileName = item.ServerFileName,
                                    DT_CREATEDATE = item.DT_CREATEDATE,
                                    DT_MODIFYDATE = DateTime.Now,
                                    ST_CREATEUSER = currentUser.UserID,
                                    ST_MODIFYUSER = currentUser.UserID,
                                    IPAddress = CommonCode.GetIP(),
                                    IsDelete = false,
                                    LinkID = vm.ID,
                                    ModuleType = (int)UploadFileType.OutContract,
                                });
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

        ///<summary>
        ///根据传进来的采购合同编号，找到客户id，将客户信息传过去
        /// </summary>

        /// <summary>
        /// 发送合同
        /// </summary>
        /// <param name="currentUser">CurrentUserServices.Me, id, vm</param>
        /// <param name="id"></param>
        /// <returns></returns>
        public VMAjaxProcessResult SendContract(VMERPUser currentUser, int id, VMSendEmail vm)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var dataFromDB = context.Purchase_OutContracts.Find(id);
                    if (dataFromDB != null)
                    {
                        dataFromDB.OutContractStatus = (int)OutContractStatusEnum.ContractSent;
                        dataFromDB.UpdateUserID = currentUser.UserID;
                        dataFromDB.UpdateDate = DateTime.Now;
                        dataFromDB.UpdateTerminal = CommonCode.GetIP();

                        context.Purchase_OutContractHis.Add(new Purchase_OutContractHis()
                        {
                            CreateDate = DateTime.Now,
                            AuditUserID = currentUser.UserID,
                            OutContractsID = id,
                            OutContractStatus = (int)OutContractStatusEnum.ContractSent,
                        });

                        int affectRows = context.SaveChanges();
                        if (affectRows == 0)
                        {
                            result.IsSuccess = false;
                            result.Msg = "发送合同失败了！";
                            return result;
                        }

                        List<string> list_Attachs = new List<string>();

                        DTOOutsourcing vmOuts = new DTOOutsourcing();
                        vmOuts.ID = id;
                        vmOuts.PageTypeID = 2;
                        var vm2 = GetDetailByID(currentUser, vmOuts);

                        if (vm.UpLoadFileList != null)
                        {
                            foreach (var item in vm.UpLoadFileList)
                            {
                                if (!item.IsDelete)
                                {
                                    string ServerFileName = ConstsMethod.ReplaceURLToLocalPath(item.ServerFileName);
                                    if (File.Exists(ServerFileName))
                                    {
                                        string di_path = Utils.GetMapPath("/data/Template/Out/OutSourcingContract/" + vm2.ID + "/Upload");
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
                            string filePath = MakeExcel(currentUser, id, "xls", vm2);
                            list_Attachs.Add(Utils.GetMapPath(filePath));
                        }

                        if (vm.IsContainMakerExcel_pdf)
                        {
                            string filePath = MakeExcel(currentUser, id, "pdf", vm2);
                            list_Attachs.Add(Utils.GetMapPath(filePath));
                        }

                        if (vm.IsContainMakerExcel_jpg)
                        {
                            string filePath = MakeExcel(currentUser, id, "jpg", vm2);
                            if (filePath.Length > 1024 * 20)
                            {
                                result.IsSuccess = false;
                                result.Msg = "标签图片.zip大小超过了20M！";
                                return result;
                            }
                            list_Attachs.Add(Utils.GetMapPath(filePath));
                        }

                        vm.Attachs = CommonCode.ListToString(list_Attachs, ";");
                        Email.SendEmail(currentUser.UserName, vm);//发送电子邮件
                        result.IsSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
                result.IsSuccess = false;
                result.Msg = "邮件发送失败！";
            }
            return result;
        }

        /// <summary>
        /// 生成代购合同的Excel
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public string MakeExcel(VMERPUser currentUser, int id, string extension, DTOOutsourcing model)
        {
            model.ID = id;
            model.PageTypeID = 2;
            if (extension == "jpg")
            {
                var list_PacksUpload = model.list_PacksUpload;
                if (list_PacksUpload.Count > 0)
                {
                    string thisDic = "/data/Template/Out/OutSourcingContract/" + id;
                    string directoryPath = Utils.GetMapPath(thisDic + "/TagImage");
                    foreach (var item in list_PacksUpload)
                    {
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        if (!File.Exists(directoryPath + "/" + item.DisplayFileName))
                        {
                            string imgPath = ConstsMethod.ReplaceURLToLocalPath(Utils.GetMapPath(item.ServerFileName));
                            FileInfo fi = new FileInfo(imgPath);
                            fi.CopyTo(directoryPath + "/" + item.DisplayFileName);
                        }
                    }

                    string newzipPath = Utils.GetMapPath(thisDic) + "/" + id + "_标签图片.zip";
                    ExcelHelper.Zip(directoryPath, newzipPath);//生成压缩文件
                    return thisDic + "/" + id + "_标签图片.zip";
                }
                return "";
            }

            string xlsPath = MakerExcel.Maker(model, MakerTypeEnum.OutSourcingContract, id.ToString());//生成采购合同的Excel

            if (extension == "xls")
            {
                return xlsPath;
            }
            if (extension == "pdf")
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

        public VMoutsourContract GetQuote(VMERPUser currentUser, int id)
        {
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                var match = context.Purchase_OutContracts.Find(id);

                return new VMoutsourContract
                {
                    ID = match.ID.ToString(),
                    OutContracNo = match.OutContracNo,
                    PurchaseNumber = context.Purchase_Contract.Find(match.ContractsID).PurchaseNumber,
                    OutContractSum = match.OutContractSum.ToString(),
                    PurchaseAmount = context.Purchase_Contract.Find(match.ContractsID).AllAmount,
                    PurchaseDate = Utils.DateTimeToStr(context.Purchase_Contract.Find(match.ContractsID).PurchaseDate),
                    CustomerCode = match.Purchase_Contract.Orders_Customers.CustomerCode,
                    OutCompany = context.Factories.Find(match.FactoryID).Abbreviation,
                    Author = match.SystemUser.UserName,
                    CreateDate = match.CreateDate.ToString(),
                    CustomerID = match.Purchase_Contract.Orders_Customers.OCID,
                };
            }
        }

        public List<DTOContacts> GetContacts(VMERPUser currentUser, int id)
        {
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                var list_vm = new List<DTOContacts>();

                var match = context.Orders_Contacts.Where(p => p.OCID == id && !p.IsDelete).OrderByDescending(p => p.CreateDate);
                foreach (var item in match)
                {
                    var newHistory = new DTOContacts
                    {
                        OCID = item.OCID,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        FullName = item.FullName,
                        Duty = item.Duty,
                        MobilePhone = item.MobilePhone,
                        TelPhone = item.TelPhone,
                        Fax = item.Fax,
                        Email = item.Email,
                        IsDefault = item.IsDefault,
                        CreateDate = item.CreateDate,
                    };
                    list_vm.Add(newHistory);
                }
                return list_vm;
            }
        }

        /// <summary>
        /// 发送邮件      厂名：		编号：  联络人：		日期：     电话：		交货地：      交货期：   客人代号：  货  号   品名及规格   数量（张）   单价    金额   暂时不管版费
        /// 要求   合计数量  合计金额
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        //public ApiResult SendEmail(VMERPUser currentUser, VMQuoteSendEmail model)
        //{
        //    var result = new ApiResult();
        //    try
        //    {
        //        if (model.StatusID == 2 && model.Contacts == null) //通过系统发送邮件
        //        {
        //            result.Success = false;
        //            result.Info = "请先选择联系人！";
        //            return result;
        //        }
        //        using (ERPEntitiesNew context = new ERPEntitiesNew())
        //        {
        //            if (model.StatusID == 2)
        //            {
        //                DTOOutsourcing vmOuts = new DTOOutsourcing();
        //                vmOuts.ID = model.ID;
        //                vmOuts.PageTypeID = 2;
        //                vmOuts = GetOutContract(currentUser, vmOuts);
        //                //得到文件模板
        //                //get contacts email//得到要发送给谁的邮箱地址
        //                List<string> listEmail = new List<string>();
        //                foreach (var item in model.Contacts)
        //                {
        //                    listEmail.Add(item.Email);
        //                }

        //                var query = context.Purchase_OutContracts.Where(N => N.ID == model.ID);
        //                //创建模板
        //                ////创建报价单模板

        //                outMaker.BuildQuot(vmOuts, OutTypeEnum.S333);

        //                List<string> listFile = new List<string>();
        //                long fileSize = 0;

        //                string filePath = "/data/out/out1/" + model.ID + "/" + vmOuts.OutContracNo + ".xls";
        //                FileInfo fi = new FileInfo(Utils.GetMapPath(filePath));
        //                fileSize += fi.Length;
        //                listFile.Add(System.Web.HttpContext.Current.Server.MapPath("~" + filePath));
        //                var select = context.Purchase_OutContracts.Find(model.ID);
        //                select.OutContractStatus = 5;

        //                //List<string> listFile = new List<string>();
        //                //long fileSize = 0;
        //                //foreach (var item in quotProducts)
        //                //{
        //                //    string filePath = "/data/quot/out/" + model.ID + "/" + item.No.ToString() + ".xls";
        //                //    FileInfo fi = new FileInfo(Utils.GetMapPath(filePath));
        //                //    fileSize += fi.Length;
        //                //    listFile.Add(filePath);
        //                //}

        //                result.Info = "邮件发送成功！";
        //                if (fileSize > 5 * 1024 * 1024)
        //                {
        //                    result.Info = "邮件发送成功！但是邮件附件大小超过5M可能导致客户收不到。";
        //                }
        //                Mail.Instance.SendMail(listEmail, "测试邮件", "邮件正文，链接地址：http://192.168.1.42:8010/Quote/Template/" + model.ID, listFile, MailType.Quot, currentUser.UserName);
        //            }
        //            else
        //            {
        //                var select = context.Purchase_OutContracts.Find(model.ID);
        //                select.OutContractStatus = 5;
        //                result.Info = "保存成功！";
        //            }
        //            context.SaveChanges();

        //            //将数据添加到文件中
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Success = false;
        //        result.Info = "出错消息：" + ex.Message;
        //    }
        //    return result;
        //}
    }
}