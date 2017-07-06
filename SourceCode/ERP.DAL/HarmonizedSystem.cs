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
    
    public partial class HarmonizedSystem
    {
        public HarmonizedSystem()
        {
            this.HS_Child = new HashSet<HS_Child>();
            this.Inspection_InspectionReceipt = new HashSet<Inspection_InspectionReceipt>();
        }
    
        public int ID { get; set; }
        public string HSCode { get; set; }
        public decimal Cess { get; set; }
        public System.DateTime DT_CREATEDATE { get; set; }
        public int ST_CREATEUSER { get; set; }
        public Nullable<System.DateTime> DT_MODIFYDATE { get; set; }
        public Nullable<int> ST_MODIFYUSER { get; set; }
        public string IPAddress { get; set; }
        public bool IsDelete { get; set; }
        public string CodeName { get; set; }
        public Nullable<int> DataFlag { get; set; }
        public string CodeEngName { get; set; }
        public string DutyPercentList { get; set; }
    
        public virtual ICollection<HS_Child> HS_Child { get; set; }
        public virtual ICollection<Inspection_InspectionReceipt> Inspection_InspectionReceipt { get; set; }
    }
}
