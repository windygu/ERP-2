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
    
    public partial class Inspection_InspectionReceipt
    {
        public Inspection_InspectionReceipt()
        {
            this.Inspection_InspectionReceiptHis = new HashSet<Inspection_InspectionReceiptHis>();
            this.Inspection_InspectionReceiptProduct = new HashSet<Inspection_InspectionReceiptProduct>();
        }
    
        public int InspectionReceiptID { get; set; }
        public int OrderID { get; set; }
        public int HSID { get; set; }
        public int InspectionReceiptStatus { get; set; }
        public Nullable<System.DateTime> InspectionReceiptDate { get; set; }
        public System.DateTime ClaimFaxDate { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int CreateUserID { get; set; }
        public string CreateTerminal { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public Nullable<int> UpdateUserID { get; set; }
        public string UpdateTerminal { get; set; }
        public int PortID { get; set; }
        public decimal INProductPrice { get; set; }
        public string UnitRetail { get; set; }
        public string TheBuyer { get; set; }
        public string AuditIdea { get; set; }
        public string INRemark { get; set; }
        public string SaleContractContent { get; set; }
        public int ZLRRuleNumber { get; set; }
        public string SCNO { get; set; }
        public string InvNO { get; set; }
        public Nullable<int> ApproverIndex { get; set; }
        public string TeamOfThePayment { get; set; }
        public Nullable<int> ShipmentOrderID { get; set; }
        public string ContractIDList { get; set; }
        public Nullable<int> TradeType { get; set; }
        public string HSCodeName { get; set; }
        public Nullable<int> InspectionReceiptListID { get; set; }
        public string HSCodeEngName { get; set; }
    
        public virtual SystemUser SystemUser { get; set; }
        public virtual HarmonizedSystem HarmonizedSystem { get; set; }
        public virtual Inspection_InspectionReceiptList Inspection_InspectionReceiptList { get; set; }
        public virtual ICollection<Inspection_InspectionReceiptHis> Inspection_InspectionReceiptHis { get; set; }
        public virtual ICollection<Inspection_InspectionReceiptProduct> Inspection_InspectionReceiptProduct { get; set; }
    }
}
