using Autofac;
using FluentAssertions;
using Khooversoft.Actor;
using Khooversoft.Services;
using Khooversoft.Toolbox;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Khoover.Services.Test.Identity
{
    [Trait("Category", "Identity Actor")]
    public class IdentityActorTests
    {
        private static IWorkContext _workContext = WorkContext.Empty;

        [Fact]
        public async Task CertificateSetGetTest()
        {
            IWorkContext context = _workContext.WithMethodName();

            var identityRepository = new IdentityInMemoryStore()
                .Set(_workContext, new IdentityPrincipal(new PrincipalId("client1@domain.com"), IdentityPrincipalType.User).With(new ApiKey("API Key1", DateTime.UtcNow.AddYears(1))))
                .Set(_workContext, new IdentityPrincipal(new PrincipalId("client2@domain.com"), IdentityPrincipalType.User).With(new ApiKey("API Key2", DateTime.UtcNow.AddYears(1))));

            var builder = new ContainerBuilder();
            builder.RegisterModule(new IdentityActorAutoFacModule());
            builder.RegisterInstance(identityRepository).As<IIdentityStore>();
            ILifetimeScope container = builder.Build();

            IActorManager manager = new ActorManagerBuilder()
                .AddIdentityModule(container)
                .Build();

            using (ILifetimeScope scopeContainer = container.BeginLifetimeScope())
            using (manager)
            {
                IIdentityActor clientActor1 = await manager.CreateProxyAsync<IIdentityActor>(context, new ActorKey("client1@domain.com"));
                IdentityPrincipal client1 = await clientActor1.Get(context);
                client1.Should().NotBeNull();
                client1.PrincipalId.Value.Should().Be("client1@domain.com");
                client1.ApiKey.Value.Should().Be("API Key1");

                IIdentityActor clientActor2 = await manager.CreateProxyAsync<IIdentityActor>(context, new ActorKey("client2@domain.com"));
                IdentityPrincipal client2 = await clientActor2.Get(context);
                client2.Should().NotBeNull();
                client2.PrincipalId.Value.Should().Be("client2@domain.com");
                client2.ApiKey.Value.Should().Be("API Key2");

                await clientActor2.Remove(context);
                (await clientActor2.Get(context)).Should().BeNull();

                identityRepository.Get(context, new PrincipalId("client2@domain.com")).Should().BeNull();

                await clientActor2.Set(context, new IdentityPrincipal(new PrincipalId("client2@domain.com"), IdentityPrincipalType.User));
                (await clientActor2.Get(context)).Should().NotBeNull();
                identityRepository.Get(context, new PrincipalId("client2@domain.com")).Should().NotBeNull();
            }

            await Verify.AssertExceptionAsync<ArgumentException>(async () => await manager.DeactivateAllAsync(_workContext));
        }
    }
}
