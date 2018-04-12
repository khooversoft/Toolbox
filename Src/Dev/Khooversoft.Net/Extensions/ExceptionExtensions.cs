// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.Net;

namespace Khooversoft.Net
{
    /// <summary>
    /// Exception extensions
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Convert known exception to HTTP status code
        /// </summary>
        /// <param name="exception">exception</param>
        /// <returns>http status code, or internal server error as default</returns>
        public static HttpStatusCode ToHttpStatusCode(this Exception exception)
        {
            // if it's not one of the expected exception, set it to 500
            var code = HttpStatusCode.InternalServerError;

            switch (exception)
            {
                case NotAuthorizedException x:
                    code = HttpStatusCode.Unauthorized;
                    break;

                case ETagException x:
                    code = HttpStatusCode.NotModified;
                    break;

                case NotFoundException x:
                    code = HttpStatusCode.NotFound;
                    break;

                case BadRequestException x:
                    code = HttpStatusCode.BadRequest;
                    break;

                case ArgumentNullException x:
                    code = HttpStatusCode.BadRequest;
                    break;

                case ArgumentOutOfRangeException x:
                    code = HttpStatusCode.BadRequest;
                    break;

                case ArgumentException x:
                    code = HttpStatusCode.BadRequest;
                    break;

                default:
                    code = HttpStatusCode.InternalServerError;
                    break;
            }

            return code;
        }
    }
}
