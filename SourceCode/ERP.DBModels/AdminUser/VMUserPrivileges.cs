using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DBModels.AdminUser
{
    public class VMUserPrivileges
    {
        public List<int> AllowedMenus { get; set; }
        public Dictionary<string, int> AllowedPagePermissions { get; set; }
        public Dictionary<string, string> AllowedDataPermission { get; set; }
    }
}
