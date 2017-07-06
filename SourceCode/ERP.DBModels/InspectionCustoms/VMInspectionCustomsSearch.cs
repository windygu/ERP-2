using ERP.Models.CustomEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.InspectionCustoms
{
    /// <summary>
    /// 报关单据筛选条件
    /// </summary>
    public class VMInspectionCustomsSearch : IndexPageBaseModel
    {
        /// <summary>
        /// 页面区分
        /// </summary>
        public PageTypeEnum PageType { get; set; }

        /// <summary>
        /// 报关列表数据是否允许多选
        /// </summary>
        public bool SingleSelect { get; set; }

        /// <summary>
        /// 采购合同编号
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// 工厂简称
        /// </summary>
        public string FactoryAbbreviation { get; set; }

        /// <summary>
        /// 客户代号
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// 客户PO#
        /// </summary>
        public string CustomerPO { get; set; }

        /// <summary>
        /// 销售订单交货日期——开始
        /// </summary>
        public string DeliveryDateStart { get; set; }

        /// <summary>
        /// 销售订单交货日期——结束
        /// </summary>
        public string DeliveryDateEnd { get; set; }

        /// <summary>
        /// 报关单据数据状态
        /// </summary>
        public int InspectionCustomsStatus { get; set; }
        
        public string StatusID { get; set; }

        public string PurchaseNumber { get; set; }
    }
}