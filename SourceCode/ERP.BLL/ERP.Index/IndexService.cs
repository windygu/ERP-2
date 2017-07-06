using ERP.BLL.ERP.Quote;
using ERP.Models.AdminUser;
using ERP.Models.Index;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.ERP.Index
{
    public class IndexService
    {
        private static List<IStatHandler> _handlerList;

        static IndexService()
        {
            _handlerList = new List<IStatHandler>()
            {
                new QuoteApprovalHandler(),
                //new TestHandler2(),
            };
        }

        public List<VMDashboardStat> GetAllDashboardStat(VMERPUser currentUser)
        {
            List<VMDashboardStat> statList = new List<VMDashboardStat>();

            try
            {
                foreach (var handler in _handlerList)
                {
                    // TODO 如果数量为0，是否应不显示？？
                    statList.Add(handler.GetDashboardStat(currentUser));
                }

            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return statList;
        }
    }
}
