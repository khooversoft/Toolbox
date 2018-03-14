using Khooversoft.Toolbox;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Net
{
    /// <summary>
    /// Add application headers to the request
    /// 
    /// Header information is retrieved from the WorkContext (IWorkContext) properties for classes that implement the IHttpHeaderProperty
    /// </summary>
    public class RequestHandler : DelegatingHandler
    {
        private static readonly Tag _tag = new Tag(nameof(LoggingHandler));

        public RequestHandler()
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            IWorkContext context = (request.Properties.Get<IWorkContext>() ?? WorkContext.Empty)
                .WithTag(_tag);

            // Handle headers
            context.Properties.Values.OfType<IHttpHeaderProperty>().Run(x => request.Headers.Add(x.Key, x.FormatValueForHttp()));

            // Add standard headers
            request.Headers.Date = DateTimeOffset.UtcNow;
            request.Headers.Add(RequestIdHeader.HeaderKey, Guid.NewGuid().ToString());

            HttpResponseMessage result = await base.SendAsync(request, cancellationToken);

            return result;
        }
    }
}
