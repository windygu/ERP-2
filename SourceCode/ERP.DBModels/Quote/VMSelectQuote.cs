using ERP.Models.CustomEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Quote
{
    /// <summary>
    /// 选择报价单
    /// </summary>
    public class VMSelectQuote
    {
        /// <summary>
        /// 报价单编号
        /// </summary>
        public string QuotNumber { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public int CustomerID { get; set; }

        /// <summary>
        /// 选择报价单数据的使用场合
        /// </summary>
        public SelectQuoteOccasionEnum SelectQuoteOccasion { get; set; }
    }
}