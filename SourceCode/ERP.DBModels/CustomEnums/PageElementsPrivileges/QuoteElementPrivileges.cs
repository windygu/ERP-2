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
    /// 报价单的权限
    /// </summary>
    public enum QuoteElementPrivileges
    {
        /// <summary>
        /// 新建
        /// </summary>
        [Description("新建")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_QuoteManagement_QuoteList })]
        Create = 1 << 0,

        /// <summary>
        /// 编辑
        /// </summary>
        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_QuoteManagement_QuoteList })]
        Edit = 1 << 1,

        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_QuoteManagement_QuoteList,
            ERPPage.SaleManagement_QuoteManagement_QuoteApproval })]
        View = 1 << 2,

        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_QuoteManagement_QuoteList })]
        Delete = 1 << 3,

        /// <summary>
        /// 批量删除
        /// </summary>
        [Description("批量删除")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_QuoteManagement_QuoteList })]
        BatchDelete = 1 << 4,

        /// <summary>
        /// 审核
        /// </summary>
        [Description("审核")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_QuoteManagement_QuoteApproval })]
        Check = 1 << 5,

        /// <summary>
        /// 重新报价
        /// </summary>
        [Description("重新报价")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_QuoteManagement_QuoteList })]
        ReQuote = 1 << 6,

        /// <summary>
        /// 发送邮件给客户
        /// </summary>
        [Description("发送邮件给客户")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_QuoteManagement_QuoteList })]
        SendEmail = 1 << 7,

        /// <summary>
        /// 复制报价单
        /// </summary>
        [Description("复制报价单")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_QuoteManagement_QuoteList })]
        CopyQuote = 1 << 8,

        /// <summary>
        /// 查看产品图片
        /// </summary>
        [Description("查看产品图片")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_QuoteManagement_QuoteList })]
        ViewProductList = 1 << 9,

        /// <summary>
        /// 确认
        /// </summary>
        [Description("确认")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_QuoteManagement_QuoteList })]
        ConfirmQuote = 1 << 10,

        /// <summary>
        /// 作废
        /// </summary>
        [Description("作废")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_QuoteManagement_QuoteList })]
        Abandon = 1 << 11,
    }
}