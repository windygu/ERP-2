using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.AdminUser
{
    public class DTOAuthItem
    {
        [JsonProperty(PropertyName="id")]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "checked")]
        public bool Checked { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
    }
}
