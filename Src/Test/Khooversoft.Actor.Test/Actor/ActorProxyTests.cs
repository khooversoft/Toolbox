using FluentAssertions;
using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.Actor.Test.Actor
{
    [Trait("Category", "Actor")]
    public class ActorProxyTests
    {
        private IWorkContext _context = WorkContext.Empty;

        [Fact]
        public async Task ActorProxyMultiTaskTest()
        {
            const int taskCount = 10;

            ActorManager manager = new ActorManager();
            manager.Register<ICache, StringCache>(_context);

            var tasks = new List<Task>();
            ActorKey key1 = new ActorKey("Cache/Test1");
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            for (int i = 0; i < taskCount; i++)
            {
                Task t = Task.Run(() => TestAccess(manager, key1, tokenSource.Token));
                tasks.Add(t);
            }

            await Task.Delay(TimeSpan.FromSeconds(10));
            tokenSource.Cancel();
            Task.WaitAll(tasks.ToArray());

            (await manager.DeactivateAsync<ICache>(_context, key1)).Should().BeTrue();
        }

        private async Task TestAccess(IActorManager manager, ActorKey actorKey, CancellationToken token)
        {
            const string firstText = "first";

            ICache cache1 = await manager.CreateProxyAsync<ICache>(_context, actorKey);
            while (!token.IsCancellationRequested)
            {
                bool test = await cache1.IsCached(firstText);
                await cache1.Add(firstText);
            }
        }

        public interface ICache : IActor
        {
            Task<bool> IsCached(string key);

            Task Add(string key);
        }

        public class StringCache : ActorBase, ICache
        {
            private HashSet<string> _values = new HashSet<string>();
            private int _lockValue = 0;

            public StringCache(ActorKey actorKey, IActorManager actorManager)
                : base(actorKey, actorManager)
            {
            }

            public Task<bool> IsCached(string key)
            {
                int l = Interlocked.CompareExchange(ref _lockValue, 1, 0);
                if (l == 1)
                {
                    throw new InvalidOperationException("Locked accessed violation");
                }

                Interlocked.CompareExchange(ref _lockValue, 0, 1);
                return Task.FromResult(_values.Contains(key));
            }

            public Task Add(string key)
            {
                int l = Interlocked.CompareExchange(ref _lockValue, 1, 0);
                if (l == 1)
                {
                    throw new InvalidOperationException("Locked accessed violation");
                }

                Interlocked.CompareExchange(ref _lockValue, 0, 1);
                _values.Add(key);

                return Task.FromResult(0);
            }
        }
    }
}