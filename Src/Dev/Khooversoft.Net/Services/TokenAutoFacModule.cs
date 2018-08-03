// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Autofac;
using Khooversoft.Toolbox;
using Khooversoft.Toolbox.Services;

namespace Khooversoft.Net
{
    /// <summary>
    /// Construct AutoFac configuration for Server and/or Client token managers
    /// </summary>
    public class TokenAutoFacModule : Module
    {
        private readonly bool _client;
        private readonly bool _server;
        private readonly IRestClientConfiguration _restClientConfiguration;

        public TokenAutoFacModule()
        {
            _server = true;
        }

        public TokenAutoFacModule(bool client, bool server, IRestClientConfiguration restClientConfiguration)
        {
            Verify.IsNotNull(nameof(restClientConfiguration), restClientConfiguration);

            _client = client;
            _server = server;
            _restClientConfiguration = restClientConfiguration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            if (_client)
            {
                builder.RegisterType<ClientTokenManager>()
                    .As<IClientTokenManager>()
                    .WithParameter(new TypedParameter(typeof(IRestClientConfiguration), _restClientConfiguration));

                builder.RegisterType<TokenClientRepository>()
                    .As<ITokenClientRepository>();

                if (_restClientConfiguration != null)
                {
                    builder.RegisterType<TokenClientActor>()
                        .As<ITokenClientActor>()
                        .WithParameter(new TypedParameter(typeof(IRestClientConfiguration), _restClientConfiguration));
                }
                else
                {
                    builder.RegisterType<TokenClientActor>().As<ITokenClientActor>();
                }
            }

            if (_server)
            {
                builder.RegisterType<ServerTokenManager>().As<IServerTokenManager>();
            }
        }
    }

    public static class TokenManagerAutoFacModuleExtension
    {
        public static ContainerBuilder AddTokenServerModule(this ContainerBuilder self)
        {
            Verify.IsNotNull(nameof(self), self);

            self.RegisterModule(new TokenAutoFacModule());
            return self;
        }

        public static ContainerBuilder AddTokenModule(this ContainerBuilder self, bool client, bool server, IRestClientConfiguration restClientConfiguration)
        {
            Verify.IsNotNull(nameof(self), self);

            self.RegisterModule(new TokenAutoFacModule(client, server, restClientConfiguration));
            return self;
        }
    }
}
