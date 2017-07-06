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
    /// 结汇
    /// </summary>
    public enum InspectionExchangeElementPrivileges
    {
        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_InspectionExchange_Index, ERPPage.DocumentsManagement_InspectionExchange_PassedApprovalList })]
        Watch = 1 << 0,

        /// <summary>
        /// 结汇
        /// </summary>
        [Description("结汇")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_InspectionExchange_Index })]
        Exchange = 1 << 1,

        /// <summary>
        /// 编辑
        /// </summary>
        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_InspectionExchange_Index })]
        Edit = 1 << 2,

        /// <summary>
        /// 审核
        /// </summary>
        [Description("审核")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_InspectionExchange_Index })]
        Approval = 1 << 3,

        /// <summary>
        /// 下载
        /// </summary>
        [Description("下载")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_InspectionExchange_PassedApprovalList })]
        DownLoad = 1 << 4,

        /// <summary>
        /// 上传
        /// </summary>
        [Description("上传")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_InspectionExchange_PassedApprovalList })]
        UpLoadModify = 1 << 5,
    }
}