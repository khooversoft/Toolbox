//using FluentAssertions;
//using Khooversoft.Toolbox;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Xunit;

//namespace Khooversoft.Actor.Test.Actor
//{
//    [Trait("Category", "Actor")]
//    public class ActorActivatorTests
//    {
//        private IWorkContext _context = WorkContext.Empty;

//        [Fact]
//        public async Task ActorActivatorSimpleTest()
//        {
//            IActorManager manager = new ActorManager();
//            manager.Register<ICache, StringCache>(_context);

//            ActorKey key = new ActorKey("cache/test");
//            ICache cache = await manager.CreateProxyAsync<ICache>(_context, key);

//            await manager.DeactivateAsync<ICache>(_context, key);
//        }

//        [Fact]
//        public async Task ActorActivatorMultipleTest()
//        {
//            IActorManager manager = new ActorManager();
//            manager.Register<ICache, StringCache>(_context);

//            ActorKey key1 = new ActorKey("Cache/Test1");
//            ICache cache1 = await manager.CreateProxyAsync<ICache>(_context, key1);

//            ActorKey key2 = new ActorKey("Cache/Test2");
//            ICache cache2 = await manager.CreateProxyAsync<ICache>(_context, key2);

//            (await manager.DeactivateAsync<ICache>(_context, key1)).Should().BeTrue();
//            (await manager.DeactivateAsync<ICache>(_context, key2)).Should().BeTrue();
//        }

//        [Fact]
//        public async Task ActorActivatorMethodTest()
//        {
//            IActorManager manager = new ActorManager();
//            manager.Register<ICache, StringCache>(_context);

//            ActorKey key1 = new ActorKey("Cache/Test1");
//            ICache cache1 = await manager.CreateProxyAsync<ICache>(_context, key1);

//            const string firstText = "first";

//            bool test = await cache1.IsCached(firstText);
//            test.Should().BeFalse();
//            await cache1.Add(firstText);
//            test = await cache1.IsCached(firstText);
//            test.Should().BeTrue();

//            (await manager.DeactivateAsync<ICache>(_context, key1)).Should().BeTrue();
//        }

//        [Fact]
//        public async Task ActorActivatorSameInstanceTest()
//        {
//            IActorManager manager = new ActorManager();
//            manager.Register<ICache, StringCache>(_context);

//            ActorKey key1 = new ActorKey("Cache/Test1");
//            ICache cache1 = await manager.CreateProxyAsync<ICache>(_context, key1);

//            ActorKey key2 = new ActorKey("Cache/Test2");
//            ICache cache2 = await manager.CreateProxyAsync<ICache>(_context, key2);

//            const string firstText = "first";
//            const string secondText = "secondFirst";

//            await cache1.Add(firstText);
//            bool test = await cache1.IsCached(firstText);
//            test.Should().BeTrue();

//            await cache2.Add(secondText);
//            bool test2 = await cache2.IsCached(secondText);
//            test2.Should().BeTrue();

//            ICache cache1Dup = await manager.CreateProxyAsync<ICache>(_context, key1);
//            test = await cache1Dup.IsCached(firstText);
//            test.Should().BeTrue();
//            test = await cache1Dup.IsCached(secondText);
//            test.Should().BeFalse();

//            (await manager.DeactivateAsync<ICache>(_context, key1)).Should().BeTrue();
//            (await manager.DeactivateAsync<ICache>(_context, key2)).Should().BeTrue();
//        }

//        private interface ICache : IActor
//        {
//            Task<bool> IsCached(string key);

//            Task Add(string key);
//        }

//        private class StringCache : ActorBase, ICache
//        {
//            private HashSet<string> _values = new HashSet<string>();

//            public StringCache(ActorKey actorKey, IActorManager actorManager)
//                : base(actorKey, actorManager)
//            {
//            }

//            protected override Task OnActivate(IWorkContext context)
//            {
//                return base.OnActivate(context);
//            }

//            protected override Task OnDeactivate(IWorkContext context)
//            {
//                return base.OnDeactivate(context);
//            }

//            public Task<bool> IsCached(string key)
//            {
//                return Task.FromResult(_values.Contains(key));
//            }

//            public Task Add(string key)
//            {
//                _values.Add(key);
//                return Task.FromResult(0);
//            }
//        }
//    }
//}
