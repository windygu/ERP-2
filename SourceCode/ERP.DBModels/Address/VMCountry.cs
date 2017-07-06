using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Address
{
    public class VMCountry
    {
        public int COID { get; set; }

        /// <summary>
        /// 国家名称
        /// </summary>
        public string CountryName { get; set; }
        
        public string Abbreviation { get; set; }
        public string AbbreviationA { get; set; }
    }
}
