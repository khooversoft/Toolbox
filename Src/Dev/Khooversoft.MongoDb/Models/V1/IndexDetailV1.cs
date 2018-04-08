using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb.Models.V1
{
    [DebuggerDisplay("Name={Name}, Unique={Unique}, Sparse={Sparse}")]
    public class IndexDetailV1 : IEquatable<IndexDetailV1>
    {
        public string Name { get; set; }

        public int Version { get; set; }

        public bool Unique { get; set; }

        public bool Sparse { get; set; }

        public IList<IndexKey> Keys { get; set; }

        public string Namespace { get; set; }

        public bool Equals(IndexDetailV1 other)
        {
            if (other == null)
            {
                return false;
            }

            bool equal = Name.Equals(other.Name) &&
                Version == other.Version &&
                Unique == other.Unique &&
                Sparse == other.Sparse &&
                Namespace.Equals(other.Name) &&
                Keys == other.Keys &&
                Keys?.Count == other?.Keys.Count;

            if (!equal)
            {
                return false;
            }

            if (Keys?.Count() > 0)
            {
                return Keys
                    .OrderBy(x => x.FieldName)
                    .Zip(other.Keys.OrderBy(x => x.FieldName), (f, s) => new { F = s, S = s })
                    .All(x => x.F.Equals(x.S));
            }

            return true;
        }
    }
}
