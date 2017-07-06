using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomAttribute
{
    public class PageMenuAttribute : Attribute
    {
        public string IconClass { get; set; }
        public int ParentID { get; set; }

        public string URL { get; set; }
        public string WinType { get; set; }
        public string WinSize { get; set; }

        public Type PageElementEnumType { get; set; }
    }
}
