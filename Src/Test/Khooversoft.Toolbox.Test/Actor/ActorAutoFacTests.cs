﻿using Autofac;
using FluentAssertions;
using Khooversoft.Toolbox;
using Khooversoft.Toolbox.Actor;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.Toolbox.Test.Actor
{
    [Trait("Category", "Actor")]
    public class ActorAutoFacTests
    {
        private IWorkContext _context = WorkContext.Empty;

        [Fact]
        public async Task ActorAutoFacActionSimpleTest()
        {
            IActorManager manager = new ActorManager();

            var builder = new ContainerBuilder();
            builder.RegisterType<StringCache>().As<ICache>();
            builder.RegisterInstance(manager).As<IActorManager>();
            IContainer container = builder.Build();

            using (container.BeginLifetimeScope())
            {
                manager.Register<ICache>(_context,
                    (c, k, m) =>
                    {
                        return container.Resolve<ICache>(new TypedParameter(typeof(ActorKey), k));
                    });

                ActorKey key = new ActorKey("cache/test");
                ICache cache = await manager.CreateProxyAsync<ICache>(_context, key);

                (await cache.GetCount()).Should().Be(1);
                await manager.DeactivateAsync<ICache>(_context, key);
                (await cache.GetCount()).Should().Be(2);
            }

            await manager.DeactivateAllAsync(_context);
        }

        [Fact]
        public async Task ActorAutoFacSimpleTest()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<StringCache>().As<ICache>();
            ILifetimeScope container = builder.Build();

            IActorManager manager = new ActorManagerBuilder()
                .Register<ICache>((c, k, m) => container.Resolve<ICache>(new TypedParameter(typeof(ActorKey), k), new TypedParameter(typeof(IActorManager), m)))
                .Build();

            using (container.BeginLifetimeScope())
            {
                ActorKey key = new ActorKey("cache/test");
                ICache cache = await manager.CreateProxyAsync<ICache>(_context, key);

                (await cache.GetCount()).Should().Be(1);
                await manager.DeactivateAsync<ICache>(_context, key);
                (await cache.GetCount()).Should().Be(2);
            }

            await manager.DeactivateAllAsync(_context);
        }

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
                _count++;
                return base.OnDeactivate(context);
            }

            public Task<int> GetCount()
            {
                return Task.FromResult(_count);
            }
        }
    }
}
