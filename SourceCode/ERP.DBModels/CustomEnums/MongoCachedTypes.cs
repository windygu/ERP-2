using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums
{
    /// <summary>
    /// Mongo缓存类型
    /// </summary>
    public enum MongoCachedTypes
    {
        /// <summary>
        /// 编辑产品
        /// </summary>
        ProductEditing,

        /// <summary>
        /// 编辑报价单
        /// </summary>
        QuoteEditing,

        /// <summary>
        /// 编辑销售订单
        /// </summary>
        OrderEditing,
    }
}