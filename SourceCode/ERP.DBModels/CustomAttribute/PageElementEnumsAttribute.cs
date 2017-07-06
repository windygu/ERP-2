using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomAttribute
{
    public class PageElementEnumsAttribute : Attribute
    {
        public Enum Privilege { get; set; }
    }
}
