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
    /// 寄样管理操作按钮权限枚举
    /// </summary>
    public enum SampleElementPrivileges
    {
        /// <summary>
        /// 新建打样
        /// </summary>
        [Description("新建打样")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_SampleManagement_Manufacturing })]
        AddManu = 1 << 0,

        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_SampleManagement_Manufacturing })]
        DelData = 1 << 1,

        /// <summary>
        /// 确认接单
        /// </summary>
        [Description("确认接单")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_SampleManagement_Manufacturing })]
        AcceptedOrders = 1 << 2,

        /// <summary>
        /// 安排生产
        /// </summary>
        [Description("安排生产")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_SampleManagement_Manufacturing })]
        Schedule = 1 << 3,

        /// <summary>
        /// 生产跟踪
        /// </summary>
        [Description("生产跟踪")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_SampleManagement_Manufactured })]
        Tracking = 1 << 4,

        /// <summary>
        /// 样品确认
        /// </summary>
        [Description("样品确认")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_SampleManagement_Manufactured })]
        AffirmSample = 1 << 5,

        /// <summary>
        /// 寄出需求
        /// </summary>
        [Description("寄出需求")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_SampleManagement_Manufactured })]
        SendDemand = 1 << 6,

        /// <summary>
        /// 样品寄出
        /// </summary>
        [Description("样品寄出")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_SampleManagement_Sending })]
        SampleSend = 1 << 7,

        /// <summary>
        /// 确认收货
        /// </summary>
        [Description("确认收货")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_SampleManagement_Sent })]
        AffirmReceving = 1 << 8,

        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_SampleManagement_Manufacturing, 
            ERPPage.SaleManagement_SampleManagement_Manufactured,
            ERPPage.SaleManagement_SampleManagement_Sending,
            ERPPage.SaleManagement_SampleManagement_Sent })]
        ReadOnly = 1 << 9,

        /// <summary>
        /// 编辑样品单
        /// </summary>
        [Description("编辑样品单")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SaleManagement_SampleManagement_Manufacturing })]
        Edit = 1 << 10,

    }
}
