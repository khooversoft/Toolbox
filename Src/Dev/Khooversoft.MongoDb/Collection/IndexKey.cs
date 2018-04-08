using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    [DebuggerDisplay("FieldName={FieldName}, Descending={Descending}")]
    public class IndexKey : IEquatable<IndexKey>, IVerify
    {
        public string FieldName { get; set; }

        public bool Descending { get; set; }

        public bool Equals(IndexKey other)
        {
            if (other == null)
            {
                return false;
            }

            return FieldName.Equals(other.FieldName) &&
                Descending == other.Descending;
        }

        public bool IsValid()
        {
            return FieldName.IsNotEmpty();
        }
    }
}
