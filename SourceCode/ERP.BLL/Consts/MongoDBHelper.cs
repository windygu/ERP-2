using ERP.Models.AdminUser;
using ERP.Models.CustomEnums;
using ERP.Models.Product;
using ERP.Tools.Logs;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.Consts
{
    public class MongoDBHelper
    {
        static string _mongoDBConnectString = ConfigurationManager.AppSettings["mongoConn"];
        static string _DBName = ConfigurationManager.AppSettings["mongoDBName"];
        public const string CollectionName_UserCache = "UserCachedData";

        public List<DTOMongoLog> GetLogs(string type, string dateFrom, string keyword, int currentPage, int pageSize, out int totalRows)
        {
            totalRows = 0;

            try
            {
                DateTime dtFrom = DateTime.Now;
                DateTime.TryParse(dateFrom, out dtFrom);
                IMongoClient client = new MongoClient(new MongoUrl(_mongoDBConnectString));
                IMongoDatabase _database = client.GetDatabase(_DBName);
                var collection = _database.GetCollection<BsonDocument>(type);
                var builder = Builders<BsonDocument>.Filter;
                var filter = builder.Gte("timestamp", new BsonDateTime(dtFrom));
                if (!string.IsNullOrEmpty(keyword))
                {
                    filter &= builder.Regex("message", new BsonRegularExpression(keyword));
                }

                var query = collection.Find(filter).SortByDescending(p => p["timestamp"]);

                totalRows = (int)query.Count();
                var list = query.Skip((currentPage - 1) * pageSize).Limit(pageSize).ToList();

                var dto = (from r in list
                           let d = r.AsBsonDocument
                           select new DTOMongoLog()
                           {
                               Date = d["timestamp"].ToUniversalTime().AddHours(8).ToString(),
                               Message = d["message"].ToString()
                           }).ToList();
                return dto;
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return null;
        }

        /// <summary>
        /// 如果filter不为null，使用的是Upsert模式，如果有了记录则修改，若没有该记录则新增。
        /// 如果filter为null，使用Insert模式。
        /// </summary>
        /// <param name="document"></param>
        public void UpsetOne(BsonDocument document, string collectionName, FilterDefinition<BsonDocument> filter = null)
        {
            IMongoClient client = new MongoClient(new MongoUrl(_mongoDBConnectString));
            IMongoDatabase _database = client.GetDatabase(_DBName);
            var collection = _database.GetCollection<BsonDocument>(collectionName);
            if (filter != null)
            {
                collection.ReplaceOne(filter, document, new UpdateOptions() { IsUpsert = true });
            }
            else
            {
                collection.InsertOne(document);
            }
        }

        public BsonDocument GetOneRecord(FilterDefinition<BsonDocument> filter, string collectionName)
        {
            IMongoClient client = new MongoClient(new MongoUrl(_mongoDBConnectString));
            IMongoDatabase _database = client.GetDatabase(_DBName);
            var collection = _database.GetCollection<BsonDocument>(collectionName);

            var result = collection.Find(filter).ToList();
            return result.FirstOrDefault();
        }

        public bool RemoveOneRecord(FilterDefinition<BsonDocument> filter, string collectionName)
        {
            IMongoClient client = new MongoClient(new MongoUrl(_mongoDBConnectString));
            IMongoDatabase _database = client.GetDatabase(_DBName);
            var collection = _database.GetCollection<BsonDocument>(collectionName);
            var deleteResult = collection.DeleteOne(filter);
            return deleteResult.DeletedCount == 1;
        }
    }
}
