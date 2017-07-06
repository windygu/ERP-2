using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using ERP.Models.CustomAttribute;

namespace ERP.Models.CustomEnums.PageElementsPrivileges
{
    public enum ExchangeElementPrivileges
    {
        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.AuditClearanceList, ERPPage.AuditedClearanceList })]
        Watch = 1 << 0,

        /// <summary>
        /// 结汇
        /// </summary>
        [Description("结汇")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.AuditClearanceList })]
        Customs = 1 << 1,

        /// <summary>
        /// 编辑
        /// </summary>
        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.AuditClearanceList })]
        Edit = 1 << 2,

        /// <summary>
        /// 审核
        /// </summary>
        [Description("审核")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.AuditedClearanceList })]
        Audit = 1 << 3,

    }
}
