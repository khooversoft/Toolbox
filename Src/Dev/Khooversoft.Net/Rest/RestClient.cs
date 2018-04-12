// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Net
{
    /// <summary>
    /// REST Client provides a builder pattern for making a REST API call.
    /// 
    /// Note: if the Absolute URI is specified, this URI is used and not build
    /// process is not used.
    /// </summary>
    public class RestClient
    {
        private HttpClient _client;

        /// <summary>
        /// Create REST client and use provided HttpClient
        /// </summary>
        /// <param name="client"></param>
        public RestClient(HttpClient client)
            : base()
        {
            Verify.IsNotNull(nameof(client), client);

            _client = client;
        }

        /// <summary>
        /// Create REST client based on configuration
        /// </summary>
        /// <param name="restClientConfiguration"></param>
        public RestClient(IRestClientConfiguration restClientConfiguration)
        {
            Verify.IsNotNull(nameof(restClientConfiguration), restClientConfiguration);

            RestClientConfiguration = restClientConfiguration;
            _client = RestClientConfiguration.HttpClient;
        }

        /// <summary>
        /// Use absolute URI, URI is not built
        /// </summary>
        public Uri AbsoluteUri { get; private set; }

        /// <summary>
        /// Base URI
        /// </summary>
        public Uri BaseAddress { get; private set; }

        /// <summary>
        /// Path elements
        /// </summary>
        public StringVector PathItems { get; private set; } = StringVector.Empty;

        /// <summary>
        /// Header items
        /// </summary>
        public IDictionary<string, string> HeaderItems { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Query items
        /// </summary>
        public IDictionary<string, string> QueryItems { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Properties to be passed in the context
        /// </summary>
        public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Get REST client configuration
        /// </summary>
        public IRestClientConfiguration RestClientConfiguration { get; }

        /// <summary>
        /// Content for the REST API call
        /// </summary>
        public HttpContent Content { get; private set; }

        /// <summary>
        /// If specified, create IF header
        /// </summary>
        public string IfMatch { get; private set; }

        /// <summary>
        /// Clear all settings
        /// </summary>
        /// <returns></returns>
        public RestClient Clear()
        {
            HeaderItems.Clear();
            QueryItems.Clear();
            PathItems = StringVector.Empty;
            AbsoluteUri = null;
            BaseAddress = null;

            return this;
        }

        /// <summary>
        /// Set the absolute URI to use, overrides the build of the URI based on path, headers, and query
        /// </summary>
        /// <param name="uri">URI</param>
        /// <returns>this</returns>
        public RestClient SetAbsoluteUri(Uri uri)
        {
            AbsoluteUri = uri;
            return this;
        }

        /// <summary>
        /// Set base address of the REST URI
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <returns>this</returns>
        public RestClient SetBaseAddress(Uri baseAddress)
        {
            Verify.IsNotNull(nameof(baseAddress), baseAddress);

            var build = new UriBuilder(baseAddress);
            build.Path = null;
            build.Query = null;

            BaseAddress = build.Uri;
            return this;
        }

        /// <summary>
        /// Add path item (string vector(s))
        /// </summary>
        /// <param name="value">path(s)</param>
        /// <param name="values">additional path(s)</param>
        /// <returns>this</returns>
        public RestClient AddPath(string value, params string[] values)
        {
            Verify.IsNotEmpty(nameof(value), value);

            PathItems = PathItems.With(value);
            if (values != null)
            {
                PathItems = PathItems.With(values);
            }

            return this;
        }

        /// <summary>
        /// Add header item
        /// </summary>
        /// <param name="name">header name</param>
        /// <param name="value">value</param>
        /// <returns>this</returns>
        public RestClient AddHeader(string name, string value)
        {
            Verify.IsNotEmpty(nameof(name), name);
            Verify.IsNotEmpty(nameof(value), value);

            HeaderItems.Add(name, value);
            return this;
        }

        /// <summary>
        /// Add query item
        /// </summary>
        /// <param name="name">name of query</param>
        /// <param name="value">value of query</param>
        /// <returns>this</returns>
        public RestClient AddQuery(string name, string value)
        {
            Verify.IsNotEmpty(nameof(name), name);

            if (value.IsNotEmpty())
            {
                QueryItems.Add(name, value);
            }

            return this;
        }

        /// <summary>
        /// Set content
        /// </summary>
        /// <param name="content">HTTP content</param>
        /// <returns>this</returns>
        public RestClient SetContent(HttpContent content)
        {
            Content = content;
            return this;
        }

        /// <summary>
        /// Set content
        /// </summary>
        /// <typeparam name="T">type to serialize</typeparam>
        /// <param name="value">value type instance</param>
        /// <param name="required">true if required, false return with out setting</param>
        /// <returns></returns>
        public RestClient SetContent<T>(T value, bool required = true)
        {
            if (required)
            {
                Verify.IsNotNull(nameof(value), value);
            }

            string jsonString = JsonConvert.SerializeObject(value);
            Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return this;
        }

        /// <summary>
        /// Set the IF match value
        /// </summary>
        /// <param name="value">IF match value</param>
        /// <returns>this</returns>
        public RestClient SetIfMatch(string value)
        {
            IfMatch = value;
            return this;
        }

        /// <summary>
        /// Send request
        /// </summary>
        /// <param name="context">work context</param>
        /// <param name="requestMessage">request message</param>
        /// <param name="token">cancellation tokens</param>
        /// <returns>state of HTTP response</returns>
        public async Task<HttpResponseMessage> SendAsync(IWorkContext context, HttpRequestMessage requestMessage, CancellationToken? token = null)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(requestMessage), requestMessage);

            context = context.WithMethodName();
            token = token ?? CancellationToken.None;

            HttpResponseMessage response;
            try
            {
                using (var scope = new TimerScope(
                    () => NetEventSource.Log.ActivityStart(context),
                    x => NetEventSource.Log.ActivityStop(context, durationMs: x)))
                {
                    response = await _client.SendAsync(requestMessage, (CancellationToken)token);
                }
            }
            catch (Exception ex)
            {
                NetEventSource.Log.Error(context, nameof(SendAsync), ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Issue Get
        /// </summary>
        /// <param name="context">work context</param>
        /// <param name="token">cancellation tokens</param>
        /// <returns>this</returns>
        public Task<HttpResponseMessage> GetAsync(IWorkContext context, CancellationToken? token = null)
        {
            Verify.IsNotNull(nameof(context), context);

            token = token ?? CancellationToken.None;
            return SendAsync(context, BuildRequestMessage(context, HttpMethod.Get), token);
        }

        /// <summary>
        /// Issue Delete
        /// </summary>
        /// <param name="context">work context</param>
        /// <param name="token">cancellation tokens</param>
        /// <returns>this</returns>
        public Task<HttpResponseMessage> DeleteAsync(IWorkContext context, CancellationToken? token = null)
        {
            Verify.IsNotNull(nameof(context), context);

            token = token ?? CancellationToken.None;
            return SendAsync(context, BuildRequestMessage(context, HttpMethod.Delete), token);
        }

        /// <summary>
        /// Issue Post
        /// </summary>
        /// <param name="context">work context</param>
        /// <param name="token">cancellation tokens</param>
        /// <returns>this</returns>
        public Task<HttpResponseMessage> PostAsync(IWorkContext context, CancellationToken? token = null)
        {
            Verify.IsNotNull(nameof(context), context);

            token = token ?? CancellationToken.None;
            return SendAsync(context, BuildRequestMessage(context, HttpMethod.Post), token);
        }

        /// <summary>
        /// Issue Put
        /// </summary>
        /// <param name="context">work context</param>
        /// <param name="token">cancellation tokens</param>
        /// <returns>this</returns>
        public Task<HttpResponseMessage> PutAsync(IWorkContext context, CancellationToken? token = null)
        {
            Verify.IsNotNull(nameof(context), context);

            token = token ?? CancellationToken.None;
            return SendAsync(context, BuildRequestMessage(context, HttpMethod.Put), token);
        }

        /// <summary>
        /// Build HTTP request
        /// </summary>
        /// <param name="context">work context</param>
        /// <param name="method">HTTP method</param>
        /// <returns>HTTP request message</returns>
        public HttpRequestMessage BuildRequestMessage(IWorkContext context, HttpMethod method)
        {
            Uri uri = AbsoluteUri ??
                new RestUriBuilder()
                .SetBaseUri(BaseAddress)
                .AddPath(PathItems)
                .AddQuery<string>(QueryItems)
                .Build();

            var request = new HttpRequestMessage(method, uri)
            {
                Content = Content,
            };

            // ====================================================================================
            // Set Headers
            if (IfMatch.IsNotEmpty())
            {
                request.Headers.IfMatch.Add(new EntityTagHeaderValue(IfMatch));
            }

            HeaderItems.Run(x => request.Headers.Add(x.Key, x.Value));

            context.Properties.Values
                .OfType<IHttpHeaderProperty>()
                .Run(x => request.Headers.Add(x.Key, x.FormatValueForHttp()));

            Properties.Values
                .OfType<IHttpHeaderProperty>()
                .Run(x => request.Headers.Add(x.Key, x.FormatValueForHttp()));

            // ====================================================================================
            // Set properties
            Properties.Run(x => request.Properties.Add(x.Key, x.Value));
            request.Properties.Set(context);

            return request;
        }
    }
}
