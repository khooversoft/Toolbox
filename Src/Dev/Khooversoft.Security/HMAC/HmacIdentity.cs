using Khooversoft.Toolbox;

namespace Khooversoft.Security
{
    public class HmacIdentity
    {
        public HmacIdentity(string credential)
        {
            Verify.IsNotEmpty(nameof(credential), credential);

            Credential = credential;
        }

        public string Credential { get; }
    }
}
