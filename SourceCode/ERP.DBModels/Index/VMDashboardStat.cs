using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Index
{
    public class VMDashboardStat
    {
        [JsonProperty("num")]
        public int Num { get; set; }
        [JsonProperty("desc")]
        public string Desc { get; set; }
        [JsonProperty("href")]
        public string Href { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
        [JsonProperty("bgColor")]
        public string BgColor { get; set; }
        [JsonProperty("bottomColor")]
        public string BottomColor { get; set; }
    }
}
