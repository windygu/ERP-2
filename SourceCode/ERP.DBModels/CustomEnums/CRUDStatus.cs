using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums
{
    public enum DBOperationStatus : short
    {
        /// <summary>
        /// 失败
        /// </summary>
        Failed = 0,

        /// <summary>
        /// 成功
        /// </summary>
        Success = 1,

        /// <summary>
        /// 没有受影响的记录
        /// </summary>
        NoAffect = 2,

        /// <summary>
        /// 不允许删除
        /// </summary>
        NotAllowed = 3,

        /// <summary>
        /// 记录已存在
        /// </summary>
        ExistingRecord = 4,
    }
}
