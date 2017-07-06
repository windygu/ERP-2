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
    /// 采购合同的页面权限设置
    /// </summary>
    public enum PurchaseContractElementPrivileges
    {
        /// <summary>
        /// 新建
        /// </summary>
        [Description("新建")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_ContractManagement_Index, })]
        Create = 1 << 0,

        /// <summary>
        /// 编辑
        /// </summary>
        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_ContractManagement_Index })]
        Edit = 1 << 1,

        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_ContractManagement_Index,
            ERPPage.PurchaseManagement_ContractManagement_PassedCheck })]
        View = 1 << 2,

        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_ContractManagement_Index })]
        Delete = 1 << 3,

        /// <summary>
        /// 批量删除
        /// </summary>
        [Description("批量删除")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_ContractManagement_Index })]
        BatchDelete = 1 << 4,

        /// <summary>
        /// 审核
        /// </summary>
        [Description("审核")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_ContractManagement_Index })]
        Check = 1 << 5,

        /// <summary>
        /// 上传回签合同
        /// </summary>
        [Description("上传回签合同")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_ContractManagement_PassedCheck })]
        UpLoad = 1 << 6,

        /// <summary>
        /// 发送合同
        /// </summary>
        [Description("发送合同")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_ContractManagement_PassedCheck })]
        SendContract = 1 << 7,
        
    }
}