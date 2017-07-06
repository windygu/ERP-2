using ERP.Models.CustomEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Inspection
{
    public class VMInspectionSearch : IndexPageBaseModel
    {
        /// <summary>
        /// 采购订单编号
        /// </summary>
        public string PurchaseNumber { get; set; }

        /// <summary>
        /// 工厂名称
        /// </summary>
        public string FactoryName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string StatusID { get; set; }

        /// <summary>
        /// 第三方验厂名称
        /// </summary>
        public string InspectionName { get; set; }

        /// <summary>
        /// 1：验厂 2：检测 3：抽检
        /// </summary>
        public InspectionTypeEnum TypeID { get; set; }

        public PageTypeEnum PageType { get; set; }
    }
}