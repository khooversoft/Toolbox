using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    public enum MongoTokenType
    {
        Root,
        Match,
        Where,
        Equal,
        In,
        LessThan,
        GreaterThan,
        Or,
    };
}
