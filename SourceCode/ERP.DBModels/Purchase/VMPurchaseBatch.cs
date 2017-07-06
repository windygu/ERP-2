using ERP.Models.ProductFitting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Purchase
{
    /// <summary>
    /// 采购合同——批次
    /// </summary>
    public class VMPurchaseBatch
    {
        public int ID { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal BatchAmount { get; set; }

        /// <summary>
        /// 次数
        /// </summary>
        public int Times { get; set; }

        /// <summary>
        /// 开始交货日期
        /// </summary>
        public DateTime DateStart { get; set; }

        /// <summary>
        /// 结束交货日期
        /// </summary>
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// 销售订单的产品列表
        /// </summary>
        public List<VMPurchaseProduct> listProduct { get; set; }

        public List<VMProductFittingInfo> listProductFitting { get; set; }

        public string DateStartForamt { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string CurrentSign { get; set; }
    }
}