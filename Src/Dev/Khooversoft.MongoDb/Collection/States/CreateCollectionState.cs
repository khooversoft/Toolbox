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
        private readonly Tag _tag = new Tag(nameof(CreateCollectionState));

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

            await Parent.Database.CreateCollection(context, Parent.Model.CollectionName);
            return true;
        }

        public async Task<bool> Test(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithTag(_tag);

            bool status = await Parent.Database.CollectionExist(context, Parent.Model.CollectionName);
            MongoDbEventSource.Log.Info(context, $"Collection {Parent.Model.CollectionName}, exists={status}");
            return status;
        }
    }
}
