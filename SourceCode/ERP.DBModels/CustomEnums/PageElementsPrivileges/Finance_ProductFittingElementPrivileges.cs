using ERP.Models.CustomAttribute;
using System.ComponentModel;

namespace ERP.Models.CustomEnums.PageElementsPrivileges
{
    public enum Finance_ProductFittingElementPrivileges
    {
        /// <summary>
        /// 维护
        /// </summary>
        [Description("维护")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.FinanceManagement_Maintain_ProductFitting_Index })]
        Maintenance = 1 << 0,

        /// <summary>
        /// 编辑
        /// </summary>
        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.FinanceManagement_Maintain_ProductFitting_Index })]
        Edit = 1 << 1,

        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.FinanceManagement_Maintain_ProductFitting_Index })]
        View = 1 << 2,
        
    }
}