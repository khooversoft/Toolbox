using Khooversoft.Toolbox;
using System.Threading.Tasks;

namespace Khooversoft.Services
{
    public interface ITokenClientRepository
    {
        Task SetConfiguration(IWorkContext context, IClientTokenManagerConfiguration clientTokenManagerConfiguration);

        Task<string> GetApiKey(IWorkContext context, TokenKey tokenKey);
    }
}
