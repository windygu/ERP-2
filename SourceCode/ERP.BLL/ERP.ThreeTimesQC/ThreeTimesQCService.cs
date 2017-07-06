using ERP.BLL.Consts;
using ERP.BLL.ERP.Order;
using ERP.BLL.Workflow;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.Product;
using ERP.Models.ThirdPartyVerification;
using ERP.Models.ThreeTimesQC;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ERP.BLL.ERP.ThreeTimesQC
{
    public class ThreeTimesQCService
    {
        private ERP.Dictionary.DictionaryServices _dictionaryServices = new ERP.Dictionary.DictionaryServices();

        #region HelperMethod

        /// <summary>
        /// 获取状态描述的内容
        /// </summary>
        /// <param name="StatusID"></param>
        /// <returns></returns>
        private static string GetStatusEnum_Description(int StatusID)
        {
            return EnumHelper.GetCustomEnumDesc(typeof(ThreeTimesQCStatusEnum), (ThreeTimesQCStatusEnum)StatusID);
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
                    var query = context.Purchase_ThreeTimesQC.Include("Purchase_Contract").Where(d => !d.IsDelete);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForPurchase);

                    if (currentUser.HierachyType == HierachyType.Agency)
                    {
                        var userHierachy = new List<int?>()
                        {
                            currentUser.HierachyID,
                            context.SystemUsers.FirstOrDefault(p => p.HierarchyID == currentUser.HierachyID).Hierarchy.Hierarchy2.HierarchyID
                        };

                        query = query.Where(p => userHierachy.Contains(p.Purchase_Contract.Factory.Hierarchy));
                    }

                    if (vm_search.PageType == PageTypeEnum.PassedCheckList)//已审核
                    {
                        query = query.Where(d => d.StatusID >= (short)ThreeTimesQCStatusEnum.PassedCheck);
                    }
                    else
                    {
                        query = query.Where(d => d.StatusID < (short)ThreeTimesQCStatusEnum.PassedCheck);

                        if (vm_search.PageType == PageTypeEnum.PendingCheckList)
                        {
                            query = query.Where(d => d.ApprovalStatus == 0);
                        }
                        else if (vm_search.PageType == PageTypeEnum.PendingCheckList_Two)
                        {
                            query = query.Where(d => d.ApprovalStatus == 1);
                        }
                        else if (vm_search.PageType == PageTypeEnum.PendingCheckList_Three)
                        {
                            query = query.Where(d => d.ApprovalStatus == 2);
                        }
                    }

                    #region 筛选条件

                    if (!string.IsNullOrEmpty(vm_search.PurchaseNumber))
                    {
                        query = query.Where(d => d.Purchase_Contract.PurchaseNumber.Contains(vm_search.PurchaseNumber));
                    }
                    if (!string.IsNullOrEmpty(vm_search.FactoryAbbreviation))
                    {
                        query = query.Where(d => d.Purchase_Contract.Factory.Abbreviation.Contains(vm_search.FactoryAbbreviation));
                    }
                    if (!string.IsNullOrEmpty(vm_search.PurchaseDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.PurchaseDateStart);
                        query = query.Where(d => d.Purchase_Contract.PurchaseDate >= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.PurchaseDateEnd))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.PurchaseDateEnd);
                        query = query.Where(d => d.Purchase_Contract.PurchaseDate <= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.DateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.DateStart);
                        query = query.Where(d => d.Purchase_Contract.DateStart >= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.DateEnd))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.DateEnd);
                        query = query.Where(d => d.Purchase_Contract.DateStart <= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.CustomerCode))
                    {
                        query = query.Where(p => p.Purchase_Contract.Orders_Customers.CustomerCode == vm_search.CustomerCode);
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
                            int ST_CREATEUSER = entity.ST_CREATEUSER;
                            if (entity.ApproverIndex.HasValue && entity.ApproverIndex.Value == 10)
                            {
                                ST_CREATEUSER = entity.ST_CREATEUSER2 ?? 0;
                            }

                            bool IsHasApprovalPermission = true;

                            #region 判断是否有审批流的权限

                            if (vm_search.PageType != PageTypeEnum.PassedCheckList)
                            {
                                IsHasApprovalPermission = ApprovalService.HasApprovalPermission(WorkflowTypes.ApprovalThirdPeriodQC, currentUser, ST_CREATEUSER, entity.ApproverIndex, entity.Purchase_Contract.CustomerID);
                            }

                            #endregion 判断是否有审批流的权限

                            List<VMUpLoadFile> UpLoadStatus_One = ConstsMethod.GetUploadFileList(entity.ID, UploadFileType.ThreeTimesQC_One);
                            List<VMUpLoadFile> UpLoadStatus_Two = ConstsMethod.GetUploadFileList(entity.ID, UploadFileType.ThreeTimesQC_Two);
                            List<VMUpLoadFile> UpLoadStatus_Three = ConstsMethod.GetUploadFileList(entity.ID, UploadFileType.ThreeTimesQC_Three);
                            listModel.Add(new DTOPurchaseContract()
                            {
                                ID = entity.ID,
                                PurchaseNumber = entity.Purchase_Contract.PurchaseNumber,
                                FactoryAbbreviation = entity.Purchase_Contract.Factory.Abbreviation,
                                CustomerCode = entity.Purchase_Contract.Orders_Customers.CustomerCode,
                                PurchaseDate = Utils.DateTimeToStr(entity.Purchase_Contract.PurchaseDate),
                                AfterDate = entity.Purchase_Contract.AfterDate,
                                PortName = entity.Purchase_Contract.Port,
                                DT_MODIFYDATE = Utils.DateTimeToStr2(entity.DT_MODIFYDATE),
                                DateStart = Utils.DateTimeToStr(entity.Purchase_Contract.DateStart),
                                DateEnd = Utils.DateTimeToStr(entity.Purchase_Contract.DateEnd),
                                UpLoadStatus_One = UpLoadStatus_One.Count() > 0 ? "已上传" : "未上传",
                                UpLoadStatus_Two = UpLoadStatus_Two.Count() > 0 ? "已上传" : "未上传",
                                UpLoadStatus_Three = UpLoadStatus_Three.Count() > 0 ? "已上传" : "未上传",
                                StatusID = entity.StatusID,
                                StatusName = GetStatusEnum_Description((int)entity.StatusID),
                                ApproverIndex = entity.ApproverIndex,
                                ST_CREATEUSER = ST_CREATEUSER,
                                CustomerID = entity.Purchase_Contract.CustomerID,
                                PurchaseStatusID = entity.StatusID,
                                IsHasApprovalPermission = IsHasApprovalPermission,
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

        public string CreatePDF(VMThreeTimesQC vm)
        {
            List<string> filesAsolutePathList = new List<string>();
            List<string> filesPhysicalPathList = new List<string>();
            List<string> list_ShippingMark = new List<string>();

            if (vm.UpLoadFileList_One != null)
            {
                foreach (var item in vm.UpLoadFileList_One.Where(d => !d.IsDelete))
                {
                    string filePath = CopyFile(vm.ID, item);

                    filesAsolutePathList.Add(filePath);
                    filesPhysicalPathList.Add(filePath);
                    list_ShippingMark.Add("前期的三期QC附件");
                }
            }
            if (vm.UpLoadFileList_Two != null)
            {
                foreach (var item in vm.UpLoadFileList_Two.Where(d => !d.IsDelete))
                {
                    string filePath = CopyFile(vm.ID, item);

                    filesAsolutePathList.Add(filePath);
                    filesPhysicalPathList.Add(filePath);
                    list_ShippingMark.Add("中期的三期QC附件");
                }
            }
            if (vm.UpLoadFileList_Three != null)
            {
                foreach (var item in vm.UpLoadFileList_Three.Where(d => !d.IsDelete))
                {
                    string filePath = CopyFile(vm.ID, item);

                    filesAsolutePathList.Add(filePath);
                    filesPhysicalPathList.Add(filePath);
                    list_ShippingMark.Add("尾期的三期QC附件");
                }
            }

            string pdfFileName = CommonCode.CreatePdfList(filesAsolutePathList, filesPhysicalPathList, list_ShippingMark);

            return pdfFileName;
        }

        private string CopyFile(int id, VMUpLoadFile item)
        {
            string thisDic = "/data/Template/Out/ThreeTimesQC/" + id;
            string directoryPath = Utils.GetMapPath(thisDic + "/UploadImage");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string imgPath = ConstsMethod.ReplaceURLToLocalPath(Utils.GetMapPath(item.ServerFileName));
            FileInfo fi = new FileInfo(imgPath);
            if (!File.Exists(directoryPath + "/" + fi.Name))
            {
                fi.CopyTo(directoryPath + "/" + fi.Name);
            }
            return directoryPath + "/" + fi.Name;
        }

        public string MakeExcel(VMERPUser currentUser, int id)
        {
            var vm = GetDetailByID(currentUser, id);
            List<string> filesAsolutePathList = new List<string>();
            List<string> filesPhysicalPathList = new List<string>();
            List<string> list_ShippingMark = new List<string>();

            if (vm.ApprovalStatus == 0)
            {
                if (vm.UpLoadFileList_One != null)
                {
                    foreach (var item in vm.UpLoadFileList_One.Where(d => !d.IsDelete))
                    {
                        string filePath = CopyFile(vm.ID, item);

                        filesAsolutePathList.Add(filePath);
                        filesPhysicalPathList.Add(filePath);
                        list_ShippingMark.Add("前期的三期QC附件");
                    }
                }

            }
            else if (vm.ApprovalStatus == 1)
            {
                if (vm.UpLoadFileList_Two != null)
                {
                    foreach (var item in vm.UpLoadFileList_Two.Where(d => !d.IsDelete))
                    {
                        string filePath = CopyFile(vm.ID, item);

                        filesAsolutePathList.Add(filePath);
                        filesPhysicalPathList.Add(filePath);
                        list_ShippingMark.Add("中期的三期QC附件");
                    }
                }

            }
            else if (vm.ApprovalStatus == 2)
            {
                if (vm.UpLoadFileList_Three != null)
                {
                    foreach (var item in vm.UpLoadFileList_Three.Where(d => !d.IsDelete))
                    {
                        string filePath = CopyFile(vm.ID, item);

                        filesAsolutePathList.Add(filePath);
                        filesPhysicalPathList.Add(filePath);
                        list_ShippingMark.Add("尾期的三期QC附件");
                    }
                }

            }

            string fileName = vm.PurchaseContract.PurchaseNumber + " " + DateTime.Now.ToString("yyyyMMddHHmmss");
            string pdfFileName = CommonCode.CreatePdfList(filesAsolutePathList, filesPhysicalPathList, list_ShippingMark, fileName);

            return pdfFileName;
        }

        public string GetToAddress(int customerID)
        {
            List<string> list_Email = new List<string>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.UserCustomerRelationships.Where(d => d.CustomerID == customerID);
                    var listID = query.Select(d => d.UserID).ToList();

                    var query2 = context.SystemUsers.Where(d => listID.Contains(d.UserID) && (d.Status == (int)AdminUserStatus.NotActived || d.Status == (int)AdminUserStatus.Normal));
                    list_Email = query2.Select(d => d.Email).ToList();

                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return CommonCode.ListToString(list_Email, ";");
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        public VMThreeTimesQC GetDetailByID(VMERPUser currentUser, int id)
        {
            VMThreeTimesQC vm = null;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Purchase_ThreeTimesQC.Find(id);


                    var list_history = new List<ThreeTimesQCHistory>();
                    var query_history = context.Purchase_ThreeTimesQCHistory.Where(d => d.ThreeTimesQCID == query.ID);
                    List<DAL.SystemUser> list_SystemUser = context.SystemUsers.ToList();
                    foreach (var item_history in query_history)
                    {
                        list_history.Add(new ThreeTimesQCHistory()
                        {
                            ST_CREATEUSER = list_SystemUser.Find(d => d.UserID == item_history.ST_CREATEUSER).UserName,
                            DT_CREATEDATE = Utils.DateTimeToStr2(item_history.DT_CREATEDATE),
                            Comment = item_history.Comment,
                            CheckSuggest = item_history.CheckSuggest,
                        });
                    }

                    vm = new VMThreeTimesQC()
                    {
                        ID = query.ID,
                        Comment = query.Comment,
                        Suggest = query.Suggest,
                        PurchaseContract = new Purchase.PurchaseContractService().GetDetailByID(currentUser, query.PurchaseContractID),
                        UpLoadFileList_One = ConstsMethod.GetUploadFileList(id, UploadFileType.ThreeTimesQC_One),
                        UpLoadFileList_Two = ConstsMethod.GetUploadFileList(id, UploadFileType.ThreeTimesQC_Two),
                        UpLoadFileList_Three = ConstsMethod.GetUploadFileList(id, UploadFileType.ThreeTimesQC_Three),
                        list_history = list_history,
                        StatusID = query.StatusID,
                        ApprovalStatus = query.ApprovalStatus,
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
        /// 保存
        /// </summary>
        public VMAjaxProcessResult Save(VMERPUser currentUser, VMThreeTimesQC vm)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Purchase_ThreeTimesQC.Find(vm.ID);
                    if (vm.StatusID == (int)ThreeTimesQCStatusEnum.ReplySuggest)//业务经理/业务总监/总经理 可以回复意见
                    {
                        query.Suggest = vm.Suggest;
                        query.DT_MODIFYDATE = DateTime.Now;

                        DAL.Purchase_ThreeTimesQCHistory history2 = new Purchase_ThreeTimesQCHistory()
                        {
                            DT_CREATEDATE = DateTime.Now,
                            ST_CREATEUSER = currentUser.UserID,
                            IPAddress = CommonCode.GetIP(),
                            Comment = GetStatusEnum_Description((int)vm.StatusID),
                            CheckSuggest = vm.Suggest,
                        };
                        query.Purchase_ThreeTimesQCHistory.Add(history2);

                        context.SaveChanges();
                        result.IsSuccess = true;
                        return result;
                    }

                    bool isUpLoadFileList_One = (vm.UpLoadFileList_One != null && vm.UpLoadFileList_One.Where(d => !d.IsDelete && d.ID <= 0).Count() >= 0);
                    bool isUpLoadFileList_Two = (vm.UpLoadFileList_Two != null && vm.UpLoadFileList_Two.Where(d => !d.IsDelete && d.ID <= 0).Count() >= 0);
                    bool isUpLoadFileList_Three = (vm.UpLoadFileList_Three != null && vm.UpLoadFileList_Three.Where(d => !d.IsDelete && d.ID <= 0).Count() >= 0);

                    string msg = "";
                    if (!isUpLoadFileList_One && query.ApprovalStatus == 0)
                    {
                        msg = "请上传前期附件！";
                    }
                    if (!isUpLoadFileList_Two && query.ApprovalStatus == 1)
                    {
                        msg = "请上传中期附件！";
                    }
                    if (!isUpLoadFileList_Three && query.ApprovalStatus == 2)
                    {
                        msg = "请上传尾期附件！";
                    }

                    if (!string.IsNullOrEmpty(msg))
                    {
                        result.IsSuccess = false;
                        result.Msg = msg;
                        return result;
                    }

                    if (vm.StatusID != (int)ThreeTimesQCStatusEnum.NotPassCheck && vm.StatusID != (int)ThreeTimesQCStatusEnum.PassedCheck)
                    {
                        query.StatusID = vm.StatusID;
                    }

                    string Comment = "";
                    if (vm.StatusID == (int)ThreeTimesQCStatusEnum.PassedCheck || vm.StatusID == (int)ThreeTimesQCStatusEnum.NotPassCheck)
                    {
                        Comment = vm.Comment;
                    }

                    query.Comment = Comment;

                    if (vm.StatusID == (int)ThreeTimesQCStatusEnum.PendingCheck && query.ApproverIndex == null)
                    {
                        query.ST_CREATEUSER2 = currentUser.UserID;
                    }

                    query.DT_MODIFYDATE = DateTime.Now;
                    query.ST_MODIFYUSER = currentUser.UserID;
                    query.IPAddress = CommonCode.GetIP();

                    ConstsMethod.SaveFileUpload(currentUser, vm.ID, vm.UpLoadFileList_One, context, UploadFileType.ThreeTimesQC_One);
                    ConstsMethod.SaveFileUpload(currentUser, vm.ID, vm.UpLoadFileList_Two, context, UploadFileType.ThreeTimesQC_Two);
                    ConstsMethod.SaveFileUpload(currentUser, vm.ID, vm.UpLoadFileList_Three, context, UploadFileType.ThreeTimesQC_Three);

                    DAL.Purchase_ThreeTimesQCHistory history = new Purchase_ThreeTimesQCHistory()
                    {
                        DT_CREATEDATE = DateTime.Now,
                        ST_CREATEUSER = currentUser.UserID,
                        IPAddress = CommonCode.GetIP(),
                        Comment = GetStatusEnum_Description((int)vm.StatusID),
                        CheckSuggest = Comment,
                    };
                    query.Purchase_ThreeTimesQCHistory.Add(history);

                    int affectRows = context.SaveChanges();
                    if (affectRows == 0)
                    {
                        result.IsSuccess = false;
                        result.Msg = "出错了";
                    }
                    else
                    {
                        int ST_CREATEUSER = query.ST_CREATEUSER;
                        if (query.ApproverIndex.HasValue && query.ApproverIndex.Value == 10)
                        {
                            ST_CREATEUSER = query.ST_CREATEUSER2 ?? 0;
                        }

                        ExecuteApproval(ST_CREATEUSER, query.ID, "", vm.StatusID, currentUser.UserID, false);//执行审批流                        

                        result.IsSuccess = true;
                        using (ERPEntitiesNew context2 = new ERPEntitiesNew())
                        {
                            var query2 = context2.Purchase_ThreeTimesQC.Find(vm.ID);
                            if (query2.StatusID == (int)ThreeTimesQCStatusEnum.PassedCheck && query2.ApproverIndex == int.MinValue && query2.ApprovalStatus < 2)
                            {
                                query2.StatusID = 0;
                                query2.ApproverIndex = null;
                                query2.ST_CREATEUSER2 = null;
                                query2.ApprovalStatus += 1;
                                context2.SaveChanges();
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
                    var dataFromDB = context.Purchase_ThreeTimesQC.Find(id);
                    if (dataFromDB != null)
                    {
                        //dataFromDB.StatusID = (int)ThreeTimesQCStatusEnum.ContractSent;
                        dataFromDB.ST_MODIFYUSER = currentUser.UserID;
                        dataFromDB.DT_MODIFYDATE = DateTime.Now;
                        dataFromDB.IPAddress = CommonCode.GetIP();

                        context.Purchase_ThreeTimesQCHistory.Add(new Purchase_ThreeTimesQCHistory()
                        {
                            DT_CREATEDATE = DateTime.Now,
                            ST_CREATEUSER = currentUser.UserID,
                            IPAddress = CommonCode.GetIP(),
                            ThreeTimesQCID = id,
                            Comment = GetStatusEnum_Description((int)ThreeTimesQCStatusEnum.ContractSent),
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


                                if (vm.UpLoadFileList != null)
                                {
                                    foreach (var item in vm.UpLoadFileList)
                                    {
                                        if (!item.IsDelete)
                                        {
                                            string ServerFileName = ConstsMethod.ReplaceURLToLocalPath(item.ServerFileName);
                                            if (File.Exists(ServerFileName))
                                            {
                                                string di_path = Utils.GetMapPath("/data/Template/Out/ThreeTimesQC/" + id);
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
                                    string filePath = MakeExcel(currentUser, id);//生成采购合同的xls
                                    list_Attachs.Add(Utils.GetMapPath(filePath));
                                }

                                result = Email.SendEmail(vm.ToAddress, vm.Subject, vm.BodyContent, list_Attachs, MailType.ThreeTimesQC, currentUser, vm.CcAddress, vm.BccAddress);

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
            if (StatusID == (int)ThreeTimesQCStatusEnum.NotPassCheck)
            {
                isPass = false;
            }

            List<int> validWaitingApproveStatus = new List<int>(){
                            (int)ThreeTimesQCStatusEnum.PendingCheck,
                            (int)ThreeTimesQCStatusEnum.NotPassCheck
                        };
            ApprovalService.ExcuteApproval(new ApprovalInfo
            {
                WorkflowType = WorkflowTypes.ApprovalThirdPeriodQC,
                IsPass = isPass,
                IdentityID = identityID,
                ValidWaitingApproveStatus = validWaitingApproveStatus,
                StatusApproving = (int)ThreeTimesQCStatusEnum.PendingCheck,
                StatusNextTo = (int)ThreeTimesQCStatusEnum.PassedCheck,
                StatusRejected = (int)ThreeTimesQCStatusEnum.NotPassCheck,
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