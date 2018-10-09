using Autofac;
using FluentAssertions;
using Khooversoft.Toolbox;
using Khooversoft.Toolbox.Actor;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.Toolbox.Test.Actor
{
    [Trait("Category", "Actor")]
    public class ActorChainCallTests
    {
        private const string sumActorName = "actorSum";
        private IWorkContext _context = WorkContext.Empty;

        [Fact]
        public async Task ActorSingleChainTest()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ActorNode>().As<IActorNode>();
            builder.RegisterType<ActorSum>().As<IActorSum>();
            ILifetimeScope container = builder.Build();

            IActorManager manager = new ActorConfigurationBuilder()
                .Register<IActorNode>((c, k, m) => container.Resolve<IActorNode>(new TypedParameter(typeof(ActorKey), k), new TypedParameter(typeof(IActorManager), m)))
                .Register<IActorSum>((c, k, m) => container.Resolve<IActorSum>(new TypedParameter(typeof(ActorKey), k), new TypedParameter(typeof(IActorManager), m)))
                .Build()
                .ToActorManager();

            using (container)
            {
                ActorKey key = new ActorKey("node/test");
                IActorNode node = await manager.CreateProxyAsync<IActorNode>(_context, key);

                int sum = 0;
                for (int i = 0; i < 10; i++)
                {
                    await node.Add(_context, i);
                    sum += i;
                }

                IActorSum sumActor = await manager.CreateProxyAsync<IActorSum>(_context, new ActorKey(sumActorName));
                (await sumActor.GetSum()).Should().Be(sum);
            }

            await manager.DeactivateAllAsync(_context);
        }

        private interface IActorNode : IActor
        {
            Task Add(IWorkContext context, int value);
        }

        private interface IActorSum : IActor
        {
            Task Add(int value);

            Task<int> GetSum();
        }

        public class ActorNode : ActorBase, IActorNode
        {
            private IActorSum _actorSum;

            public ActorNode(ActorKey actorKey, IActorManager actorManager)
                : base(actorKey, actorManager)
            {
            }

            public async Task Add(IWorkContext context, int value)
            {
                _actorSum = _actorSum ?? (await ActorManager.CreateProxyAsync<IActorSum>(context, new ActorKey(sumActorName)));
                await _actorSum.Add(value);
            }
        }

        public class ActorSum : ActorBase, IActorSum
        {
            private int _sum;

            public ActorSum(ActorKey actorKey, IActorManager actorManager)
                : base(actorKey, actorManager)
            {
            }

            public Task Add(int value)
            {
                _sum += value;
                return Task.FromResult(0);
            }

            public Task<int> GetSum()
            {
                return Task.FromResult(_sum);
            }
        }
    }
}
