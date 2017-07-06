using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Customer
{
    public class VMCustomerSearchModel : IndexPageBaseModel
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public int Country { get; set; }
        public int Province { get; set; }
    }
}
