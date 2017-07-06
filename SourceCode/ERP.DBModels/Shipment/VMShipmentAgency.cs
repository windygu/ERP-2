using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Shipment
{
    public class VMShipmentAgency
    {
        public int ShippingAgencyID { get; set; }
        public string ShippingAgencyName { get; set; }
        public int CreateUserID { get; set; }
        public DateTime CreateDate { get; set; }
        public string LastModifyBy { get; set; }
        public DateTime? LastModifyDate { get; set; }
        public bool IsDeleted { get; set; }

        public DTOShipmentAgentFees CurrentShipmentAgentFees { get; set; }

        public  List<DTOShipmentAgentFees> ShipmentAgentFeesHistory { get; set; }

        /// <summary>
        /// 船代公司地址
        /// </summary>
        public string AgencyAddress { get; set; }

        /// <summary>
        /// 仓库地址
        /// </summary>
        public string WarehouseAddress { get; set; }
    }
}
