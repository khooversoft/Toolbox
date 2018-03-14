using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Khooversoft.Security
{
    /// <summary>
    /// Configuration details for HMAC signature. Can be used for both signing and verification.
    /// </summary>
    public class HmacClient
    {
        private static Tag _tag = new Tag(nameof(HmacClient));
        private Func<IWorkContext, Task<string>> _getHmacKey;

        private HmacClient(IHmacConfiguration hmacConfiguration, string credential)
        {
            Verify.IsNotNull(nameof(hmacConfiguration), hmacConfiguration);
            Verify.IsNotEmpty(nameof(credential), credential);

            HmacConfiguration = hmacConfiguration;
            Credential = credential;
        }

        public HmacClient(IHmacConfiguration hmacConfiguration, string credential, string hmacKey)
            : this(hmacConfiguration, credential)
        {
            Verify.IsNotEmpty(nameof(hmacKey), hmacKey);

            HmacKey = hmacKey;
        }

        public HmacClient(IHmacConfiguration hmacConfiguration, string credential, Func<IWorkContext, Task<string>> getHmacKey)
            : this(hmacConfiguration, credential)
        {
            Verify.IsNotNull(nameof(getHmacKey), getHmacKey);

            _getHmacKey = getHmacKey;
        }

        public IHmacConfiguration HmacConfiguration { get; }

        public string Credential { get; }

        public string HmacKey { get; private set; }

        public async Task ValidateHmacKey(IWorkContext context)
        {
            Verify.IsNotNull(nameof(_getHmacKey), _getHmacKey);
            Verify.IsNotNull(nameof(context), context);
            context = context.WithTag(_tag);

            HmacKey = await _getHmacKey(context);
        }

        public async Task<string> CreateSignature(IWorkContext context, string method, Uri url, IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
        {
            context = context.WithTag(_tag);
            if (_getHmacKey != null)
            {
                await ValidateHmacKey(context);
            }

            Verify.IsNotEmpty(nameof(HmacKey), HmacKey);
            return new HmacSignature(HmacConfiguration).CreateSignature(context, Credential,HmacKey, method, url, headers);
        }
    }
}
