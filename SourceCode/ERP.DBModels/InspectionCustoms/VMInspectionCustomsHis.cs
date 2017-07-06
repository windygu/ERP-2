using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.InspectionCustoms
{
    /// <summary>
    /// 报关管理->报关历史记录
    /// </summary>
    public class VMInspectionCustomsHis
    {
        /// <summary>
        /// 报关履历自编号
        /// </summary>
        public int ICHISID { get; set; }

        /// <summary>
        /// 报关信息自编号
        /// </summary>
        public int InspectionCustomsID { get; set; }

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
        /// 报关状态
        /// </summary>
        public string InspectionCustomsStatus { get; set; }
    }
}