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
    
    public partial class Reg_Country
    {
        public Reg_Country()
        {
            this.Orders_Customers = new HashSet<Orders_Customers>();
            this.Reg_Area = new HashSet<Reg_Area>();
        }
    
        public int COID { get; set; }
        public string CountryName { get; set; }
        public string Abbreviation { get; set; }
        public string AbbreviationA { get; set; }
    
        public virtual ICollection<Orders_Customers> Orders_Customers { get; set; }
        public virtual ICollection<Reg_Area> Reg_Area { get; set; }
    }
}
