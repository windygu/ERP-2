using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.AdminUser
{
    public class VMPrivEdit
    {
        public int RoleID { get; set; }
        public int MenuID { get; set; }
        public List<DTOAuthItem> Items { get; set; }
        //public bool IsPageView { get; set; }
    }
}
