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
    /// 第三方检测权限
    /// </summary>
    public enum InspectionDetectElementPrivileges
    {
        /// <summary>
        /// 录入检测通知
        /// </summary>
        [Description("录入检测通知")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ThirdParty_Inspection_DetectNotice })]
        InputNotice = 1 << 0,

        /// <summary>
        /// 第三方检测 编辑
        /// </summary>
        [Description(" 编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ThirdParty_Inspection_DetectNotice })]
        Edit = 1 << 1,

        /// <summary>
        /// 发送检测通知
        /// </summary>
        [Description("发送检测通知")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ThirdParty_Inspection_DetectNotice })]
        SendNotice = 1 << 2,

        /// <summary>
        /// 录入检测结果
        /// </summary>
        [Description("录入检测结果")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ThirdParty_Inspection_DetectNotice })]
        InputResult = 1 << 3,
        
        /// <summary>
        /// 第三方检测 查看
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ThirdParty_Inspection_DetectNotice })]
        View = 1 << 4,

        /// <summary>
        /// 录入检测费用
        /// </summary>
        [Description("录入检测费用")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ThirdParty_Inspection_DetectNotice })]
        InputFees = 1 << 5,
    }
}