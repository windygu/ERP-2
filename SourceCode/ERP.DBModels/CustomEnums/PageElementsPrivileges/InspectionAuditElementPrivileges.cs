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
    /// 第三方验厂权限
    /// </summary>
    public enum InspectionAuditElementPrivileges
    {
        /// <summary>
        /// 录入验厂通知
        /// </summary>
        [Description("录入验厂通知")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ThirdParty_Inspection_AuditNotice })]
        InputNotice = 1 << 0,

        /// <summary>
        /// 编辑
        /// </summary>
        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ThirdParty_Inspection_AuditNotice })]
        Edit = 1 << 1,

        /// <summary>
        /// 发送验厂通知
        /// </summary>
        [Description("发送验厂通知")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ThirdParty_Inspection_AuditNotice })]
        SendNotice = 1 << 2,

        /// <summary>
        /// 录入验厂结果
        /// </summary>
        [Description("录入验厂结果")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ThirdParty_Inspection_AuditNotice })]
        InputResult = 1 << 3,

        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ThirdParty_Inspection_AuditNotice })]
        View = 1 << 4,

        /// <summary>
        /// 录入验厂费用
        /// </summary>
        [Description("录入验厂费用")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ThirdParty_Inspection_AuditNotice })]
        InputFees = 1 << 5,
    }
}