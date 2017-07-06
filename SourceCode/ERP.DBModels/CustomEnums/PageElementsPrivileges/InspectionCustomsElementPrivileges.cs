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
    /// 报关
    /// </summary>
    public enum InspectionCustomsElementPrivileges
    {
        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_InspectionCustoms_Index, ERPPage.DocumentsManagement_InspectionCustoms_PassedApprovalList })]
        Watch = 1 << 0,

        /// <summary>
        /// 报关
        /// </summary>
        [Description("报关")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_InspectionCustoms_Index })]
        Customs = 1 << 1,

        /// <summary>
        /// 编辑
        /// </summary>
        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_InspectionCustoms_Index })]
        Edit = 1 << 2,

        /// <summary>
        /// 审核
        /// </summary>
        [Description("审核")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_InspectionCustoms_Index })]
        Approval = 1 << 3,

        /// <summary>
        /// 下载
        /// </summary>
        [Description("下载")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_InspectionCustoms_PassedApprovalList })]
        DownLoad = 1 << 4
    }
}