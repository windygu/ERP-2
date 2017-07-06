using ERP.Models.CustomEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.InspectionClearance
{
    /// <summary>
    /// 报关单据筛选条件
    /// </summary>
    public class VMInspectionClearanceSearch : IndexPageBaseModel
    {
        /// <summary>
        /// 页面区分
        /// </summary>
        public PageTypeEnum PageType { get; set; }

        /// <summary>
        /// 采购合同编号
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// 客户代号
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// 销售订单交货日期——开始
        /// </summary>
        public string DeliveryDateStart { get; set; }

        /// <summary>
        /// 销售订单交货日期——结束
        /// </summary>
        public string DeliveryDateEnd { get; set; }

        public string StatusID { get; set; }
    }
}