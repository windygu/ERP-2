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
    /// 出运通知的按钮枚举
    /// </summary>
    public enum ShipmentNotificationElementPrivileges
    {
        /// <summary>
        /// 维护
        /// </summary>
        [Description("维护")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DeliveryManagement_ShipmentNotification_Index })]
        Maintenance = 1 << 0,

        /// <summary>
        /// 编辑
        /// </summary>
        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DeliveryManagement_ShipmentNotification_Index })]
        Edit = 1 << 1,

        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DeliveryManagement_ShipmentNotification_Index,
            ERPPage.DeliveryManagement_ShipmentNotification_PassedApprovalList })]
        View = 1 << 2,

        /// <summary>
        /// 审核
        /// </summary>
        [Description("审核")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DeliveryManagement_ShipmentNotification_Index })]
        Approval = 1 << 3,

        /// <summary>
        /// 维护拉柜费用
        /// </summary>
        [Description("维护")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DeliveryManagement_ShipmentNotification_RegisterFees })]
        Maintenance_RegisterFees = 1 << 4,

        /// <summary>
        /// 编辑拉柜费用
        /// </summary>
        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DeliveryManagement_ShipmentNotification_RegisterFees })]
        Edit_RegisterFees = 1 << 5,

        /// <summary>
        /// 查看拉柜费用
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DeliveryManagement_ShipmentNotification_RegisterFees })]
        View_RegisterFees = 1 << 6,
    }
}