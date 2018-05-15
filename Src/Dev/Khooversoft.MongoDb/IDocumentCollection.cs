using Khooversoft.Toolbox;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    public interface IDocumentCollection<TDocument>
    {
        string CollectionName { get; }

        IMongoCollection<TDocument> MongoCollection { get; }

        IDocumentIndex<TDocument> Index { get; }

        Task Insert(IWorkContext context, TDocument document);

        Task<IEnumerable<TDocument>> Find(IWorkContext context, Expression<Func<TDocument, bool>> filter, FindOptions options = null);

        Task<IEnumerable<TDocument>> Find(IWorkContext context, FilterDefinition<TDocument> filter);

        Task Delete(IWorkContext context, FilterDefinition<TDocument> filter);

        Task<long> Count(IWorkContext context, FilterDefinition<TDocument> filter = null);
    }
}
