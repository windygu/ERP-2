using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.InspectionReceipt
{
    /// <summary>
    /// 报检历史记录
    /// </summary>
    public class VMInspectionReceiptHis
    {
        /// <summary>
        /// 审批人
        /// </summary>
        public string AuditUserName { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string AuditIdea { get; set; }

        /// <summary>
        /// 审批时间
        /// </summary>
        public string AuditCreateDate { get; set; }

        /// <summary>
        /// 报检状态
        /// </summary>
        public string InspectionReceiptStatus { get; set; }
    }
}