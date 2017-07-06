using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Customer
{
    public class VMAcceptInformation
    {
        public int AIID { get; set; }
        public int OCID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string MobilePhone { get; set; }
        public string TelPhone { get; set; }
        public Nullable<short> AddressType { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public int Region { get; set; }
        public int Province { get; set; }
        public int Country { get; set; }
        public string PostalCode { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string IsDefault { get; set; }
        public string Comment { get; set; }
    }
}
