using ERP.Models.CustomAttribute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums.PageElementsPrivileges
{
    public enum ProductFittingElementPrivileges
    {
        [Description("新建配件产品")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProductManagement_ProductFitting })]
        CreateProduct = 1 << 0,

        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProductManagement_ProductFitting })]
        EditProduct = 1 << 1,

        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProductManagement_ProductFitting })]
        ViewProduct = 1 << 2,

        [Description("删除")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProductManagement_ProductFitting })]
        DeleteProduct = 1 << 3,

        [Description("批量删除")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProductManagement_ProductFitting })]
        BatchDelete = 1 << 4,

    }
}