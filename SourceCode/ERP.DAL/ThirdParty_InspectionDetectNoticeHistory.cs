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
    
    public partial class ThirdParty_InspectionDetectNoticeHistory
    {
        public int ID { get; set; }
        public int InspectionID { get; set; }
        public System.DateTime FinishTime { get; set; }
        public int StatusID { get; set; }
        public int ProductID { get; set; }
        public int Qty { get; set; }
        public string Comment { get; set; }
        public System.DateTime DT_CREATEDATE { get; set; }
        public int ST_CREATEUSER { get; set; }
        public string IPAddress { get; set; }
        public bool IsDelete { get; set; }
    }
}
