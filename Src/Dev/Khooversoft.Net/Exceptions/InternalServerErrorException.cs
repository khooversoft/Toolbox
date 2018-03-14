using Khooversoft.Toolbox;
using System;
using System.Runtime.Serialization;

namespace Khooversoft.Net
{
    [Serializable]
    public class InternalServerErrorException : WorkException
    {
        public InternalServerErrorException(string message, IWorkContext workContext)
            : base(message, workContext)
        {
        }

        public InternalServerErrorException(string message, IWorkContext workContext, Exception inner)
            : base(message, workContext, inner)
        {
        }

        protected InternalServerErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
