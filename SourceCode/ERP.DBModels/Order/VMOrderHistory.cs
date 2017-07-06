using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Order
{
    public class VMOrderHistory
    {
        /// <summary>
        /// 历史记录ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 订单编号ID
        /// </summary>
        public int OrderID { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 意见/备注
        /// </summary>
        public string CheckSuggest { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public string DT_CREATEDATE { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public string ST_CREATEUSER { get; set; }
    }
}