using ERP.Models.CustomEnums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Common
{
    public class DTOQRCode
    {
        [JsonProperty(PropertyName="t")]
        public QRCodeType Type { get; set; }

        [JsonProperty(PropertyName = "v")]
        public object Value { get; set; }
    }
}
