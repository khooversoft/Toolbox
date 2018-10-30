using Khooversoft.MongoDb.Collection.States;
using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    public class CollectionModelPackage
    {
        private readonly IStateManager _stateManager;
        private readonly Tag _tag = new Tag(nameof(CollectionModelPackage));

        public CollectionModelPackage(IDocumentDatabase database, CollectionModel model, CollectionModelSettings settings)
        {
            Verify.IsNotNull(nameof(database), database);
            Verify.IsNotNull(nameof(model), model);
            Verify.IsNotNull(nameof(settings), settings);
            Verify.IsValid(nameof(model), model);

            Database = database;
            Model = model;
            Settings = settings;

            StateManagerBuilder builder = new StateManagerBuilder();

            if (Settings.Remove || Settings.ReCreate)
            {
                builder.Add(new RemoveCollectionState(this));
            }

            if (!Settings.Remove)
            {
                builder.Add(new CreateCollectionState(this));
                builder.Add(new RemoveIndexesNotInSource(this));

                foreach (var item in Model.Indexes ?? Enumerable.Empty<CollectionIndex>())
                {
                    builder.Add(new CreateIndexState(this, item));
                }
            }

            _stateManager = builder.Build();
        }

        public IDocumentDatabase Database { get; }

        public CollectionModel Model { get; }

        public CollectionModelSettings Settings { get; }

        public async Task<bool> Apply(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithTag(_tag);

            StateContext stateContext = await _stateManager.RunAsync(context);
            return stateContext.IsSuccessFul;
        }

        public async Task<bool> Test(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithTag(_tag);

            StateContext stateContext = await _stateManager.TestAsync(context);
            return stateContext.IsSuccessFul;
        }
    }
}
