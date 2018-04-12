// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Khooversoft.Net
{
    public static class RestResponseExtensions
    {
        public static async Task<RestResponse> ToRestResponseAsync(this Task<HttpResponseMessage> httpResponse, IWorkContext context)
        {
            var response = await httpResponse;
            return new RestResponse(response);
        }

        public static async Task<RestResponse> EnsureSuccessStatusCodeAsync(this RestResponse httpResponseResult, IWorkContext context, HttpStatusCode[] acceptedCodes = null)
        {
            try
            {
                httpResponseResult.AssertSuccessStatusCode(context, acceptedCodes: acceptedCodes);
            }
            catch (RestResponseException)
            {
                await httpResponseResult.GetErrorMessageAsync(context);
                throw;
            }

            return httpResponseResult;
        }

        public static async Task<RestResponse> EnsureSuccessStatusCodeAsync(this Task<RestResponse> httpResponseResult, IWorkContext context, HttpStatusCode[] acceptedCodes = null)
        {
            var response = await httpResponseResult;

            try
            {
                response.AssertSuccessStatusCode(context, acceptedCodes: acceptedCodes);
            }
            catch (RestResponseException)
            {
                await response.GetErrorMessageAsync(context);
                throw;
            }

            return response;
        }

        public static async Task<RestResponse<T>> GetContentAsync<T>(this Task<RestResponse> httpResponseResult, IWorkContext context)
        {
            var response = await httpResponseResult;

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return response.ToRestResponse<T>();
            }

            // Check to see if test dump header was returned
            TestDumpHeader dumpHeader = response.GetHeader<TestDumpHeader>();
            if (dumpHeader?.Value != TestDumpHeader.Commands.Reponse)
            {
                var content = await response.HttpResponseMessage.DeserializeObjectAsync<T>(context);
                return response.ToRestResponse<T>(content);
            }

            // Parse response and get dump information
            DebugDataResponse<T> debugData = await response.HttpResponseMessage.DeserializeObjectToDebugAsync<T>(context);

            return response.ToRestResponse(debugData.Value, debugData.DebugEvents);
        }

        public static RestResponse<T> ToRestResponse<T>(this HttpResponseMessage httpResponseMessage, T value = default(T))
        {
            return new RestResponse<T>(httpResponseMessage, value);
        }

        public static RestResponse<T> ToRestResponse<T>(this RestResponse httpResponse, T value = default(T))
        {
            return new RestResponse<T>(httpResponse, value);
        }

        public static RestResponse<T> ToRestResponse<T>(this RestResponse httpResponse, T value, DebugEventContractV1 debugEvent)
        {
            return new RestResponse<T>(httpResponse, value, debugEvent);
        }

        /// <summary>
        /// Get header from rest response
        /// </summary>
        /// <typeparam name="T">header property type</typeparam>
        /// <param name="httpResponseResult">header if found, null if not</param>
        /// <returns></returns>
        public static T GetHeader<T>(this RestResponse httpResponseResult) where T : IHttpHeaderProperty
        {
            IEnumerable<string> properties;
            if (httpResponseResult.HttpResponseMessage.Headers.TryGetValues(TestDumpHeader.HeaderKey, out properties))
            {
                return (T)(IHttpHeaderProperty)(new TestDumpHeader(properties.ToArray()));
            }

            return default(T);
        }
    }
}
