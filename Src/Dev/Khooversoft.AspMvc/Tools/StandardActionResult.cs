// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.EventFlow;
using Khooversoft.Net;
using Khooversoft.Toolbox;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.AspMvc
{
    public class StandardActionResult : IActionResult
    {
        public StandardActionResult(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);

            Context = context;
            StatusCode = HttpStatusCode.OK;
        }

        public StandardActionResult(IWorkContext context, HttpStatusCode statusCode)
            : this(context)
        {
            StatusCode = statusCode;
        }

        public IWorkContext Context { get; }

        public HttpStatusCode StatusCode { get; }

        public object Content { get; private set; }

        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>();

        public StandardActionResult SetHeader<TValue>(TValue value) where TValue : IHttpHeaderProperty
        {
            if (value != null)
            {
                Headers.Add(value.GetType().Name, value);
            }

            return this;
        }

        public StandardActionResult SetContent<T>(T value)
        {
            Content = value;
            return this;
        }

        public Task ExecuteResultAsync(ActionContext actionContext)
        {
            actionContext.HttpContext.Response.StatusCode = (int)StatusCode;

            // Requested headers
            Headers.Values.OfType<IHttpHeaderProperty>()
                .Run(x => actionContext.HttpContext.Response.Headers[x.Key] = new StringValues(x.FormatValueForHttp()));

            // Standard headers
            actionContext.HttpContext.Response.Headers[CvHeader.HeaderKey] = new CvHeader(Context.Cv).FormatValueForHttp();

            DebugEventContractV1 debugObject = ProcessDebug(actionContext);

            if (Content != null)
            {
                if (debugObject == null)
                {
                    var objectResult = new ObjectResult(Content);
                    return objectResult.ExecuteResultAsync(actionContext);
                }

                // Append debug data to response
                if (Content != null && debugObject != null)
                {
                    // Embedded debug data
                    JObject contentObj = JObject.FromObject(Content);
                    JObject debugObj = JObject.FromObject(debugObject);
                    contentObj.Add("debug", debugObj);

                    // Indicate that the debug data is present
                    var header = new TestDumpHeader(TestDumpHeader.Commands.Reponse);
                    actionContext.HttpContext.Response.Headers[header.Key] = header.FormatValueForHttp();

                    var jsonResult = new JsonResult(contentObj);
                    return jsonResult.ExecuteResultAsync(actionContext);
                }
            }

            if (debugObject != null)
            {
                var objectResult = new ObjectResult(debugObject);
                return objectResult.ExecuteResultAsync(actionContext);
            }

            return Task.FromResult(0);
        }

        private DebugEventContractV1 ProcessDebug(ActionContext actionContext)
        {
            // Is the event data being requested
            TestDumpHeader testHeader = Context.Properties.Get<TestDumpHeader>();
            if (testHeader?.Value != TestDumpHeader.Commands.Request)
            {
                return null;
            }

            IEventDataBuffer eventDataBuffer = Context.Properties.Get<IEventDataBuffer>();
            if (eventDataBuffer == null)
            {
                return null;
            }

            IList<EventDetailContractV1> eventData = eventDataBuffer.SearchForBaseCv(Context.Cv.Value)
                .Select(x => x.ConvertTo())
                .ToList();

            return new DebugEventContractV1 { EventData = eventData };
        }
    }
}
