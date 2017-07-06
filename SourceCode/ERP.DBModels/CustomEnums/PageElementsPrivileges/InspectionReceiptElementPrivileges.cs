using ERP.Models.CustomAttribute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums.PageElementsPrivileges
{
    /// <summary>
    /// 报检
    /// </summary>
    public enum InspectionReceiptElementPrivileges
    {
        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_InspectionReceipt_Index, ERPPage.DocumentsManagement_InspectionReceipt_PassedApprovalList })]
        Watch = 1 << 0,

        /// <summary>
        /// 报检
        /// </summary>
        [Description("报检")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_InspectionReceipt_Index })]
        Receipt = 1 << 1,

        /// <summary>
        /// 编辑
        /// </summary>
        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_InspectionReceipt_Index })]
        Edit = 1 << 2,

        /// <summary>
        /// 审核
        /// </summary>
        [Description("审核")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_InspectionReceipt_Index })]
        Audit = 1 << 3,

        /// <summary>
        /// 发送工厂
        /// </summary>
        [Description("发送工厂")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_InspectionReceipt_PassedApprovalList })]
        Sending = 1 << 4,

        /// <summary>
        /// 上传凭条
        /// </summary>
        [Description("上传凭条")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_InspectionReceipt_PassedApprovalList })]
        UploadFile = 1 << 5,

        /// <summary>
        /// 下载
        /// </summary>
        [Description("下载")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_InspectionReceipt_PassedApprovalList })]
        DownLoad = 1 << 6,
    }
}