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
    /// 三期QC验货权限
    /// </summary>
    public enum ThreeTimesQCElementPricileges
    {
        ///// <summary>
        ///// 维护
        ///// </summary>
        //[Description("维护")]
        //[PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_ThreeTimesQCManagement_Index })]
        //Maintenance = 1 << 0,

        /// <summary>
        /// 编辑
        /// </summary>
        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_ThreeTimesQCManagement_Index,
            ERPPage.PurchaseManagement_ThreeTimesQCManagement_Two,
            ERPPage.PurchaseManagement_ThreeTimesQCManagement_Three})]
        Edit = 1 << 1,

        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_ThreeTimesQCManagement_Index,
            ERPPage.PurchaseManagement_ThreeTimesQCManagement_Two,
            ERPPage.PurchaseManagement_ThreeTimesQCManagement_Three,
            ERPPage.PurchaseManagement_ThreeTimesQCManagement_PassedApproval})]
        View = 1 << 2,

        /// <summary>
        /// 审核
        /// </summary>
        [Description("审核")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_ThreeTimesQCManagement_Index,
            ERPPage.PurchaseManagement_ThreeTimesQCManagement_Two,
            ERPPage.PurchaseManagement_ThreeTimesQCManagement_Three})]
        Approval = 1 << 3,

        /// <summary>
        /// 回复意见
        /// </summary>
        [Description("回复意见")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_ThreeTimesQCManagement_Index,
            ERPPage.PurchaseManagement_ThreeTimesQCManagement_Two,
            ERPPage.PurchaseManagement_ThreeTimesQCManagement_Three,
            ERPPage.PurchaseManagement_ThreeTimesQCManagement_PassedApproval})]
        ReplySuggest = 1 << 4,

        /// <summary>
        /// 发送合同
        /// </summary>
        [Description("发送合同")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_ThreeTimesQCManagement_Index,
            ERPPage.PurchaseManagement_ThreeTimesQCManagement_Two,
            ERPPage.PurchaseManagement_ThreeTimesQCManagement_Three})]
        SendContract = 1 << 5,
    }
}