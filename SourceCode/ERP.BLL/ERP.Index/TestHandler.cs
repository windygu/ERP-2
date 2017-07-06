using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.ERP.Index
{
    public class TestHandler:IStatHandler
    {
        int num = 99;
        public Models.Index.VMDashboardStat GetDashboardStat(Models.AdminUser.VMERPUser currentUser)
        {
            return new Models.Index.VMDashboardStat 
            {
                BgColor="#578ebe",
                BottomColor = "#4884b8",
                Icon = "fa fa-comments",
                Num = num++,
                Desc = "待审核报价单条目",
                Href = "/Product",
            };
        }
    }

    public class TestHandler2 : IStatHandler
    {
        public Models.Index.VMDashboardStat GetDashboardStat(Models.AdminUser.VMERPUser currentUser)
        {
            return new Models.Index.VMDashboardStat
            {
                BgColor = "#e35b5a",
                BottomColor = "#e04a49",
                Icon = "fa fa-bar-chart-o",
                Num = 999,
                Desc = "报关",
                Href = "/Product",
            };
        }
    }
}
