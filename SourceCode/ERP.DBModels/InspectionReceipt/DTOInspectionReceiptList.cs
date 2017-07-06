using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.InspectionReceipt
{
    /// <summary>
    /// 报检单据信息数据列表视图模型
    /// </summary>
    public class DTOInspectionReceiptList : PendingApproveBasePage
    {
        public int StatusID;

        /// <summary>
        /// 审批流权限值
        /// </summary>
        public int? ApproverIndex { get; set; }

        /// <summary>
        /// 采购合同产品所在销售订单自编号
        /// </summary>
        public int OrderID { get; set; }

        /// <summary>
        /// 采购合同自编号
        /// </summary>
        public int? ContractID { get; set; }

        /// <summary>
        /// 采购合同编号
        /// </summary>
        public string PurchaseNumber { get; set; }

        /// <summary>
        /// 工厂简称
        /// </summary>
        public string FactoryAbbreviation { get; set; }

        /// <summary>
        /// 客户代号
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// 报检自编号
        /// </summary>
        public int? HSID { get; set; }

        /// <summary>
        /// 报检编号
        /// </summary>
        public string HsCode { get; set; }

        /// <summary>
        /// 报检名称
        /// </summary>
        public string HsName { get; set; }

        public string UpdateDateForamtter { get; set; }
        public string OrderNumber { get; set; }
        public string Merge { get; set; }
        public string OrderNumberList { get; set; }
        public string OrderIDList { get; set; }
        public int ID { get; set; }
        public string StatusName { get; set; }
        public int ST_CREATEUSER { get; set; }
        public int CustomerID { get; set; }
        public int? ShipmentOrderID { get; set; }
        public bool IsHasApprovalPermission { get; set; }
        public string CurrencyName { get; set; }

        /// <summary>
        /// 是否需要我司报检报关（需要：true）
        /// </summary>
        public bool? IsNeedInspection { get; set; }
        public string IsNeedInspectionName { get; set; }
    }
}