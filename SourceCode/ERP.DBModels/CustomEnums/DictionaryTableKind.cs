using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums
{
    public enum DictionaryTableKind
    {
        /// <summary>
        /// 商品单位
        /// </summary>
        [Description("单位")]
        ProductUnit = 101,

        /// <summary>
        /// 商品样式
        /// </summary>
        [Description("款式")]
        ProductStyle = 102,

        /// <summary>
        /// 出运港
        /// </summary>
        [Description("出运港")]
        OutPort = 103,

        /// <summary>
        /// 包装方式
        /// </summary>
        [Description("包装方式")]
        Packing = 104,

        /// <summary>
        /// 币种
        /// </summary>
        [Description("币种")]
        Currency = 105,

        ///<summary>
        ///Packaging Type
        ///</summary>
        [Description("Packaging Type")]
        PaType = 106,

        ///<summary>
        ///报检项目
        ///</summary>
        [Description("报检项目")]
        IsCheck = 107,

        ///<summary>
        ///付款方式
        ///</summary>
        [Description("付款方式")]
        ExchangeType = 108,

        ///<summary>
        ///目的港
        ///</summary>
        [Description("目的港")]
        GoalPort = 109,

        ///<summary>
        ///季节
        ///</summary>
        [Description("季节")]
        Season = 110,

        ///<summary>
        ///部门
        ///</summary>
        [Description("部门")]
        Department = 111,

        ///<summary>
        ///贸易方式
        ///</summary>
        [Description("贸易方式")]
        TradeType = 112,

        ///<summary>
        ///颜色
        ///</summary>
        [Description("颜色")]
        Color = 113,

        ///<summary>
        ///中转港
        ///</summary>
        [Description("中转港")]
        TransshipmentPort = 114,
    }
}