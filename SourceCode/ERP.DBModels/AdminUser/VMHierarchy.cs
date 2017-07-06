using ERP.Models.CustomEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.AdminUser
{
    public class VMHierarchy
    {
        public int HierarchyID { get; set; }

        public string Name { get; set; }

        public DateTime CreateDate { get; set; }

        public HierachyType HierachyType { get; set; }

        public VMHierarchy ParentHierarchy { get; set; }

        public List<VMHierarchy> SubHierarchies { get; set; }
    }
}
