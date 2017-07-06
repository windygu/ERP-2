using ERP.Models.CustomAttribute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums.PageElementsPrivileges
{
    public enum ShippingMarkElementPrivileges
    {
        /// <summary>
        /// 维护
        /// </summary>
        [Description("维护")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_ShippingMark_Index })]
        Maintenance = 1 << 0,

        /// <summary>
        /// 编辑
        /// </summary>
        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_ShippingMark_Index })]
        Edit = 1 << 1,

        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_ShippingMark_Index,
            ERPPage.PurchaseManagement_ShippingMark_PassedCheck })]
        View = 1 << 2,

        /// <summary>
        /// 审核
        /// </summary>
        [Description("审核")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_ShippingMark_Index })]
        Approval = 1 << 3,

        //唛头资料：发给客户确认==》客户确认排版==》通知工厂排版==》我司确认排版==》通知大货印刷

        /// <summary>
        /// 发给客户确认
        /// </summary>
        [Description("发给客户确认")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_ShippingMark_PassedCheck })]
        SendToCustomer = 1 << 4,

        /// <summary>
        /// 客户确认排版
        /// </summary>
        [Description("客户确认排版")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_ShippingMark_PassedCheck })]
        CustomerConfirm = 1 << 5,

        /// <summary>
        /// 通知工厂排版
        /// </summary>
        [Description("通知工厂排版")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_ShippingMark_PassedCheck })]
        NotificationFactory = 1 << 6,

        /// <summary>
        /// 我司确认排版
        /// </summary>
        [Description("我司确认排版")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_ShippingMark_PassedCheck })]
        OurConfirm = 1 << 7,

        /// <summary>
        /// 通知大货印刷
        /// </summary>
        [Description("通知大货印刷")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_ShippingMark_PassedCheck })]
        NotificationPrinted = 1 << 8,

        /// <summary>
        /// 上传
        /// </summary>
        [Description("上传")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.PurchaseManagement_ShippingMark_PassedCheck })]
        UpLoad = 1 << 9,
    }
}