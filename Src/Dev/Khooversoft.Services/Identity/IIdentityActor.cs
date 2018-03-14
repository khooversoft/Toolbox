using Khooversoft.Actor;
using Khooversoft.Toolbox;
using System.Threading.Tasks;

namespace Khooversoft.Services
{
    public interface IIdentityActor : IActor
    {
        Task Set(IWorkContext context, IdentityPrincipal identityPrincipal);

        Task<IdentityPrincipal> Get(IWorkContext context);

        Task<IdentityPrincipal> Remove(IWorkContext context);
    }
}
