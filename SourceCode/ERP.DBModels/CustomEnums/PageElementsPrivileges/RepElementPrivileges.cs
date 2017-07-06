using ERP.Models.CustomAttribute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums.PageElementsPrivileges
{
    public enum RepElementPrivileges
    {
        /// <summary>
        /// 创建
        /// </summary>
        [Description("创建")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.RepManagement_List })]
        Create = 1 << 0,

        /// <summary>
        /// 批量删除
        /// </summary>
        [Description("批量删除")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.RepManagement_List })]
        BatchDelete = 1 << 1,

        /// <summary>
        /// 编辑
        /// </summary>
        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.RepManagement_List })]
        Edit = 1 << 2,

        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.RepManagement_List })]
        View = 1 << 3,

        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.RepManagement_List })]
        Delete = 1 << 4
    }
}