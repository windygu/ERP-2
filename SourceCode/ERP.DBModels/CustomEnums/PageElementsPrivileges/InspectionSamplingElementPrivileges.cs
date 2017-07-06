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
    /// 第三方抽检权限
    /// </summary>
    public enum InspectionSamplingElementPrivileges
    {
        /// <summary>
        /// 录入抽检通知
        /// </summary>
        [Description("录入抽检通知")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ThirdParty_Inspection_SamplingNotice })]
        InputNotice = 1 << 0,

        /// <summary>
        /// 编辑
        /// </summary>
        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ThirdParty_Inspection_SamplingNotice })]
        Edit = 1 << 1,

        /// <summary>
        /// 发送抽检通知
        /// </summary>
        [Description("发送抽检通知")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ThirdParty_Inspection_SamplingNotice })]
        SendNotice = 1 << 2,

        /// <summary>
        /// 录入抽检结果
        /// </summary>
        [Description("录入抽检结果")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ThirdParty_Inspection_SamplingNotice })]
        InputResult = 1 << 3,

        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ThirdParty_Inspection_SamplingNotice })]
        View = 1 << 4,

        /// <summary>
        /// 录入抽检费用
        /// </summary>
        [Description("录入抽检费用")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ThirdParty_Inspection_SamplingNotice })]
        InputFees = 1 << 5,
    }
}