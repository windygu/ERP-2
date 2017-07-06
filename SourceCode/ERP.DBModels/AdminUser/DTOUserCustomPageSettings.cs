using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.AdminUser
{
    public class DTOUserCustomPageSettings
    {
        public int UserID { get; set; }
        public int PageID { get; set; }
        public string DatagridColumnVisibility { get; set; }
    }
}
