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
    
    public partial class Quot_Quot
    {
        public Quot_Quot()
        {
            this.Quot_QuotHistory = new HashSet<Quot_QuotHistory>();
            this.Quot_QuotProduct = new HashSet<Quot_QuotProduct>();
            this.Sale_SendSamples = new HashSet<Sale_SendSamples>();
        }
    
        public int ID { get; set; }
        public string QuotNumber { get; set; }
        public int CustomerID { get; set; }
        public System.DateTime QuotDate { get; set; }
        public System.DateTime ValidDate { get; set; }
        public int AuthorID { get; set; }
        public int StatusID { get; set; }
        public bool IsImmediatelySend { get; set; }
        public int QuotTimes { get; set; }
        public string OrderID { get; set; }
        public bool IsDelete { get; set; }
        public System.DateTime DT_CREATEDATE { get; set; }
        public int ST_CREATEUSER { get; set; }
        public System.DateTime DT_MODIFYDATE { get; set; }
        public int ST_MODIFYUSER { get; set; }
        public string IPAddress { get; set; }
        public string CheckSuggest { get; set; }
        public Nullable<int> ApproverIndex { get; set; }
        public Nullable<System.DateTime> TemplateLastCreateTime { get; set; }
        public Nullable<decimal> ExchangeRate { get; set; }
        public Nullable<decimal> CurrencyExchangeUSD { get; set; }
        public Nullable<decimal> CurrencyExchangeRMB { get; set; }
        public Nullable<int> TermsID { get; set; }
        public Nullable<int> PortID { get; set; }
    
        public virtual SystemUser SystemUser { get; set; }
        public virtual Orders_Customers Orders_Customers { get; set; }
        public virtual ICollection<Quot_QuotHistory> Quot_QuotHistory { get; set; }
        public virtual ICollection<Quot_QuotProduct> Quot_QuotProduct { get; set; }
        public virtual ICollection<Sale_SendSamples> Sale_SendSamples { get; set; }
    }
}