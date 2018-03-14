using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Khooversoft.Net
{
    /// <summary>
    /// REST (HttpClient) configuration
    /// 
    /// Provides a HttpClient with handlers constructed based on a REST configuration.
    /// It is recommended that all REST calls use the same REST configuration for performance.
    /// </summary>
    public class RestClientConfiguration : IRestClientConfiguration
    {
        private RestClientConfiguration()
        {
        }

        public RestClientConfiguration(RestClientConfigurationBuilder builder)
        {
            CreateHttpClient(builder);
            Properties = builder.Properties.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// HttpClient 
        /// </summary>
        public HttpClient HttpClient { get; private set; }

        /// <summary>
        /// Properties
        /// </summary>
        public IReadOnlyDictionary<string, object> Properties { get; private set; }

        /// <summary>
        /// Build new REST client configuration with new property
        /// </summary>
        /// <typeparam name="T">type of value</typeparam>
        /// <param name="value">value</param>
        /// <returns>new REST client configuration</returns>
        public IRestClientConfiguration WithProperty<T>(T value) where T : class
        {
            Dictionary<string, object> dict = Properties.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
            dict.Set(value);

            return new RestClientConfiguration
            {
                HttpClient = HttpClient,
                Properties = dict,
            };
        }

        /// <summary>
        /// Default rest client configuration
        /// </summary>
        public static IRestClientConfiguration Default { get; } = new RestClientConfigurationBuilder().Build();

        /// <summary>
        /// Build HttpClient with custom message if specified
        /// </summary>
        /// <param name="builder">builder</param>
        private void CreateHttpClient(RestClientConfigurationBuilder builder)
        {
            var handlers = new List<DelegatingHandler>(builder.Handlers.Select(x => x()))
                .ToArray();

            if (builder.HttpMessageHandler != null)
            {
                HttpClient = HttpClientFactory.Create(builder.HttpMessageHandler, handlers);
            }
            else
            {
                HttpClient = HttpClientFactory.Create(handlers);
            }
        }
    }
}

