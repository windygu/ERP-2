using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Customer
{
   public class VMFreightRate
    {
       public int ID { get; set; }

       public int OCID { get; set; }

       public int PortID { get; set; }

       public decimal? FreightRate { get; set; }

       public int Type { get; set; }

       public bool IsDelete { get; set; }
    }
}
