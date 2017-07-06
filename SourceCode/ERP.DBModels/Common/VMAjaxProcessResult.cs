using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Common
{
    public class VMAjaxProcessResult
    {
        [JsonProperty(PropertyName = "ok")]
        public bool IsSuccess { get; set; }

        [JsonProperty(PropertyName = "msg")]
        public string Msg { get; set; }

        [JsonProperty(PropertyName = "data")]
        public object Data { get; set; }

        public string identity { get; set; }
        public VMAjaxProcessResult()
        {
            IsSuccess = true;
            Msg = "OK";
           
        }
    }
}