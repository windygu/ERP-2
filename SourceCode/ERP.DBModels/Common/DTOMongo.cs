using ERP.Models.CustomEnums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Common
{
    public class DTOMongo<T> where T : class
    {
        [BsonId]
        [JsonProperty(PropertyName = "_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ObjectId _id { get; set; }

        [BsonElement]
        [JsonProperty(PropertyName = "LastUpdateDate", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime LastUpdateDate { get; set; }

        [BsonElement]
        [JsonProperty(PropertyName = "UserID", Required = Required.Always)]
        public int UserID { get; set; }

        [BsonElement]
        [JsonProperty(PropertyName = "Type", Required = Required.Always)]
        public MongoCachedTypes Type { get; set; }

        [BsonElement]
        [JsonProperty(PropertyName = "Data", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public T Data { get; set; }
    }
}
