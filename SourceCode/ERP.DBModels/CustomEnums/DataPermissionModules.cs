using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums
{
    /// <summary>
    /// 这个枚举值和数据库中的[Auth].[SystemUserDataPermissions]中的字段相对应，这样做的目的是比较灵活
    /// </summary>
    public enum DataPermissionModules
    {
        /// <summary>
        /// 产品
        /// </summary>
        [Description("产品")]
        ForProduct,

        /// <summary>
        /// 报价
        /// </summary>
        [Description("报价")]
        ForQuote,

        /// <summary>
        /// 寄样
        /// </summary>
        [Description("寄样")]
        ForSample,

        /// <summary>
        /// 销售
        /// </summary>
        [Description("销售")]
        ForOrder,

        /// <summary>
        /// 采购
        /// </summary>
        [Description("采购")]
        ForPurchase,

        /// <summary>
        /// 包装
        /// </summary>
        [Description("包装")]
        ForPacks,

        /// <summary>
        /// 代印
        /// </summary>
        [Description("代印")]
        ForOutsourcing,

        /// <summary>
        /// 工厂
        /// </summary>
        [Description("工厂")]
        ForFactory,

        /// <summary>
        /// 客户
        /// </summary>
        [Description("客户")]
        ForCustomer,

        /// <summary>
        /// 船代
        /// </summary>
        [Description("船代")]
        ForShipmentAgency,

        /// <summary>
        /// 出运
        /// </summary>
        [Description("出运")]
        ForDelivery,

        /// <summary>
        /// 报检
        /// </summary>
        [Description("报检")]
        ForInspectionReceipt,

        /// <summary>
        /// 报关
        /// </summary>
        [Description("报关")]
        ForInspectionCustoms,

        /// <summary>
        /// 清关
        /// </summary>
        [Description("清关")]
        ForInspectionClearance,

        /// <summary>
        /// 结汇
        /// </summary>
        [Description("结汇")]
        ForInspectionExchange,

        /// <summary>
        /// 生产计划
        /// </summary>
        [Description("生产")]
        ForProducePlan,

        /// <summary>
        /// 第三方检验
        /// </summary>
        [Description("第三方检验")]
        ForThirdParty,

        /// <summary>
        /// 唛头资料
        /// </summary>
        [Description("唛头资料")]
        ForShippingMark,

        /// <summary>
        /// 单证索引
        /// </summary>
        [Description("单证索引")]
        ForDocumentsIndexing,

        /// <summary>
        /// 财务
        /// </summary>
        [Description("财务")]
        ForFinance,
    }
}