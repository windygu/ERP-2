using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums
{
    public enum WorkflowPassType : short
    {
        /// <summary>
        /// 同级中任意一级表决通过
        /// </summary>
        AnyPassedWillPass,

        /// <summary>
        /// 同级中所有人（或角色）表决通过
        /// </summary>
        AllPassedWillPass,
    }
}
