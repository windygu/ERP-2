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
    /// 清关
    /// </summary>
    public enum InspectionClearanceElementPrivileges
    {
        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_InspectionClearance_Index, ERPPage.DocumentsManagement_InspectionClearance_PassedApprovalList })]
        Watch = 1 << 0,

        /// <summary>
        /// 清关
        /// </summary>
        [Description("清关")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_InspectionClearance_Index })]
        Clearance = 1 << 1,

        /// <summary>
        /// 编辑
        /// </summary>
        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_InspectionClearance_Index })]
        Edit = 1 << 2,

        /// <summary>
        /// 审核
        /// </summary>
        [Description("审核")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_InspectionClearance_Index })]
        Approval = 1 << 3,

        /// <summary>
        /// 上传FCR
        /// </summary>
        [Description("上传FCR")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_InspectionClearance_PassedApprovalList })]
        UpLoadFCR = 1 << 4,

        /// <summary>
        /// 下载
        /// </summary>
        [Description("下载")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_InspectionClearance_PassedApprovalList })]
        DownLoad = 1 << 5,

        /// <summary>
        /// 上传
        /// </summary>
        [Description("上传")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_InspectionClearance_PassedApprovalList })]
        UpLoadModify = 1 << 6,

        /// <summary>
        /// 生成信用证文件
        /// </summary>
        [Description("生成信用证文件")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_InspectionClearance_Index })]
        DownLoad_CreditNumber = 1 << 7,
    }
}