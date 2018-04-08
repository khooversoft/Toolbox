//using Khooversoft.Toolbox;
//using MongoDB.Bson;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Khooversoft.MongoDb
//{
//    public class DbCollection
//    {
//        readonly KeyedDictionary<string, DbCollectionKey> _keys = new KeyedDictionary<string, DbCollectionKey>((x) => x.Name);
//        readonly KeyedDictionary<string, DbCollectionDependency> _dependency;
//        DbClient _dbClient;

//        public DbCollection(string collectionName)
//        {
//            Verify.IsNotEmpty(nameof(collectionName), collectionName);

//            _dependency = new KeyedDictionary<string, DbCollectionDependency>(x => x.ChildCollectionName, x => { x.Parent = this; return x; });
//            CollectionName = collectionName;
//        }

//        /// <summary>
//        /// Parent reference
//        /// </summary>
//        public DbCollectionManagement Parent { get; internal set; }

//        /// <summary>
//        /// Get DB client
//        /// </summary>
//        public DbClient Client
//        {
//            get
//            {
//                if (_dbClient == null)
//                {
//                    _dbClient = DbClient.Open(Parent.DbContext, CollectionName);
//                }

//                return _dbClient;
//            }
//        }

//        /// <summary>
//        /// Collection name
//        /// </summary>
//        public string CollectionName { get; private set; }

//        /// <summary>
//        /// Primary key
//        /// </summary>
//        public string PrimaryKey { get; set; }

//        /// <summary>
//        /// Root element name
//        /// </summary>
//        public string RootElementName { get; set; }

//        /// <summary>
//        /// Keys
//        /// </summary>
//        public KeyedDictionary<string, DbCollectionKey> Keys { get { return _keys; } }

//        /// <summary>
//        /// Dependencies (collection to collection)
//        /// </summary>
//        public KeyedDictionary<string, DbCollectionDependency> Dependencies { get { return _dependency; } }

//        /// <summary>
//        /// Build indexes
//        /// </summary>
//        /// <returns>Done</returns>
//        public async Task BuildIndexes()
//        {
//            DbClient dbClient = DbClient.Open(Parent.DbContext, CollectionName);

//            if (PrimaryKey != null)
//            {
//                await dbClient.CreateIndex(PrimaryKey, true);
//            }

//            foreach (var key in Keys.Values)
//            {
//                await dbClient.CreateIndex(key.Name, unique: key.Unique, sparse: key.Sparse, compoundKeys: key.CompoundKeys, partialFilter: key.PartialFilter);
//            }
//        }

//        /// <summary>
//        /// Remove collection
//        /// </summary>
//        /// <param name="dbContext">database context</param>
//        /// <returns>Done</returns>
//        public async Task DropCollection()
//        {
//            await Parent.DbContext.RemoveCollectionAsync(CollectionName);
//        }

//        /// <summary>
//        /// Delete documents based on search
//        /// </summary>
//        /// <param name="search">search</param>
//        /// <returns>Guid list of primary keys</returns>
//        public async Task<IList<Guid>> Delete(BsonDocument search)
//        {
//            var projections = new BsonDocument
//            {
//                { PrimaryKey, "1" }
//            };

//            IList<BsonDocument> results = await Client.Search(search, projections);
//            List<Guid> deleteList = new List<Guid>(results.Select((doc) => Guid.Parse(((BsonElement)doc.TryGetElementAt(PrimaryKey)).Value.AsString)));

//            return await Delete(this, deleteList);
//        }

//        /// <summary>
//        /// Delete record from collection, and any child dependencies
//        /// </summary>
//        /// <param name="recordKey">record key</param>
//        /// <returns>List of keys that have been deleted</returns>
//        public async Task<IList<Guid>> Delete(Guid recordKey)
//        {
//            Verify.IsNotDefault(nameof(recordKey), recordKey);

//            if (PrimaryKey.IsEmpty())
//            {
//                return Enumerable.Empty<Guid>().ToList();
//            }

//            return await Delete(this, new Guid[] { recordKey });
//        }

//        /// <summary>
//        /// Delete record from collection, and any child dependencies
//        /// </summary>
//        /// <param name="parentCollection">parent collection</param>
//        /// <param name="recordKeys">keys to be deleted</param>
//        /// <returns>list of keys deleted</returns>
//        async Task<IList<Guid>> Delete(DbCollection parentCollection, IList<Guid> recordKeys)
//        {
//            Verify.IsNotNull(nameof(parentCollection), parentCollection);
//            Verify.IsNotNull(nameof(recordKeys), recordKeys);

//            List<Guid> deletedList = new List<Guid>();

//            foreach (var recordKey in recordKeys)
//            {
//                // Delete collection record
//                var search = new BsonDocument
//                {
//                    { PrimaryKey, recordKey.ToString() }
//                };

//                await parentCollection.Client.Collection.DeleteOneAsync(search);
//                deletedList.Add(recordKey);

//                // Delete children, if any
//                foreach (var child in parentCollection.Dependencies.Values)
//                {
//                    var collection = Parent.Collections[child.ChildCollectionName];

//                    // Get list of keys that will be deleted
//                    var childSearch = new BsonDocument
//                    {
//                        { child.OwnerKeyPath, recordKey.ToString() }
//                    };

//                    var projections = new BsonDocument
//                    {
//                        { collection.PrimaryKey, "1" }
//                    };

//                    IList<BsonDocument> documents = await collection.Client.Search(childSearch, projections);
//                    List<Guid> currentDeleteList = new List<Guid>(documents.Select((doc) => Guid.Parse(doc.GetElementAt(collection.PrimaryKey).Value.AsString)));
//                    deletedList.AddRange(currentDeleteList);

//                    // Delete child
//                    await collection.Client.Collection.DeleteManyAsync(childSearch);

//                    // Now, process dependencies
//                    var keyList = await Delete(collection, currentDeleteList);
//                    deletedList.AddRange(keyList);
//                }
//            }

//            return deletedList.Distinct().ToList();
//        }
//    }
//}
