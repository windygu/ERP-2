using ERP.Models.CustomAttribute;
using System.ComponentModel;

namespace ERP.Models.CustomEnums.PageElementsPrivileges
{
    /// <summary>
    /// 港杂费发票
    /// </summary>
    public enum PortChargesInvoiceElementPrivileges
    {
        /// <summary>
        /// 维护
        /// </summary>
        [Description("维护")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_PortChargesInvoice })]
        Maintenance = 1 << 0,

        /// <summary>
        /// 编辑
        /// </summary>
        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_PortChargesInvoice })]
        Edit = 1 << 1,

        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DocumentsManagement_PortChargesInvoice })]
        View = 1 << 2,
    }
}