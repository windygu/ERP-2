using ERP.Models.CustomEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Quote
{
    public class VMQuoteSearch : IndexPageBaseModel
    {
        public PageTypeEnum PageType { get; set; }

        /// <summary>
        /// 报价单号
        /// </summary>
        public string QuotNumber { get; set; }

        /// <summary>
        /// 报价日期——开始
        /// </summary>
        public string QuotDateStart { get; set; }

        /// <summary>
        /// 报价日期——结束
        /// </summary>
        public string QuotDateEnd { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// 工厂
        /// </summary>
        public string FactoryName { get; set; }

        /// <summary>
        /// 报价单状态
        /// </summary>
        public string StatusID { get; set; }
    }
}