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
    
    public partial class Delivery_ShipmentOrderProduct
    {
        public Delivery_ShipmentOrderProduct()
        {
            this.Inspection_InspectionCustomsProduct2 = new HashSet<Inspection_InspectionCustomsProduct2>();
            this.Inspection_InspectionReceiptProduct = new HashSet<Inspection_InspectionReceiptProduct>();
        }
    
        public int ID { get; set; }
        public int ShipmentOrderCabinetID { get; set; }
        public int Qty { get; set; }
        public int OrderProductID { get; set; }
        public bool IsDelete { get; set; }
        public System.DateTime DT_CREATEDATE { get; set; }
        public int ST_CREATEUSER { get; set; }
        public System.DateTime DT_MODIFYDATE { get; set; }
        public int ST_MODIFYUSER { get; set; }
        public string IPAddress { get; set; }
        public string InvoiceNo { get; set; }
        public string SCNo { get; set; }
        public Nullable<int> BoxQty { get; set; }
        public bool IsProductMixed { get; set; }
        public Nullable<int> ParentProductMixedID { get; set; }
    
        public virtual Delivery_ShipmentOrderCabinet Delivery_ShipmentOrderCabinet { get; set; }
        public virtual OrderProduct OrderProduct { get; set; }
        public virtual ICollection<Inspection_InspectionCustomsProduct2> Inspection_InspectionCustomsProduct2 { get; set; }
        public virtual ICollection<Inspection_InspectionReceiptProduct> Inspection_InspectionReceiptProduct { get; set; }
    }
}
