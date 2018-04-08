using Khooversoft.MongoDb.Models.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    public static class ConvertToExtensions
    {
        internal static DatabaseDetailV1 ConvertTo(this InternalDatabaseDetail self)
        {
            if (self == null)
            {
                return null;
            }

            return new DatabaseDetailV1
            {
                Name = self.Name,
                SizeOnDisk = self.SizeOnDisk,
                Empty = self.Empty,
            };
        }

        internal static CollectionDetailV1 ConvertTo(this InternalCollectionDetail self)
        {
            if (self == null)
            {
                return null;
            }

            return new CollectionDetailV1
            {
                Name = self.Name,
                Type = self.Type,
                Readonly = self.Info.Readonly,
            };
        }
    }
}
