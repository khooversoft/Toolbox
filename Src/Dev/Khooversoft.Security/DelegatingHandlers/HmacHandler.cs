// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Security
{
    /// <summary>
    /// Calculates HMAC signature based on HMAC configuration of client is present in properties
    /// </summary>
    public class HmacHandler : DelegatingHandler
    {
        private static readonly Tag _tag = new Tag(nameof(HmacHandler));

        public HmacHandler()
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            IWorkContext context = (request.Properties.Get<IWorkContext>() ?? WorkContext.Empty)
                .WithTag(_tag);

            // Check to see if we have a HMAC configuration
            HmacClient hmacClient = request.Properties.Get<HmacClient>();
            if (hmacClient == null)
            {
                return await base.SendAsync(request, cancellationToken);
            }

            SecurityEventSource.Log.Verbose(context, "Processing HMAC");

            // Collection header values
            List<KeyValuePair<string, IEnumerable<string>>> headers = request.Headers.ToList();

            if (request.Content != null)
            {
                await AddContentMd5Hash(request);

                IEnumerable<KeyValuePair<string, IEnumerable<string>>> contentHeaders = request.Content.Headers;
                headers.AddRange(contentHeaders);
            }

            string siguature = await hmacClient.CreateSignature(context, request.Method.Method, request.RequestUri, headers);
            request.Headers.Authorization = new AuthenticationHeaderValue("hmac", siguature);

            HttpResponseMessage result = await base.SendAsync(request, cancellationToken);
            return result;
        }

        private async Task AddContentMd5Hash(HttpRequestMessage request)
        {
            // Calculate MD5 hash for content
            using (var ms = new MemoryStream())
            {
                byte[] content = await request.Content.ReadAsByteArrayAsync();
                if (content.Length > 0)
                {
                    request.Content.Headers.ContentMD5 = content.ToMd5Hash();
                }
            }
        }
    }
}
