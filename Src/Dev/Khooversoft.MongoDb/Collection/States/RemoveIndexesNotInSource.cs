using Khooversoft.MongoDb.Models.V1;
using Khooversoft.Toolbox;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb.Collection.States
{
    public class RemoveIndexesNotInSource : IStateItem
    {
        private readonly Tag _tag = new Tag(nameof(CreateIndexState));

        public RemoveIndexesNotInSource(CollectionModelPackage parent)
        {
            Verify.IsNotNull(nameof(parent), parent);

            Parent = parent;
        }

        public CollectionModelPackage Parent { get; }

        public string Name => "RemoveIndexesNotInSource";

        public bool IgnoreError => false;

        public async Task<bool> Set(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithTag(_tag);

            IEnumerable<string> exceptList = await GetIndexesNotInSource(context);
            if (exceptList == null || exceptList.Count() == 0)
            {
                MongoDbEventSource.Log.Info(context, $"No indexes for collection {Parent.Model.CollectionName}");
                return true;
            }

            IDocumentCollection<BsonDocument> collection = Parent.Database.GetCollection<BsonDocument>(Parent.Model.CollectionName);
            foreach (var name in exceptList)
            {
                await collection.Index.DropIndex(context, name);
                MongoDbEventSource.Log.Info(context, $"Index {name} was dropped in collection {Parent.Model.CollectionName}");
            }

            return true;
        }

        public async Task<bool> Test(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithTag(_tag);

            IEnumerable<string> exceptList = await GetIndexesNotInSource(context);
            if (exceptList == null || exceptList.Count() == 0)
            {
                MongoDbEventSource.Log.Info(context, $"No indexes for collection {Parent.Model.CollectionName}");
                return true;
            }

            MongoDbEventSource.Log.Info(context, $"Indexes {string.Join(", ", exceptList)} are not in source for collection {Parent.Model.CollectionName}");
            return false;
        }

        private async Task<IEnumerable<string>> GetIndexesNotInSource(IWorkContext context)
        {
            IDocumentCollection<BsonDocument> collection = Parent.Database.GetCollection<BsonDocument>(Parent.Model.CollectionName);

            IEnumerable<IndexDetailV1> indexList = await collection.Index.ListIndexes(context);
            if (indexList == null && indexList.Count() == 0)
            {
                return null;
            }

            var excludeList = new HashSet<string>
            {
                "_id",
                "_id_",
            };

            return indexList
                .Select(x => x.Name)
                .Where(x => !excludeList.Contains(x))
                .Except(Parent.Model.Indexes.Select(x => x.Name)).ToList();
        }
    }
}
