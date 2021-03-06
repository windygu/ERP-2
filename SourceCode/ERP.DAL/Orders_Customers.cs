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
    
    public partial class Orders_Customers
    {
        public Orders_Customers()
        {
            this.Customer_Commission = new HashSet<Customer_Commission>();
            this.Orders_AcceptInformation = new HashSet<Orders_AcceptInformation>();
            this.Orders_Contacts = new HashSet<Orders_Contacts>();
            this.Orders = new HashSet<Order>();
            this.Orders_FreightRate = new HashSet<Orders_FreightRate>();
            this.Products = new HashSet<Product>();
            this.Purchase_Contract = new HashSet<Purchase_Contract>();
            this.Quot_Quot = new HashSet<Quot_Quot>();
            this.Quot_QuotProduct = new HashSet<Quot_QuotProduct>();
            this.Quot_QuotProductHistory = new HashSet<Quot_QuotProductHistory>();
            this.Sale_SendSamples = new HashSet<Sale_SendSamples>();
            this.UserCustomerRelationships = new HashSet<UserCustomerRelationship>();
        }
    
        public int OCID { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Abbreviation { get; set; }
        public string AbbreviationA { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public Nullable<int> Region { get; set; }
        public Nullable<int> Province { get; set; }
        public Nullable<int> Country { get; set; }
        public string FullAddress { get; set; }
        public string PostalCode { get; set; }
        public decimal MiscImportLoadAmount { get; set; }
        public decimal Commission { get; set; }
        public decimal Agent { get; set; }
        public decimal Allowance { get; set; }
        public int CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<int> LastModifyBy { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public bool IsDelete { get; set; }
        public string QuoteTemplateFileName { get; set; }
        public Nullable<decimal> MU { get; set; }
        public Nullable<decimal> Palletpc { get; set; }
        public Nullable<decimal> PcsPallet { get; set; }
        public Nullable<decimal> CtnsPallet { get; set; }
        public Nullable<decimal> FinalFOB { get; set; }
        public Nullable<decimal> FOBNET { get; set; }
        public string Code { get; set; }
        public Nullable<decimal> ELCFill { get; set; }
        public Nullable<int> PaymentType { get; set; }
        public string SelectCustomer { get; set; }
    
        public virtual SystemUser SystemUser { get; set; }
        public virtual ICollection<Customer_Commission> Customer_Commission { get; set; }
        public virtual ICollection<Orders_AcceptInformation> Orders_AcceptInformation { get; set; }
        public virtual ICollection<Orders_Contacts> Orders_Contacts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual Reg_Area Reg_Area { get; set; }
        public virtual Reg_Country Reg_Country { get; set; }
        public virtual ICollection<Orders_FreightRate> Orders_FreightRate { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Purchase_Contract> Purchase_Contract { get; set; }
        public virtual ICollection<Quot_Quot> Quot_Quot { get; set; }
        public virtual ICollection<Quot_QuotProduct> Quot_QuotProduct { get; set; }
        public virtual ICollection<Quot_QuotProductHistory> Quot_QuotProductHistory { get; set; }
        public virtual ICollection<Sale_SendSamples> Sale_SendSamples { get; set; }
        public virtual ICollection<UserCustomerRelationship> UserCustomerRelationships { get; set; }
    }
}
