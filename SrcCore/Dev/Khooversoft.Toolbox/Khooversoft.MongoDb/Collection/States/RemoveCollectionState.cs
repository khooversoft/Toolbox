using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb.Collection.States
{
    internal class RemoveCollectionState : IStateItem
    {
        private static readonly Tag _tag = new Tag(nameof(RemoveCollectionState));

        public RemoveCollectionState(CollectionModelPackage parent)
        {
            Parent = parent;
        }

        public CollectionModelPackage Parent { get; }

        public string Name { get; } = "RemoveDatabaseState";

        public bool IgnoreError => false;

        public async Task<bool> Set(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithTag(_tag);

            await Parent.Database.DropCollection(context, Parent.Model.CollectionName);
            return true;
        }

        public async Task<bool> Test(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithTag(_tag);

            bool status = !(await Parent.Database.CollectionExist(context, Parent.Model.CollectionName));
            context.EventLog.Info(context, $"Collection {Parent.Model.CollectionName}, exists={status}");
            return status;
        }
    }
}
