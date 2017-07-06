using ERP.BLL.ERP.Index;
using ERP.Models.AdminUser;
using ERP.Models.Index;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.ERP.Quote
{
    public class QuoteApprovalHandler : IStatHandler
    {
        VMDashboardStat IStatHandler.GetDashboardStat(VMERPUser currentUser)
        {
            return new VMDashboardStat
            {
                BgColor = "#578ebe",
                BottomColor = "#4884b8",
                Icon = "fa fa-comments",
                Num = new QuoteService().GetPendingApproveCountByUser(currentUser),
                Desc = "待审核报价单条目",
                Href = "/Quote/PassedCheck",
            };
        }
    }
}
