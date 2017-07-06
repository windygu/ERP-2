using ERP.Models.CustomAttribute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums.PageElementsPrivileges
{
    public enum HsElementsPrivileges
    {
        /// 添加
        /// </summary>
        [Description("添加")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SystemManagement_HSCode })]
        HsAdd = 1 << 0,

        /// <summary>
        /// 编辑
        /// </summary>
        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SystemManagement_HSCode })]
        HsEdit = 1 << 1,

        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SystemManagement_HSCode })]
        HsDelete = 1 << 2,
    }
}