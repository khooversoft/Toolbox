using Khooversoft.Actor;
using Khooversoft.Toolbox;
using System.Threading.Tasks;

namespace Khooversoft.Services
{
    public interface ITokenClientActor : IActor
    {
        Task SetConfiguration(IWorkContext context, IClientTokenManagerConfiguration clientTokenManagerConfiguration);

        Task<string> GetApiKey(IWorkContext context);
    }
}
