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
    
    public partial class Delivery_EncasementsHis
    {
        public int AuditHisID { get; set; }
        public int EncasementsID { get; set; }
        public int AuditUserID { get; set; }
        public string AuditIdea { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public int EncasementsStatus { get; set; }
    
        public virtual SystemUser SystemUser { get; set; }
        public virtual Delivery_Encasements Delivery_Encasements { get; set; }
    }
}
