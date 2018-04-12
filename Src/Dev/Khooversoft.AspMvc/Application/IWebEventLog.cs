// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Net;
using Khooversoft.Toolbox;
using Microsoft.AspNetCore.Http;

namespace Khooversoft.AspMvc
{
    public interface IWebEventLog : IEventLog
    {
        void HttpRequestStart(IWorkContext context, RequestContext requestContext);

        void HttpRequestStop(IWorkContext context, RequestContext requestContext, long durationMs);

        void HttpError(IWorkContext context, HttpRequest request, HttpResponse response, string message);
    }
}
