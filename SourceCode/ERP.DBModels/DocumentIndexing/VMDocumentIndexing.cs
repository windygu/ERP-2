using ERP.Models.Common;
using ERP.Models.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.DocumentIndexing
{
    public class VMDocumentIndexing : PendingApproveBasePage
    {
        public int OrderID { get; set; }

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
        /// 销售订单ID
        /// </summary>
        public string OrderNumber { get; set; }

        public string PortName { get; set; }

        public string DestinationPortName { get; set; }

        public string CustomerNo { get; set; }

        public string OrderStatusName { get; set; }

        public List<VMDocumentIndexingHistory> list_history { get; set; }

        public int ST_CREATEUSER { get; set; }

        public CustomEnums.PageTypeEnum PageType { get; set; }

        public int? ApproverIndex { get; set; }

        public int StatusID { get; set; }
        public string StatusName { get; set; }
        public string Managers_Date { get; set; }
        public string Managers_UserName { get; set; }
        public int ID { get; set; }

        /// <summary>
        /// 客户代号
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// 发票号码
        /// </summary>
        public string InvoiceNoList { get; set; }

        /// <summary>
        /// 报关单位
        /// </summary>
        public int? CustomsUnit { get; set; }

        /// <summary>
        /// 船期
        /// </summary>
        public string ShippingDate { get; set; }

        public string ShippingDateForamtter { get; set; }

        /// <summary>
        /// 收款方式
        /// </summary>
        public string PaymentType { get; set; }

        /// <summary>
        /// 收款日期
        /// </summary>
        public string PaymentDate { get; set; }

        /// <summary>
        /// 是否保函出货
        /// </summary>
        public bool IsGuaranteeShipments { get; set; }

        /// <summary>
        /// 合同金额
        /// </summary>
        public decimal? AllAmount { get; set; }

        /// <summary>
        /// 指定船代金额
        /// </summary>
        public decimal? DesignatedAgencyAmount { get; set; }

        /// <summary>
        /// 我司船代金额
        /// </summary>
        public decimal? OurAgencyAmount { get; set; }

        /// <summary>
        /// 附件——港杂费发票
        /// </summary>
        public List<VMUpLoadFile> list_UploadFile_PortChargesInvoice { get; set; }

        /// <summary>
        /// 附件——采购合同
        /// </summary>
        public List<VMUpLoadFile> list_UploadFile_PurchaseContract { get; set; }

        /// <summary>
        /// 附件——配仓单
        /// </summary>
        public List<VMUpLoadFile> list_UploadFile_ShipmentNotification { get; set; }

        public List<VMDocumentIndexing_Purchase> list_Purchase { get; set; }

        /// <summary>
        /// 是否拉柜
        /// </summary>
        public bool IsPullCabinet { get; set; }

        /// <summary>
        /// 是否入仓
        /// </summary>
        public bool IsPutInStorage { get; set; }

        /// <summary>
        /// 是否有合约
        /// </summary>
        public bool IsCustomsOrder { get; set; }

        /// <summary>
        /// 是否有港杂费发票
        /// </summary>
        public bool IsPortChargesInvoice { get; set; }

        /// <summary>
        /// 是否有配仓单
        /// </summary>
        public bool IsShipmentNotification { get; set; }

        public decimal? OtherFee { get; set; }
        public bool IsOther { get; set; }
        public string Comment { get; set; }
        public string CheckSuggest { get; set; }
        public string DT_MODIFYDATE { get; set; }
        public bool IsHasApprovalPermission { get; set; }

        /// <summary>
        /// 是否上传了采购合同的附件
        /// </summary>
        public bool IsPurchaseContract { get; set; }

        public List<VMDocumentIndexing_RegisterFees> list_RegisterFee { get; set; }
        public List<VMDocumentIndexing_Outsourcing> list_Outsourcing { get; set; }
        public bool IsOutsourcing { get; set; }
        public bool IsInspection { get; set; }
        public List<VMDocumentIndexing_Inspection> list_Inspection { get; set; }
        public int? ShipmentOrderID { get; set; }
        public string Merge { get; set; }
        public string OrderNumberList { get; set; }

        /// <summary>
        /// 是否有FCR附件
        /// </summary>
        public bool IsFCRUploadFile { get; set; }

        /// <summary>
        /// FCR收到日期
        /// </summary>
        public string FCRReceiveDateFormatter { get; set; }

        /// <summary>
        /// 是否保函出货
        /// </summary>
        public string GuaranteeShipments { get; set; }
        public List<VMUpLoadFile> list_UploadFile_FCRUploadFile { get; set; }
        /// <summary>
        /// 合同金额($)
        /// </summary>
        public decimal AllAmount_USD { get; set; }

        /// <summary>
        /// 合同金额(¥)
        /// </summary>
        public decimal AllAmount_RMB { get; set; }

        public List<VMDocumentIndexing_OtherFees> list_OtherFees { get; set; }
        public List<VMUpLoadFile> list_UploadFile_PurchaseContract_MakeMoney { get; set; }
        public bool IsUpload_Order { get; set; }
        public List<VMUpLoadFile> list_Upload_Order { get; set; }
        public string SelectCustomer { get; set; }
    }


    /// <summary>
    /// 采购合同
    /// </summary>
    public class VMDocumentIndexing_Purchase
    {
        public string AllAmount { get; set; }
        public string FactoryAbbreviation { get; set; }
        public int PurchaseID { get; set; }
        public bool IsUpload { get; set; }
        /// <summary>
        /// 最晚支付工厂时间
        /// </summary>
        public string LatestTimePaymentFactory { get; set; }

        /// <summary>
        /// 实际支付货款时间
        /// </summary>
        public string ActualPaymentTime { get; set; }
        public bool IsUpload_MakeMoney { get; set; }
        public int ID { get; set; }
        public bool? DocumentsIndexing_IsGuaranteeShipments { get; set; }
    }

    /// <summary>
    /// 出运通知的拖柜费
    /// </summary>
    public class VMDocumentIndexing_RegisterFees
    {
        public int? FactoryID { get; set; }
        public string FactoryAbbreviation { get; set; }
        public decimal? AllAmount { get; set; }
        public decimal? SumOuterVolume { get; set; }
        public int CabinetIndex { get; set; }
    }

    /// <summary>
    /// 吊卡费、标签费
    /// </summary>
    public class VMDocumentIndexing_Outsourcing
    {
        public int PurchaseContractID { get; set; }
        public string FactoryAbbreviation { get; set; }
        public string AllAmount { get; set; }
    }

    /// <summary>
    /// 第三方检测费
    /// </summary>
    public class VMDocumentIndexing_Inspection
    {
        public int PurchaseContractID { get; set; }
        public string FactoryAbbreviation { get; set; }

        /// <summary>
        /// 第三方验货费
        /// </summary>
        public string AllAmount_ThirdVerification { get; set; }

        /// <summary>
        /// 第三方验厂费
        /// </summary>
        public string AllAmount_ThirdAudits { get; set; }

        /// <summary>
        /// 第三方检测费
        /// </summary>
        public string AllAmount_ThirdTest { get; set; }

        /// <summary>
        /// 第三方抽检费
        /// </summary>
        public string AllAmount_ThirdSampling { get; set; }
    }

    /// <summary>
    /// 其他费用
    /// </summary>
    public class VMDocumentIndexing_OtherFees
    {
        public int PurchaseContractID { get; set; }
        public decimal? DocumentsIndexing_OtherFees { get; set; }
        public string FactoryAbbreviation { get; set; }
    }
}