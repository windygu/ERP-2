using ERP.Models.CustomEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Workflow
{
    public class WorkflowPassFilter
    {
        public string PropertyName { get; set; }

        public WorkflowConditionOperations Operation { get; set; }

        public object Value { get; set; }

        public bool IsComplexProperty { get; set; }

        public string ValueType { get; set; }

        public string DBPropertyValueType { get; set; }
    }
}
