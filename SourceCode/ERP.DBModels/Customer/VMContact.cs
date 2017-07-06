using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Customer
{
    public class VMContact
    {
        public int OLID { get; set; }
        public int OCID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Duty { get; set; }
        public string MobilePhone { get; set; }
        public string TelPhone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string IsDefault { get; set; }
        public int? SeasonID { get; set; }
        public string SeasonIDList { get; set; }
    }
}
