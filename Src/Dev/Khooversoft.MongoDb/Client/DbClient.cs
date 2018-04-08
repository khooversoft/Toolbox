//using Khooversoft.Toolbox;
//using MongoDB.Bson;
//using MongoDB.Driver;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Khooversoft.MongoDb
//{
//    public class DbClient
//    {
//        DbClient(MongoDbContext dbContext, string collectionName)
//        {
//            Verify.IsNotNull(nameof(dbContext), dbContext);
//            Verify.IsNotEmpty(nameof(collectionName), collectionName);

//            DbContext = dbContext;
//            CollectionName = collectionName;
//        }

//        /// <summary>
//        /// Open client
//        /// </summary>
//        /// <param name="dbContext">Db context</param>
//        /// <param name="collectionName">Collection name</param>
//        /// <returns>Client</returns>
//        public static DbClient Open(MongoDbContext dbContext, string collectionName)
//        {
//            DbClient client = new DbClient(dbContext, collectionName);
//            client.Collection = dbContext.MongoDatabase.GetCollection<BsonDocument>(collectionName);

//            return client;
//        }

//        /// <summary>
//        /// Open client
//        /// </summary>
//        /// <param name="dbContext">Db connection</param>
//        /// <param name="collectionName">Collection name</param>
//        /// <returns>Client</returns>
//        public static Task<DbClient> OpenAsync(MongoDbContext dbContext, string collectionName)
//        {
//            DbClient client = new DbClient(dbContext, collectionName);
//            client.Collection = dbContext.MongoDatabase.GetCollection<BsonDocument>(collectionName);

//            return Task.FromResult(client);
//        }

//        /// <summary>
//        /// DB context being used
//        /// </summary>
//        public MongoDbContext DbContext { get; private set; }

//        /// <summary>
//        /// Collection
//        /// </summary>
//        public IMongoCollection<BsonDocument> Collection { get; private set; }

//        /// <summary>
//        /// Collection name
//        /// </summary>
//        public string CollectionName { get; private set; }

//        /// <summary>
//        /// Clear all documents in the collection
//        /// </summary>
//        /// <returns>Done</returns>
//        public async Task ClearAllDocuments()
//        {
//            await Collection.DeleteManyAsync(new BsonDocument());
//        }

//        /// <summary>
//        /// Create unique index
//        /// </summary>
//        /// <param name="keyName">key name</param>
//        /// <param name="unique">Create a unique index</param>
//        /// <param name="sparse">create a spare index</param>
//        /// <param name="compoundKeys">compound keys (if this list is not empty, use these keys for the index)</param>
//        /// <param name="filter">filter data</param>
//        /// <returns>Task</returns>
//        public async Task CreateIndex(string keyName, bool unique = false, bool sparse = false, IList<string> compoundKeys = null, DbCollectionPartialFilter partialFilter = null)
//        {
//            Verify.IsNotEmpty(nameof(keyName), keyName);
//            Verify.Assert(sparse == false || (compoundKeys == null || compoundKeys.Count == 0), "sparse index does not support compound indexes");

//            using (var cursor = await Collection.Indexes.ListAsync())
//            {
//                var indexes = await cursor.ToListAsync();
//                if (indexes.Count(index => index["name"] == keyName + "_1") == 0)
//                {
//                    if (compoundKeys == null || compoundKeys.Count == 0)
//                    {
//                        var keys = Builders<BsonDocument>.IndexKeys.Ascending(keyName);
//                        await Collection.Indexes.CreateOneAsync(keys, new CreateIndexOptions() { Unique = unique, Version = 1, Sparse = sparse });

//                        return;
//                    }

//                    // Create compound keys
//                    var multiKeys = Builders<BsonDocument>.IndexKeys.Ascending(compoundKeys[0]);
//                    foreach (var item in compoundKeys.Skip(1))
//                    {
//                        multiKeys = multiKeys.Ascending(item);
//                    }

