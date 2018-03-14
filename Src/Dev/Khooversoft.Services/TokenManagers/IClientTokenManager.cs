using Khooversoft.Net;
using Khooversoft.Security;
using Khooversoft.Toolbox;
using System.Threading.Tasks;

namespace Khooversoft.Services
{
    public interface IClientTokenManager
    {
        Task<string> CreateRequestToken(IWorkContext context, string subject = null);

        Task<RestResponse<string>> RequestServerAuthorizationToken(IWorkContext context, string requestToken);

        Task<JwtTokenDetails> ParseAuthorizationToken(IWorkContext context, string authorizationToken);
    }
}
