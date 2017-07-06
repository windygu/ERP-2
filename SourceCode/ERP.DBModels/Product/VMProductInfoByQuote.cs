using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Product
{
    /// <summary>
    ///获取产品信息(报价单页面)
    /// </summary>
    public class VMProductInfoByQuote
    {
        /// <summary>
        /// 产品编号
        /// </summary>
        public string idList { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public int CustomerID { get; set; }

        /// <summary>
        /// 人民币换汇
        /// </summary>
        public decimal CurrencyExchangeRMB { get; set; }

        /// <summary>
        /// 美元换汇
        /// </summary>
        public decimal CurrencyExchangeUSD { get; set; }

        /// <summary>
        /// 预期市场汇率
        /// </summary>
        public decimal ExchangeRate { get; set; }

        /// <summary>
        /// Terms
        /// </summary>
        public int TermsID { get; set; }

        /// <summary>
        /// PortID
        /// </summary>
        public int PortID { get; set; }
        public int? ParentProductMixedID { get; set; }
    }
}