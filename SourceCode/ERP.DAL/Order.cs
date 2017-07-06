//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ERP.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Order
    {
        public Order()
        {
            this.Delivery_ShipmentOrder = new HashSet<Delivery_ShipmentOrder>();
            this.DocumentsIndexings = new HashSet<DocumentsIndexing>();
            this.Purchase_ThirdPartyVerification = new HashSet<Purchase_ThirdPartyVerification>();
            this.Sale_SendSamples = new HashSet<Sale_SendSamples>();
            this.OrderHistories = new HashSet<OrderHistory>();
            this.OrderProducts = new HashSet<OrderProduct>();
        }
    
        public int OrderID { get; set; }
        public string OrderNumber { get; set; }
        public string POID { get; set; }
        public int CustomerID { get; set; }
        public System.DateTime CustomerDate { get; set; }
        public decimal OrderAmount { get; set; }
        public decimal OrderRate { get; set; }
        public System.DateTime OrderDateStart { get; set; }
        public System.DateTime OrderDateEnd { get; set; }
        public string OrderOrigin { get; set; }
        public Nullable<int> OrderType { get; set; }
        public Nullable<int> PortID { get; set; }
        public bool IsDelete { get; set; }
        public System.DateTime DT_CREATEDATE { get; set; }
        public int ST_CREATEUSER { get; set; }
        public string Comment { get; set; }
        public System.DateTime DT_MODIFYDATE { get; set; }
        public int ST_MODIFYUSER { get; set; }
        public string IPAddress { get; set; }
        public Nullable<decimal> OrderRate_En { get; set; }
        public string CheckSuggest { get; set; }
        public Nullable<int> ApproverIndex { get; set; }
        public bool IsThirdVerification { get; set; }
        public bool IsThirdAudits { get; set; }
        public string EHIPO { get; set; }
        public bool IsThirdTest { get; set; }
        public Nullable<int> DestinationPortID { get; set; }
        public string ShippingType { get; set; }
        public int OrderStatusID { get; set; }
        public string CabinetRemark { get; set; }
        public Nullable<int> QuotID { get; set; }
        public Nullable<decimal> CurrencyExchange { get; set; }
        public Nullable<decimal> Additional { get; set; }
        public int Finance_StatusID { get; set; }
        public Nullable<System.DateTime> Finance_DT_MODIFYDATE { get; set; }
        public Nullable<int> Finance_ST_MODIFYUSER { get; set; }
        public bool IsThirdSampling { get; set; }
        public Nullable<decimal> DesignatedAgencyAmount { get; set; }
        public Nullable<decimal> OurAgencyAmount { get; set; }
        public Nullable<int> PortChargesInvoice_CreateUserID { get; set; }
        public Nullable<System.DateTime> PortChargesInvoice_UpdateDate { get; set; }
        public Nullable<int> PortChargesInvoice_StatusID { get; set; }
        public string CategoryManager { get; set; }
        public string PaymentTerms { get; set; }
        public Nullable<int> TransportType { get; set; }
        public string TestingStandardsFilename { get; set; }
    
        public virtual SystemUser SystemUser { get; set; }
        public virtual ICollection<Delivery_ShipmentOrder> Delivery_ShipmentOrder { get; set; }
        public virtual ICollection<DocumentsIndexing> DocumentsIndexings { get; set; }
        public virtual Orders_Customers Orders_Customers { get; set; }
        public virtual ICollection<Purchase_ThirdPartyVerification> Purchase_ThirdPartyVerification { get; set; }
        public virtual ICollection<Sale_SendSamples> Sale_SendSamples { get; set; }
        public virtual ICollection<OrderHistory> OrderHistories { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
