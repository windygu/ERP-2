using ERP.Models.CustomAttribute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums.PageElementsPrivileges
{
    public enum PacksElementsPrivileges
    {
        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_Packs_PendingApproval, 
            ERPPage.PurchaseManagement_Packs_PassedApproval })]
        View = 1 << 0,

        /// <summary>
        /// 编辑
        /// </summary>
        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_Packs_PendingApproval })]
        Edit = 1 << 1,

        /// <summary>
        /// 审核
        /// </summary>
        [Description("审核")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_Packs_PendingApproval })]
        Check = 1 << 2,

        /// <summary>
        /// 上传
        /// </summary>
        [Description("上传")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_Packs_PassedApproval })]
        Upload = 1 << 3,

        /// <summary>
        /// 下载
        /// </summary>
        [Description("下载")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_Packs_PassedApproval })]
        Download = 1 << 4,

        /// <summary>
        /// 标签已通知
        /// </summary>
        [Description("标签已通知")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_Packs_PassedApproval })]
        HadNotification = 1 << 5,

        /// <summary>
        /// 标签已确认
        /// </summary>
        [Description("标签已确认")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_Packs_PassedApproval })]
        HadConfirm = 1 << 6,

        /// <summary>
        /// 大货印刷完成
        /// </summary>
        [Description("大货印刷完成")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_Packs_PassedApproval })]
        HadFinish = 1 << 7,

    }
}