using ERP.Models.CustomAttribute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums.PageElementsPrivileges
{
    public enum ProductMixedElementPrivileges
    {
        [Description("新建混装产品")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProductManagement_ProductMixed })]
        CreateProduct = 1 << 0,

        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProductManagement_ProductMixed })]
        EditProduct = 1 << 1,

        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProductManagement_ProductMixed })]
        ViewProduct = 1 << 2,

        [Description("删除")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProductManagement_ProductMixed })]
        DeleteProduct = 1 << 3,

        [Description("批量删除")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProductManagement_ProductMixed })]
        BatchDelete = 1 << 4,

        [Description("导出产品资料")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProductManagement_ProductMixed })]
        ExportProductExcel = 1 << 5,

        [Description("查看产品目录")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProductManagement_ProductMixed })]
        ViewProductCatelog = 1 << 6,

        [Description("查看标签目录")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProductManagement_ProductMixed })]
        ViewTagCatelog = 1 << 7,

        [Description("下载标签")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProductManagement_ProductMixed })]
        DownloadCatelog = 1 << 8,

        ///// <summary>
        ///// 批量导入产品
        ///// </summary>
        //[Description("批量导入产品")]
        //[PageElementImpactOnPages(Pages = new[] { ERPPage.ProductManagement_ProductMixed })]
        //BatchImport = 1 << 9,


    }
}