using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.AdminUser
{
    public class VMERPRoles
    {
        public int RoleID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime? LastModifyDate { get; set; }

        public DateTime CreateDate { get; set; }

        public bool CanView { get; set; }
    }
}
