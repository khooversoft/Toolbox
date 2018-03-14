using Autofac;
using FluentAssertions;
using Khooversoft.Actor;
using Khooversoft.Net;
using Khooversoft.Security;
using Khooversoft.Services;
using Khooversoft.Toolbox;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Khoover.Services.Test.Manager
{
    [Trait("Category", "Manager")]
    public class TokenManagerTests : IDisposable
    {
        private readonly IWorkContext _workContext;
        private readonly Tag _tag = new Tag(nameof(TokenManagerTests));
        private readonly ILifetimeScope _container;

        public TokenManagerTests()
        {
            var builder = new ContainerBuilder();
            builder.AddTokenModule(true, true, RestClientConfiguration.Default);
            builder.AddCertificateModule();
            builder.AddIdentityModule();
            builder.RegisterInstance(new TestTokenConfiguration()).As<IServerTokenManagerConfiguration>().As<IClientTokenManagerConfiguration>();
            builder.Register(ctx => new ActorManagerBuilder().Set(_container).Build()).SingleInstance();
            _container = builder.Build();

            _workContext = new WorkContextBuilder()
                .SetContainer(_container)
                .Build();
        }

        public void Dispose()
        {
            _container?.Dispose();
        }

        [Fact]
        public async Task CreateTokenRequestTest()
        {
            var context = _workContext.WithTag(_tag);

            IClientTokenManagerConfiguration clientConfiguation = context.Container.Resolve<IClientTokenManagerConfiguration>();
            clientConfiguation.Should().NotBeNull();

            IIdentityRepository identityRepository = context.Container.Resolve<IIdentityRepository>();
            identityRepository.Should().NotBeNull();
            await identityRepository.SetAsync(context, new IdentityPrincipal(new PrincipalId(clientConfiguation.TokenKey.RequestingSubject), IdentityPrincipalType.User));

            IServerTokenManager tokenManager = context.Container.Resolve<IServerTokenManager>();
            tokenManager.Should().NotBeNull();

            // Create token request (client side)
            IClientTokenManager clientManager = context.Container.Resolve<IClientTokenManager>();
            clientManager.Should().NotBeNull();

            string requestToken = await clientManager.CreateRequestToken(context, clientConfiguation.TokenKey.RequestingSubject);
            requestToken.Should().NotBeNullOrWhiteSpace();

            // Validate request token and create authorization token
            string authorizationToken = await tokenManager.CreateAutorizationToken(context, requestToken);
            authorizationToken.Should().NotBeNullOrWhiteSpace();

            // Parse and lookup user
            JwtTokenDetails JwtTokenDetails = await clientManager.ParseAuthorizationToken(context, authorizationToken);
            JwtTokenDetails.Should().NotBeNull();
            JwtTokenDetails.ApiKey.Should().NotBeNullOrWhiteSpace();
        }
    }
}
