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
    internal class CreateIndexState : IStateItem
    {
        private static readonly Tag _tag = new Tag(nameof(CreateIndexState));

        public CreateIndexState(CollectionModelPackage parent, CollectionIndex index)
        {
            Verify.IsNotNull(nameof(parent), parent);
            Verify.IsNotNull(nameof(index), index);
            Verify.IsNotEmpty(nameof(index.Name), index.Name);

            Parent = parent;
            Index = index;
        }

        public CollectionModelPackage Parent { get; }

        public CollectionIndex Index { get; }

        public string Name => "CreateIndexState";

        public bool IgnoreError => false;

        public async Task<bool> Set(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithTag(_tag);

            IDocumentCollection<BsonDocument> collection = Parent.Database.GetCollection<BsonDocument>(Parent.Model.CollectionName);

            if ((await collection.Index.IndexExist(context, Index.Name)))
            {
                await collection.Index.DropIndex(context, Index.Name);
            }

            await collection.Index.CreateIndex(context, Index);
            return true;
        }

        public async Task<bool> Test(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithTag(_tag);

            IDocumentCollection<BsonDocument> collection = Parent.Database.GetCollection<BsonDocument>(Parent.Model.CollectionName);

            IndexDetailV1 detail = await collection.Index.GetIndexDetail(context, Index.Name);
            if (detail == null)
            {
                context.EventLog.Info(context, $"Index {Index.Name} for collection {Parent.Model.CollectionName} does not exist");
                return false;
            }

            bool status = Index.IsEquals(detail);
            context.EventLog.Info(context, $"Index {Index.Name} for collection {Parent.Model.CollectionName}, Equals={status}");
            return status;
        }
    }
}
