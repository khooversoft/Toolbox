using Khooversoft.Toolbox;

namespace Khooversoft.Services
{
    public interface IIdentityStore : IIdentityRepository
    {
        IdentityPrincipal Get(IWorkContext context, PrincipalId principalId);

        IdentityInMemoryStore Set(IWorkContext context, IdentityPrincipal identityPrincipal);

        IdentityPrincipal Remove(IWorkContext context, PrincipalId principalId);
    }
}
