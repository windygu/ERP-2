using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums
{
    /// <summary>
    /// ShippingMark的枚举类型
    /// </summary>
    public enum ShippingMarkEnum
    {
        [Description("10")]
        DG,

        [Description("20")]
        F20,

        [Description("30")]
        S13,

        [Description("40")]
        S135,

        [Description("50")]
        S164,

        [Description("60")]
        S188,

        [Description("70")]
        S235,

        [Description("80")]
        S60,

        [Description("90")]
        S05,

        [Description("100")]
        S220,

        [Description("110")]
        S10,

        [Description("120")]
        S56,

    }

    /// <summary>
    /// S188唛头的色带图片
    /// </summary>
    public enum ShippingMark_S188_Image
    {
        /// <summary>
        /// 情人节
        /// </summary>
        [Description("Valentines")]
        Valentines,

        /// <summary>
        /// 复活节
        /// </summary>
        [Description("复活节")]
        Easter,

        /// <summary>
        /// 秋天
        /// </summary>
        [Description("秋天")]
        Autumn,

        /// <summary>
        /// 鬼节
        /// </summary>
        [Description("Halloween")]
        Halloween,

        /// <summary>
        /// 圣诞
        /// </summary>
        [Description("Christmas")]
        Christmas,

        /// <summary>
        /// 春天花园
        /// </summary>
        [Description("Lawn & Garden")]
        Spring_Garden,
    }

    /// <summary>
    /// S188唛头的分类图标。Description里面是从数据字典里面季节的其他匹配。Icon的路径名称是枚举里的字符串
    /// </summary>
    public enum ShippingMark_S188_Icon
    {
        /// <summary>
        /// Valentines Candles/Gifts Novelties
        /// </summary>
        [Description("Valentines Candles/Gifts Novelties")]
        Valentines_Candles_Gifts_Novelties,

        /// <summary>
        /// Easter Baskets Pails
        /// </summary>
        [Description("Easter Baskets Pails")]
        Easter_Baskets_Pails,

        /// <summary>
        /// Easter Décor
        /// </summary>
        [Description("Easter Décor")]
        Easter_Décor,

        /// <summary>
        /// Harvest
        /// </summary>
        [Description("Harvest")]
        Harvest,

        /// <summary>
        /// Halloween Candles/Gifts
        /// </summary>
        [Description("Halloween Candles/Gifts")]
        Halloween_Candles_Gifts,

        /// <summary>
        /// Halloween Costume Accessories
        /// </summary>
        [Description("Halloween Costume Accessories")]
        Halloween_Costume_Accessories,

        /// <summary>
        /// Halloween Home Décor
        /// </summary>
        [Description("Halloween Home Décor")]
        Halloween_Home_Décor,

        /// <summary>
        /// Easter Novelties
        /// </summary>
        [Description("Easter Novelties")]
        Easter_Novelties,

        /// <summary>
        /// Halloween Novelties
        /// </summary>
        [Description("Halloween Novelties")]
        Halloween_Novelties,

        /// <summary>
        /// Christmas Ornaments & Garland
        /// </summary>
        [Description("Christmas Ornaments & Garland")]
        Christmas_Ornaments_Garland,

        /// <summary>
        /// Christmas Décor/Candles/Gifts
        /// </summary>
        [Description("Christmas Décor/Candles/Gifts")]
        Christmas_Décor_Candles_Gifts,

        /// <summary>
        /// Christmas Housewares
        /// </summary>
        [Description("Christmas Housewares")]
        Christmas_Housewares,

        /// <summary>
        /// Christmas Santa Hats & Stockings
        /// </summary>
        [Description("Christmas Santa Hats & Stockings")]
        Christmas_Santa_Hats_Stockings,

        /// <summary>
        /// Lawn & Garden Sundries
        /// </summary>
        [Description("Lawn & Garden Sundries")]
        Lawn_Garden_Sundries,

        /// <summary>
        /// Spring & Summer Home Décor
        /// </summary>
        [Description("Spring & Summer Home Décor")]
        Spring_Summer_Home_Décor,
    }

    /// <summary>
    /// S188唛头的Space Code
    /// </summary>
    public enum ShippingMark_S188_SpaceCode
    {
        RR,
        C,
        Q,
        E,
        D,
        F,
        FF,
        JJ,
        U,
    }
}