//                    var indexOption = new CreateIndexOptions<BsonDocument>
//                    {
//                        Name = keyName + "_1",
//                        Unique = unique,
//                        Version = 1,
//                        Sparse = sparse,
//                    };

//                    if (partialFilter != null)
//                    {
//                        var filterClause = new BsonDocument();

//                        if (partialFilter.Value.GetType() == typeof(string))
//                        {
//                            filterClause.Add("$eq", (string)partialFilter.Value);
//                        }
//                        if (partialFilter.Value.GetType() == typeof(bool))
//                        {
//                            filterClause.Add("$eq", (bool)partialFilter.Value);
//                        }

//                        var partial = new BsonDocument
//                        {
//                            { partialFilter.Key, filterClause }
//                        };

//                        indexOption.PartialFilterExpression = partial;
//                    }
//                    await Collection.Indexes.CreateOneAsync(multiKeys, indexOption);
//                    return;
//                }
//            }
//        }

//        /// <summary>
//        /// Create composite unique index
//        /// </summary>
//        /// <param name="keyName">key names</param>
//        /// <param name="unique">Create a unique key</param>
//        /// <returns>Task</returns>
//        public async Task CreateIndex(IEnumerable<string> keyNames, bool unique = false)
//        {
//            Verify.IsNotNull(nameof(keyNames), keyNames);

//            string keyName = string.Join("_", keyNames.Select((x) => x + "_1"));

//            using (var cursor = await Collection.Indexes.ListAsync())
//            {
//                var indexes = await cursor.ToListAsync();
//                if (indexes.Count(index => index["name"] == keyName + "_1") == 0)
//                {
//                    var keys = Builders<BsonDocument>.IndexKeys;
//                    IndexKeysDefinition<BsonDocument> keyReference = null;

//                    foreach (var key in keyNames)
//                    {
//                        keyReference = keys.Ascending(key);
//                    }

//                    await Collection.Indexes.CreateOneAsync(keyReference, new CreateIndexOptions() { Unique = unique, Version = 1 });
//                }
//            }
//        }

//        /// <summary>
//        /// Remove all indexes
//        /// </summary>
//        /// <returns>Done</returns>
//        public async Task RemoveAllIndexes()
//        {
//            using (var cursor = await Collection.Indexes.ListAsync())
//            {
//                var indexes = await cursor.ToListAsync();

//                foreach (var item in indexes)
//                {
//                    string name = item["name"].AsString;

//                    if (name.StartsWith("_") == true) continue;

//                    var query = new BsonDocument
//                    {
//                        { name, "1" }
//                    };

//                    await Collection.DeleteOneAsync(query);
//                }
//            }
//        }

//        /// <summary>
//        /// Search database
//        /// </summary>
//        /// <param name="query">query optional</param>
//        /// <param name="projection">projects (field to return, optional)</param>
//        /// <param name="start">start</param>
//        /// <param name="count">count</param>
//        /// <returns>Bson document list</returns>
//        public async Task<IList<BsonDocument>> Search(BsonDocument query, BsonDocument projection = null, long start = 0, long count = long.MaxValue)
//        {
//            var itemList = new List<BsonDocument>();
//            int itemCount = 0;

//            FindOptions<BsonDocument, BsonDocument> findOptions = null;
//            if (projection != null)
//            {
//                findOptions = new FindOptions<BsonDocument, BsonDocument>();
//                findOptions.Projection = projection;
//            }

//            using (var cursor = await Collection.FindAsync(query, findOptions))
//            {
//                var existing = await cursor.ToListAsync();

//                foreach (var item in existing)
//                {
//                    if (itemCount++ < start) continue;

//                    itemList.Add(item);
//                    if (itemList.Count > count) return itemList;
//                }
//            }

//            return itemList;
//        }

