using Autofac;
using FluentAssertions;
using Khooversoft.Toolbox;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.Actor.Test.Actor
{
    [Trait("Category", "Actor")]
    public class ActorRetirementTests
    {
        private IWorkContext _context = WorkContext.Empty;

        // Disabled test because of changes in actor collection
        //[Fact]
        //public async Task ActorRetirementSimpleTest()
        //{
        //    var builder = new ContainerBuilder();
        //    builder.RegisterType<StringCache>().As<ICache>();
        //    ILifetimeScope container = builder.Build();

        //    IActorManager manager = new ActorManagerBuilder()
        //        .Set(container)
        //        .SetActorRetirementPeriod(TimeSpan.FromSeconds(2))
        //        .SetInactivityScanPeriod(TimeSpan.FromSeconds(1))
        //        .Build();

        //    using (container)
        //    {
        //        ActorKey key = new ActorKey("retirement/test");
        //        ICache cache = await manager.CreateProxyAsync<ICache>(_context, key);

        //        (await cache.GetCount()).Should().Be(1);
        //        await Task.Delay(TimeSpan.FromSeconds(10));

        //        (await cache.GetCount()).Should().Be(0);
        //    }

        //    await manager.DeactivateAllAsync(_context);
        //}

        private interface ICache : IActor
        {
            Task<int> GetCount();
        }

        private class StringCache : ActorBase, ICache
        {
            private int _count = 0;

            public StringCache(ActorKey actorKey, IActorManager actorManager)
                : base(actorKey, actorManager)
            {
            }

            protected override Task OnActivate(IWorkContext context)
            {
                _count++;
                return base.OnActivate(context);
            }

            protected override Task OnDeactivate(IWorkContext context)
            {
                _count--;
                return base.OnDeactivate(context);
            }

            public Task<int> GetCount()
            {
                return Task.FromResult(_count);
            }
        }
    }
}
