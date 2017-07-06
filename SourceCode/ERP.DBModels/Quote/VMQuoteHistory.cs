using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Quote
{
    public class VMQuoteHistory
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 报价单ID
        /// </summary>
        public int QuotID { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime DT_CREATEDATE { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public string ST_CREATEUSER { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// 审核意见
        /// </summary>
        public string CheckSuggest { get; set; }
    }
}