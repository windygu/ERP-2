﻿using ERP.Models.Common;
using ERP.Models.CustomEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.InspectionClearance
{
    public class DTOInspectionClearance : PendingApproveBasePage
    {
        /// <summary>
        /// 清关编号
        /// </summary>
        public int InspectionClearanceID { get; set; }

        /// <summary>
        /// 销售订单编号
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// 客户自编号
        /// </summary>
        public int CustomerID { get; set; }

        /// <summary>
        /// 客户代号
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// 审批流权限值
        /// </summary>
        public int? ApproverIndex { get; set; }

        public int ShipmentOrderID { get; set; }
        public int StatusID { get; set; }
        public string StatusName { get; set; }
        public string DT_MODIFYDATEFormatter { get; set; }
        public int ST_CREATEUSER { get; set; }
        public bool IsHasApprovalPermission { get; set; }
        public string PortName { get; set; }
        public string DestinationPortName { get; set; }
        public string OrderDateStartFormatter { get; set; }
        public string OrderDateEndFormatter { get; set; }
        public string Merge { get; set; }
        public string OrderNumberList { get; set; }
        public string SelectCustomer { get; set; }
    }
}