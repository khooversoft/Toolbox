using Khooversoft.Toolbox;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Khooversoft.Net
{
    /// <summary>
    /// REST response
    /// </summary>
    public class RestResponse
    {
        protected RestResponse()
        {
        }

        public RestResponse(HttpResponseMessage httpResponse)
        {
            Verify.IsNotNull(nameof(httpResponse), httpResponse);

            HttpResponseMessage = httpResponse;
            StatusCode = httpResponse.StatusCode;
            IsSuccessStatusCode = httpResponse.IsSuccessStatusCode;
        }

        public HttpResponseMessage HttpResponseMessage { get; protected set; }

        public HttpStatusCode StatusCode { get; protected set; }

        public bool IsSuccessStatusCode { get; protected set; }

        public ErrorMessageContractV1 ErrorMessage { get; protected set; }

        /// <summary>
        /// Throw exception if success status = failure
        /// </summary>
        /// <param name="context">work context</param>
        /// <param name="message">message</param>
        /// <returns>this</returns>
        /// <exception cref="RestResponseException">if HTTP status is failure</exception>
        public RestResponse AssertSuccessStatusCode(IWorkContext context, string message = null)
        {
            Verify.IsNotNull(nameof(context), context);

            if (!IsSuccessStatusCode)
            {
                message = message ?? StatusCode.ToString();
                throw new RestResponseException(context, this, message);
            }

            return this;
        }

        /// <summary>
        /// Assert status code
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="code">code to assert</param>
        /// <param name="message">message (optional)</param>
        /// <returns>this</returns>
        public RestResponse AssertStatus(IWorkContext context, HttpStatusCode code, string message = null)
        {
            Verify.IsNotNull(nameof(context), context);

            if (StatusCode != code)
            {
                throw new RestResponseException(context, this, message ?? $"Returned status code {StatusCode}, should be {code}");
            }

            return this;
        }

        /// <summary>
        /// Get error message from HTTP response
        /// </summary>
        /// <param name="context">work context</param>
        /// <param name="httpResponse">HTTP response message</param>
        /// <returns>this</returns>
        public async Task<RestResponse> GetErrorMessageAsync(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);

            ErrorMessage = await HttpResponseMessage.TryDeserializeObjectAsync<ErrorMessageContractV1>(context);
            return this;
        }
    }
}
