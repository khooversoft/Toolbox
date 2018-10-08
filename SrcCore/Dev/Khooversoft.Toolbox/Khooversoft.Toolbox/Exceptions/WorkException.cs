// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Khooversoft.Toolbox
{
    [Serializable]
    public class WorkException : Exception
    {
        protected WorkException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public WorkException(string message, IWorkContext workContext)
            : base(message)
        {
            WorkContext = workContext;
        }

        public WorkException(string message, IWorkContext workContext, Exception inner)
            : base(message, inner)
        {
            WorkContext = workContext;
        }

        public IWorkContext WorkContext { get; }

        public override string ToString()
        {
            var lines = new List<string>();

            lines.Add(Message);
            lines.Add($"Response CV: {WorkContext.Cv}");
            lines.Add(StackTrace);

            return string.Join(", ", lines);
        }
    }
}
