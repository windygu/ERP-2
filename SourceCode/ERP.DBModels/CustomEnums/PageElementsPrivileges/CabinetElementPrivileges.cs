using ERP.Models.CustomAttribute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums.PageElementsPrivileges
{
   public enum CabinetElementPrivileges
    {
        [Description("创建")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ShipmentManagement_Cabinet })]
        Create = 1 << 0,

        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ShipmentManagement_Cabinet })]
        Edit = 1 << 1,

        [Description("删除")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ShipmentManagement_Cabinet })]
        delete = 1 << 2,

        [Description("批量删除")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ShipmentManagement_Cabinet })]
        deleteAll = 1 << 3,

        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ShipmentManagement_Cabinet })]
        Watch = 1 << 4,

       
    }
}
