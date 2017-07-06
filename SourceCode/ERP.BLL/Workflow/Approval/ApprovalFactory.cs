using ERP.Models.CustomEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.Workflow.Approval
{
    public static class ApprovalFactory
    {
        public static IApproval GetApproval(ApprovalInfo approvalInfo)
        {
            switch (approvalInfo.WorkflowType)
            {
                case WorkflowTypes.ApprovalQuot:
                    return new ApprovalQuot(approvalInfo);

                case WorkflowTypes.ApprovalOrder:
                    return new ApprovalOrder(approvalInfo);

                case WorkflowTypes.ApprovalPurchaseContract:
                    return new ApprovalPurchaseContract(approvalInfo);

                case WorkflowTypes.ApprovalPacking:
                    return new ApprovalPacking(approvalInfo);

                case WorkflowTypes.ApprovalOutsourcingContract:
                    return new ApprovalOutsourcingContract(approvalInfo);

                case WorkflowTypes.ApprovalProducePlan:
                    return new ApprovalProducePlan(approvalInfo);

                case WorkflowTypes.ApprovalThirdPeriodQC:
                    return new ApprovalThirdPeriodQC(approvalInfo);

                case WorkflowTypes.ApprovalShipping:
                    return new ApprovalShipping(approvalInfo);

                case WorkflowTypes.ApprovalShipmentOrder:
                    return new ApprovalShipmentOrder(approvalInfo);

                case WorkflowTypes.ApprovalShipmentNotification:
                    return new ApprovalShipmentNotification(approvalInfo);

                case WorkflowTypes.ApprovalShippingMark:
                    return new ApprovalShippingMark(approvalInfo);

                case WorkflowTypes.ApprovalInspectionReceipt:
                    return new ApprovalInspectionReceipt(approvalInfo);

                case WorkflowTypes.ApprovalInspectionCustoms:
                    return new ApprovalInspectionCustoms(approvalInfo);

                case WorkflowTypes.ApprovalInspectionClearance:
                    return new ApprovalInspectionClearance(approvalInfo);

                case WorkflowTypes.ApprovalInspectionExchange:
                    return new ApprovalInspectionExchange(approvalInfo);

                case WorkflowTypes.ApprovalDocumentsIndexing:
                    return new ApprovalDocumentsIndexing(approvalInfo);

                default:
                    break;
            }
            return null;
        }
    }
}