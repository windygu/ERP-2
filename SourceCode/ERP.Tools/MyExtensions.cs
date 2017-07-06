using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Tools
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class MyExtensions
    {
        /// <summary>
        /// 对小数进行取位，默认保留2个小数点
        /// </summary>
        /// <param name="dcmal"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static decimal Round(this decimal dcmal, int point = 2)
        {
            return Math.Round(dcmal, point);
        }

        /// <summary>
        /// 对小数进行取位，默认保留2个小数点
        /// </summary>
        /// <param name="dcmal"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static decimal Round(this decimal? dcmal, int point = 2)
        {
            return Math.Round(dcmal ?? 0, point);
        }

        /// <summary>
        /// 对小数进行取位，默认保留2个小数点
        /// </summary>
        /// <param name="dcmal"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static double Round(this double dcmal, int point = 2)
        {
            return Math.Round(dcmal, point);
        }

        /// <summary>
        /// 单位换算：从厘米到英尺
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static double CmToInch(this double db)
        {
            return (db / 2.54).Round();
        }

        /// <summary>
        /// 从KGS换算到LBS
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static double KGSToLBS(this double db)
        {
            return (db * 2.2).Round();
        }

        /// <summary>
        /// 四舍五入2位 .ToString("#0.00")
        /// </summary>
        /// <param name="dcmal"></param>
        /// <returns></returns>
        public static string RoundToString(this decimal? dcmal)
        {
            if (dcmal.HasValue)
            {
                return dcmal.Value.ToString("#0.00");
            }
            return null;
        }

        public static string StrToUpper(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                return str.ToUpper();
            }
            return null;
        }
        
        public static string StrTrim(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                return str.Trim();
            }
            return null;
        }

        public static decimal ToNumber(this decimal? str)
        {
            if (str.HasValue)
            {
                return str.Value;
            }
            return 0;
        }

    }
}