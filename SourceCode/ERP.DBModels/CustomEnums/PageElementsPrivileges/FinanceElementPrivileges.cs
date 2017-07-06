using ERP.Models.CustomAttribute;
using System.ComponentModel;

namespace ERP.Models.CustomEnums.PageElementsPrivileges
{
    public enum FinanceElementPrivileges
    {
        /// <summary>
        /// 维护
        /// </summary>
        [Description("维护")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.FinanceManagement_Maintain_Index })]
        Maintenance = 1 << 0,

        /// <summary>
        /// 编辑
        /// </summary>
        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.FinanceManagement_Maintain_Index })]
        Edit = 1 << 1,

        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.FinanceManagement_Maintain_Index })]
        View = 1 << 2,

        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.FinanceManagement_StatisticsList_Factory })]
        View_ForFactory = 1 << 3,

        /// <summary>
        /// 导出Excel
        /// </summary>
        [Description("导出Excel")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.FinanceManagement_StatisticsList_Factory })]
        Export_Factory = 1 << 4,

        /// <summary>
        /// 导出Excel
        /// </summary>
        [Description("导出Excel")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.FinanceManagement_StatisticsList_Analysis })]
        Export_Analysis = 1 << 5,

        /// <summary>
        /// 导出Excel
        /// </summary>
        [Description("导出Excel")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.FinanceManagement_StatisticsList_SelfExportList })]
        Export_SelfExportList = 1 << 6,

        /// <summary>
        /// 导出Excel
        /// </summary>
        [Description("导出Excel")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.FinanceManagement_StatisticsList_DetailList })]
        Export_DetailList = 1 << 7,
    }
}