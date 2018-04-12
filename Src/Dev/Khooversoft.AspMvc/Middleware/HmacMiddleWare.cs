// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Net;
using Khooversoft.Security;
using Khooversoft.Services;
using Khooversoft.Toolbox;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Khooversoft.AspMvc
{
    public class HmacMiddleware
    {
        private static Tag _tag = new Tag(nameof(HmacMiddleware));
        private readonly RequestDelegate _next;
        private readonly IIdentityRepository _identityRepository;
        private readonly IHmacConfiguration _hmacConfiguration;

        public HmacMiddleware(RequestDelegate next, IIdentityRepository identityRepository, IHmacConfiguration hmacConfiguration)
        {
            Verify.IsNotNull(nameof(next), next);

            _next = next;
            _identityRepository = identityRepository;
            _hmacConfiguration = hmacConfiguration;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            RequestContext requestContext = httpContext.Items.Get<RequestContext>();
            IWorkContext context = (requestContext.Context ?? WorkContext.Empty).WithTag(_tag);

            // Does the request has an authorization header
            string authorizationValue = httpContext.Request.Headers["Authorization"];
            if (authorizationValue.IsEmpty())
            {
                await _next(httpContext);
                return;
            }

            // Get authorization parts
            AuthenticationHeaderValue authorization;
            if (!AuthenticationHeaderValue.TryParse(authorizationValue, out authorization) || authorization.Scheme != "hmac")
            {
                await _next(httpContext);
                return;
            }

            AspMvcEventSource.Log.Verbose(context, $"Authorization {authorization.Parameter}");
            string[] signatureParts = authorization.Parameter.Split(':');
            if (signatureParts.Length != 2)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }

            // Lookup the credential
            IdentityPrincipal identity = await _identityRepository.GetAsync(context, new PrincipalId(signatureParts[0]));
            if (identity == null || identity.ApiKey == null || identity.ApiKey.HasExpired)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }

            // Try get date
            RequestHeaders requestHeaders = httpContext.Request.GetTypedHeaders();
            HmacSignature hmac = new HmacSignature(_hmacConfiguration);
            Uri url = new Uri(httpContext.Request.GetEncodedUrl());
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers = httpContext.Request.Headers.Select(x => new KeyValuePair<string, IEnumerable<string>>(x.Key, x.Value));

            // Validate HMAC signature
            if (!hmac.ValidateSignature(context, authorization.Parameter, identity.ApiKey.Value, httpContext.Request.Method, url, headers, requestHeaders.Date))
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }

            // Set identity of the caller
            httpContext.Items.Set(new HmacIdentity(authorization.Parameter));

            // Next
            await _next(httpContext);
        }
    }
}
