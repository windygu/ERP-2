﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Purchase
{
    /// <summary>
    /// 采购合同——历史记录
    /// </summary>
    public class VMPurchaseHistory
    {
        /// <summary>
        /// 创建用户
        /// </summary>
        public string ST_CREATEUSER { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime DT_CREATEDATE { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 意见\备注
        /// </summary>
        public string CheckSuggest { get; set; }
    }
}