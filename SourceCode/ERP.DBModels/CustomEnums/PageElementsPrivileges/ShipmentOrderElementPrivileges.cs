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
    /// 订舱管理的按钮枚举
    /// </summary>
    public enum ShipmentOrderElementPrivileges
    {
        /// <summary>
        /// 编辑
        /// </summary>
        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DeliveryManagement_ShipmentOrder_Index })]
        Edit = 1 << 0,

        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DeliveryManagement_ShipmentOrder_Index,
            ERPPage.DeliveryManagement_ShipmentOrder_PassedApprovalList })]
        View = 1 << 1,

        /// <summary>
        /// 订舱
        /// </summary>
        [Description("订舱")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DeliveryManagement_ShipmentOrder_Index })]
        Add = 1 << 2,

        /// <summary>
        /// 审核
        /// </summary>
        [Description("审核")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DeliveryManagement_ShipmentOrder_Index })]
        Approval = 1 << 3,

        /// <summary>
        /// 合并订舱
        /// </summary>
        [Description("合并订舱")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.DeliveryManagement_ShipmentOrder_Index })]
        Merge = 1 << 4,
    }
}