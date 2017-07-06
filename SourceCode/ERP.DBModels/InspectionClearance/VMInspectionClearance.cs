using ERP.Models.Common;
using ERP.Models.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.InspectionClearance
{
    public class VMInspectionClearance
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
        /// 采购合同——交货地
        /// </summary>
        public string PortName { get; set; }

        /// <summary>
        /// 报关数据状态ID
        /// </summary>
        public int InspectionClearanceStatusID { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string AuditIdea { get; set; }

        /// <summary>
        /// 清关历史记录
        /// </summary>
        public List<VMInspectionClearanceHis> InspectionClearanceHis { get; set; }

        public List<VMOrderProduct> list_OrderProduct_PackingList { get; set; }

        /// <summary>
        /// 发票号
        /// </summary>
        public string InvoiceNO { get; set; }

        public int? PortID { get; set; }
        public string POID { get; set; }
        public int? DestinationPortID { get; set; }
        public int ShipmentOrderID { get; set; }
        public string DestinationPortName { get; set; }
        public string CustomerDate { get; set; }
        public int InspectionClearanceID { get; set; }

        /// <summary>
        /// 清关 其他里面的附件
        /// </summary>
        public List<VMUpLoadFile> list_ClearanceOther { get; set; }

        public List<VMUpLoadFile> list_UploadFCR { get; set; }

        public List<VMUpLoadFile> list_UploadModify { get; set; }

        /// <summary>
        /// FCR收到日期——开始
        /// </summary>
        public string FCRReceiveDateStartFormatter { get; set; }

        /// <summary>
        /// FCR收到日期——结束
        /// </summary>
        public string FCRReceiveDateEndFormatter { get; set; }

        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 船开日期
        /// </summary>
        public string ShipDateStartForamtter { get; set; }

        /// <summary>
        /// 卸货港
        /// </summary>
        public int? PortOfEntry { get; set; }

        /// <summary>
        /// 总的品名描述
        /// </summary>
        public string InvoiceOF { get; set; }

        ///// <summary>
        ///// Misc% Import Load%
        ///// </summary>
        //public decimal? MiscPercent { get; set; }

        /// <summary>
        /// 箱号
        /// </summary>
        public string BoxNumber { get; set; }

        /// <summary>
        /// 封号
        /// </summary>
        public string SealNumber { get; set; }

        public string EHIPO { get; set; }
        public string PaymentType { get; set; }

        /// <summary>
        /// 卸货港名称
        /// </summary>
        public string PortOfEntryName { get; set; }

        /// <summary>
        /// 船开日期
        /// </summary>
        public DateTime? ShipDateStart { get; set; }

        /// <summary>
        /// 中转港
        /// </summary>
        public int? TransshipmentPortID { get; set; }

        /// <summary>
        /// 中转港
        /// </summary>
        public string TransshipmentPortName { get; set; }

        /// <summary>
        /// 信用证号
        /// </summary>
        public string CreditNumber { get; set; }

        /// <summary>
        /// 实际发船日期
        /// </summary>
        public DateTime? ActualShippingDate { get; set; }

        /// <summary>
        /// 实际发船日期
        /// </summary>
        public string ActualShippingDateFormatter { get; set; }

        public string SelectCustomer { get; set; }

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

        public DateTime CustomerDate2 { get; set; }
        public string FactoryIDList { get; set; }
        public List<Factory.DTOFactory> list_Factory { get; set; }

        /// <summary>
        /// 仓库地址
        /// </summary>
        public string WarehouseAddress { get; set; }

        /// <summary>
        /// 船代公司地址
        /// </summary>
        public string AgencyAddress { get; set; }

        /// <summary>
        /// 贸易方式 
        /// </summary>
        public string TradeTypeName { get; set; }

        /// <summary>
        /// 取客户里面的付款方式
        /// </summary>
        public string TeamOfThePayment { get; set; }

        /// <summary>
        /// 柜号列表：取订舱里面的柜号，多个用,隔开
        /// </summary>
        public string CabinetNumberList { get; set; }

        /// <summary>
        /// 航名航次列表：取订舱里面的Ocean Vessel / Voy No.，多个用,隔开
        /// </summary>
        public string OceanVessel { get; set; }
        public string CreateDateForamtter { get; set; }

        public int? TradeType { get; set; }
        public int CustomerID { get; set; }
        public int? ShipTo { get; set; }
        public string AcceptInformation_StreetAddress { get; set; }
        public string AcceptInformation_CustomerReg { get; set; }
        public string AcceptInformation_CompanyName { get; set; }

        /// <summary>
        /// 是否试单
        /// </summary>
        public bool IsTest { get; set; }
        
        /// <summary>
        /// 订舱的柜号/柜型/封号,箱号
        /// </summary>
        public List<string> list_CabinetNumber_Cabinet_SealingNumber { get; set; }

        /// <summary>
        /// 订舱的箱号/柜号/封号
        /// </summary>
        public List<string> list_BoxNumber_CabinetNumber_SealingNumber { get; set; }
        
        public List<VMOrderProduct> list_OrderProduct_Invoice { get; set; }
        public string AcceptInformation_Comment { get; set; }
    }
}