using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    [DebuggerDisplay("Name={Name}, Unique={Unique}, Sparse={Sparse}")]
    public class CollectionIndex : IVerify
    {
        public string Name { get; set; }

        public bool Unique { get; set; }

        public bool Sparse { get; set; }

        public IList<IndexKey> Keys { get; set; }

        public bool IsValid()
        {
            return Name.IsNotEmpty() &&
                Keys != null &&
                Keys.All(x => x.IsValid());
        }
    }
}
