using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]

namespace Khooversoft.MongoDb.Test
{
    internal static class Constants
    {
        internal static string ConnectionString = "mongodb://adminUser:Mark4096@localhost:27017";
    }
}
