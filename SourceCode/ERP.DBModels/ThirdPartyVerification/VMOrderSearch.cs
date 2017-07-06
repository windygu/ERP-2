using ERP.Models.CustomEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.ThirdPartyVerification
{
    /// <summary>
    /// 第三方验货管理的
    /// </summary>
    public class VMOrderSearch : IndexPageBaseModel
    {
        /// <summary>
        /// 销售订单编号
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// 客户下单日期——开始
        /// </summary>
        public string CustomerDateStart { get; set; }

        /// <summary>
        /// 客户下单日期——结束
        /// </summary>
        public string CustomerDateEnd { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// 要求交货期——开始
        /// </summary>
        public string OrderDateStart { get; set; }

        /// <summary>
        /// 要求交货期——结束
        /// </summary>
        public string OrderDateEnd { get; set; }

        /// <summary>
        /// 客户PO
        /// </summary>
        public string POID { get; set; }

        /// <summary>
        /// 订单来源
        /// </summary>
        public string OrderOrigin { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public string OrderStatusID { get; set; }

        /// <summary>
        /// 页面类型
        /// </summary>
        public PageTypeEnum PageType { get; set; }
    }
}