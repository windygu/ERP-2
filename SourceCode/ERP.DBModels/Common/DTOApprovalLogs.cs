using ERP.Models.CustomEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Common
{
    public class DTOApprovalLogs
    {
        public WorkflowTypes ApprovalType { get; set; }

        public DateTime DateTime { get; set; }

        public string Message { get; set; }

        public object CurrentyEntity { get; set; }

        public object CurrentWorkflowDetail { get; set; }

        public string ApproveOpinion { get; set; }

        public int? ApproverID { get; set; }

        public int CurrentStatus { get; set; }

        public int? CurrentApproveIndex { get; set; }
    }
}
