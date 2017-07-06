using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Order
{
    public class DTOOrder : PendingApproveBasePage
    {
        #region Model

        /// <summary>
        /// OrderID
        /// </summary>
        public int OrderID { get; set; }

        /// <summary>
        /// 销售订单ID
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// 客户PO
        /// </summary>
        public string POID { get; set; }

        /// <summary>
        /// 客户代号
        /// </summary>
        public string CustomerNo { get; set; }

        /// <summary>
        /// 客户下单日期
        /// </summary>
        public string CustomerDate { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal OrderAmount { get; set; }

        /// <summary>
        /// 订单毛利率
        /// </summary>
        public decimal OrderRate { get; set; }

        /// <summary>
        /// 开始交货日期
        /// </summary>
        public string OrderDateStart { get; set; }

        /// <summary>
        /// 结束交货日期
        /// </summary>
        public string OrderDateEnd { get; set; }

        /// <summary>
        /// 订单来源
        /// </summary>
        public string OrderOrigin { get; set; }

        /// <summary>
        /// 订单状态（Sale.OrderStatus）
        /// </summary>
        public int OrderStatusID { get; set; }

        /// <summary>
        /// 订单状态（Sale.OrderStatus）
        /// </summary>
        public string OrderStatusName { get; set; }

        /// <summary>
        /// 订单类型（1：待核算，2：已核算，3：已审批）
        /// </summary>
        public int? OrderType { get; set; }

        /// <summary>
        /// 港口ID
        /// </summary>
        public int? PortID { get; set; }

        /// <summary>
        /// 是否删除（0：未删除，1：已删除）
        /// </summary>
        public int? IsDelete { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime DT_CREATEDATE { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public int ST_CREATEUSER { get; set; }

        /// <summary>
        /// 核算意见
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public string DT_MODIFYDATE { get; set; }

        /// <summary>
        /// 修改用户
        /// </summary>
        public int ST_MODIFYUSER { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// 订单毛利率 英文
        /// </summary>
        public decimal? OrderRate_En { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string CheckSuggest { get; set; }

        /// <summary>
        /// 审批人
        /// </summary>
        public int? ApproverIndex { get; set; }

        /// <summary>
        /// 是否第三方审计（0：否，1：是）
        /// </summary>
        public int? IsThirdVerification { get; set; }

        /// <summary>
        /// 是否第三方验厂（0：否，1：是）
        /// </summary>
        public int? IsThirdAudits { get; set; }

        #endregion Model

        public string EHIPO { get; set; }

        /// <summary>
        /// 判断待审核页面是否显示审批按钮
        /// </summary>
        public bool IsHasApprovalPermission { get; set; }

        public int Finance_StatusID { get; set; }
        public string Finance_DT_MODIFYDATEFormatter { get; set; }
        public string Finance_StatusName { get; set; }
        public int CustomerID { get; set; }
        public string FactoryAbbreviation { get; set; }
        public int PortChargesInvoice_StatusID { get; set; }
        public string PortChargesInvoice_StatusName { get; set; }
        public string PortChargesInvoice_UpdateDate { get; set; }
        public decimal? OurAgencyAmount { get; set; }
        public decimal? DesignatedAgencyAmount { get; set; }
        public string SelectCustomer { get; set; }
    }
}