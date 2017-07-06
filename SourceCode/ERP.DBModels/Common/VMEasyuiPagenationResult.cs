using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Common
{
    public class VMEasyuiPagenationResult
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("rows")]
        public object Rows { get; set; }

        [JsonProperty("footer")]
        public object Fotter { get; set; }
    }
}
