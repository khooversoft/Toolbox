using Khooversoft.Net;
using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.AspMvc
{
    public static class Utility
    {
        public static HttpStatusCode CalculateStatusCode(Exception exception)
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
