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
    /// 出货样管理权限
    /// </summary>
    public enum ShipmentSampleElementPrivileges
    {
        /// <summary>
        /// 收回登记
        /// </summary>
        [Description("收回登记")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_ShipmentSampleManagement_Index })]
        Recovery = 1 << 0,

        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_ShipmentSampleManagement_Index,
            ERPPage.PurchaseManagement_ShipmentSampleManagement_HadRecovered})]
        View = 1 << 2,
    }
}