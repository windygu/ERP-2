using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Tools.Enum
{
    /// <summary>
    /// 采购合同状态
    /// </summary>
    internal enum PurchaseContractStatus
    {
        /// <summary>
        /// 草稿
        /// </summary>
        OutLine,

        /// <summary>
        /// 待审核
        /// </summary>
        PendingCheck,

        /// <summary>
        /// 审核中
        /// </summary>
        Checking,

        /// <summary>
        /// 审核未通过
        /// </summary>
        NotPassCheck,

        /// <summary>
        /// 审核已通过
        /// </summary>
        PassedCheck,
    }
}