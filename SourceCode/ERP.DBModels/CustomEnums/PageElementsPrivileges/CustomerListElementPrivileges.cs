using ERP.Models.CustomAttribute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums.PageElementsPrivileges
{
    public enum CustomerListElementPrivileges
    {
        [Description("创建")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.CustomerManagement_List })]
        CreateCustomer = 1 << 0,

        [Description("批量删除")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.CustomerManagement_List })]
        BatchDelete = 1 << 1,

        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.CustomerManagement_List })]
        EditCustomer = 1 << 2,

        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.CustomerManagement_List })]
        ViewCustomer = 1 << 3,

        [Description("删除")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.CustomerManagement_List })]
        DeleteCustomer = 1 << 4
    }
}