using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Net
{
    [Serializable]
    public class RestNotFoundException : RestResponseException
    {
        protected RestNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public RestNotFoundException(IWorkContext context, RestResponse restResponse, string message)
            : base(context, restResponse, message)
        {
        }
    }
}
