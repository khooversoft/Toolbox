// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Net
{
    public static class HttpExtensions
    {
        public static async Task<T> DeserializeObjectAsync<T>(this HttpResponseMessage message, IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithMethodName();

            try
            {
                string json = await message.Content.ReadAsStringAsync();

                if (typeof(T) == typeof(string))
                {
                    return (T)(object)json;
                }

                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                NetEventSource.Log.Error(context, nameof(DeserializeObjectAsync), ex);
                throw;
            }
        }

        public static async Task<DebugDataResponse<T>> DeserializeObjectToDebugAsync<T>(this HttpResponseMessage message, IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithMethodName();

            try
            {
                string json = await message.Content.ReadAsStringAsync();

                if (typeof(T) == typeof(string))
                {
                    return new DebugDataResponse<T> { Value = (T)(object)json };
                }

                JToken content = JObject.Parse(json);
                JToken debug = content["debug"];

                if (debug == null)
                {
                    return new DebugDataResponse<T> { Value = JsonConvert.DeserializeObject<T>(json) };
                }

                return new DebugDataResponse<T>
                {
                    Value = JsonConvert.DeserializeObject<T>(json),
                    DebugEvents = debug.ToObject<DebugEventContractV1>(),
                };
            }
            catch (Exception ex)
            {
                NetEventSource.Log.Error(context, nameof(DeserializeObjectToDebugAsync), ex);
                throw;
            }
        }

        public static async Task<T> TryDeserializeObjectAsync<T>(this HttpResponseMessage message, IWorkContext context) where T : class
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithMethodName();

            T data;

            try
            {
                var json = await message.Content.ReadAsStringAsync();
                data = JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                return null;
            }

            return data;
        }
    }
}
