using Khooversoft.MongoDb.Models.V1;
using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb.Collection.States
{
    internal class CreateCollectionState : IStateItem
    {
        private static readonly Tag _tag = new Tag(nameof(CreateCollectionState));

        public CreateCollectionState(CollectionModelPackage parent)
        {
            Parent = parent;
        }

        public CollectionModelPackage Parent { get; }

        public string Name { get; } = "CreateDatabaseState";

        public bool IgnoreError => false;

        public async Task<bool> Set(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithTag(_tag);

            if ((await Parent.Database.CollectionExist(context, Parent.Model.CollectionName)))
            {
                if (!Parent.Settings.AllowDataLoss)
                {
                    throw new InvalidOperationException($"Collection {Parent.Model.CollectionName} exist but allow data loss is not set.  Cannot re-create");
                }

                await Parent.Database.DropCollection(context, Parent.Model.CollectionName);
            }

            CappedCollectionModel cappedModel = Parent.Model as CappedCollectionModel;
            if (cappedModel != null)
            {
                await Parent.Database.CreateCappedCollection(context, cappedModel.CollectionName, cappedModel.MaxNumberOfDocuments, cappedModel.MaxSizeInBytes);
                return true;
            }

            await Parent.Database.CreateCollection(context, Parent.Model.CollectionName);
            return true;
        }

        public async Task<bool> Test(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithTag(_tag);

            CollectionDetailV1 detail = await Parent.Database.GetCollectionDetail(context, Parent.Model.CollectionName);
            if (detail == null)
            {
                MongoDbEventSource.Log.Info(context, $"Collection {Parent.Model.CollectionName}, does not exists");
                return false;
            }

            CappedCollectionModel cappedModel = Parent.Model as CappedCollectionModel;
            if (cappedModel != null)
            {
                bool status = detail.Capped == true &&
                    cappedModel.MaxNumberOfDocuments == (int)detail.MaxDocuments &&
                    cappedModel.MaxSizeInBytes == (long)detail.MaxSizeInBytes;

                MongoDbEventSource.Log.Info(context, $"Collection {Parent.Model.CollectionName} is capped, isEqual={status}");
            }

            MongoDbEventSource.Log.Info(context, $"Collection {Parent.Model.CollectionName}, exists");
            return true;
        }
    }
}
