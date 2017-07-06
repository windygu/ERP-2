using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.AdminUser
{
    public class VMCustomer
    {
        public int CustomerID { get; set; }

        public string CustomerName { get; set; }

        public string CustomerCode { get; set; }
        
        public bool CanView { get; set; }
    }
}
