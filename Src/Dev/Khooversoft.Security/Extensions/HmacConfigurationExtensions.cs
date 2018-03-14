using Khooversoft.Net;
using Khooversoft.Toolbox;

namespace Khooversoft.Security
{
    public static class HmacConfigurationExtensions
    {
        /// <summary>
        /// Enable HMAC for this REST call
        /// copy the HMAC configuration reference from the REST configuration to the REST client
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static RestClient EnableHmac(this RestClient self)
        {
            Verify.IsNotNull(nameof(self), self);

            HmacClient hmacClient = self.RestClientConfiguration.Properties.Get<HmacClient>();
            Verify.IsNotNull(nameof(hmacClient), hmacClient);

            self.Properties.Set(hmacClient);
            return self;
        }

        /// <summary>
        /// Clear HMAC
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static RestClient DisableHmac(this RestClient self)
        {
            Verify.IsNotNull(nameof(self), self);

            self.Properties.Remove<HmacClient>();

            return self;
        }
    }
}
