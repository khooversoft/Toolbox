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
    public interface IDocumentCollection<TDocument>
    {
        string CollectionName { get; }

        IMongoCollection<TDocument> MongoCollection { get; }

        IDocumentIndex<TDocument> Index { get; }

        Task Insert(IWorkContext context, TDocument document);

        Task<IEnumerable<TDocument>> Find(IWorkContext context, BsonDocument search, BsonDocument projection = null);

        Task Delete(IWorkContext context, FilterDefinition<TDocument> filter);
    }
}
