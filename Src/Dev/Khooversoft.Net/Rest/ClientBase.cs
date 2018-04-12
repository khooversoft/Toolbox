// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;

namespace Khooversoft.Net
{
    /// <summary>
    /// Base API client
    /// </summary>
    public abstract class ClientBase
    {
        public ClientBase(Uri baseUri)
        {
            Verify.IsNotNull(nameof(baseUri), baseUri);

            BaseUri = baseUri;
        }

        public ClientBase(Uri baseUri, IRestClientConfiguration restClientConfiguration)
            :this(baseUri)
        {
            Verify.IsNotNull(nameof(restClientConfiguration), restClientConfiguration);

            RestClientConfiguration = restClientConfiguration;
        }

        /// <summary>
        /// Base URL address
        /// </summary>
        public Uri BaseUri { get; }

        /// <summary>
        /// Get REST client configuration
        /// </summary>
        public IRestClientConfiguration RestClientConfiguration { get; }

        /// <summary>
        /// Create REST client, setting the base URL
        /// </summary>
        /// <returns>REST client</returns>
        protected virtual RestClient CreateClient()
        {
            return new RestClient(RestClientConfiguration)
                .SetBaseAddress(BaseUri);
        }
    }
}
