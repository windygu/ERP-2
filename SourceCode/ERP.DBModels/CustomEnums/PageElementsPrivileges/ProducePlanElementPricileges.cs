using ERP.Models.CustomAttribute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums.PageElementsPrivileges
{
    public enum ProducePlanElementPricileges
    {
       
        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProduceManagement_Index,ERPPage.ProduceManagement_ApprovalList,ERPPage.ProduceManagement_PassedApprovalList })]
        Watch = 1 << 0,

        /// <summary>
        /// 上传
        /// </summary>
        [Description("上传")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProduceManagement_Index })]
        Upload = 1 << 1,

        /// <summary>
        /// 提交
        /// </summary>
        [Description("提交")]
        [PageElementImpactOnPages(Pages = new[] {ERPPage.ProduceManagement_Index})]
        Submit = 1 << 2,

        /// <summary>
        /// 审核
        /// </summary>
        [Description("审核")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.ProduceManagement_ApprovalList })]
        Check = 1 << 3,

    }
}
