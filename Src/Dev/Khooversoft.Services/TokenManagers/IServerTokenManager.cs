using Khooversoft.Toolbox;
using System.Threading.Tasks;

namespace Khooversoft.Services
{
    public interface IServerTokenManager
    {
        Task<string> CreateAutorizationToken(IWorkContext context, string requestToken);
    }
}
