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
    
    public partial class User
    {
        public User()
        {
            this.UserRoles = new HashSet<UserRole>();
            this.ERPUserWorkflows = new HashSet<ERPUserWorkflow>();
            this.Orders_Customers = new HashSet<Orders_Customers>();
            this.Purchase_AuditPacksHis = new HashSet<Purchase_AuditPacksHis>();
            this.Purchase_Contract = new HashSet<Purchase_Contract>();
            this.Purchase_OutContractHis = new HashSet<Purchase_OutContractHis>();
            this.Purchase_OutContracts = new HashSet<Purchase_OutContracts>();
            this.Quot_Quot = new HashSet<Quot_Quot>();
            this.Sale_SendSamples = new HashSet<Sale_SendSamples>();
            this.Classifies = new HashSet<Classify>();
            this.Orders = new HashSet<Order>();
            this.OrderProducts = new HashSet<OrderProduct>();
            this.Products = new HashSet<Product>();
        }
    
        public int UserID { get; set; }
        public string Token { get; set; }
        public string UserNo { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public Nullable<System.DateTime> BirthDay { get; set; }
        public Nullable<System.DateTime> dtJoinWork { get; set; }
        public Nullable<System.DateTime> dtInCorp { get; set; }
        public Nullable<System.DateTime> dtOutCorp { get; set; }
        public Nullable<int> DeptID { get; set; }
        public Nullable<int> AddressID { get; set; }
        public string Sex { get; set; }
        public string QQ { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string SubPhone { get; set; }
        public string Fax { get; set; }
        public string Photo { get; set; }
        public string IP { get; set; }
        public string mac { get; set; }
        public string UserTypeID { get; set; }
        public string UserStatus { get; set; }
        public string Memo { get; set; }
        public Nullable<System.DateTime> dtLogin { get; set; }
        public int CreateUserID { get; set; }
        public System.DateTime dtCreate { get; set; }
    
        public virtual UserHierarchy UserHierarchy { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<ERPUserWorkflow> ERPUserWorkflows { get; set; }
        public virtual ICollection<Orders_Customers> Orders_Customers { get; set; }
        public virtual ICollection<Purchase_AuditPacksHis> Purchase_AuditPacksHis { get; set; }
        public virtual ICollection<Purchase_Contract> Purchase_Contract { get; set; }
        public virtual ICollection<Purchase_OutContractHis> Purchase_OutContractHis { get; set; }
        public virtual ICollection<Purchase_OutContracts> Purchase_OutContracts { get; set; }
        public virtual ICollection<Quot_Quot> Quot_Quot { get; set; }
        public virtual ICollection<Sale_SendSamples> Sale_SendSamples { get; set; }
        public virtual ICollection<Classify> Classifies { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
