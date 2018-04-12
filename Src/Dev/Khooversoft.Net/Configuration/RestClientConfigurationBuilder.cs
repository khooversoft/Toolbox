// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Khooversoft.Net
{
    /// <summary>
    /// REST client configuration builder.  Provides the standard handlers and the ability
    /// to add others.
    /// 
    /// Also supports adding properties that are used on all REST calls.
    /// </summary>
    public class RestClientConfigurationBuilder
    {
        public RestClientConfigurationBuilder()
        {
            Handlers = new List<Func<DelegatingHandler>>();
            Properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public IList<Func<DelegatingHandler>> Handlers { get; } = new List<Func<DelegatingHandler>>
        {
            () => new RequestHandler(),
            () => new LoggingHandler()
        };

        public HttpMessageHandler HttpMessageHandler { get; set; }

        public Dictionary<string, object> Properties { get; }

        public RestClientConfigurationBuilder SetMessageHandler(HttpMessageHandler httpMessageHandler)
        {
            HttpMessageHandler = httpMessageHandler;
            return this;
        }

        public RestClientConfigurationBuilder AddHandler(Func<DelegatingHandler> createHandler)
        {
            Verify.IsNotNull(nameof(createHandler), createHandler);

            Handlers.Add(createHandler);
            return this;
        }

        public RestClientConfigurationBuilder SetProperty(string key, object value)
        {
            Verify.IsNotEmpty(nameof(key), key);

            Properties[key] = value;
            return this;
        }

        public RestClientConfigurationBuilder SetProperty<T>(T value) where T : class
        {
            Verify.IsNotNull(nameof(value), value);

            Properties.Set(value);
            return this;
        }

        public IRestClientConfiguration Build()
        {
            return new RestClientConfiguration(this);
        }
    }
}
