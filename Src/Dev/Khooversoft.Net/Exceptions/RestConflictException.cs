// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.Runtime.Serialization;

namespace Khooversoft.Net
{
    [Serializable]
    public class RestConflictException : RestResponseException
    {
        protected RestConflictException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public RestConflictException(IWorkContext context, RestResponse restResponse, string message)
            : base(context, restResponse, message)
        {
        }
    }
}
