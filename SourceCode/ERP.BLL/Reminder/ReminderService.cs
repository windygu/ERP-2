using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.CustomEnums;
using ERP.Tools.Logs;
using System;
using System.Linq;

namespace ERP.BLL.Reminder
{
    /// <summary>
    /// 消息提醒服务
    /// </summary>
    public class ReminderService
    {
        /// <summary>
        /// 获取待审核总数——报价单
        /// </summary>
        public int GetPendingCheckCountByQuote(VMERPUser currentUser)
        {
            int count = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Quot_Quot.Where(p => !p.IsDelete && p.StatusID == (short)QuoteStatusEnum.PendingCheck);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForQuote);
                    count = query.Count();//获取总条数
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return count;
        }

        /// <summary>
        /// 获取待核算总数——销售订单
        /// </summary>
        public int GetPendingCheckCountByOrder(VMERPUser currentUser)
        {
            int count = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Orders.Where(p => !p.IsDelete && p.OrderStatusID == (short)OrderStatusEnum.PendingApproval);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForOrder);
                    count = query.Count();//获取总条数
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return count;
        }

        /// <summary>
        /// 获取待审批总数——销售订单
        /// </summary>
        public int GetPendingApprovalCountByOrder(VMERPUser currentUser)
        {
            int count = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Orders.Where(p => !p.IsDelete && p.OrderStatusID == (short)OrderStatusEnum.PendingApproval);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForOrder);
                    count = query.Count();//获取总条数
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return count;
        }

        /// <summary>
        /// 获取待审批总数——采购合同
        /// </summary>
        public int GetPendingCheckCountByPurchaseContract(VMERPUser currentUser)
        {
            int count = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Purchase_Contract.Where(d => !d.IsDelete && d.PurchaseStatus == (short)PurchaseStatusEnum.PendingCheck && d.ContractType == (int)ContractTypeEnum.Default);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForPurchase);
                    count = query.Count();//获取总条数
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return count;
        }
    }
}