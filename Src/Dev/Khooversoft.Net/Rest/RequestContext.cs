// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;

namespace Khooversoft.Net
{
    public class RequestContext
    {
        public RequestContext(IWorkContext context, string httpMethod, Uri uri)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotEmpty(nameof(httpMethod), httpMethod);
            Verify.IsNotNull(nameof(uri), uri);

            Context = context;
            HttpMethod = httpMethod;
            Uri = uri;
        }

        public IWorkContext Context { get; }

        public string HttpMethod { get; }

        public Uri Uri { get; }
    }
}
