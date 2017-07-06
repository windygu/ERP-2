using ERP.Models.Common;
using ERP.Models.ShipmentOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.InspectionCustoms
{
    public class VMInspectionCustoms
    {
        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 客户所在街道
        /// </summary>
        public string CustomerStreet { get; set; }

        /// <summary>
        /// 客户所在城市,地区,国家,邮编
        /// </summary>
        public string CustomerReg { get; set; }

        /// <summary>
        /// 销售订单下单日期
        /// </summary>
        public string OrderSaleDate { get; set; }

        /// <summary>
        /// 产品订单箱数（求和）:采购合同->一个HSID对应的产品所在销售订单的订单箱数之和
        /// 产品箱数=产品数量/外箱率
        /// </summary>
        public int ProductsBoxNum { get; set; }

        /// <summary>
        /// 总毛重=单箱毛重×产品箱数
        /// </summary>
        public decimal WeightGrossSum { get; set; }

        /// <summary>
        /// 总净重=单箱净重×总箱数
        /// </summary>
        public decimal WeightNetSum { get; set; }

        /// <summary>
        /// 采购合同——交货地
        /// </summary>
        public string PortName { get; set; }

        /// <summary>
        /// 报关数据状态ID
        /// </summary>
        public int InspectionCustomsStatusID { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string AuditIdea { get; set; }

        /// <summary>
        /// 报检历史记录
        /// </summary>
        public List<VMInspectionCustomsHis> InspectionCustomsHis { get; set; }

        /// <summary>
        /// 报关单据：详细信息->Tab静态分部数据
        /// </summary>
        public List<VMOrderProducts> list_OrdersProducts { get; set; }

        /// <summary>
        /// 报关凭条（取需要报检的报检凭条）
        /// </summary>
        public List<VMUpLoadFile> list_UploadReceipts { get; set; }

        /// <summary>
        /// 报关申报要素（上传附件）
        /// </summary>
        public List<VMUpLoadFile> list_DeclareElements { get; set; }

        /// <summary>
        /// 报关委托书（上传附件）
        /// </summary>
        public List<VMUpLoadFile> list_CustomsCommission { get; set; }

        /// <summary>
        /// 发票号
        /// </summary>
        public string InvoiceNO { get; set; }

        public int? PortID { get; set; }
        public string POID { get; set; }
        public string SCNo { get; set; }
        public string ShipmentOrderProductIDList { get; set; }

        /// <summary>
        /// 获取客户里面的付款方式
        /// </summary>
        public string TeamOfThePayment { get; set; }

        public string SaleContractContent { get; set; }

        /// <summary>
        /// 工厂主要联系人
        /// </summary>
        public string CallPeople { get; set; }

        public int? DestinationPortID { get; set; }

        /// <summary>
        /// 运输方式编号
        /// </summary>
        public int TransportType { get; set; }

        /// <summary>
        /// 运输方式名称
        /// </summary>
        public string TransportTypeName { get; set; }

        /// <summary>
        /// 结汇方式编号
        /// </summary>
        public int ExchangeType { get; set; }

        public string CountryName { get; set; }

        /// <summary>
        /// 境内货源地
        /// </summary>
        public string SourceAreaWithin { get; set; }

        /// <summary>
        /// 成交方式
        /// </summary>
        public string TransactionType { get; set; }

        public int ShipmentOrderID { get; set; }
        public List<string> list_InvoiceNO { get; set; }
        public string ShippingMark { get; set; }
        public string DestinationPortName { get; set; }
        public List<string> list_SCNo { get; set; }
        public string Letter { get; set; }
        public string Letter_ForSCNo { get; set; }
        public DateTime CustomerDate { get; set; }
        public string CustomerDateFormatter { get; set; }
        public int InspectionCustomsID { get; set; }
        public string TheBuyer { get; set; }
        public string CustomerCode { get; set; }

        /// <summary>
        /// 境内货源地
        /// </summary>
        public string TerritoryOfOriginOfGoods { get; set; }

        public List<VMShipmentOrderProduct> list_ShipmentOrderProduct { get; set; }
        public string ShippingDateStart { get; set; }

        /// <summary>
        /// 贸易方式
        /// </summary>
        public int? TradeType { get; set; }

        /// <summary>
        /// 贸易方式名称
        /// </summary>
        public string TradeTypeName { get; set; }

        /// <summary>
        /// 目的港——国家
        /// </summary>
        public string DestinationPort_Country { get; set; }

        /// <summary>
        /// 目的港——省份
        /// </summary>
        public string DestinationPort_Province { get; set; }

        /// <summary>
        /// 目的港——城市
        /// </summary>
        public string DestinationPort_City { get; set; }

        /// <summary>
        /// 目的港——街道地址
        /// </summary>
        public string DestinationPort_StreetAddress { get; set; }

        /// <summary>
        /// 目的港——邮编
        /// </summary>
        public string DestinationPort_ZipCode { get; set; }

        /// <summary>
        /// 目的港——公司名称
        /// </summary>
        public string DestinationPort_CompanyName { get; set; }

        /// <summary>
        /// 目的港——地址。格式：City,State 邮编 US
        /// </summary>
        public string DestinationPort_Address { get; set; }

        public string SelectCustomer { get; set; }
        public string OrderDateStart { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateDateFormatter { get; set; }

        /// <summary>
        /// 结汇方式名称
        /// </summary>
        public string ExchangeTypeName { get; set; }

        public int InspectionCustomsDetailID { get; set; }
    }
}