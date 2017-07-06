using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Order
{
    /// <summary>
    /// 选择销售订单
    /// </summary>
    public class VMSelectOrder
    {
        /// <summary>
        /// 客户编号
        /// </summary>
        public int CustomerID { get; set; }

        /// <summary>
        /// POID
        /// </summary>
        public string POID { get; set; }

        /// <summary>
        /// 交货期——开始
        /// </summary>
        public DateTime DateStart { get; set; }

        /// <summary>
        /// 交货期——结束
        /// </summary>
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// 场合枚举值，用于指定默认查询条件
        /// </summary>
        public int usingOccasion { get; set; }

        public string OrderNumber { get; set; }
    }
}