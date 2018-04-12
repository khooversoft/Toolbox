// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Net
{
    /// <summary>
    /// Log request and response, including performance
    /// </summary>
    public class LoggingHandler : DelegatingHandler
    {
        private static readonly Tag _tag = new Tag(nameof(LoggingHandler));

        public LoggingHandler()
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            IWorkContext context = (request.Properties.Get<IWorkContext>() ?? WorkContext.Empty)
                .WithTag(_tag);

            IEventLog eventLog = request.Properties.Get<IEventLog>() ?? NetEventSource.Log;

            var logDetails = new List<string>();
            logDetails.Add($"Method: {request.Method}");
            logDetails.Add($"Uri:{request.RequestUri}");

            if (request.Headers.Count() > 0)
            {
                logDetails.Add($"Headers: {request.Headers.Select(x => x.Key + "=" + x.Value).Aggregate(";")}");
            }

            try
            {
                using (var scope = new ActivityScope(context, string.Join(",", logDetails), eventLog))
                {
                    HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

                    logDetails.Add($"Response: StatusCode: {response.StatusCode}");
                    eventLog.Verbose(context, string.Join(",", logDetails));

                    return response;
                }
            }
            catch (Exception ex)
            {
                eventLog.Error(context, string.Join(",", logDetails), ex);
                throw;
            }
        }
    }
}
