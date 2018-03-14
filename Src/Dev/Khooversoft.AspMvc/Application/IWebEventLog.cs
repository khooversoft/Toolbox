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
