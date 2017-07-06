using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums
{
    public enum HierachyType : short
    {
        /// <summary>
        /// 等同于其他部门
        /// </summary>
        Normal,

        /// <summary>
        /// 办事处
        /// </summary>
        Agency,

        /// <summary>
        /// 设计部
        /// </summary>
        Designer,

        /// <summary>
        /// 业务部
        /// </summary>
        Business,
    }
}
