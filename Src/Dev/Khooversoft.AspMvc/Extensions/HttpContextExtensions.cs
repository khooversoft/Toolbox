// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Net;
using Khooversoft.Toolbox;
using Microsoft.AspNetCore.Http;

namespace Khooversoft.AspMvc
{
    public static class HttpContextExtensions
    {
        public static RequestContext GetRequestContext(this HttpContext httpContext)
        {
            Verify.IsNotNull(nameof(httpContext), httpContext);

            return httpContext.Items.Get<RequestContext>(throwNotFound: true);
        }
    }
}
