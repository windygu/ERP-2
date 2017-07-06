using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums
{
    public enum AdminUserStatus
    {

        /// <summary>
        /// 未激活
        /// </summary>
        [Description("未激活")]
        NotActived = 0,

        /// <summary>
        /// 冻结
        /// </summary>
        [Description("冻结")]
        Locked = 1,

        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 2,

        /// <summary>
        /// 没找到用户名
        /// </summary>
        [Description("没找到用户名")]
        NotFind = 3,
    }
}
