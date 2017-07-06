using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Country
{
    public class ChinseProvince
    {
        /// <summary>
        /// 省份id
        /// </summary>
        public int ARID { get; set; }

        /// <summary>
        /// 省份名
        /// </summary>
        public string ProvinceName { get; set; }

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