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
