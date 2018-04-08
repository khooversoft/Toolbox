//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Khooversoft.Toolbox;
//using MongoDB.Bson;
//using MongoDB.Driver;
//using Newtonsoft.Json;

//namespace Khooversoft.MongoDb
//{
//    public class MongoDbContext
//    {
//        protected MongoDbContext(string connectionString, string databaseName)
//        {
//            Verify.IsNotEmpty(nameof(connectionString), connectionString);
//            Verify.IsNotEmpty(nameof(databaseName), databaseName);

//            this.ConnectionString = connectionString;
//            this.DatabaseName = databaseName;
//        }

//        public string ConnectionString { get; private set; }

//        public string DatabaseName { get; private set; }

//        public MongoClient Client { get; private set; }

//        public IMongoDatabase MongoDatabase { get; private set; }

//        /// <summary>
//        /// Open database
//        /// </summary>
//        /// <param name="connectionString">Connection string</param>
//        /// <param name="databaseName">Database name</param>
//        /// <returns>this</returns>
//        public static MongoDbContext Open(string connectionString, string databaseName)
//        {
//            MongoDbContext context = new MongoDbContext(connectionString, databaseName);

//            context.Client = new MongoClient(connectionString);
//            context.MongoDatabase = context.Client.GetDatabase(databaseName);

//            return context;
//        }

//        /// <summary>
//        /// Open database
//        /// </summary>
//        /// <param name="connectionString">Connection string</param>
//        /// <param name="databaseName">Database name</param>
//        /// <returns>this</returns>
//        public static Task<MongoDbContext> OpenAsync(string connectionString, string databaseName)
//        {
//            return Task.FromResult(MongoDbContext.Open(connectionString, databaseName));
//        }

//        /// <summary>
//        /// Remove collection from database
//        /// </summary>
//        /// <param name="name"></param>
//        /// <returns></returns>
//        public async Task RemoveCollectionAsync(string name)
//        {
//            Verify.IsNotEmpty(nameof(name), name);

//            await this.MongoDatabase.DropCollectionAsync(name);
//        }

//        public async Task<IList<CollectionDetails>> GetCollectionDetails()
//        {
//            List<CollectionDetails> details = new List<CollectionDetails>();

//            using (IAsyncCursor<BsonDocument> cursor = await this.MongoDatabase.ListCollectionsAsync())
//            {
//                IList<BsonDocument> list = await cursor.ToListAsync();

//                foreach (var item in list)
//                {
//                    string name = item["name"].ToString();
//                    if (name == "system.indexes") continue;

//                    details.Add(new CollectionDetails { CollectionName = name });
//                }
//            }

//            foreach (var item in details)
//            {
//                DbClient client = DbClient.Open(this, item.CollectionName);
//                item.NumberOfDocuments = (int)client.Collection.Count(new BsonDocument());
//            }

//            return details;
//        }
//    }
//}
