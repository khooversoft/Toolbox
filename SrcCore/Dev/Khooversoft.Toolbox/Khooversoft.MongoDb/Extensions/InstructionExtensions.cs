using Khooversoft.Toolbox;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    public static class InstructionExtensions
    {
        public static async Task<IEnumerable<T>> Find<T>(this IDocumentCollection<T> self, IWorkContext context, IInstructionCollection instruction)
        {
            Verify.IsNotNull(nameof(self), self);
            Verify.IsNotNull(nameof(instruction), instruction);

            IInstructionCollection and = instruction as And;
            IInstructionCollection or = instruction as Or;
            IInstructionCollection andOr = and ?? or;

            if (andOr != null)
            {
                return await self.Find(context, andOr.ToDocument());
            }

            OrderBy orderBy = instruction.OfType<OrderBy>().FirstOrDefault();
            Projection projection = instruction.OfType<Projection>().FirstOrDefault();

            FindOptions<T, T> options = new FindOptions<T, T>
            {
                Sort = orderBy?.ToDocument(),
                Projection = projection?.ToDocument(),
            };

            Query query = instruction.OfType<Query>().FirstOrDefault();

            return await self.Find(context, query?.ToDocument() ?? new BsonDocument(), options);
        }
    }
}
