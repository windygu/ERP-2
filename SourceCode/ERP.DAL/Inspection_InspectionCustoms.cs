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
    
    public partial class Inspection_InspectionCustoms
    {
        public Inspection_InspectionCustoms()
        {
            this.Inspection_InspectionCustomsDetail = new HashSet<Inspection_InspectionCustomsDetail>();
            this.Inspection_InspectionCustomsHis = new HashSet<Inspection_InspectionCustomsHis>();
        }
    
        public int InspectionCustomsID { get; set; }
        public int StatusID { get; set; }
        public Nullable<int> ApproverIndex { get; set; }
        public int ShipmentOrderID { get; set; }
        public bool IsDelete { get; set; }
        public System.DateTime DT_CREATEDATE { get; set; }
        public int ST_CREATEUSER { get; set; }
        public System.DateTime DT_MODIFYDATE { get; set; }
        public int ST_MODIFYUSER { get; set; }
        public string IPAddress { get; set; }
        public string ShipmentOrderProductIDList { get; set; }
        public Nullable<bool> IsNeedInspection { get; set; }
        public bool IsShowNeedInspection { get; set; }
    
        public virtual SystemUser SystemUser { get; set; }
        public virtual Delivery_ShipmentOrder Delivery_ShipmentOrder { get; set; }
        public virtual ICollection<Inspection_InspectionCustomsDetail> Inspection_InspectionCustomsDetail { get; set; }
        public virtual ICollection<Inspection_InspectionCustomsHis> Inspection_InspectionCustomsHis { get; set; }
    }
}
