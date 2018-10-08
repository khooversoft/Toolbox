using Autofac;
using FluentAssertions;
using Khooversoft.Toolbox.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.Toolbox.Test.Actor
{
    [Trait("Category", "Actor")]
    public class ActorTimerTests
    {
        private IWorkContext _context = WorkContext.Empty;

        [Fact]
        public async Task ActorSimpleTimerTest()
        {
            IActorManager manager = new ActorManager();
            manager.Register<ITimerActor>(_context, (context, k, m) => new TimeActor(k, m));

            ActorKey key = new ActorKey("timer/test");
            ITimerActor timerActor = await manager.CreateProxyAsync<ITimerActor>(_context, key);

            foreach (var index in Enumerable.Range(0, 20))
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                int count = await timerActor.GetCount();
                if (count > 2) break;
            }

            (await timerActor.GetCount()).Should().BeGreaterThan(2);
            await manager.DeactivateAsync<ITimerActor>(_context, key);
        }

        //[Fact]
        //public async Task ActorTimerAutoFacTest()
        //{
        //    var builder = new ContainerBuilder();
        //    builder.RegisterType<TimeActor>().As<ITimerActor>();
        //    ILifetimeScope container = builder.Build();

        //    IActorManager manager = new ActorManagerBuilder()
        //        .Set(container)
        //        .Build();

        //    using (container)
        //    {
        //        ActorKey key = new ActorKey("timer/test");
        //        ITimerActor timerActor = await manager.CreateProxyAsync<ITimerActor>(_context, key);

        //        foreach (var index in Enumerable.Range(0, 20))
        //        {
        //            await Task.Delay(TimeSpan.FromSeconds(1));
        //            int count = await timerActor.GetCount();
        //            if (count > 2) break;
        //        }

        //        (await timerActor.GetCount()).Should().BeGreaterThan(2);
        //    }

        //    await manager.DeactivateAllAsync(_context);
        //}

        private interface ITimerActor : IActor
        {
            Task<int> GetCount();
        }

        private class TimeActor : ActorBase, ITimerActor
        {
            private HashSet<string> _values = new HashSet<string>();
            private int _count = 0;

            public TimeActor(ActorKey actorKey, IActorManager actorManager)
                : base(actorKey, actorManager)
            {
            }

            protected override Task OnActivate(IWorkContext context)
            {
                SetTimer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

                return base.OnActivate(context);
            }

            protected override Task OnTimer()
            {
                _count++;
                return Task.FromResult(0);
            }

            public Task<int> GetCount()
            {
                return Task.FromResult(_count);
            }
        }
    }
}