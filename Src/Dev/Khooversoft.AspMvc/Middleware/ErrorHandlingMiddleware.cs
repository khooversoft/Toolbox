using Khooversoft.EventFlow;
using Khooversoft.Net;
using Khooversoft.Toolbox;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.AspMvc
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceConfiguration _serviceConfiguration;
        private readonly Tag _tag = new Tag(nameof(ErrorHandlingMiddleware));
        private readonly IWebEventLog _webEventLog;
        private readonly IEventDataBuffer _eventDataBuffer;
        private readonly bool _verboseOnErrors;

        public ErrorHandlingMiddleware(RequestDelegate next, IServiceConfiguration middleWareContext)
        {
            Verify.IsNotNull(nameof(next), next);
            Verify.IsNotNull(nameof(middleWareContext), middleWareContext);

            _next = next;
            _serviceConfiguration = middleWareContext;

            _webEventLog = _serviceConfiguration.Get<IWebEventLog>() ?? AspMvcEventSource.Log;
            _eventDataBuffer = _serviceConfiguration.Get<IEventDataBuffer>();
            _verboseOnErrors = _serviceConfiguration.Get<VerboseOnErrors>()?.ShowErrors ?? false;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                // must be awaited
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            HttpStatusCode code = Utility.CalculateStatusCode(exception);

            RequestContext requestContext = httpContext.Items.Get<RequestContext>();
            IWorkContext context = requestContext.Context ?? WorkContext.Empty;

            if (code == HttpStatusCode.InternalServerError)
            {
                _serviceConfiguration.EventLog.Error(context, exception.Message, exception);
            }

            CorrelationVector cv = null;
            Tag tag = null;

            if (requestContext != null)
            {
                cv = requestContext.Context.Cv;
                tag = requestContext.Context.Tag;
            }

            _webEventLog.HttpError(
                requestContext.Context,
                httpContext.Request,
                httpContext.Response,
                exception.Message);

            return WriteExceptionAsync(httpContext, exception, code, cv, tag);
        }

        private Task WriteExceptionAsync(HttpContext httpContext, Exception exception, HttpStatusCode code, CorrelationVector cv, Tag tag)
        {
            var response = httpContext.Response;

            response.ContentType = "application/json";
            response.StatusCode = (int)code;

            ErrorMessageContractV1 errorMessage;

            if (_verboseOnErrors)
            {
                errorMessage = new ErrorMessageContractV1
                {
                    HttpStatus = (int)code,
                    RequestUrl = httpContext.Request.GetDisplayUrl(),
                    Message = exception.Message,
                    ExceptionType = exception.GetType().FullName,
                    DetailMessage = exception.ToString(),
                    Cv = cv,
                    Tag = tag,
                };

                if (cv != null)
                {
                    IEnumerable<EventDetailContractV1> eventData = _eventDataBuffer.SearchForBaseCv(cv.Value).Select(x => x.ConvertTo());
                    if (eventData.Count() > 0)
                    {
                        errorMessage.EventDetails = new List<EventDetailContractV1>(eventData);
                    }
                }
            }
            else
            {
                errorMessage = new ErrorMessageContractV1
                {
                    HttpStatus = (int)code,
                    RequestUrl = httpContext.Request.GetDisplayUrl(),
                    Message = exception.Message,
                    ExceptionType = exception.GetType().FullName,
                    Cv = cv,
                };
            }

            if (cv != null)
            {
                response.Headers.Add(CvHeader.HeaderKey, new StringValues(cv));
            }

            return response.WriteAsync(JsonConvert.SerializeObject(errorMessage));
        }
    }
}
