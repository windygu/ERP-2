using ERP.Models.AdminUser;
using ERP.Models.Index;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.ERP.Index
{
    interface IStatHandler
    {
        VMDashboardStat GetDashboardStat(VMERPUser currentUser);
    }
}
