using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Models.Shipment
{
    public class DTOShipmentAgentFees
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// ShippingAgencyID
        /// </summary>
        public int ShippingAgencyID { get; set; }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 文本费
        /// </summary>
        public decimal FeeDocument { get; set; }

        /// <summary>
        /// 码头操作费
        /// </summary>
        public decimal FeeDockOperation { get; set; }

        /// <summary>
        /// 附加提还箱费
        /// </summary>
        public decimal FeeYangShanPicking { get; set; }

        /// <summary>
        /// 散货设备管理费
        /// </summary>
        public decimal FeeFacilityManagement { get; set; }

        /// <summary>
        /// 散货港口设备安保费
        /// </summary>
        public decimal FeePortSecurity { get; set; }

        /// <summary>
        /// 进口商安保归类
        /// </summary>
        public decimal FeeImporterSecurityClassify { get; set; }

        /// <summary>
        /// 散货入仓费
        /// </summary>
        public decimal FeeWarehousing { get; set; }

        /// <summary>
        /// 提单费
        /// </summary>
        public decimal FeePicking { get; set; }

        /// <summary>
        /// 报关费
        /// </summary>
        public decimal FeeCustomDeclaration { get; set; }

        /// <summary>
        /// CreateUserID
        /// </summary>
        public int CreateUserID { get; set; }

        /// <summary>
        /// CreateDate
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// LastModifyBy
        /// </summary>
        public string LastModifyBy { get; set; }

        /// <summary>
        /// LastModifyDate
        /// </summary>
        public DateTime? LastModifyDate { get; set; }

        /// <summary>
        /// IsValid
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 船运公司名称
        /// </summary>
        public string ShippingAgencyName { get; set; }
    }
}