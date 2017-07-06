using ERP.Models.CustomEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.InspectionReceipt
{
    /// <summary>
    /// 报检单据筛选条件
    /// </summary>
    public class VMFilterInspectionReceipt : IndexPageBaseModel
    {
        public PageTypeEnum PageType { get; set; }

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

        public string StatusID { get; set; }
    }
}