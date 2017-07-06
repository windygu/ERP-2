using ERP.Models.CustomAttribute;
using System.ComponentModel;

namespace ERP.Models.CustomEnums.PageElementsPrivileges
{
    /// <summary>
    /// 出运明细的权限按钮
    /// </summary>
    public enum EncasementElementPrivileges
    {
        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DeliveryManagement_Details_Index, ERPPage.DeliveryManagement_Details_PassedApprovalList })]
        EncasementWatch = 1 << 0,

        /// <summary>
        /// 编辑
        /// </summary>
        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DeliveryManagement_Details_Index })]
        EncasementEdit = 1 << 1,

        /// <summary>
        /// 审核
        /// </summary>
        [Description("审核")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DeliveryManagement_Details_Index })]
        EncasementCheck = 1 << 2,

        /// <summary>
        /// 删除本订单的其他数据
        /// </summary>
        [Description("删除本订单的其他数据")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DeliveryManagement_Details_PassedApprovalList })]
        ClearData = 1 << 3,
    }
}