//        /// <summary>
//        /// Search database
//        /// </summary>
//        /// <typeparam name="T">document to return</typeparam>
//        /// <param name="rootElementName">document name</param>
//        /// <param name="query">query</param>
//        /// <param name="projection">project (optional)</param>
//        /// <param name="start">starting index (optional)</param>
//        /// <param name="count">count (optional)</param>
//        /// <returns>IList of documents</returns>
//        public async Task<IList<T>> Search<T>(string rootElementName, BsonDocument query, BsonDocument projection = null, long start = 0, long count = long.MaxValue)
//        {
//            Verify.IsNotEmpty(nameof(rootElementName), rootElementName);
//            Verify.IsNotNull(nameof(query), query);

//            var itemList = new List<T>();

//            var documents = await Search(query, projection, start, count);

//            foreach (var item in documents)
//            {
//                var json = item[rootElementName].ToJsonStrict();
//                var newItem = JsonConvert.DeserializeObject<T>(json, JsonUtility.JsonSetting);

//                itemList.Add(newItem);
//            }

//            return itemList;
//        }

//        /// <summary>
//        /// Find one and update
//        /// </summary>
//        /// <typeparam name="T">document to return</typeparam>
//        /// <param name="rootElementName">document name</param>
//        /// <param name="query">query</param>
//        /// <param name="update">update fields</param>
//        /// <param name="options">options (optional)</param>
//        /// <returns>Update document</returns>
//        public async Task<T> FindOneAndUpdate<T>(string rootElementName, BsonDocument query, BsonDocument update, FindOneAndUpdateOptions<BsonDocument> options = null)
//        {
//            Verify.IsNotEmpty(nameof(rootElementName), rootElementName);
//            Verify.IsNotNull(nameof(query), query);
//            Verify.IsNotNull(nameof(update), update);

//            var result = await Collection.FindOneAndUpdateAsync(query, update, options);
//            if (result == null) return default(T);

//            return JsonConvert.DeserializeObject<T>(result[rootElementName].ToJsonStrict(), JsonUtility.JsonSetting);
//        }

//        /// <summary>
//        /// Update one record based on query
//        /// </summary>
//        /// <param name="query">query</param>
//        /// <param name="update">update</param>
//        /// <param name="updateOptions">options</param>
//        /// <returns>true if record was update or inserted</returns>
//        public async Task<bool> UpdateOne(BsonDocument query, BsonDocument update, UpdateOptions updateOptions = null)
//        {
//            Verify.IsNotNull(nameof(query), query);
//            Verify.IsNotNull(nameof(update), update);

//            var result = await Collection.UpdateOneAsync(query, update, updateOptions);

//            if (updateOptions != null && updateOptions.IsUpsert == true)
//            {
//                return result.IsAcknowledged == true && result.UpsertedId != null;
//            }

//            return result.IsAcknowledged == true && result.ModifiedCount > 0;
//        }

//        /// <summary>
//        /// Aggregate documents based on pipeline instructions
//        /// </summary>
//        /// <typeparam name="T">type to return</typeparam>
//        /// <param name="pipeline">pipeline instructions</param>
//        /// <param name="options">options</param>
//        /// <returns>list of types from aggregation</returns>
//        public async Task<IList<T>> Aggregate<T>(BsonDocument[] pipeline, AggregateOptions options = null)
//        {
//            Verify.IsNotNull(nameof(pipeline), pipeline);

//            var docList = new List<BsonDocument>();

//            PipelineDefinition<BsonDocument, BsonDocument> pipelineDef = pipeline;

//            using (var cursor = await Collection.AggregateAsync(pipelineDef, options))
//            {
//                var existing = await cursor.ToListAsync();

//                foreach (var item in existing)
//                {
//                    docList.Add(item);
//                }
//            }

//            var itemList = new List<T>();
//            foreach (var item in docList)
//            {
//                var json = item.ToJsonStrict();
//                var newItem = JsonConvert.DeserializeObject<T>(json, JsonUtility.JsonSetting);

//                itemList.Add(newItem);
//            }

//            return itemList;
//        }

//    }
//}
