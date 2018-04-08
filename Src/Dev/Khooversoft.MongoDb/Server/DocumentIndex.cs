using Khooversoft.MongoDb.Models.V1;
using Khooversoft.Toolbox;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    public class DocumentIndex<TDocument> : IDocumentIndex<TDocument>
    {
        private readonly Toolbox.Tag _tag = new Toolbox.Tag(nameof(DocumentIndex<TDocument>));

        internal DocumentIndex(DocumentCollection<TDocument> parent)
        {
            Verify.IsNotNull(nameof(parent), parent);

            Parent = parent;
        }

        public DocumentCollection<TDocument> Parent { get; }

        /// <summary>
        /// List indexes
        /// </summary>
        /// <param name="context">work context</param>
        /// <returns>list of index details</returns>
        public async Task<IEnumerable<IndexDetailV1>> ListIndexes(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);

            var list = new List<IndexDetailV1>();

            using (var cursor = await Parent.MongoCollection.Indexes.ListAsync())
            {
                List<BsonDocument> resultList = await cursor.ToListAsync();

                foreach (var item in resultList)
                {
                    var indexDetail = new IndexDetailV1
                    {
                        Name = item.Get<string>("name"),
                        Version = item.Get<int>("v"),
                        Unique = item.Get<bool>("unique", required: false),
                        Sparse = item.Get<bool>("sparse", required: false),
                        Namespace = item.Get<string>("ns"),
                    };

                    list.Add(indexDetail);

                    BsonDocument keyDocument = (BsonDocument)item["key"];
                    var keyList = new List<IndexKey>();
                    foreach (var kValue in keyDocument.Elements)
                    {
                        var indexKey = new IndexKey
                        {
                            FieldName = kValue.Name,
                            Descending = kValue.Value.AsInt32 == 1 ? false : true,
                        };

                        keyList.Add(indexKey);
                    }

                    indexDetail.Keys = keyList;
                }

                return list;
            }
        }

        /// <summary>
        /// Get index detail
        /// </summary>
        /// <param name="context"></param>
        /// <param name="indexName"></param>
        /// <returns></returns>
        public async Task<IndexDetailV1> GetIndexDetail(IWorkContext context, string indexName)
        {
            Verify.IsNotEmpty(nameof(indexName), indexName);

            return (await ListIndexes(context))
                .FirstOrDefault(x => x.Name == indexName);
        }

        /// <summary>
        /// Check to see if index exists
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="indexName">index name</param>
        /// <returns>true if index exist, false if not</returns>
        public async Task<bool> IndexExist(IWorkContext context, string indexName)
        {
            Verify.IsNotEmpty(nameof(indexName), indexName);

            return (await ListIndexes(context))
                .Any(x => x.Name == indexName);
        }

        /// <summary>
        /// Create unique index
        /// </summary>
        /// <param name="context">work context</param>
        /// <param name="collectionIndex">collection index information</param>
        /// <returns>Task</returns>
        public async Task CreateIndex(IWorkContext context, CollectionIndex collectionIndex)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(collectionIndex), collectionIndex);
            Verify.IsNotEmpty(nameof(collectionIndex.Name), collectionIndex.Name);
            Verify.Assert(collectionIndex.Sparse == false || (collectionIndex?.Keys?.Count() == 0), "sparse index does not support compound indexes");
            Verify.Assert(collectionIndex.Keys?.Count > 0, "requires key definitions");
            context = context.WithTag(_tag);

            var options = new CreateIndexOptions
            {
                Name = collectionIndex.Name,
                Version = 1,
                Unique = collectionIndex.Unique,
                Sparse = collectionIndex.Sparse,
            };

            IndexKeysDefinition<TDocument> keys = null;
            foreach (var key in collectionIndex.Keys)
            {
                if (key.Descending)
                {
                    keys = keys?.Descending(key.FieldName) ?? Builders<TDocument>.IndexKeys.Descending(key.FieldName);
                }
                else
                {
                    keys = keys?.Ascending(key.FieldName) ?? Builders<TDocument>.IndexKeys.Ascending(key.FieldName);
                }
            }

            MongoDbEventSource.Log.Info(context, $"Creating index={collectionIndex.Name}");
            await Parent.MongoCollection.Indexes.CreateOneAsync(keys, options, context.CancellationToken);
        }

        /// <summary>
        /// Drop index
        /// </summary>
        /// <param name="context"></param>
        /// <param name="collectionIndex"></param>
        /// <returns></returns>
        public async Task DropIndex(IWorkContext context, string indexName)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotEmpty(nameof(indexName), indexName);
            context = context.WithTag(_tag);

            MongoDbEventSource.Log.Info(context, $"Dropping index={indexName}");
            await Parent.MongoCollection.Indexes.DropOneAsync(indexName, context.CancellationToken);
        }
    }
}
