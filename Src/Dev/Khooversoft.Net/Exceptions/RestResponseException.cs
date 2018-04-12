// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;

namespace Khooversoft.Net
{
    [Serializable]
    public class RestResponseException : WorkException
    {
        protected RestResponseException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public RestResponseException(IWorkContext context, RestResponse restResponse, string message)
            : base(message, context)
        {
            RestResponse = restResponse;
        }

        public RestResponse RestResponse { get; }

        public override string ToString()
        {
            var lines = new List<string>();

            lines.Add(Message);
            lines.Add($"Response CV: {WorkContext.Cv}");

            if (RestResponse != null)
            {
                if (RestResponse.ErrorMessage != null)
                {
                    lines.Add($"Response error: {RestResponse.ErrorMessage}");
                    lines.Add($"RequestUrl: {RestResponse.ErrorMessage.RequestUrl}");
                    lines.Add($"Message.: {RestResponse.ErrorMessage.Message}");
                    lines.Add($"ExceptionType: {RestResponse.ErrorMessage.ExceptionType}");
                    lines.Add($"DetailMessage: {RestResponse.ErrorMessage.DetailMessage}");
                    lines.Add($"Cv: {RestResponse.ErrorMessage.Cv}");
                    lines.Add($"Tag: {RestResponse.ErrorMessage.Tag}");
                }
            }

            lines.Add(StackTrace);
            return string.Join(", ", lines);
        }
    }
}
