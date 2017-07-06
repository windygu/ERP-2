using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Country
{
    public class WorldCountry
    {
        /// <summary>
        /// 国家id
        /// </summary>
        public int CIID { get; set; }

        /// <summary>
        /// 国家名称
        /// </summary>
        public string CountryName
        {
            get;
            set;
        }
    }
}