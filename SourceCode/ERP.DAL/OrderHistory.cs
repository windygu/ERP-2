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
    
    public partial class OrderHistory
    {
        public int ID { get; set; }
        public int OrderID { get; set; }
        public string Comment { get; set; }
        public string CheckSuggest { get; set; }
        public System.DateTime DT_CREATEDATE { get; set; }
        public int ST_CREATEUSER { get; set; }
        public string IPAddress { get; set; }
    
        public virtual Order Order { get; set; }
    }
}
