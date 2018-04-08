using Khooversoft.MongoDb.Models.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    public static class UtilityExtensions
    {
        public static bool IsEquals(this CollectionIndex self, IndexDetailV1 detail)
        {
            bool equal = self.Name.Equals(detail.Name) &&
                self.Unique == detail.Unique &&
                self.Sparse == detail.Sparse &&
                self.Keys?.Count == detail?.Keys.Count;

            if (!equal)
            {
                return false;
            }

            if (self.Keys?.Count() > 0)
            {
                return self.Keys
                    .OrderBy(x => x.FieldName)
                    .Zip(detail.Keys.OrderBy(x => x.FieldName), (f, s) => new { F = s, S = s })
                    .All(x => x.F.Equals(x.S));
            }

            return true;
        }
    }
}
