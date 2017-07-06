using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Product
{
    public class VMProductSearchModel : IndexPageBaseModel
    {
        public string ProductNO { get; set; }
        public string CustomerNO { get; set; }
        public string FactoryName { get; set; }
        public string Keyword { get; set; }
    }
}
