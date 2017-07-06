using ERP.BLL.Consts;
using ERP.BLL.Workflow;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.ProducePlan;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.ERP.ProducePlan
{
    public class ProducePlanServices
    {
        public static string EnumName(int id)
        {
            return CommonCode.GetStatusEnumName(id, typeof(ProducePlanStatusEnum));
        }

        /// <summary>
        /// 查询所有信息
        /// </summary>
        /// <returns></returns>
        public List<DTOProducePlan> GetAll(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows, VMDTOProduceSearch vm_search)
        {
            List<DTOProducePlan> list = new List<DTOProducePlan>();
            totalRows = 0;

            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                try
                {
                    var query = context.Plan_ProducePlan.Where(d => !d.IsDelete);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForProducePlan);

                    if (currentUser.HierachyType == HierachyType.Agency)
                    {
                        var userHierachy = new List<int?>()
                        {
                            currentUser.HierachyID,
                            context.SystemUsers.FirstOrDefault(p => p.HierarchyID == currentUser.HierachyID).Hierarchy.Hierarchy2.HierarchyID
                        };

                        query = query.Where(p => userHierachy.Contains(p.Purchase_Contract.Factory.Hierarchy));
                    }

                    if (vm_search.PageType == PageTypeEnum.PendingMaintenanceList)
                    {
                        query = query.Where(n => n.Status == (int)ProducePlanStatusEnum.PendingUpload || n.Status == (int)ProducePlanStatusEnum.PendingSubmit || n.Status == (int)ProducePlanStatusEnum.NotPassCheck);
                    }
                    if (vm_search.PageType == PageTypeEnum.PendingCheckList)
                    {
                        query = query.Where(n => n.Status == (int)ProducePlanStatusEnum.PendingCheck);
                    }
                    if (vm_search.PageType == PageTypeEnum.PassedCheckList)
                    {
                        query = query.Where(n => n.Status == (int)ProducePlanStatusEnum.PassedCheck);
                    }

                    #region 筛选条件

                    if (!string.IsNullOrEmpty(vm_search.PurchaseNumber))
                    {
                        query = query.Where(p => p.Purchase_Contract.PurchaseNumber.Contains(vm_search.PurchaseNumber));
                    }
                    if (!string.IsNullOrEmpty(vm_search.OrderNumber))
                    {
                        query = query.Where(p => p.OrderList.Contains(vm_search.OrderNumber));
                    }
                    if (!string.IsNullOrEmpty(vm_search.FactoryAbbreviation))
                    {
                        query = query.Where(p => p.Purchase_Contract.Factory.Abbreviation.Contains(vm_search.FactoryAbbreviation));
                    }
                    if (!string.IsNullOrEmpty(vm_search.PurchaseDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.PurchaseDateStart);
                        query = query.Where(p => p.Purchase_Contract.PurchaseDate >= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.PurchaseDateEnd))
                    {
                        DateTime dt = CommonCode.GetDateEnd(vm_search.PurchaseDateEnd);
                        query = query.Where(p => p.Purchase_Contract.PurchaseDate <= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.CustomerCode))
                    {
                        query = query.Where(p => p.Purchase_Contract.Orders_Customers.CustomerCode == vm_search.CustomerCode);
                    }
                    if (!string.IsNullOrEmpty(vm_search.StatusID))
                    {
                        int a = int.Parse(vm_search.StatusID);
                        query = query.Where(p => p.Status == a);
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
                        foreach (var item in dataFromDB)
                        {
                            bool IsHasApprovalPermission = true;

                            #region 判断是否有审批流的权限

                            if (vm_search.PageType == PageTypeEnum.PendingCheckList)//待审核
                            {
                                IsHasApprovalPermission = ApprovalService.HasApprovalPermission(WorkflowTypes.ApprovalProducePlan, currentUser, item.ST_CREATEUSER2 ?? 0, item.ApproverIndex, item.Purchase_Contract.CustomerID);
                            }

                            #endregion 判断是否有审批流的权限

                            list.Add(new DTOProducePlan()
                            {
                                ID = item.ID,
                                PurchaseDate = Utils.DateTimeToStr((context.Purchase_Contract.Where(d => d.ID == item.PurchaseID && d.ContractType == (int)ContractTypeEnum.Default).FirstOrDefault().PurchaseDate)),
                                PurchaseNumber = item.Purchase_Contract.PurchaseNumber,
                                OrderID = item.OrderList,
                                FactoryAbbreviation = context.Factories.Where(n => n.ID == item.Purchase_Contract.FactoryID).FirstOrDefault().Abbreviation,
                                CustomerID = item.Purchase_Contract.CustomerID,
                                CustomerCode = context.Orders_Customers.Find(item.Purchase_Contract.CustomerID).CustomerCode,
                                StatusName = EnumName(item.Status),
                                IsHasApprovalPermission = IsHasApprovalPermission,
                                ApproverIndex = item.ApproverIndex,
                                ST_CREATEUSER2 = item.ST_CREATEUSER2,
                                DT_MODIFYDATEFormatter = Utils.DateTimeToStr2(item.DT_MODIFYDATE),
                            });
                        }
                    }

                    #endregion 给Model赋值
                }
                catch (Exception ex)
                {
                    LogHelper.WriteError(ex);
                }
            }
            return list;
        }

        /// <summary>
        /// 审核通过或者不通过
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        public DBOperationStatus WaitCheck(VMERPUser currentUser, DTOProducePlan vm)
        {
            DBOperationStatus result = default(DBOperationStatus);
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                try
                {
                    var query = context.Plan_ProducePlan.Find(vm.ID);
                    query.Status = vm.Status;
                    query.SomeThing = vm.SomeThing;

                    query.ST_MODIFYUSER = currentUser.UserID;
                    query.DT_MODIFYDATE = DateTime.Now;
                    query.IPAdress = CommonCode.GetIP();

                    DAL.Plan_ProducePlanHistory produce = new Plan_ProducePlanHistory();
                    produce.ProduceID = vm.ID;
                    produce.HistoryProduceResult = vm.Status;
                    produce.HistoryProducePeople = currentUser.UserName;
                    produce.HistorySomeThing = vm.SomeThing;
                    produce.HistoryProduceDate = DateTime.Now;
                    context.Plan_ProducePlanHistory.Add(produce);

                    int length = context.SaveChanges();
                    if (length > 0)
                    {
                        result = DBOperationStatus.Success;

                        ExecuteApproval(vm.ST_CREATEUSER2 ?? 0, vm.ID, "" + vm.SomeThing, query.Status, vm.ST_MODIFYUSER, false, "");//执行审批流
                    }
                    else
                    {
                        result = DBOperationStatus.NoAffect;
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteError(ex);
                }
            }

            return result;
        }

        /// <summary>
        /// 提交审核
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public DBOperationStatus Save(int id, VMERPUser currentUser)
        {
            DBOperationStatus result = default(DBOperationStatus);
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                var query = context.Plan_ProducePlan.Find(id);
                query.Status = (int)ProducePlanStatusEnum.PendingCheck;
                query.DT_MODIFYDATE = DateTime.Now;

                DAL.Plan_ProducePlanHistory history = new Plan_ProducePlanHistory();
                history.HistoryProducePeople = currentUser.UserName;
                history.HistoryProduceDate = DateTime.Now;
                history.ProduceID = id;
                history.HistoryProduceResult = (int)ProducePlanStatusEnum.PendingCheck;
                context.Plan_ProducePlanHistory.Add(history);

                int length = context.SaveChanges();
                if (length > 0)
                {
                    result = DBOperationStatus.Success;
                }
                else
                {
                    result = DBOperationStatus.Failed;
                }
            }

            return result;
        }

        public DTOProducePlan GetDetailByID(VMERPUser currentUser, int id)
        {
            DTOProducePlan vm = null;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Plan_ProducePlan.Find(id);

                    vm = new DTOProducePlan()
                    {
                        ID = query.ID,

                        PurchaseContract = new Purchase.PurchaseContractService().GetDetailByID(currentUser, query.PurchaseID),
                        list_UpLoadFile = ConstsMethod.GetUploadFileList(id, UploadFileType.ProducePlan),
                    };

                    List<DTOProducePlanHistory> list_history = new List<DTOProducePlanHistory>();
                    foreach (var item in query.Plan_ProducePlanHistory)
                    {
                        DTOProducePlanHistory vm_history = new DTOProducePlanHistory();
                        vm_history.ID = item.ID;
                        vm_history.ProducePeople = item.HistoryProducePeople;
                        vm_history.ProduceResultName = EnumName(int.Parse(item.HistoryProduceResult.ToString()));
                        vm_history.ProduceDate = Utils.DateTimeToStr2(item.HistoryProduceDate);
                        vm_history.HistorySomeThing = item.HistorySomeThing;
                        list_history.Add(vm_history);
                    }
                    vm.list_history = list_history;
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
        public VMAjaxProcessResult SaveUpLoad(VMERPUser currentUser, DTOProducePlan vm)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Plan_ProducePlan.Find(vm.ID);
                    if (query != null)
                    {
                        query.Status = vm.ButtonNum;

                        query.ST_CREATEUSER2 = currentUser.UserID;
                        query.ST_MODIFYUSER = currentUser.UserID;
                        query.DT_MODIFYDATE = DateTime.Now;
                        query.UploadTime = DateTime.Now;

                        bool isSuccess = true;
                        if (vm.list_UpLoadFile == null)
                        {
                            isSuccess = false;
                        }
                        else if (vm.list_UpLoadFile.Where(d => !d.IsDelete).Count() == 0)
                        {
                            isSuccess = false;
                        }
                        if (!isSuccess)
                        {
                            result.IsSuccess = false;
                            result.Msg = "上传附件不能为空！";
                            return result;
                        }
                        ConstsMethod.SaveFileUpload(currentUser, vm.ID, vm.list_UpLoadFile, context, UploadFileType.ProducePlan);

                        DAL.Plan_ProducePlanHistory history = new Plan_ProducePlanHistory();
                        history.ProduceID = vm.ID;
                        history.HistoryProduceResult = vm.ButtonNum;
                        history.HistoryProducePeople = currentUser.UserName;
                        history.HistoryProduceDate = DateTime.Now;
                        context.Plan_ProducePlanHistory.Add(history);

                        int affectRows = context.SaveChanges();
                        if (affectRows == 0)
                        {
                            result.IsSuccess = false;
                        }
                        else
                        {
                            result.IsSuccess = true;
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

        #region 审批流

        /// <summary>
        /// 执行审批流
        /// </summary>
        /// <param name="createUserID">创建人</param>
        /// <param name="identityID">主键ID</param>
        /// <param name="CheckSuggest">审批意见</param>
        /// <param name="StatusID">报价单的状态</param>
        /// <param name="UserID">当前用户ID</param>
        private static void ExecuteApproval(int createUserID, int identityID, string CheckSuggest, int StatusID, int UserID, bool historyAdded, string username)
        {
            bool isPass = true;
            if (StatusID == (int)ProducePlanStatusEnum.NotPassCheck)
            {
                isPass = false;
            }

            List<int> validWaitingApproveStatus = new List<int>(){
                            (int)ProducePlanStatusEnum.PendingCheck,
                            (int)ProducePlanStatusEnum.NotPassCheck
                        };
            ApprovalService.ExcuteApproval(new ApprovalInfo
            {
                WorkflowType = WorkflowTypes.ApprovalProducePlan,
                IsPass = isPass,
                IdentityID = identityID,
                ValidWaitingApproveStatus = validWaitingApproveStatus,
                StatusApproving = (int)ProducePlanStatusEnum.PendingCheck,
                StatusNextTo = (int)ProducePlanStatusEnum.PassedCheck,
                StatusRejected = (int)ProducePlanStatusEnum.NotPassCheck,
                ApproveOpinion = CheckSuggest,
                ApproveUserID = UserID,
                LogMethod = () =>
                {
                    return null;
                }
            });
        }

        #endregion 审批流
    }
}