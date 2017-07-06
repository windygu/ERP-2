using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Finance
{
    public class VMFinanceProduct
    {

        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// OrderProductID
        /// </summary>
        public int OrderProductID { get; set; }

        /// <summary>
        /// 实际船期
        /// </summary>
        public DateTime? ShippingDate { get; set; }

        /// <summary>
        /// 实际工厂交货期
        /// </summary>
        public DateTime? FactoryDate { get; set; }

        /// <summary>
        /// 实际开票金额
        /// </summary>
        public decimal? InvoicedAmount { get; set; }

        /// <summary>
        /// 付款日
        /// </summary>
        public DateTime? PaymentDate { get; set; }

        /// <summary>
        /// 付款金额
        /// </summary>
        public decimal? PaymentAmount { get; set; }

        /// <summary>
        /// 发票认证日期
        /// </summary>
        public DateTime? CertifiedInvoiceDate { get; set; }

        /// <summary>
        /// 美元汇率
        /// </summary>
        public decimal? USDExchangeRate { get; set; }

        /// <summary>
        /// 结汇日期
        /// </summary>
        public DateTime? ClearanceDate { get; set; }

        /// <summary>
        /// 结汇金额($)
        /// </summary>
        public decimal? ClearanceAmount { get; set; }

        /// <summary>
        /// 银行费用($)
        /// </summary>
        public decimal? BankFees { get; set; }

        /// <summary>
        /// 报关单号
        /// </summary>
        public string CustomsNumber { get; set; }

        /// <summary>
        /// 报关金额($)
        /// </summary>
        public string CustomsAmount { get; set; }
        /// <summary>
        /// 预录单状态
        /// </summary>
        public int PreRecordedStatus { get; set; }

        /// <summary>
        /// 其他应扣费用(￥)
        /// </summary>
        public decimal? OtherExpensesDeduction { get; set; }

        /// <summary>
        /// 应补工厂费用(￥)
        /// </summary>
        public decimal? FactoryFees { get; set; }

        /// <summary>
        /// 包装检测费(￥)
        /// </summary>
        public decimal? PacksDetectFees { get; set; }

        /// <summary>
        /// 港杂费(￥)
        /// </summary>
        public decimal? PortCharges { get; set; }

        /// <summary>
        /// 国际快递费(￥)
        /// </summary>
        public decimal? InternationalCourierFees { get; set; }

        /// <summary>
        /// 其他费用(￥)
        /// </summary>
        public decimal? OtherFees { get; set; }

        /// <summary>
        /// 公司综合管理费率(%)
        /// </summary>
        public decimal? CompanyManagementRate { get; set; }

        /// <summary>
        /// 退税率(%)
        /// </summary>
        public decimal? RefundRate { get; set; }

        public int ProductID { get; set; }
        public string Image { get; set; }

        /// <summary>
        /// 货号
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 工厂
        /// </summary>
        public string FactoryAbbreviation { get; set; }

        /// <summary>
        /// 采购合同号
        /// </summary>
        public string PurchaseNumber { get; set; }

        /// <summary>
        /// 要求交货期
        /// </summary>
        public string DateStartFormatter { get; set; }

        /// <summary>
        /// 实际船期
        /// </summary>
        public string ShippingDateFormatter { get; set; }

        /// <summary>
        /// 实际工厂交货期
        /// </summary>
        public string FactoryDateFormatter { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 工厂价格
        /// </summary>
        public decimal PriceFactory { get; set; }

        /// <summary>
        /// 采购合同金额
        /// </summary>
        public decimal AllAmount { get; set; }

        /// <summary>
        /// 付款日
        /// </summary>
        public string PaymentDateFormatter { get; set; }

        /// <summary>
        /// 实际开票金额
        /// </summary>
        public string CertifiedInvoiceDateFormatter { get; set; }

        /// <summary>
        /// PO单价($)
        /// </summary>
        public decimal? POPrice { get; set; }

        /// <summary>
        /// PO金额($)
        /// </summary>
        public decimal? POAmount { get; set; }

        /// <summary>
        /// 佣金比例(%)
        /// </summary>
        public decimal? CommissionPercent { get; set; }

        /// <summary>
        /// 佣金金额($)
        /// </summary>
        public decimal? CommissionAmount { get; set; }

        /// <summary>
        /// FOB净值美元
        /// </summary>
        public decimal? FOBUSDAmount { get; set; }

        /// <summary>
        /// FOB净值人民币
        /// </summary>
        public decimal? FOBRMBAmount { get; set; }

        /// <summary>
        /// 结汇日期
        /// </summary>
        public string ClearanceDateFormatter { get; set; }

        /// <summary>
        /// 报关品名
        /// </summary>
        public string HSCodeName { get; set; }

        /// <summary>
        /// 报关编码
        /// </summary>
        public string HSCode { get; set; }

        /// <summary>
        /// 采购合同日期
        /// </summary>
        public string PurchaseDateFormatter { get; set; }

        public int? SortIndex { get; set; }

        /// <summary>
        /// 立方数(cuft)
        /// </summary>
        public decimal? VolumeCUFT { get; set; }

        /// <summary>
        /// 代印及快递费(￥)
        /// </summary>
        public decimal? AllOutContractAmount { get; set; }

        /// <summary>
        /// 拖柜费(￥)
        /// </summary>
        public decimal? RegisterFees { get; set; }

        /// <summary>
        /// 合计应扣工厂费用(￥)
        /// </summary>
        public decimal? AllAmount_Factory { get; set; }

        /// <summary>
        /// 公司综合管理费(￥)
        /// </summary>
        public decimal? CompanyManagementAmount { get; set; }

        /// <summary>
        /// 合计我司成本(￥)
        /// </summary>
        public decimal? AllAmount_CompanyManagement { get; set; }

        /// <summary>
        /// 退税金额(￥)
        /// </summary>
        public decimal? RefundAmount { get; set; }

        /// <summary>
        /// 毛利(￥)
        /// </summary>
        public decimal? GrossProfitAmount { get; set; }

        /// <summary>
        /// 毛利率(%)
        /// </summary>
        public decimal? GrossProfitPercent { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comment { get; set; }

        public string CurrencySign { get; set; }
        public string OrderNumber { get; set; }

        /// <summary>
        /// 客户下单日期
        /// </summary>
        public DateTime CustomerDate { get; set; }

        /// <summary>
        /// 客户代号
        /// </summary>
        public string CustomerCode { get; set; }

        public string POID { get; set; }

        /// <summary>
        /// 采购合同日期
        /// </summary>
        public DateTime PurchaseDate { get; set; }

        /// <summary>
        /// 要求交货期
        /// </summary>
        public DateTime DateStart { get; set; }

        /// <summary>
        /// 发票号
        /// </summary>
        public string InvoiceNo { get; set; }

        public int? FactoryID { get; set; }

        /// <summary>
        /// 季节
        /// </summary>
        public int? Season { get; set; }

        public string SeasonName { get; set; }

        /// <summary>
        /// 第三方验货
        /// </summary>
        public decimal? InspectionVerificationFee { get; set; }

        /// <summary>
        /// 第三方验货
        /// </summary>
        public decimal? InspectionVerificationFee_ForFactory { get; set; }

        /// <summary>
        /// 第三方验厂
        /// </summary>
        public decimal? InspectionAuditFee { get; set; }

        /// <summary>
        /// 第三方验厂
        /// </summary>
        public decimal? InspectionAuditFee_ForFactory { get; set; }

        /// <summary>
        /// 第三方检测
        /// </summary>
        public decimal? InspectionDetectFee { get; set; }

        /// <summary>
        /// 第三方检测
        /// </summary>
        public decimal? InspectionDetectFee_ForFactory { get; set; }

        /// <summary>
        /// 第三方抽检
        /// </summary>
        public decimal? InspectionSamplingFee { get; set; }

        /// <summary>
        /// 第三方抽检
        /// </summary>
        public decimal? InspectionSamplingFee_ForFactory { get; set; }

        public bool IsMaintain { get; set; }
        /// <summary>
        /// 结算期
        /// </summary>
        public string SettlementPeriod { get; set; }

        /// <summary>
        /// 单证索引上传日期
        /// </summary>
        public string DocumentIndexUpLoadDate { get; set; }

        /// <summary>
        /// 买手简称
        /// </summary>
        public string BuyersAbbreviation { get; set; }
        public string OrderDateStart { get; set; }
        public string OrderDateEnd { get; set; }
        public int? ParentID { get; set; }
        public int? RootID { get; set; }
    }
}