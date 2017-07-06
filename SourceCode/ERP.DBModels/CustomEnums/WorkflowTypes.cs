using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums
{
    public enum WorkflowTypes : short
    {
        /// <summary>
        /// 报价单审批流
        /// </summary>
        ApprovalQuot = 0,

        /// <summary>
        /// 订单核算表审批流
        /// </summary>
        ApprovalOrder = 10,

        /// <summary>
        /// 采购订单-采购合同审批流
        /// </summary>
        ApprovalPurchaseContract = 20,

        /// <summary>
        /// 采购订单-包装资料审批流
        /// </summary>
        ApprovalPacking = 21,

        /// <summary>
        /// 采购订单-代购合同审批流
        /// </summary>
        ApprovalOutsourcingContract = 22,

        /// <summary>
        /// 采购订单-唛头资料审批流
        /// </summary>
        ApprovalShippingMark = 23,

        /// <summary>
        /// 生产计划审批流
        /// </summary>
        ApprovalProducePlan = 30,

        /// <summary>
        /// 三期QC审批流
        /// </summary>
        ApprovalThirdPeriodQC = 40,

        /// <summary>
        /// 出运明细审批流
        /// </summary>
        ApprovalShipping = 50,

        /// <summary>
        /// 订舱管理审批流
        /// </summary>
        ApprovalShipmentOrder = 51,

        /// <summary>
        /// 出运通知审批流
        /// </summary>
        ApprovalShipmentNotification = 52,

        /// <summary>
        /// 报检审批流
        /// </summary>
        ApprovalInspectionReceipt = 60,

        /// <summary>
        /// 报关审批流
        /// </summary>
        ApprovalInspectionCustoms = 61,

        /// <summary>
        /// 清关审批流
        /// </summary>
        ApprovalInspectionClearance = 62,

        /// <summary>
        /// 结汇审批流
        /// </summary>
        ApprovalInspectionExchange = 63,

        /// <summary>
        /// 单证索引审批流
        /// </summary>
        ApprovalDocumentsIndexing = 70,
    }
}