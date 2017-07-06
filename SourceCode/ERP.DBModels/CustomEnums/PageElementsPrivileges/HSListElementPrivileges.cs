using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums.PageElementsPrivileges
{
    public enum HSListElementPrivileges
    {
        AddHS = 1 << 0,

        EditHS = 1 << 1,

        Delete = 1 << 2,
    }
}