using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    [DebuggerDisplay("CollectionName={CollectionName}")]
    public class CollectionModel : IVerify
    {
        public string CollectionName { get; set; }

        public IList<CollectionIndex> Indexes { get; set; }

        public virtual bool IsValid()
        {
            return CollectionName.IsNotEmpty() &&
                (Indexes == null || (Indexes.All(x => x.IsValid())));
        }
    }
}
