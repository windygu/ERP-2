using ERP.BLL.Consts;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.Packs;
using ERP.Tools;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.BLL.ERP.Packs
{
    public class PacksServices
    {
        public List<VMPacks> GetAll(VMFilterPacks vmFilter, VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows)
        {
            List<VMPacks> packsList = new List<VMPacks>(); ;
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    //默认查询条件：采购合同，没有被删除，数据状态在审核已通过及以后
                    var query = context.Purchase_Contract.Where(d => !d.IsDelete && d.PurchaseStatus >= (int)PurchaseStatusEnum.PassedCheck && d.ContractType == (int)ContractTypeEnum.Default);

                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForPacks);

                    if (currentUser.HierachyType == HierachyType.Agency)
                    {
                        var userHierachy = new List<int?>()
                        {
                            currentUser.HierachyID,
                            context.SystemUsers.FirstOrDefault(p => p.HierarchyID == currentUser.HierachyID).Hierarchy.Hierarchy2.HierarchyID
                        };

                        query = query.Where(p => userHierachy.Contains(p.Factory.Hierarchy));
                    }

                    #region 筛选条件

                    //已审核数据列表中，查询结果为已审核通过的
                    if (vmFilter.PageType == PageTypeEnum.PendingCheckList)
                    {
                        if (vmFilter.PacksStatus == 0)
                        {
                            query = query.Where(p => p.PacksStatus <= (int)PurchasePacksStatusEnum.NotPassCheck);
                        }
                        else
                        {
                            query = query.Where(p => p.PacksStatus == vmFilter.PacksStatus);
                        }
                    }
                    else
                    {
                        if (vmFilter.PacksStatus == 0)
                        {
                            query = query.Where(p => p.PacksStatus >= (int)PurchasePacksStatusEnum.PassedCheck);
                        }
                        else
                        {
                            query = query.Where(p => p.PacksStatus == vmFilter.PacksStatus);
                        }
                    }

                    if (!string.IsNullOrEmpty(vmFilter.PurchaseNumber))
                    {
                        query = query.Where(p => p.PurchaseNumber.Contains(vmFilter.PurchaseNumber));
                    }
                    if (!string.IsNullOrEmpty(vmFilter.FactoryAbbreviation))
                    {
                        query = query.Where(p => p.Factory.Abbreviation.Contains(vmFilter.FactoryAbbreviation));
                    }
                    if (!string.IsNullOrEmpty(vmFilter.CustomerCode))
                    {
                        query = query.Where(p => p.Orders_Customers.CustomerCode == vmFilter.CustomerCode);
                    }

                    if (!string.IsNullOrEmpty(vmFilter.PurchaseDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(vmFilter.PurchaseDateStart);
                        query = query.Where(p => p.PurchaseDate >= dt);
                    }
                    if (!string.IsNullOrEmpty(vmFilter.PurchaseDateEnd))
                    {
                        DateTime dt = CommonCode.GetDateEnd(vmFilter.PurchaseDateEnd);
                        query = query.Where(p => p.PurchaseDate <= dt);
                    }
                    if (!string.IsNullOrEmpty(vmFilter.DateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(vmFilter.DateStart);
                        query = query.Where(p => p.DateStart >= dt);
                    }
                    if (!string.IsNullOrEmpty(vmFilter.DateEnd))
                    {
                        DateTime dt = Utils.StrToDateTime(vmFilter.DateEnd);
                        query = query.Where(p => p.DateStart <= dt);
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
                        query = query.OrderByDescending(p => p.PacksUpdateDate);
                    }

                    #endregion 排序

                    totalRows = query.Count();

                    var dataFromDB = query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

                    if (dataFromDB != null)
                    {
                        var comDataDictionary = context.Com_DataDictionary.Where(p => p.IsDelete == 0);
                        VMPacks vmPacks = new VMPacks();

                        foreach (var entity in dataFromDB)
                        {
                            vmPacks = new VMPacks();
                            vmPacks = GetDTOEntityFromDB(entity, comDataDictionary, vmPacks);

                            //当加载待审核页面数据列表时，获取审批流权限值，以便前台判断用户是否具有审批数据行的按钮权限
                            if (vmFilter.PageType == PageTypeEnum.PendingCheckList)
                            {
                                vmPacks.IsCanAudit = Workflow.ApprovalService.HasApprovalPermission(WorkflowTypes.ApprovalPacking, currentUser, vmPacks.CreateUserID, vmPacks.ApproverIndex, entity.CustomerID);

                                if (vmPacks.PacksStatusID == (int)PurchasePacksStatusEnum.PendingCheck)
                                {
                                    vmPacks.NextApproverInfos = Workflow.ApprovalService.GetNextApproverInfos(WorkflowTypes.ApprovalPacking, vmPacks.CreateUserID, vmPacks.ApproverIndex, entity.CustomerID);
                                }
                            }
                            packsList.Add(vmPacks);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return packsList;
        }

        /// <summary>
        /// 装载待、已审核采购合同->包装资料到VM中，供查询列表数据显示用
        /// </summary>
        /// <param name="pc">采购合同->包装资料VM</param>
        /// <returns></returns>
        private VMPacks GetDTOEntityFromDB(Purchase_Contract pc, IQueryable<Com_DataDictionary> comDataDictionary, VMPacks vmPacks)
        {
            vmPacks.PurchaseContractID = pc.ID;
            vmPacks.PurchaseNumber = pc.PurchaseNumber;
            vmPacks.FactoryAbbreviation = pc.Factory.Abbreviation;
            vmPacks.CustomerCode = pc.Orders_Customers.CustomerCode;
            vmPacks.AllAmount = pc.AllAmount;
            vmPacks.PurchaseDate = Utils.DateTimeToStr(pc.PurchaseDate);
            vmPacks.DateStart = Utils.DateTimeToStr(pc.DateStart);
            vmPacks.DateEnd = Utils.DateTimeToStr(pc.DateEnd);
            vmPacks.PacksStatusID = pc.PacksStatus;

            var dtValidPacks = pc.Purchase_Packs.Where(p => !p.isDelete);

            vmPacks.PacksName = CombinePacksName(dtValidPacks, comDataDictionary, ",");
            vmPacks.IsOutsourcing = IsOutSourcing(dtValidPacks);

            //if (pc.Purchase_Packs.Count > 0) {
            //    vmPacks.CreateUserID = pc.Purchase_Packs.FirstOrDefault().CreateUserID;
            //    vmPacks.ApproverIndex = pc.Purchase_Packs.FirstOrDefault().ApproverIndex;
            //    vmPacks.PacksStatusID = pc.Purchase_Packs.FirstOrDefault().PacksStatus;

            //}
            vmPacks.ApproverIndex = pc.ApproverIndexPacks;
            vmPacks.PacksStatus = GetPacksStatus(pc.PacksStatus);
            vmPacks.CreateUserID = pc.ST_CREATEUSER;
            vmPacks.PacksUpdateDateFormatter = Utils.DateTimeToStr2(pc.PacksUpdateDate);

            return vmPacks;
        }

        /// <summary>
        /// 取得采购合同->包装资料编辑时的支持数据
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="PurchaseID"></param>
        /// <returns></returns>
        public VMPacks GetDetailByID(VMERPUser currentUser, int ContractID, PageTypeEnum PageType)
        {
            VMPacks vmPacks = new VMPacks();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Purchase_Contract.Where(d => d.ID == ContractID && d.ContractType == (int)ContractTypeEnum.Default);
                    if (query != null)
                    {
                        var dtDictionary = context.Com_DataDictionary.Where(p => p.IsDelete == 0);
                        UploadFileType moduleType;
                        if (PageType != PageTypeEnum.Upload)
                        {
                            moduleType = UploadFileType.Packs;
                        }
                        else
                        {
                            moduleType = UploadFileType.PacksUploadFiles;
                        }
                        var dtPacksUploadfile = context.UpLoadFiles.Where(p => !p.IsDelete && p.ModuleType == (int)moduleType);

                        vmPacks = SetVMPacksFromEFContract(query.FirstOrDefault(), dtDictionary, dtPacksUploadfile, moduleType);

                        List<DTOAuditPacksHis> listAuditPacksHis = new List<DTOAuditPacksHis>();
                        foreach (var item1 in query.FirstOrDefault().Purchase_PacksHis)
                        {
                            DTOAuditPacksHis vmAuditPacksHis = new DTOAuditPacksHis();
                            vmAuditPacksHis.AuditUserName = item1.SystemUser.UserName;
                            vmAuditPacksHis.AuditPacksIdea = item1.AuditPacksIdea;
                            vmAuditPacksHis.PacksStatus = GetPacksStatus(item1.PacksStatus);
                            vmAuditPacksHis.AuditCreateDate = Utils.DateTimeToStr2(item1.CreateDate);
                            listAuditPacksHis.Add(vmAuditPacksHis);
                        }
                        vmPacks.AuditPacksHisList = listAuditPacksHis;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return vmPacks;
        }

        /// <summary>
        /// 把查询到的采购合同对应的基本信息、包装资料及其产品信息装载到包装资料VM对象中
        /// </summary>
        /// <param name="pc"></param>
        /// <returns></returns>
        private VMPacks SetVMPacksFromEFContract(Purchase_Contract pc, IQueryable<Com_DataDictionary> listDictionary, IQueryable<UpLoadFile> listPacksUploadfile, UploadFileType moduleType)
        {
            VMPacks vmPacks = new VMPacks();

            vmPacks.PacksStatusID = pc.PacksStatus;
            vmPacks.PurchaseContractID = pc.ID;
            vmPacks.PurchaseNumber = pc.PurchaseNumber;
            vmPacks.FactoryAbbreviation = pc.Factory.Abbreviation;
            vmPacks.CustomerCode = pc.Orders_Customers.CustomerCode;
            vmPacks.AllAmount = pc.AllAmount;
            vmPacks.PurchaseDate = Utils.DateTimeToStr(pc.PurchaseDate);
            vmPacks.DateStart = Utils.DateTimeToStr(pc.DateStart);
            vmPacks.DateEnd = Utils.DateTimeToStr(pc.DateEnd);

            //把查询到的采购合同对应的包装资料及其产品信息装载到包装资料VM对象中
            var listValidPacks = pc.Purchase_Packs.Where(p => !p.isDelete);

            vmPacks.PacksList = SetVMTagsFromEFPacks(listValidPacks, listDictionary, listPacksUploadfile, moduleType);

            //采购合同->包装资料对应的产品UPC表没有记录时，需要从采购合同->产品批次表->产品表与销售订单产品表关联数据，
            //得到采购合同对应的产品信息，并可以编辑编辑产品UPC、内盒UPC、外箱UPC
            //上传附件之前，需要获产品UPC
            if (moduleType != UploadFileType.PacksUploadFiles)
            {
                if (pc.Purchase_PackProductsUPC.Count == 0 || pc.PacksStatus == (int)PurchasePacksStatusEnum.PendingMaintenance)
                {
                    var pcb = pc.Purchase_ContractBatch.Where(p => p.PurchaseContractID == pc.ID);
                    if (pcb != null)
                    {
                        vmPacks.PHProductsUPC = SetVMPHProductsUPCFromEFOP(pcb);
                    }
                }
                else
                {
                    vmPacks.PHProductsUPC = SetVMPHProductsUPCFromEF(pc.Purchase_PackProductsUPC);
                }
            }

            return vmPacks;
        }

        /// <summary>
        /// 把查询到的采购合同对应的包装资料及其产品信息装载到包装资料VM对象中
        /// </summary>
        /// <param name="listPacks">查询得到的EF</param>
        /// <returns></returns>
        private List<DTOPacks> SetVMTagsFromEFPacks(IEnumerable<Purchase_Packs> listPacks, IQueryable<Com_DataDictionary> listDictionary, IQueryable<UpLoadFile> listPacksUploadfile, UploadFileType moduleType)
        {
            List<DTOPacks> vmPacksList = new List<DTOPacks>();
            DTOPacks packs = new DTOPacks();
            List<int> packProducts = new List<int>();

            VMUpLoad vmUpLoad = new VMUpLoad();
            VMUpLoadFile vmUpLoadFile = new VMUpLoadFile(); ;

            StringBuilder sbProductsID = null;
            StringBuilder sbProductsName = null;

            foreach (var item1 in listPacks)
            {
                packs = new DTOPacks();
                packs.ID = item1.ID;
                packs.ContractsID = item1.ContractsID;
                packs.TagID = item1.TagID;

                vmUpLoad = new VMUpLoad();
                VMUpLoadFile UpLoadFile = new VMUpLoadFile();

                var objDictionary = listDictionary.Where(p => p.Code == item1.TagID).FirstOrDefault();
                if (objDictionary != null)
                {
                    packs.TagName = objDictionary.Name;
                }

                //查询包装资料标签对应的样张
                var objUpLoadFileList = listPacksUploadfile.Where(p => p.LinkID == item1.ID);

                if (objUpLoadFileList != null && objUpLoadFileList.Count() > 0)
                {
                    if (moduleType == UploadFileType.PacksUploadFiles)
                    {
                        packs.UpLoadFileList = ConstsMethod.GetUploadFileList(item1.ID, UploadFileType.PacksUploadFiles);
                    }
                    else
                    {
                        var item2 = objUpLoadFileList.First();

                        packs.DisplayFileName = item2.DisplayFileName;
                        packs.ServerFileName = item2.ServerFileName;
                    }
                }

                packs.TagDescribe = item1.TagDescribe;
                packs.TagSample = item1.TagSample;
                packs.PacksRemark = item1.PacksRemark;
                packs.IsOutsourcing = item1.IsOutsourcing;

                sbProductsID = new StringBuilder();
                sbProductsName = new StringBuilder();

                //装载产品列表至包装资料标签VM中
                foreach (var item2 in item1.Purchase_PackProducts)
                {
                    //过滤标记为删除的记录
                    if (!item2.isDelete)
                    {
                        if (string.IsNullOrEmpty(sbProductsID.ToString()))
                        {
                            sbProductsID.Append(item2.OrderProduct.ID);
                            sbProductsName.Append(item2.OrderProduct.No);
                        }
                        else
                        {
                            sbProductsID.Append("," + item2.OrderProduct.ID);
                            sbProductsName.Append("," + item2.OrderProduct.No);
                        }
                    }
                }
                packs.OrderProductID = sbProductsID.ToString();
                packs.OrderProductName = sbProductsName.ToString();

                vmPacksList.Add(packs);
            }

            return vmPacksList;
        }

        /// <summary>
        /// 把查询到的采购合同对应的包装资料及其产品信息装载到包装资料VM对象中
        /// </summary>
        /// <param name="efProductsUPCList">查询得到的EF</param>
        /// <returns></returns>
        private List<DTOPHProductsUPC> SetVMPHProductsUPCFromEF(ICollection<Purchase_PackProductsUPC> efProductsUPCList)
        {
            List<DTOPHProductsUPC> vmProductsUPCList = new List<DTOPHProductsUPC>();
            DTOPHProductsUPC vmProductsUPC = new DTOPHProductsUPC();

            foreach (var item1 in efProductsUPCList)
            {
                vmProductsUPC = new DTOPHProductsUPC();
                vmProductsUPC.PPPUPCID = item1.PPPUPCID;
                vmProductsUPC.ContractsID = item1.ContractsID;
                vmProductsUPC.ProductID = item1.OrderProduct.ProductID;
                vmProductsUPC.ProductImage = item1.OrderProduct.Image;
                vmProductsUPC.OrderProductID = item1.OrderProductID;
                vmProductsUPC.OrderProductNO = item1.OrderProduct.No;
                vmProductsUPC.OrderProductName = item1.OrderProduct.Name;
                vmProductsUPC.SkuNumber = item1.OrderProduct.SkuNumber;
                vmProductsUPC.ProductUPC = item1.ProductUPC;
                vmProductsUPC.InnerUPC = item1.InnerUPC;
                vmProductsUPC.OuterUPC = item1.OuterUPC;

                vmProductsUPCList.Add(vmProductsUPC);
            }

            return vmProductsUPCList;
        }

        private List<DTOPHProductsUPC> SetVMPHProductsUPCFromEFOP(IEnumerable<Purchase_ContractBatch> efProductsUPCList)
        {
            List<DTOPHProductsUPC> vmProductsUPCList = new List<DTOPHProductsUPC>();
            DTOPHProductsUPC vmProductsUPC = new DTOPHProductsUPC();

            foreach (var item in efProductsUPCList)
            {
                foreach (var item1 in item.Purchase_ContractProduct.Where(d => !d.IsDelete && (!d.IsProductMixed || d.IsProductMixed && d.ParentProductMixedID.HasValue)))
                {
                    vmProductsUPC = new DTOPHProductsUPC();
                    vmProductsUPC.PPPUPCID = 0;
                    vmProductsUPC.ContractsID = item.PurchaseContractID;
                    vmProductsUPC.OrderProductID = item1.OrderProductID;
                    vmProductsUPC.ProductID = item1.OrderProduct.ProductID;
                    vmProductsUPC.ProductImage = item1.OrderProduct.Image;
                    vmProductsUPC.OrderProductNO = item1.OrderProduct.No;
                    vmProductsUPC.OrderProductName = item1.OrderProduct.Name;
                    vmProductsUPC.SkuNumber = item1.OrderProduct.SkuNumber;
                    vmProductsUPC.ProductUPC = string.Empty;
                    vmProductsUPC.InnerUPC = string.Empty;
                    vmProductsUPC.OuterUPC = string.Empty;

                    vmProductsUPCList.Add(vmProductsUPC);
                }
            }

            return vmProductsUPCList;
        }

        public VMPacks GetPacksProductList(VMERPUser currentUser, int PurchaseID, int pageTypeID)
        {
            VMPacks vmPacks = new VMPacks();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query_Purchase_Contract = context.Purchase_Contract.Where(d => d.ID == PurchaseID && d.ContractType == (int)ContractTypeEnum.Default).FirstOrDefault();

                    List<DTOProductPacksData> productPacks = new List<DTOProductPacksData>();
                    var comDataDictionary = context.Com_DataDictionary.Where(p => p.IsDelete == 0);
                    var dtPacksUploadfile = context.UpLoadFiles.Where(p => !p.IsDelete && p.ModuleType == (int)UploadFileType.Packs);

                    DTOProductPacksData vmProductPacksData = null;
                    string sDownloadFilePath = string.Empty, sDataFlag = string.Empty;

                    foreach (var item1 in query_Purchase_Contract.Purchase_Packs)
                    {
                        var dtDownloadFilePath = dtPacksUploadfile.Where(u => u.LinkID == item1.ID).FirstOrDefault();

                        if (dtDownloadFilePath == null)
                        {
                            sDownloadFilePath = string.Empty;
                        }
                        else
                        {
                            sDownloadFilePath = dtDownloadFilePath.ServerFileName;
                        }

                        string sIsOutsourcing = "否";
                        if (item1.IsOutsourcing)
                        {
                            sIsOutsourcing = "是";
                        }

                        var dtTag = comDataDictionary.Where(p => p.Code == item1.TagID).FirstOrDefault();

                        if (dtTag != null)
                        {
                            sDataFlag = GetDictionaryDataFlag(dtTag.DataFlag ?? 0);
                            var dtTagFlag = comDataDictionary.Where(p => p.Code == dtTag.DataFlag && p.TableKind == (int)DictionaryTableKind.ProductUnit).FirstOrDefault();
                            if (dtTagFlag != null)
                            {
                                vmProductPacksData.TagFlag = dtTagFlag.Name;
                            }
                        }

                        foreach (var item2 in item1.Purchase_PackProducts)
                        {
                            if (!item2.isDelete)
                            {
                                vmProductPacksData = new DTOProductPacksData();
                                vmProductPacksData.IsOutsourcing = sIsOutsourcing;
                                vmProductPacksData.TagName = dtTag.Name;
                                vmProductPacksData.TagFlag = sDataFlag;
                                vmProductPacksData.TagDescribe = item1.TagDescribe;
                                vmProductPacksData.DownloadFilePath = sDownloadFilePath;

                                vmProductPacksData.PackProductsID = item2.ID;

                                vmProductPacksData.DataFlag = dtTag.DataFlag;
                                vmProductPacksData.ProductID = item2.OrderProduct.ProductID;
                                vmProductPacksData.ProductImage = item2.OrderProduct.Image;
                                vmProductPacksData.OrderProductNO = item2.OrderProduct.No;
                                //vmProductPacksData.OrderProductName = item2.OrderProduct.Name;
                                vmProductPacksData.SkuNumber = item2.OrderProduct.SkuNumber;
                                vmProductPacksData.Description = item2.OrderProduct.Desc;
                                vmProductPacksData.OrderProductFPrice = item2.OrderProduct.RetailPrice ?? 0;
                                vmProductPacksData.ProductUPC = item2.Purchase_PackProductsUPC.ProductUPC;

                                vmProductPacksData.InnerUPC = item2.Purchase_PackProductsUPC.InnerUPC;
                                vmProductPacksData.InnerBoxRate = item2.OrderProduct.InnerBoxRate ?? 0;
                                vmProductPacksData.OuterBoxRate = item2.OrderProduct.OuterBoxRate ?? 0;
                                vmProductPacksData.OuterUPC = item2.Purchase_PackProductsUPC.OuterUPC;

                                vmProductPacksData.ProductTagsNumber = item2.ProductTagsNumber;
                                vmProductPacksData.InnerTagsNumber = item2.InnerTagsNumber;
                                vmProductPacksData.OutTagsNumber = item2.OutTagsNumber;

                                productPacks.Add(vmProductPacksData);
                            }
                        }
                    }

                    vmPacks.PacksProducts = productPacks;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return vmPacks;
        }

        /// <summary>
        /// 根据采购合同自编号，查出采购合同产品信息供标签附加产品选择使用
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="sortColumnsNames"></param>
        /// <param name="sortColumnOrders"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRows"></param>
        /// <param name="pcID"></param>
        /// <returns></returns>
        public List<DTOPackProducts> GetPurchaseProducts(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows, int pcID)
        {
            List<DTOPackProducts> pList = new List<DTOPackProducts>();
            DTOPackProducts dtoP = new DTOPackProducts();
            totalRows = 0;

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var listCB = context.Purchase_ContractBatch.Where(p => p.PurchaseContractID == pcID);
                    List<int> listBID = new List<int>();
                    foreach (var item1 in listCB)
                    {
                        listBID.Add(item1.ID);
                    }
                    var query = context.Purchase_ContractProduct.Where(p => listBID.Contains(p.PurchaseContractBatchID) && (!p.IsProductMixed || p.IsProductMixed && p.ParentProductMixedID.HasValue));

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
                        var dtDataDictionary = context.Com_DataDictionary.Where(p => p.IsDelete == 0);

                        foreach (var p in dataFromDB)
                        {
                            dtoP = new DTOPackProducts();

                            dtoP.ProductID = p.OrderProduct.ProductID;
                            dtoP.ProductImage = p.OrderProduct.Image;
                            dtoP.OrderProductID = p.OrderProductID;
                            dtoP.OrderProductNO = p.OrderProduct.No;
                            dtoP.OrderProductName = p.OrderProduct.Name;
                            dtoP.OrderProductFPrice = p.OrderProduct.RetailPrice ?? 0;

                            var dictionary = dtDataDictionary.Where(d => d.Code == p.OrderProduct.PackingMannerZhID).FirstOrDefault();
                            if (dictionary != null)
                            {
                                dtoP.OrderProductPackingZH = dictionary.Name;
                            }
                            pList.Add(dtoP);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteError(e);
            }

            return pList;
        }

        public string PacksDownload(int iContractId, string sFilesPath)
        {
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var packsIdList = context.Purchase_Packs.Where(p => !p.isDelete && p.ContractsID == iContractId).Select(d => d.ID).ToList();

                    var dbUpLoadFilesList = context.UpLoadFiles.Where(p => !p.IsDelete && p.ModuleType == (int)UploadFileType.PacksUploadFiles && packsIdList.Contains(p.LinkID)).ToList();

                    List<string> filesAsolutePathList = new List<string>();
                    List<string> filesPhysicalPathList = new List<string>();

                    foreach (var item1 in dbUpLoadFilesList)
                    {
                        filesAsolutePathList.Add(item1.ServerFileName);
                        filesPhysicalPathList.Add(ConstsMethod.ReplaceURLToLocalPath(item1.ServerFileName));
                    }
                    string tempAsolutePath = "/data/Template/PacksDownload.docx";//写入数据所需要的模板文件
                    sFilesPath = "/data/Template/Out/PacksDownload/";//生成新文件所在路径
                    string sNewFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";

                    AsposeX aspose = new AsposeX();
                    aspose.MakeWordFile(filesAsolutePathList, filesPhysicalPathList, tempAsolutePath, sFilesPath, sNewFileName);
                    sFilesPath += sNewFileName;
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteError(e);
            }
            return sFilesPath;
        }

        public DBOperationStatus Save(VMERPUser currentUser, VMPacks vm)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    //更新采购合同的包装资料状态
                    var dtPContract = context.Purchase_Contract.Where(d => d.ID == vm.PurchaseContractID && d.ContractType == (int)ContractTypeEnum.Default).FirstOrDefault();

                    if (dtPContract == null)
                    {
                        return DBOperationStatus.NoAffect;
                    }

                    int affectRows = 0;
                    bool bIsOutsourcing = false;

                    //插入更新履历
                    DAL.Purchase_PacksHis efAuditPacksHis = new Purchase_PacksHis();

                    if (vm.PageType == PageTypeEnum.Add)
                    {
                        DAL.Purchase_Packs efPacks = new DAL.Purchase_Packs();
                        DAL.Purchase_PackProducts efPacksProduct = new Purchase_PackProducts();
                        DAL.Purchase_PackProductsUPC efPPPUPC = new Purchase_PackProductsUPC();

                        //把前台采购合同对应产品的编辑UPC记录添加、更新至采购合同->包装资料->产品信息->产品UPC表
                        foreach (var item in vm.PHProductsUPC)
                        {
                            var dtPPPUPC = dtPContract.Purchase_PackProductsUPC.Where(p => p.OrderProductID == item.OrderProductID).FirstOrDefault();

                            //不存在，插入记录
                            if (dtPPPUPC == null)
                            {
                                efPPPUPC = new Purchase_PackProductsUPC();

                                efPPPUPC.ContractsID = vm.PurchaseContractID;
                                efPPPUPC.OrderProductID = item.OrderProductID;
                                efPPPUPC.ProductUPC = item.ProductUPC;
                                efPPPUPC.InnerUPC = item.InnerUPC;
                                efPPPUPC.OuterUPC = item.OuterUPC;

                                dtPContract.Purchase_PackProductsUPC.Add(efPPPUPC);
                            }
                            //存在，更新记录
                            else
                            {
                                dtPPPUPC.ProductUPC = item.ProductUPC;
                                dtPPPUPC.InnerUPC = item.InnerUPC;
                                dtPPPUPC.OuterUPC = item.OuterUPC;
                            }
                        }

                        //-------------------------------------------------------------------------------------

                        //先删后插
                        //把DB中已存在的采购合同对应的包装资料标签及其产品记录、标签样张标记为已删除
                        var dtPCPacksFromDB = dtPContract.Purchase_Packs.Where(p => p.ContractsID == vm.PurchaseContractID && !p.isDelete);

                        //DB中的标签样张
                        var dbUpLoadFilesList = context.UpLoadFiles.Where(p => !p.IsDelete && p.ModuleType == (int)UploadFileType.Packs).ToList();

                        //前台添加了DB中标签种类没有的记录
                        if (dtPCPacksFromDB != null)
                        {
                            foreach (var item1 in dtPCPacksFromDB)
                            {
                                item1.isDelete = true;

                                foreach (var item2 in item1.Purchase_PackProducts)
                                {
                                    item2.isDelete = true;
                                }

                                //先删后插，删除DB中的标签样张
                                var dtPCPacksUploadFiles = dbUpLoadFilesList.Where(p => p.LinkID == item1.ID);

                                if (dtPCPacksUploadFiles != null)
                                {
                                    foreach (var item3 in dtPCPacksUploadFiles)
                                    {
                                        item3.IsDelete = true;
                                    }
                                }
                            }
                        }

                        affectRows = context.SaveChanges();

                        //-------------------------------------------------------------------------------------

                        foreach (var item1 in vm.PacksList)
                        {
                            item1.ContractsID = vm.PurchaseContractID;//采购合同自编号

                            if (item1.IsOutsourcing)
                            {
                                bIsOutsourcing = true;
                            }

                            efPacks = new DAL.Purchase_Packs();

                            efPacks.CreateDate = DateTime.Now;
                            ;
                            efPacks.CreateUserID = currentUser.UserID;
                            dtPContract.ST_CREATEUSER = currentUser.UserID;
                            efPacks.UpdateDate = DateTime.Now;
                            efPacks.UpdateUserID = currentUser.UserID;
                            efPacks.CreateTerminal = CommonCode.GetIP();

                            efPacks.PacksStatus = vm.PacksStatusID;

                            SetEFFromPacksVM(item1, efPacks);

                            if (item1.OrderProductID != null)
                            {
                                string[] arrOrderProductID = item1.OrderProductID.Split(',');

                                foreach (string item2 in arrOrderProductID)
                                {
                                    int iOrderProductID = 0;
                                    int.TryParse(item2, out iOrderProductID);

                                    efPacksProduct = new Purchase_PackProducts();

                                    efPacksProduct.OrderProductID = iOrderProductID;
                                    efPacksProduct.isDelete = false;

                                    var dbPPUPC = dtPContract.Purchase_PackProductsUPC.Where(p => p.OrderProductID == iOrderProductID).FirstOrDefault();

                                    if (dbPPUPC != null)
                                    {
                                        efPacksProduct.PPPUPCID = dbPPUPC.PPPUPCID;
                                    }

                                    var query_OrderProduct = context.OrderProducts.Find(iOrderProductID);

                                    efPacksProduct.ProductTagsNumber = query_OrderProduct.Qty;
                                    efPacksProduct.InnerTagsNumber = Calculator_InnerTagsNumber(query_OrderProduct.Qty, query_OrderProduct.InnerBoxRate ?? 0);
                                    efPacksProduct.OutTagsNumber = Calculator_OuterBoxRate(query_OrderProduct.Qty, query_OrderProduct.OuterBoxRate ?? 0);

                                    //int? DataFlag = context.Com_DataDictionary.Find(item1.TagID).DataFlag;
                                    var dtDataDictionaryInfo = context.Com_DataDictionary.Where(p => p.Code == item1.TagID).FirstOrDefault();
                                    int iTagType = 0;
                                    if (dtDataDictionaryInfo != null)
                                    {
                                        iTagType = dtDataDictionaryInfo.DataFlag ?? 0;
                                    }
                                    if (iTagType > 0)
                                    {
                                        if (iTagType != (int)DataFlagEnum.Product)
                                        {
                                            efPacksProduct.ProductTagsNumber = 0;
                                        }
                                        if (iTagType != (int)DataFlagEnum.InnerPack)
                                        {
                                            efPacksProduct.InnerTagsNumber = 0;
                                        }
                                        if (iTagType != (int)DataFlagEnum.OuterPack)
                                        {
                                            efPacksProduct.OutTagsNumber = 0;
                                        }
                                    }

                                    efPacks.Purchase_PackProducts.Add(efPacksProduct);
                                }
                            }

                            //插入包装资料标签记录
                            dtPContract.Purchase_Packs.Add(efPacks);

                            affectRows = context.SaveChanges();//提交数据之后,才能得到LinkID

                            if (affectRows > 0)
                            {
                                VMUpLoadFile uploadFile = new VMUpLoadFile()
                                {
                                    ID = 0,
                                    DisplayFileName = item1.DisplayFileName,
                                    ServerFileName = item1.ServerFileName,
                                    DT_CREATEDATE = DateTime.Now,
                                };
                                List<VMUpLoadFile> UpLoadFileList = new List<VMUpLoadFile>();
                                UpLoadFileList.Add(uploadFile);

                                item1.UpLoadFileList = UpLoadFileList;

                                ConstsMethod.SaveFileUpload(currentUser, efPacks.ID, item1.UpLoadFileList, context, UploadFileType.Packs);
                                affectRows = context.SaveChanges();
                            }
                        }

                        dtPContract.IsOutsourcing = bIsOutsourcing;
                    }
                    else
                    {
                        var dtPacksProductList = context.Purchase_PackProducts.Where(p => !p.isDelete);
                        foreach (var item1 in vm.PacksProducts)
                        {
                            var dtPacksProductInfo = dtPacksProductList.Where(p => p.ID == item1.PackProductsID).FirstOrDefault();
                            if (dtPacksProductInfo != null)
                            {
                                dtPacksProductInfo.ProductTagsNumber = item1.ProductTagsNumber;
                                dtPacksProductInfo.InnerTagsNumber = item1.InnerTagsNumber;
                                dtPacksProductInfo.OutTagsNumber = item1.OutTagsNumber;
                            }
                        }

                        //更新DB中已存在的采购合同对应的包装资料标签数据状态为
                        var dtPCPacks = dtPContract.Purchase_Packs.Where(p => p.ContractsID == vm.PurchaseContractID && !p.isDelete);

                        //前台添加了DB中标签种类没有的记录
                        if (dtPCPacks != null)
                        {
                            foreach (var item1 in dtPCPacks)
                            {
                                item1.PacksStatus = vm.PacksStatusID;
                            }
                        }
                    }

                    dtPContract.PacksStatus = vm.PacksStatusID;
                    dtPContract.PacksUpdateDate = DateTime.Now;

                    efAuditPacksHis.AuditUserID = currentUser.UserID;
                    efAuditPacksHis.PacksStatus = vm.PacksStatusID;
                    efAuditPacksHis.CreateDate = DateTime.Now;
                    efAuditPacksHis.AuditPacksIdea = vm.AuditPacksIdea;
                    dtPContract.Purchase_PacksHis.Add(efAuditPacksHis);

                    affectRows = context.SaveChanges();

                    if (affectRows == 0)
                    {
                        result = DBOperationStatus.NoAffect;
                    }
                    else
                    {
                        bool bIsPass = true;
                        if (vm.PacksStatusID == (int)PurchasePacksStatusEnum.NotPassCheck)
                        {
                            bIsPass = false;
                        }

                        List<int> listStatus = new List<int>();
                        listStatus.Add((int)PurchasePacksStatusEnum.PendingCheck);
                        listStatus.Add((int)PurchasePacksStatusEnum.NotPassCheck);
                        int[] iArrLogicStatus = { (int)PurchasePacksStatusEnum.PendingCheck, (int)PurchasePacksStatusEnum.NotPassCheck, (int)PurchasePacksStatusEnum.PassedCheck };

                        ConstsMethod.InsertAuditStream(dtPContract.ID, bIsPass, WorkflowTypes.ApprovalPacking, currentUser.UserID, listStatus, iArrLogicStatus, efAuditPacksHis.AuditPacksIdea);

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
        /// 保存包装资料审核结果至DB
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="pcid"></param>
        /// <param name="packsStatus"></param>
        /// <param name="packs"></param>
        /// <returns></returns>
        public DBOperationStatus SaveAuditingPacks(VMERPUser currentUser, VMPacks packs)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    //更新采购合同的包装资料状态
                    var pContractFromDB = context.Purchase_Contract.Where(d => d.ID == packs.PurchaseContractID && d.ContractType == (int)ContractTypeEnum.Default).FirstOrDefault();
                    if (pContractFromDB == null) { return DBOperationStatus.NoAffect; }

                    DateTime dtNow = DateTime.Now;

                    //pContractFromDB.AuditPacksIdea = packs.AuditPacksIdea;
                    pContractFromDB.PacksUpdateDate = dtNow;

                    foreach (var item1 in pContractFromDB.Purchase_Packs)
                    {
                        item1.PacksStatus = packs.PacksStatusID;

                        foreach (var item2 in item1.Purchase_PackProducts)
                        {
                            var packsProductInfo = packs.PacksProducts.Where(p => p.PackProductsID == item2.ID).FirstOrDefault();
                            if (packsProductInfo != null)
                            {
                                item2.ProductTagsNumber = packsProductInfo.ProductTagsNumber;
                                item2.InnerTagsNumber = packsProductInfo.InnerTagsNumber;
                                item2.OutTagsNumber = packsProductInfo.OutTagsNumber;
                            }
                        }
                    }

                    DAL.Purchase_PacksHis efAuditPacksHis = new Purchase_PacksHis();

                    efAuditPacksHis.AuditUserID = currentUser.UserID;
                    efAuditPacksHis.AuditPacksIdea = packs.AuditPacksIdea;
                    efAuditPacksHis.PacksStatus = packs.PacksStatusID;
                    efAuditPacksHis.CreateDate = dtNow;

                    pContractFromDB.Purchase_PacksHis.Add(efAuditPacksHis);

                    int affectRows = context.SaveChanges();

                    if (affectRows == 0)
                    {
                        result = DBOperationStatus.NoAffect;
                    }
                    else
                    {
                        result = DBOperationStatus.Success;
                        bool bIsPass = true;
                        if (packs.PacksStatusID == (int)PurchasePacksStatusEnum.NotPassCheck)
                        {
                            bIsPass = false;
                        }

                        List<int> listStatus = new List<int>();
                        listStatus.Add((int)PurchasePacksStatusEnum.PendingCheck);
                        listStatus.Add((int)PurchasePacksStatusEnum.NotPassCheck);
                        int[] iArrLogicStatus = { (int)PurchasePacksStatusEnum.PendingCheck, (int)PurchasePacksStatusEnum.NotPassCheck, (int)PurchasePacksStatusEnum.PassedCheck };

                        ConstsMethod.InsertAuditStream(packs.PurchaseContractID, bIsPass, WorkflowTypes.ApprovalPacking, currentUser.UserID, listStatus, iArrLogicStatus, efAuditPacksHis.AuditPacksIdea);
                    }
                }

                ////当数据达到终审时，把结果值赋值于采购合同主表
                //using (ERPEntitiesNew dbContext = new ERPEntitiesNew()) {
                //    var dbContractInfo = dbContext.Purchase_Contract.Where(p => p.ID == packs.PurchaseContractID).FirstOrDefault();
                //    if (dbContractInfo != null)
                //    {
                //        var dbPacksInfo = dbContractInfo.Purchase_Packs.FirstOrDefault();
                //        if (dbPacksInfo != null)
                //        {
                //            if (dbPacksInfo.PacksStatus >= (int)PurchasePacksStatus.NotPassCheck)
                //            {
                //                dbContractInfo.PacksStatus = packs.PacksStatusID;
                //                dbContext.SaveChanges();

                //            }
                //        }

                //    }
                //}
            }
            catch (Exception e)
            {
                LogHelper.WriteError(e);
            }

            return result;
        }

        /// <summary>
        /// 装载采购合同->包装资料标签样张至EF
        /// </summary>
        /// <param name="pcid"></param>
        /// <param name="packVM"></param>
        /// <param name="efPacks"></param>
        /// <returns></returns>
        public DAL.Purchase_Packs SetEFFromPacksVM(DTOPacks packVM, DAL.Purchase_Packs efPacks)
        {
            efPacks.ContractsID = packVM.ContractsID;

            efPacks.TagID = packVM.TagID;
            efPacks.TagDescribe = packVM.TagDescribe;
            efPacks.TagSample = packVM.TagSample;
            efPacks.IsOutsourcing = packVM.IsOutsourcing;
            efPacks.PacksRemark = packVM.PacksRemark;
            efPacks.isDelete = false;

            return efPacks;
        }

        public DBOperationStatus Save_Upload(VMERPUser currentUser, VMPacks packs)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    //更新采购合同的包装资料状态
                    var dtPContract = context.Purchase_Contract.Where(d => d.ID == packs.PurchaseContractID && d.ContractType == (int)ContractTypeEnum.Default).FirstOrDefault();

                    if (dtPContract == null)
                    {
                        return DBOperationStatus.NoAffect;
                    }

                    int affectRows = 0;

                    //保存上传附件列表数据
                    foreach (var item1 in packs.PacksList)
                    {
                        ConstsMethod.SaveFileUpload(currentUser, item1.ID, item1.UpLoadFileList, context, UploadFileType.PacksUploadFiles);
                    }

                    foreach (var item1 in dtPContract.Purchase_Packs)
                    {
                        item1.PacksStatus = packs.PacksStatusID;
                    }

                    dtPContract.PacksStatus = packs.PacksStatusID;
                    dtPContract.PacksUpdateDate = DateTime.Now;

                    //插入更新履历
                    DAL.Purchase_PacksHis efAuditPacksHis = new Purchase_PacksHis();
                    efAuditPacksHis.AuditUserID = currentUser.UserID;
                    efAuditPacksHis.PacksStatus = packs.PacksStatusID;
                    efAuditPacksHis.CreateDate = DateTime.Now;
                    dtPContract.Purchase_PacksHis.Add(efAuditPacksHis);

                    affectRows = context.SaveChanges();

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
        /// 把采购合同的包装资料对应的标签类别名称用指定的符号组合起来
        /// </summary>
        /// <param name="listPacks">采购合同的包装资料</param>
        /// <param name="listDictionary">数据字典数据</param>
        /// <param name="linkMark">联接符号</param>
        /// <returns></returns>
        private string CombinePacksName(IEnumerable<Purchase_Packs> listPacks, IQueryable<Com_DataDictionary> listDictionary, string linkMark)
        {
            string sResult = string.Empty;
            string sData = string.Empty;

            foreach (var p in listPacks)
            {
                var dtData = listDictionary.Where(n => n.Code == p.TagID).FirstOrDefault();
                if (dtData == null)
                {
                    sData = string.Empty;
                }
                else
                {
                    sData = dtData.Name;
                }
                if (string.IsNullOrEmpty(sResult))
                {
                    sResult += sData;
                }
                else
                {
                    sResult += linkMark + sData;
                }
            }

            return sResult;
        }

        /// <summary>
        /// 判断是否代印
        /// </summary>
        /// <param name="pp">采购合同的包装资料</param>
        /// <returns></returns>
        private string IsOutSourcing(IEnumerable<Purchase_Packs> pp)
        {
            string sResult = string.Empty;

            foreach (var item in pp)
            {
                if (item.IsOutsourcing)
                {
                    sResult = "是";
                    break;
                }
                else
                {
                    sResult = "否";
                }
            }

            return sResult;
        }

        /// <summary>
        /// 用状态编号换取状态译文(包装资料)
        /// </summary>
        /// <param name="i">状态编号</param>
        /// <returns></returns>
        public static string GetPacksStatus(int id)
        {
            return CommonCode.GetStatusEnumName(id, typeof(PurchasePacksStatusEnum));
        }

        /// <summary>
        /// 用数据字典数据标记枚举编号换取CH
        /// </summary>
        /// <param name="i">状态编号</param>
        /// <returns></returns>
        public static string GetDictionaryDataFlag(int id)
        {
            return CommonCode.GetStatusEnumName(id, typeof(DataFlagEnum));
        }

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
                        query.PacksStatus += 1;
                        query.PacksUpdateDate = DateTime.Now;

                        context.Purchase_PacksHis.Add(new Purchase_PacksHis()
                        {
                            AuditUserID = currentUser.UserID,
                            CreateDate = DateTime.Now,
                            PacksStatus = query.PacksStatus,
                            ContractsID = id,
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
        /// 计算 内盒标签数量=产品标签数量/内盒率
        /// </summary>
        /// <param name="ProductTagsNumber"></param>
        /// <param name="InnerBoxRate"></param>
        /// <returns></returns>
        public int Calculator_InnerTagsNumber(int ProductTagsNumber, int InnerBoxRate)
        {
            int iMath = 0;
            if (InnerBoxRate > 0)
            {
                iMath = (int)Math.Ceiling((double)ProductTagsNumber / InnerBoxRate);
            }

            return iMath;
        }

        /// <summary>
        /// 计算 外箱标签数量=产品标签数量/外箱率
        /// </summary>
        /// <param name="ProductTagsNumber"></param>
        /// <param name="InnerBoxRate"></param>
        /// <returns></returns>
        public int Calculator_OuterBoxRate(int ProductTagsNumber, int OuterBoxRate)
        {
            int iMath = 0;
            if (OuterBoxRate > 0)
            {
                iMath = (int)Math.Ceiling((double)ProductTagsNumber / OuterBoxRate);
            }
            return iMath;
        }
    }
}