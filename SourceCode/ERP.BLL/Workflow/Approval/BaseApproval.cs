using ERP.DAL;
using ERP.Models.Common;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.Workflow.Approval
{
    public abstract class BaseApproval : IApproval
    {
        protected ApprovalInfo approvalInfo;
        protected ERPEntitiesNew context;

        // 审批项信息，具体获得依靠子类
        // 报价单、销售订单等等
        public abstract object ItemInfo { get; }
        public abstract int ItemCreateUserID { get; }
        public abstract int ItemStatus { get; set; }
        public abstract int? ItemApproverIndex { get; set; }

        protected abstract void DoLogMethod();
        protected abstract void DoSaveItemInfo();
        
        public BaseApproval(ApprovalInfo info)
        {
            approvalInfo = info;
            context = new ERPEntitiesNew();
        }

        ~BaseApproval() {
            context.Dispose();
        }

        public void ExecuteApproval()
        {
            try
            {
                if (ItemInfo == null)
                {
                    LogApproval("数据库中找不到要审核的记录", -1, -1, null, false);
                    return;
                }

                if (!approvalInfo.ValidWaitingApproveStatus.Contains(ItemStatus))
                {
                    LogApproval("当前状态不需要审批", ItemCreateUserID, ItemStatus, ItemApproverIndex, false);
                    return;
                }
                if (approvalInfo.LogMethod != null)
                {
                    DoLogMethod();
                }
                if (!approvalInfo.IsPass.HasValue || !approvalInfo.IsPass.Value)
                {
                    LogApproval("审批未通过", ItemCreateUserID, ItemStatus, ItemApproverIndex, true);
                    ItemStatus = approvalInfo.StatusRejected;
                    ItemApproverIndex = null; // 重置审批流
                    DoSaveItemInfo();
                    return;
                }
                List<WorkflowDetail> workflowDetails = null;
                workflowDetails = context.WorkflowDetails.Where(wfd => wfd.WorkflowID == context.Workflows.Where(wf => wf.WorkflowType == (int)approvalInfo.WorkflowType).Select(wf => wf.WorkflowID).FirstOrDefault()).ToList();
                // TODO 审批条件暂时不实现(当前用户审批完，直接 OVER 整个审批流)，直接开始逐级审批
                LogApproval("开始逐级审批", ItemCreateUserID, ItemStatus, ItemApproverIndex, true);
                int? nextApproverIndex = null;
                int? resetStatusTo = null;
                ApprovalCore(workflowDetails, ItemCreateUserID, ItemStatus, ItemApproverIndex, out nextApproverIndex, out resetStatusTo);
                if (resetStatusTo.HasValue)
                    ItemStatus = resetStatusTo.Value;
                if (nextApproverIndex.HasValue)
                    ItemApproverIndex = nextApproverIndex.Value;
                DoSaveItemInfo();
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
        }

        /// <summary>
        /// 核心方法，没有特殊需求时一般不用修改，这个方法只有调用的是通过审批时才会执行，否决不走审批流
        /// </summary>
        /// <param name="userCurrentWorkflowDetails">用户的审批流明细列表</param>
        /// <param name="createUserID">数据创建者的的ID</param>
        /// <param name="currentStatus">数据当前的状态值</param>
        /// <param name="approverIndex">当前进行到的审批流序号</param>
        /// <param name="nextApproverIndex">返回下一个审批流序号，用来更新当前数据的当前审批者序号字段</param>
        /// <param name="resetStatusTo">返回当前数据记录需要变更到的状态值，用来更新当前数据的状态字段</param>
        protected void ApprovalCore(List<WorkflowDetail> userCurrentWorkflowDetails, int createUserID, int currentStatus, int? approverIndex, out int? nextApproverIndex, out int? resetStatusTo)
        {
            nextApproverIndex = null;
            resetStatusTo = null;

            if (userCurrentWorkflowDetails == null || userCurrentWorkflowDetails.Count<=0)
            {
                resetStatusTo = approvalInfo.StatusNextTo; // 如果没有审批流，状态变为待审或者在审状态之后的状态
                nextApproverIndex = int.MinValue;
                LogApproval("没有找到用户对应的审批流信息，跳出审批流，并结束审批状态", createUserID, currentStatus, approverIndex);
                return;
            }

            // 若还没审批人则从头开始进入审批流
            if (!approverIndex.HasValue)
            {
                int? findedNextIndex = null;
                // 创建者若是中间层级，取得当前用户所在层级号
                // 注：创建者有可能在工作流中有多个位置，所以需要根据用户ID和角色，查出所有的workflowdetail
                List<int> roleList = context.UserRoles.Where(ur => ur.UserID == createUserID).Select(ur => ur.RoleID).ToList();
                List<WorkflowDetail> ownDetails = userCurrentWorkflowDetails.FindAll(d => (d.CurrentOwnerType == 0 && d.CurrentOwner == createUserID) ||
                (d.CurrentOwnerType == 1&& roleList.Contains(d.CurrentOwner)));
                if (ownDetails.Count > 0)
                {
                    int createrHighestIndex = ownDetails.OrderByDescending(d => d.Index).First().Index;
                    WorkflowDetail nextDetail = userCurrentWorkflowDetails.Where(d => d.Index > createrHighestIndex).OrderBy(d => d.Index).FirstOrDefault();
                    findedNextIndex = nextDetail == null ? int.MinValue : nextDetail.Index;
                }
                // 审批层级设置为创建者的下一级，如果为空，直接结束审批流(由顶层创建)
                if (findedNextIndex.HasValue)
                { 
                    resetStatusTo = findedNextIndex.Value == int.MinValue ? approvalInfo.StatusNextTo : approvalInfo.StatusApproving;
                    nextApproverIndex = findedNextIndex;
                    string msg = findedNextIndex.Value == int.MinValue ? "检测到创建者为审批流顶层，自动结束审批流" :
                        string.Format("检测到由中间层级创建，自动进入下一级，设置状态值为：{0}，当前审批序列为{1}", resetStatusTo.Value, nextApproverIndex.Value);
                    LogApproval(msg, createUserID, currentStatus, approverIndex);
                    return;
                }

                var firstIndex = userCurrentWorkflowDetails.OrderBy(p => p.Index).FirstOrDefault().Index;
                resetStatusTo = approvalInfo.StatusApproving; // 如果还有审批流程，则数据状态还是待审或者在审状态
                nextApproverIndex = firstIndex;
                LogApproval(string.Format("进入审批流，设置状态值为：{0}，当前审批序列为{1}", resetStatusTo.Value, nextApproverIndex.Value), createUserID, currentStatus, approverIndex);
                return;
            }

            var currentLevelApprovels = userCurrentWorkflowDetails.Where(p => p.Index == approverIndex.Value).ToList();
            // 没有找到当前层级审批人，设置审批流结束
            if (currentLevelApprovels == null || currentLevelApprovels.Count <= 0)
            {
                resetStatusTo = approvalInfo.StatusNextTo;
                nextApproverIndex = int.MinValue;
                LogApproval(string.Format("没有找到更多审批流节点，审批流结束，设置状态值为：{0}，当前审批序列为{1}", resetStatusTo.Value, nextApproverIndex.Value), createUserID, currentStatus, approverIndex);
                return;
            }

            // 暂时不处理需要所有人全部通过的情况，该层级有人审批通过就进入下一级
            var nextApprover = userCurrentWorkflowDetails.OrderBy(p => p.Index).FirstOrDefault(p => p.Index > approverIndex.Value);
            // 没有找到下一级审批流，OVER审批流
            if (nextApprover == null)
            {
                resetStatusTo = approvalInfo.StatusNextTo;
                nextApproverIndex = int.MinValue;
                LogApproval(string.Format("没有找到下一级审批流，设置状态值为：{0}，当前审批序列为{1}", resetStatusTo.Value, nextApproverIndex.Value), createUserID, currentStatus, approverIndex);
                return;
              }

            // 如果还有审批流程，则还是待审或者在审状态
            resetStatusTo = approvalInfo.StatusApproving;
            nextApproverIndex = nextApprover.Index;
            LogApproval(string.Format("找到下一级审批流，设置状态值为：{0}，当前审批序列为{1}", resetStatusTo.Value, nextApproverIndex.Value), createUserID, currentStatus, approverIndex);
        }

        protected void LogApproval(string msg, int createUserID, int currentStatus, int? currentApproveIndex, bool recordOpinion = true)
        {
            try
            {
                var dto = new DTOApprovalLogs()
                {
                    DateTime = DateTime.Now,
                    ApprovalType = approvalInfo.WorkflowType,
                    Message = msg,
                    CurrentyEntity = new { UserID = createUserID, IdentityID = approvalInfo.IdentityID },
                    ApproveOpinion = recordOpinion ? approvalInfo.ApproveOpinion : "",
                    ApproverID = approvalInfo.ApproveUserID,
                    CurrentStatus = currentStatus,
                    CurrentApproveIndex = currentApproveIndex
                };
                var dtoJSON = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
                LogHelper.WriteApproval(dtoJSON);
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
        }
    }
}
