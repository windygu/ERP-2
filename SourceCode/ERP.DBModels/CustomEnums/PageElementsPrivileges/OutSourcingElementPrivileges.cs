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
    /// 代印合同
    /// </summary>
    public enum OutSourcingElementPrivileges
    {
        /// <summary>
        /// 新建
        /// </summary>
        [Description("新建")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_OutSourcing_Index })]
        OutAdd = 1 << 0,

        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_OutSourcing_Index })]
        OutDelete = 1 << 1,

        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_OutSourcing_Index, ERPPage.PurchaseManagement_OutSourcing_PassedApproval })]
        OutWatch = 1 << 2,

        /// <summary>
        /// 编辑
        /// </summary>
        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_OutSourcing_Index })]
        OutEdit = 1 << 3,

        /// <summary>
        /// 审核
        /// </summary>
        [Description("审核")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_OutSourcing_Index })]
        OutCheck = 1 << 4,

        /// <summary>
        /// 上传合同
        /// </summary>
        [Description("上传合同")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_OutSourcing_PassedApproval })]
        OutUpload = 1 << 5,

        /// <summary>
        /// 发送合同
        /// </summary>
        [Description("发送合同")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_OutSourcing_PassedApproval })]
        OutDelivery = 1 << 6,
    }
}