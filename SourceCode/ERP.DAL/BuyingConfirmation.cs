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
    
    public partial class BuyingConfirmation
    {
        public int ID { get; set; }
        public string OrderIDList { get; set; }
        public string GeneratedFileList { get; set; }
        public string UploadFileList { get; set; }
        public bool IsDelete { get; set; }
        public System.DateTime DT_CREATEDATE { get; set; }
        public int ST_CREATEUSER { get; set; }
        public System.DateTime DT_MODIFYDATE { get; set; }
        public int ST_MODIFYUSER { get; set; }
        public string IPAddress { get; set; }
    }
}
