using ERP.Models.CustomAttribute;
using System.ComponentModel;

namespace ERP.Models.CustomEnums.PageElementsPrivileges
{
    public enum DocumentsIndexing_ProductFittingElementPrivileges
    {
        /// <summary>
        /// 维护
        /// </summary>
        [Description("维护")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsIndexingManagement_ProductFitting_Maintain })]
        Maintenance = 1 << 0,

        /// <summary>
        /// 编辑
        /// </summary>
        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsIndexingManagement_ProductFitting_Maintain })]
        Edit = 1 << 1,

        /// <summary>
        /// 审核
        /// </summary>
        [Description("审核")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsIndexingManagement_PendingApprovalList })]
        Approval = 1 << 2,

        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsIndexingManagement_ProductFitting_Maintain,
            ERPPage.DocumentsIndexingManagement_ProductFitting_PendingApprovalList,
            ERPPage.DocumentsIndexingManagement_ProductFitting_PassedApprovalList
        })]
        View = 1 << 3,

        /// <summary>
        /// 下载单证附件
        /// </summary>
        [Description("下载单证附件")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsIndexingManagement_ProductFitting_PassedApprovalList })]
        DownLoad = 1 << 4,
    }
}