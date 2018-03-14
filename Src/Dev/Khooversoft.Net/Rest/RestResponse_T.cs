using Khooversoft.Toolbox;
using System.Net.Http;

namespace Khooversoft.Net
{
    /// <summary>
    /// REST response with value
    /// </summary>
    /// <typeparam name="T">return type</typeparam>
    public class RestResponse<T> : RestResponse
    {
        private const string _httpRequestFailedText = "HTTP Request Failed";

        private RestResponse()
            : base()
        {
        }

        /// <summary>
        /// Construct response with value
        /// </summary>
        /// <param name="httpResponse">http response</param>
        /// <param name="value">value (optional)</param>
        public RestResponse(HttpResponseMessage httpResponse, T value = default(T))
            : base(httpResponse)
        {
            Value = value;
        }

        /// <summary>
        /// Conversion from RestReponse
        /// </summary>
        /// <param name="httpResponse">http response</param>
        /// <param name="value">value (optional)</param>
        public RestResponse(RestResponse httpResponse, T value = default(T))
        {
            Verify.IsNotNull(nameof(httpResponse), httpResponse);

            HttpResponseMessage = httpResponse.HttpResponseMessage;
            StatusCode = httpResponse.StatusCode;
            IsSuccessStatusCode = httpResponse.IsSuccessStatusCode;
            Value = value;
            ErrorMessage = httpResponse.ErrorMessage;
        }

        /// <summary>
        /// Construct response with value and debug event
        /// </summary>
        /// <param name="httpResponse">http response</param>
        /// <param name="value">value</param>
        /// <param name="debugEvent">debug event</param>
        public RestResponse(RestResponse httpResponse, T value, DebugEventContractV1 debugEvent)
            : this(httpResponse, value)
        {
            Verify.IsNotNull(nameof(debugEvent), debugEvent);

            DebugEvent = debugEvent;
        }

        /// <summary>
        /// Value (optional)
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// Debug event (optional)
        /// </summary>
        public DebugEventContractV1 DebugEvent { get; private set; }

        /// <summary>
        /// Create new instance with new error message
        /// </summary>
        /// <param name="errorMessage">error message to use</param>
        /// <returns>this</returns>
        public RestResponse<T> WithValue(T value)
        {
            return new RestResponse<T>
            {
                StatusCode = StatusCode,
                IsSuccessStatusCode = IsSuccessStatusCode,
                Value = value,
                ErrorMessage = ErrorMessage,
            };
        }

        /// <summary>
        /// Throw exception if success status = failure
        /// </summary>
        /// <param name="context">work context</param>
        /// <param name="message">message</param>
        /// <returns>this</returns>
        /// <exception cref="RestResponseException">if HTTP status is failure</exception>
        new public RestResponse<T> AssertSuccessStatusCode(IWorkContext context, string message = null)
        {
            Verify.IsNotNull(nameof(context), context);

            if (!IsSuccessStatusCode)
            {
                throw new RestResponseException(context, this, message ?? _httpRequestFailedText);
            }

            return this;
        }
    }
}
