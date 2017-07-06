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
    /// 第三方验货权限
    /// </summary>
    public enum ThirdPartyVerificationElementPrivileges
    {
        /// <summary>
        /// 上传验货报告
        /// </summary>
        [Description("上传验货报告")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ThirdParty_Inspection_Verification })]
        UpLoad = 1 << 0,

        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ThirdParty_Inspection_Verification })]
        View = 1 << 2,
    }
}