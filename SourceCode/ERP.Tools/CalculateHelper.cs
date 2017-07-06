using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Tools
{
    public class CalculateHelper
    {
        /// <summary>
        /// 箱数 = 数量/外箱率
        /// </summary>
        /// <param name="Qty"></param>
        /// <param name="OuterBoxRate"></param>
        /// <returns></returns>
        public static int? GetBoxQty(int Qty, int? OuterBoxRate)
        {
            decimal SumBoxQty = 0;
            if (OuterBoxRate.HasValue && OuterBoxRate != 0)
            {
                SumBoxQty = (decimal)Qty / OuterBoxRate ?? 0;
            }
            return (int)Math.Ceiling(SumBoxQty);
        }


        /// <summary>
        /// 外箱体积=（外箱长*外箱宽*外箱高）/1000000，值保留两位小数
        /// </summary>
        /// <param name="OuterLength">外箱长</param>
        /// <param name="OuterWidth">外箱宽</param>
        /// <param name="OuterHeight">外箱高</param>
        /// <returns></returns>
        public static decimal? GetOuterVolume(decimal? OuterLength, decimal? OuterWidth, decimal? OuterHeight)
        {
            return (OuterLength * OuterWidth * OuterHeight / 1000000).Round(2);
        }


        /// <summary>
        /// 外箱材积=（外箱长*外箱宽*外箱高）/1000000 * 35.315m，值保留四位小数
        /// </summary>
        /// <param name="OuterLength">外箱长</param>
        /// <param name="OuterWidth">外箱宽</param>
        /// <param name="OuterHeight">外箱高</param>
        /// <returns></returns>
        public static decimal? GetOuterVolume_CUFT(decimal? OuterLength, decimal? OuterWidth, decimal? OuterHeight)
        {
            return ((OuterLength * OuterWidth * OuterHeight / 1000000) * 35.315m).Round(4);
        }


        /// <summary>
        /// 总体积=（外箱长*外箱宽*外箱高）/1000000 * 箱数，值保留两位小数
        /// </summary>
        /// <param name="OuterLength">外箱长</param>
        /// <param name="OuterWidth">外箱宽</param>
        /// <param name="OuterHeight">外箱高</param>
        /// <param name="BoxQty">箱数</param>
        /// <returns></returns>
        public static decimal? GetSumOuterVolume(decimal? OuterLength, decimal? OuterWidth, decimal? OuterHeight, int? BoxQty)
        {
            return (OuterLength * OuterWidth * OuterHeight / 1000000 * BoxQty).Round(2);
        }

    }
}
