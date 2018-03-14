using Khooversoft.Toolbox;
using System.Threading.Tasks;

namespace Khooversoft.Services
{
    public interface IIdentityRepository
    {
        Task<IdentityPrincipal> GetAsync(IWorkContext context, PrincipalId principalId);

        Task SetAsync(IWorkContext context, IdentityPrincipal identityPrincipal);

        Task<IdentityPrincipal> RemoveAsync(IWorkContext context, PrincipalId principalId);
    }
}
