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
    
    public partial class Hierarchy
    {
        public Hierarchy()
        {
            this.Factories = new HashSet<Factory>();
            this.Hierarchy1 = new HashSet<Hierarchy>();
            this.SystemUsers = new HashSet<SystemUser>();
        }
    
        public int HierarchyID { get; set; }
        public Nullable<int> ParentID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string LastModifyBy { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public short Status { get; set; }
        public bool IsDeleted { get; set; }
        public short HierachyType { get; set; }
    
        public virtual ICollection<Factory> Factories { get; set; }
        public virtual ICollection<Hierarchy> Hierarchy1 { get; set; }
        public virtual Hierarchy Hierarchy2 { get; set; }
        public virtual ICollection<SystemUser> SystemUsers { get; set; }
    }
}
