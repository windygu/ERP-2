using ERP.Models.CustomAttribute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums.PageElementsPrivileges
{
    public enum ShipmentAgencyElementPrivileges
    {
        [Description("新增")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ShipmentManagement_Agencies })]
        Create = 1 << 0,

        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ShipmentManagement_Agencies })]
        View = 1 << 1,

        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ShipmentManagement_Agencies })]
        Edit = 1 << 2,

        [Description("删除")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ShipmentManagement_Agencies })]
        Delete = 1 << 3,

        [Description("批量删除")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ShipmentManagement_Agencies })]
        BatchDelete = 1 << 4

    }
}
