using ERP.BLL.ERP.AdminUser;
using ERP.BLL.Workflow.Approval;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.CustomEnums;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.Workflow
{
    public sealed class ApprovalInfo
    {
        /// <summary>
        /// 审批类型
        /// </summary>
        public WorkflowTypes WorkflowType { get; set; }

        /// <summary>
        /// 当前审批项ID
        /// </summary>
        public int IdentityID { get; set; }

        /// <summary>
        /// 允许进行审核的状态值列表
        /// </summary>
        public List<int> ValidWaitingApproveStatus { get; set; }

        /// <summary>
        /// 表示数据还在审核中的状态值
        /// </summary>
        public int StatusApproving { get; set; }

        /// <summary>
        /// 审批流结束后的状态
        /// </summary>
        public int StatusNextTo { get; set; }

        /// <summary>
        /// 审批拒绝后的状态
        /// </summary>
        public int StatusRejected { get; set; }

        /// <summary>
        /// 日志方法
        /// </summary>
        public Func<object> LogMethod { get; set; }

        // -----------------------以上为审批项信息，以下为该次审批信息----------------------------- //
        /// <summary>
        /// 当前审批人
        /// </summary>
        public int? ApproveUserID { get; set; }

        /// <summary>
        /// 是否通过
        /// </summary>
        public bool? IsPass { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string ApproveOpinion { get; set; }

        public bool IsValid
        {
            get
            {
                return !(ValidWaitingApproveStatus == null ||
                    ValidWaitingApproveStatus.Count <= 0 ||
                    IdentityID == -1 ||
                    StatusApproving == -1 ||
                    StatusNextTo == -1 ||
                    StatusRejected == -1);
            }
        }

        public ApprovalInfo()
        {
            IdentityID = StatusApproving = StatusNextTo = StatusRejected = -1;
        }
    }

    public static class ApprovalService
    {
        /// <summary>
        /// 执行审批流
        /// </summary>
        /// <param name="approvalInfo">审批信息</param>
        public static void ExcuteApproval(ApprovalInfo approvalInfo)
        {
            if (approvalInfo == null || !approvalInfo.IsValid)
                throw new Exception("approvalInfo 不正确");

            try
            {
                IApproval approvaler = ApprovalFactory.GetApproval(approvalInfo);
                approvaler.ExecuteApproval();
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
        }

        /// <summary>
        /// 是否有审批权限。
        /// </summary>
        /// <param name="approvalType">审批类型</param>
        /// <param name="approveUser">当前登录用户</param>
        /// <param name="createUserID">该数据的创建用户</param>
        /// <param name="workflowDetailIndex">审批流的索引</param>
        /// <returns></returns>
        public static bool HasApprovalPermission(WorkflowTypes approvalType, VMERPUser approveUser, int createUserID, int? workflowDetailIndex, int? customerID = null)
        {
            if (approveUser.IsSuperAdmin) return true;
            if (approveUser.UserID == createUserID) return false;
            if (workflowDetailIndex.HasValue && workflowDetailIndex.Value == int.MinValue) return false;    // 注：index当前设计从10开始，审批流结束时为int.MinValue

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    List<WorkflowDetail> details = (from wfd in context.WorkflowDetails where wfd.WorkflowID == (from wf in context.Workflows where wf.WorkflowType == (int)approvalType select wf.WorkflowID).FirstOrDefault() select wfd).ToList();
                    details = workflowDetailIndex.HasValue ?
                       details.Where(wfd => wfd.Index == workflowDetailIndex.Value).ToList() :
                        details.Where(wfd => wfd.Index == details.Select(d => d.Index).First()).ToList();
                    var usrRoles = details.Where(d => d.CurrentOwnerType == 1).Select(d => d.CurrentOwner);

                    var totalPermissionUsrIDs = details.Where(d => d.CurrentOwnerType == 0).Select(d => d.CurrentOwner).Union(
                          context.UserRoles.Where(ur => usrRoles.Contains(ur.RoleID)).Select(ur => ur.UserID)
                         ).ToList();

                    //目前只支持采购合同、代购合同、出运、单证的业务员
                    switch (approvalType)
                    {
                        //case WorkflowTypes.ApprovalQuot:
                        //case WorkflowTypes.ApprovalOrder:
                        case WorkflowTypes.ApprovalPurchaseContract:
                        case WorkflowTypes.ApprovalPacking:
                        case WorkflowTypes.ApprovalShippingMark:
                        case WorkflowTypes.ApprovalOutsourcingContract:
                        case WorkflowTypes.ApprovalProducePlan:
                        case WorkflowTypes.ApprovalShipping:
                        case WorkflowTypes.ApprovalShipmentOrder:
                        case WorkflowTypes.ApprovalShipmentNotification:

                        case WorkflowTypes.ApprovalInspectionReceipt:
                        case WorkflowTypes.ApprovalInspectionCustoms:
                        case WorkflowTypes.ApprovalInspectionClearance:
                        case WorkflowTypes.ApprovalInspectionExchange:

                        case WorkflowTypes.ApprovalDocumentsIndexing:
                            if (customerID.HasValue && (workflowDetailIndex.HasValue && workflowDetailIndex == 10))
                            {
                                var mappings = context.UserCustomerRelationships.Where(p => p.CustomerID == customerID.Value).Select(p => p.UserID);
                                if (mappings != null && mappings.Count() > 0)
                                {
                                    totalPermissionUsrIDs = totalPermissionUsrIDs.Intersect(mappings).ToList();
                                }
                            }

                            break;

                        case WorkflowTypes.ApprovalThirdPeriodQC:
                            if (customerID.HasValue && (workflowDetailIndex.HasValue && workflowDetailIndex == 20))
                            {
                                var mappings = context.UserCustomerRelationships.Where(p => p.CustomerID == customerID.Value).Select(p => p.UserID);
                                if (mappings != null && mappings.Count() > 0)
                                {
                                    totalPermissionUsrIDs = totalPermissionUsrIDs.Intersect(mappings).ToList();
                                }
                            }

                            break;
                        default:
                            break;
                    }

                    if (totalPermissionUsrIDs.Contains(approveUser.UserID))
                    {
                        // 需要处理业务A部（2、4、电商），和B部（1、5）的报检单特殊需求：两个总监能查看所有数据，但是不能互相审批对方负责的部门的报价单
                        // 由于角色名称是手动插入数据库的，暂时也没有界面可以修改，因此这里按角色名称写死做判断，易于实现像这样极个别的特殊需求
                        if (approvalType == WorkflowTypes.ApprovalQuot && approveUser.RoleNames != null && approveUser.RoleNames.Contains("业务总监"))
                        {
                            var isCurrentDeptSupervisor = context.SystemUsers.FirstOrDefault(usr => usr.UserID == createUserID && usr.Hierarchy.Hierarchy2.HierarchyID == approveUser.HierachyID) != null;
                            return isCurrentDeptSupervisor;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
                return false;
            }
        }

        /// <summary>
        /// 获取下一级审批人信息
        /// </summary>
        /// <returns></returns>
        public static List<VMERPUser> GetNextApproverInfos(WorkflowTypes approvalType, int createUserID, int? workflowDetailIndex, int? customerID = null)
        {
            List<VMERPUser> nextApprovers = new List<VMERPUser>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    // 下面的查询比较耗性能，需要缓存一下
                    string cacheID = string.Format("approve_usr_ids_{0}_{1}_{2}", approvalType, createUserID, workflowDetailIndex);
                    MemoryCache cache = MemoryCache.Default;
                    List<int> usrIds = null; // cache[cacheID] as List<int>;
                    if (usrIds == null)
                    {
                        CacheItemPolicy policy = new CacheItemPolicy();
                        policy.SlidingExpiration = TimeSpan.FromHours(1);

                        SystemUser createUserInfo = context.SystemUsers.Include("Hierarchy").Where(p => p.UserID == createUserID).FirstOrDefault();

                        List<WorkflowDetail> details = (from wfd in context.WorkflowDetails where wfd.WorkflowID == (from wf in context.Workflows where wf.WorkflowType == (int)approvalType select wf.WorkflowID).FirstOrDefault() select wfd).ToList();
                        details = workflowDetailIndex.HasValue ?
                           details.Where(wfd => wfd.Index == workflowDetailIndex.Value).ToList() :
                            details.Where(wfd => wfd.Index == details.Select(d => d.Index).First()).ToList();
                        var usrRoles = details.Where(d => d.CurrentOwnerType == 1).Select(d => d.CurrentOwner);

                        usrIds = details.Where(d => d.CurrentOwnerType == 0).Select(d => d.CurrentOwner).Union(
                             context.UserRoles.Where(ur => usrRoles.Contains(ur.RoleID) && ur.ERPRole.Name != "业务总监" && ur.SystemUser.HierarchyID == createUserInfo.HierarchyID).Select(ur => ur.UserID)
                            ).Union(
                             context.UserRoles.Where(ur => usrRoles.Contains(ur.RoleID) && ur.ERPRole.Name == "业务总监" && ur.SystemUser.HierarchyID == createUserInfo.Hierarchy.Hierarchy2.HierarchyID).Select(ur => ur.UserID)
                            ).Union(
                             context.UserRoles.Where(ur => usrRoles.Contains(ur.RoleID) && ur.ERPRole.Name == "总经理").Select(ur => ur.UserID)
                            ).Union(
                             context.UserRoles.Where(ur => usrRoles.Contains(ur.RoleID) && ur.ERPRole.Name == "财务主管").Select(ur => ur.UserID)
                            ).ToList();

                        //目前只支持采购合同、代购合同、出运、单证的业务员
                        switch (approvalType)
                        {
                            //case WorkflowTypes.ApprovalQuot:
                            //case WorkflowTypes.ApprovalOrder:
                            case WorkflowTypes.ApprovalPurchaseContract:
                            case WorkflowTypes.ApprovalPacking:
                            case WorkflowTypes.ApprovalShippingMark:
                            case WorkflowTypes.ApprovalOutsourcingContract:
                            case WorkflowTypes.ApprovalProducePlan:
                            case WorkflowTypes.ApprovalShipping:
                            case WorkflowTypes.ApprovalShipmentOrder:
                            case WorkflowTypes.ApprovalShipmentNotification:

                            case WorkflowTypes.ApprovalInspectionReceipt:
                            case WorkflowTypes.ApprovalInspectionCustoms:
                            case WorkflowTypes.ApprovalInspectionClearance:
                            case WorkflowTypes.ApprovalInspectionExchange:

                            case WorkflowTypes.ApprovalDocumentsIndexing:

                                if (customerID.HasValue && (workflowDetailIndex.HasValue && workflowDetailIndex == 10))
                                {
                                    var mappings = context.UserCustomerRelationships.Where(p => p.CustomerID == customerID.Value).Select(p => p.UserID);
                                    if (mappings != null && mappings.Count() > 0)
                                    {
                                        usrIds = usrIds.Intersect(mappings).ToList();
                                    }
                                }
                                break;

                            case WorkflowTypes.ApprovalThirdPeriodQC:

                                if (customerID.HasValue && (workflowDetailIndex.HasValue && workflowDetailIndex == 20))
                                {
                                    var mappings = context.UserCustomerRelationships.Where(p => p.CustomerID == customerID.Value).Select(p => p.UserID);
                                    if (mappings != null && mappings.Count() > 0)
                                    {
                                        usrIds = usrIds.Intersect(mappings).ToList();
                                    }
                                }
                                break;
                            default:
                                break;
                        }

                        cache.Set(cacheID, usrIds, policy);
                    }

                    var users = context.SystemUsers.Where(usr => usrIds.Contains(usr.UserID) && usr.IsTestAccount == false && usr.Status != 1).ToList();//不显示冻结的用户
                    if (users != null)
                    {
                        users.ForEach(p => nextApprovers.Add(new VMERPUser() { UserID = p.UserID, UserName = p.UserName, DisplayName = p.DisplayName }));
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return nextApprovers;
        }
    }
}