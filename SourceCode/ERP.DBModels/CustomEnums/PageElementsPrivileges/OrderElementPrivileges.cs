using ERP.Models.CustomAttribute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums.PageElementsPrivileges
{
    public enum OrderElementPrivileges
    {
        /// <summary>
        /// 新建
        /// </summary>
        [Description("新建")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_OrderManagement_Index })]
        Create = 1 << 0,

        /// <summary>
        /// 编辑
        /// </summary>
        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_OrderManagement_Index })]
        Edit = 1 << 1,

        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_OrderManagement_Index,
            ERPPage.SaleManagement_OrderManagement_PassedApproval })]
        View = 1 << 2,

        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_OrderManagement_Index })]
        Delete = 1 << 3,

        /// <summary>
        /// 批量删除
        /// </summary>
        [Description("批量删除")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_OrderManagement_Index })]
        BatchDelete = 1 << 4,

        /// <summary>
        /// 审批
        /// </summary>
        [Description("审批")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_OrderManagement_Index })]
        Check = 1 << 5,

        /// <summary>
        /// 查看产品图片
        /// </summary>
        [Description("查看产品图片")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_OrderManagement_Index })]
        ViewProductList = 1 << 6,
        
    }
}