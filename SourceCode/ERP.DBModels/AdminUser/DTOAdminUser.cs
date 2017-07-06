using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.AdminUser
{
    public class DTOAdminUser
    {
        public int UserID { get; set; }

        public string UserName { get; set; }

        public string UserPhoto { get; set; }

        public string LoginName { get; set; }
    }
}
