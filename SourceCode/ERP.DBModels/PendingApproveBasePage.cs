using ERP.Models.AdminUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models
{
    public abstract class PendingApproveBasePage
    {
        public string NextApproverDisplayNames
        {
            get
            {
                if (NextApproverInfos != null)
                {
                    StringBuilder sbNames = new StringBuilder();
                    foreach (var ui in NextApproverInfos)
                    {
                        sbNames.AppendFormat("{0},", ui.DisplayName);
                    }
                    return sbNames.ToString().TrimEnd(new char[] { ',' });
                }

                return string.Empty;
            }
        }

        public List<VMERPUser> NextApproverInfos { get; set; }
    }
}
