using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.ProducePlan
{
    public class DTOProducePlanHistory
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 生产计划id
        /// </summary>
        public int ProduceID { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public string ProducePeople { get; set; }

        /// <summary>
        /// 审核结果
        /// </summary>
        public int ProduceResult { get; set; }

        /// <summary>
        /// 审核结果文本
        /// </summary>
        public string ProduceResultName { get; set; }

        /// <summary>
        /// 审核意见
        /// </summary>
        public string HistorySomeThing { get; set; }

        /// <summary>
        /// 审核日期
        /// </summary>
        public string ProduceDate { get; set; }

        /// <summary>
        /// 销售订单编号集合
        /// </summary>
        public string OrderList { get; set; }
    }
}