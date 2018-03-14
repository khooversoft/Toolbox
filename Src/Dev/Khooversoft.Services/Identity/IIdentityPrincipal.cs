using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Services
{
    public interface IIdentityPrincipal
    {
        PrincipalId PrincipalId { get; }

        ApiKey ApiKey { get; }
    }
}
