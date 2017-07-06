using ERP.Models.CustomAttribute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums.PageElementsPrivileges
{
    public enum ProductListElementPrivileges
    {
        [Description("新建产品")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProductManagement_Info })]
        CreateProduct = 1 << 0,

        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProductManagement_Info, ERPPage.ProductManagement_QuoteProduct_Management })]
        EditProduct = 1 << 1,

        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProductManagement_Info, ERPPage.ProductManagement_QuoteProduct_Management })]
        ViewProduct = 1 << 2,

        [Description("删除")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProductManagement_Info, ERPPage.ProductManagement_QuoteProduct_Management })]
        DeleteProduct = 1 << 3,

        [Description("批量删除")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProductManagement_Info })]
        BatchDelete = 1 << 4,

        [Description("导出产品资料")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProductManagement_Info })]
        ExportProductExcel = 1 << 5,

        [Description("查看产品目录")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProductManagement_Info })]
        ViewProductCatelog = 1 << 6,

        [Description("查看标签目录")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProductManagement_Info })]
        ViewTagCatelog = 1 << 7,

        [Description("下载标签")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProductManagement_Info })]
        DownloadCatelog = 1 << 8,

        /// <summary>
        /// 批量导入产品
        /// </summary>
        [Description("批量导入产品")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProductManagement_Info })]
        BatchImport = 1 << 9,

        [Description("添加报价产品")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProductManagement_QuoteProduct_Management })]
        CreateAsQuote = 1 << 10,

        [Description("报价产品》导出产品资料")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProductManagement_QuoteProduct_Management })]
        ExportProductExcelForQuote = 1 << 11,

        [Description("报价产品》查看产品目录")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProductManagement_QuoteProduct_Management })]
        ViewProductCatelogForQuote = 1 << 12,
    }
}