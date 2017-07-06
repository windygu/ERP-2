using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.ProductFitting
{
    public class VMProductFittingSearchModel : IndexPageBaseModel
    {
        public string No { get; set; }
        public int? FactoryID { get; set; }
        public string FactoryAbbreviation { get; set; }
        public bool isAll { get; set; }
    }
}
