using ERP.Models.Common;
using ERP.Models.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Order
{
    public class VMOrderEdit
    {
        public int OrderStatusID { get; set; }

        public int OrderID { get; set; }

        public string OrderOrigin { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal OrderAmount { get; set; }

        /// <summary>
        /// 订单毛利率
        /// </summary>
        public decimal OrderRate { get; set; }

        /// <summary>
        /// 订单毛利率 英文
        /// </summary>
        public decimal? OrderRate_En { get; set; }

        public string OrderDateStart { get; set; }

        public string OrderDateEnd { get; set; }

        /// <summary>
        /// 客户下单日期
        /// </summary>
        public string CustomerDate { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public int CustomerID { get; set; }

        public int? PortID { get; set; }

        /// <summary>
        /// 客户PO
        /// </summary>
        public string POID { get; set; }

        /// <summary>
        /// EHI PO#
        /// </summary>
        public string EHIPO { get; set; }

        /// <summary>
        /// 第三方验货（0：否，1：是）
        /// </summary>
        public bool IsThirdVerification { get; set; }

        /// <summary>
        /// 第三方验厂（0：否，1：是）
        /// </summary>
        public bool IsThirdAudits { get; set; }

        /// <summary>
        /// 第三方检测 （0：否，1：是）
        /// </summary>
        public bool IsThirdTest { get; set; }

        /// <summary>
        /// 第三方抽检 （0：否，1：是）
        /// </summary>
        public bool IsThirdSampling { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string CheckSuggest { get; set; }

        /// <summary>
        /// 核算意见
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 目的港编号
        /// </summary>
        public int? DestinationPortID { get; set; }

        /// <summary>
        /// 出货方式
        /// </summary>
        public string ShippingType { get; set; }

        /// <summary>
        /// 柜型备注
        /// </summary>
        public string CabinetRemark { get; set; }

        /// <summary>
        /// 销售订单ID
        /// </summary>
        public string OrderNumber { get; set; }

        public string PortName { get; set; }

        public string DestinationPortName { get; set; }

        public string CustomerNo { get; set; }

        public string OrderStatusName { get; set; }

        public List<VMOrderProduct> list_OrderProduct { get; set; }

        public List<VMOrderHistory> list_OrderHistory { get; set; }

        public int ST_CREATEUSER { get; set; }

        public CustomEnums.PageTypeEnum PageType { get; set; }

        public int? ApproverIndex { get; set; }

        public int? QuotID { get; set; }

        /// <summary>
        /// 换汇
        /// </summary>
        public decimal? CurrencyExchange { get; set; }

        /// <summary>
        /// 其他费用
        /// </summary>
        public decimal? Additional { get; set; }

        public int Finance_StatusID { get; set; }
        public List<VMFinanceProduct> list_FinanceProduct { get; set; }

        /// <summary>
        /// 指定船代金额
        /// </summary>
        public decimal? DesignatedAgencyAmount { get; set; }

        /// <summary>
        /// 我司船代金额
        /// </summary>
        public decimal? OurAgencyAmount { get; set; }

        /// <summary>
        /// 上传的港杂费发票
        /// </summary>
        public List<VMUpLoadFile> list_UploadPortChargesInvoice { get; set; }

        public int? PortChargesInvoice_StatusID { get; set; }

        public string CategoryManager { get; set; }
        public string PaymentTerms { get; set; }

        /// <summary>
        /// 运输方式
        /// </summary>
        public int? TransportType { get; set; }
        
        /// <summary>
        /// 检测标准文件名
        /// </summary>
        public string TestingStandardsFilename { get; set; }
        public string SelectCustomer { get; set; }
        public List<VMOrderProduct> listProducts_Mixed { get; set; }
    }
}