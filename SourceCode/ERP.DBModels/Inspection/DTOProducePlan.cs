using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Inspection
{
    public class DTOProducePlan
    {
        /// <summary>
        /// id号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 采购合同编号
        /// </summary>
        public int PurchaseID { get; set; }

        /// <summary>
        /// 采购日期
        /// </summary>
        public DateTime PurchaseDate { get; set; }

        /// <summary>
        /// 销售订单编号
        /// </summary>
        public int OrderID { get; set; }

        /// <summary>
        /// 工厂
        /// </summary>
        public string FactoryName { get; set; }

        /// <summary>
        /// 客户po
        /// </summary>
        public string CustomerPo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        public int Result { set; get; }

        /// <summary>
        /// 审核意见
        /// </summary>
        public string SomeThing { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime DT_CREATEDATE { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public int ST_CREATEUSER { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime DT_MODIFYDATE { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public int ST_MODIFYUSER { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        public string IPAdress { get; set; }

        /// <summary>
        /// 生产计划历史id
        /// </summary>
        public int ProducePlanHistory { get; set; }

        /// <summary>
        /// id号
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
        /// 审核	意见
        /// </summary>
        public string HistorySomeThing { get; set; }

        /// <summary>
        /// 审核日期
        /// </summary>
        public DateTime ProduceDate { get; set; }
    }
}