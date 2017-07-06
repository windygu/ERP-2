using ERP.Models.CustomEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomAttribute
{
    public class PageElementImpactOnPagesAttribute : Attribute
    {
        public ERPPage[] Pages { get; set; }
    }
}
