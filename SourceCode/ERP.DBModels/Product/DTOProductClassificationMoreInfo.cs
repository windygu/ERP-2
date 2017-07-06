using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Product
{
    public class DTOProductClassificationMoreInfo:IndexPageBaseModel
    {
        [JsonProperty(PropertyName = "id")]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "parentid")]
        public int? ParentID { get; set; }

        [JsonProperty(PropertyName = "parentname")]
        public string ParentName { get; set; }

        [JsonProperty(PropertyName = "image")]
        public string Image { get; set; }

        [JsonProperty(PropertyName = "show")]
        public bool Show { get; set; }
    }
}
