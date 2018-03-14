using FluentAssertions;
using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.Actor.Test.Actor
{
    [Trait("Category", "Actor")]
    public class ActorTests
    {
        private IWorkContext _context = WorkContext.Empty;

        [Fact]
        public async Task ActorSimpleTest()
        {
            int count = 0;

            IActorManager manager = new ActorManager();
            manager.Register<ICache>(_context, (c, k, m) => new StringCache(k, m, y => count += y));

            ActorKey key = new ActorKey("cache/test");
            ICache cache = await manager.CreateProxyAsync<ICache>(_context, key);

            count.Should().Be(1);
            await manager.DeactivateAsync<ICache>(_context, key);
            count.Should().Be(0);

            await manager.DeactivateAllAsync(_context);
            count.Should().Be(0);
        }

        [Fact]
        public async Task ActorDeactivateAllTest()
        {
            int count = 0;

            var manager = new ActorManager();
            manager.Register<ICache>(_context, (c, x, m) => new StringCache(x, m, y => count += y));

            ActorKey key = new ActorKey("cache/test");
            ICache cache = await manager.CreateProxyAsync<ICache>(_context, key);

            count.Should().Be(1);
            await manager.DeactivateAllAsync(_context);

            count.Should().Be(0);
        }

        [Fact]
        public async Task ActorMultipleTest()
        {
            int count = 0;

            ActorManager manager = new ActorManager();
            manager.Register<ICache>(_context, (c, x, m) => new StringCache(x, m, y => count += y));

            ActorKey key1 = new ActorKey("Cache/Test1");
            ICache cache1 = await manager.CreateProxyAsync<ICache>(_context, key1);
            count.Should().Be(1);

            ActorKey key2 = new ActorKey("Cache/Test2");
            ICache cache2 = await manager.CreateProxyAsync<ICache>(_context, key2);
            count.Should().Be(2);

            await manager.DeactivateAsync<ICache>(_context, key1);
            count.Should().Be(1);

            await manager.DeactivateAsync<ICache>(_context, key2);
            count.Should().Be(0);

            await manager.DeactivateAllAsync(_context);
        }

        [Fact]
        public async Task ActorMethodTest()
        {
            int count = 0;

            ActorManager manager = new ActorManager();
            manager.Register<ICache>(_context, (c, x, m) => new StringCache(x, m, y => count += y));

            ActorKey key1 = new ActorKey("Cache/Test1");
            ICache cache1 = await manager.CreateProxyAsync<ICache>(_context, key1);
            count.Should().Be(1);

            const string firstText = "first";

            bool test = await cache1.IsCached(firstText);
            test.Should().BeFalse();
            await cache1.Add(firstText);
            test = await cache1.IsCached(firstText);
            test.Should().BeTrue();

            await manager.DeactivateAsync<ICache>(_context, key1);
            count.Should().Be(0);

            await manager.DeactivateAllAsync(_context);
        }

        [Fact]
        public async Task ActorSameInstanceTest()
        {
            int count = 0;

            var manager = new ActorManager();

            manager.Register<ICache>(_context, (c, x, m) => new StringCache(x, manager, y => count += y));

            ActorKey key1 = new ActorKey("Cache/Test1");
            ICache cache1 = await manager.CreateProxyAsync<ICache>(_context, key1);
            count.Should().Be(1);

            ActorKey key2 = new ActorKey("Cache/Test2");
            ICache cache2 = await manager.CreateProxyAsync<ICache>(_context, key2);
            count.Should().Be(2);

            const string firstText = "first";
            const string secondText = "secondFirst";

            await cache1.Add(firstText);
            bool test = await cache1.IsCached(firstText);
            test.Should().BeTrue();

            await cache2.Add(secondText);
            bool test2 = await cache2.IsCached(secondText);
            test2.Should().BeTrue();

            ICache cache1Dup = await manager.CreateProxyAsync<ICache>(_context, key1);
            test = await cache1Dup.IsCached(firstText);
            test.Should().BeTrue();
            test = await cache1Dup.IsCached(secondText);
            test.Should().BeFalse();

            await manager.DeactivateAsync<ICache>(_context, key1);
            await manager.DeactivateAsync<ICache>(_context, key2);
            count.Should().Be(0);

            await manager.DeactivateAllAsync(_context);
        }

        private interface ICache : IActor
        {
            Task<bool> IsCached(string key);

            Task Add(string key);
        }

        private class StringCache : ActorBase, ICache
        {
            private HashSet<string> _values = new HashSet<string>();

            public StringCache(ActorKey actorKey, IActorManager actorManager, Action<int> increment)
                : base(actorKey, actorManager)
            {
                Increment = increment;
            }

            private Action<int> Increment { get; }

            protected override Task OnActivate(IWorkContext context)
            {
                Increment(1);
                return base.OnActivate(context);
            }

            protected override Task OnDeactivate(IWorkContext context)
            {
                Increment(-1);
                return base.OnDeactivate(context);
            }

            public Task<bool> IsCached(string key)
            {
                return Task.FromResult(_values.Contains(key));
            }

            public Task Add(string key)
            {
                _values.Add(key);
                return Task.FromResult(0);
            }
        }
    }
}