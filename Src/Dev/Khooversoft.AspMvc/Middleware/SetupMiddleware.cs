// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Net;
using Khooversoft.Toolbox;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Threading.Tasks;

namespace Khooversoft.AspMvc
{
    /// <summary>
    /// Setup processing middleware,
    ///   (1) Builds request context
    ///   (2) Looks for well known headers
    ///   (3) Looks for and extends CV, or creates
    /// </summary>
    public class SetupMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHeaderFactory _headerFactory;
        private readonly IServiceConfiguration _serviceConfiguration;
        private readonly IWebEventLog _webLogEvent;

        public SetupMiddleware(RequestDelegate next, IHeaderFactory headerFactory, IServiceConfiguration middleWareContext)
        {
            Verify.IsNotNull(nameof(next), next);
            Verify.IsNotNull(nameof(headerFactory), headerFactory);
            Verify.IsNotNull(nameof(middleWareContext), middleWareContext);

            _next = next;
            _headerFactory = headerFactory;
            _serviceConfiguration = middleWareContext;
            _webLogEvent = _serviceConfiguration.Get<IWebEventLog>() ?? AspMvcEventSource.Log;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            IWorkContext workContext = httpContext.Items.Get<IWorkContext>() ?? WorkContext.Empty;
            var builder = new WorkContextBuilder(workContext);
            builder.SetContainer(_serviceConfiguration.Container);

            foreach (var item in httpContext.Request.Headers)
            {
                var property = _headerFactory.Create(item.Key, item.Value);
                if (property != null)
                {
                    builder.Properties.Set(property, derivedType: property.GetType());
                }
            }

            // Correlation Vector
            var headerCv = builder.Properties.Get<CvHeader>();
            if (headerCv != null)
            {
                builder.Cv = new CorrelationVector(headerCv.Value);
            }

            workContext = builder.Build();

            Uri url = new Uri(httpContext.Request.GetEncodedUrl());
            var requestContext = new RequestContext(workContext, httpContext.Request.Method, url);
            httpContext.Items.Set(requestContext);

            using (var scope = new TimerScope(
                () => _webLogEvent.HttpRequestStart(workContext, requestContext),
                (x) => _webLogEvent.HttpRequestStop(workContext, requestContext, x)))
            {
                await _next.Invoke(httpContext);

                if (httpContext?.Response?.StatusCode != null)
                {
                    AspMvcEventSource.Log.Verbose(workContext, $"REST response result: {httpContext.Response.StatusCode}");
                }
            }
        }
    }
}
