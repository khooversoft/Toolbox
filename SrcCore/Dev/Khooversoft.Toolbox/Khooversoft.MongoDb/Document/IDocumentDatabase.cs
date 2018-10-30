using Khooversoft.MongoDb.Models.V1;
using Khooversoft.Toolbox;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    public interface IDocumentDatabase
    {
        IDocumentServer DocumentServer { get; }

        IMongoDatabase MongoDatabase { get; }

        Task<IEnumerable<CollectionDetailV1>> ListCollections(IWorkContext context);

        Task<CollectionDetailV1> GetCollectionDetail(IWorkContext context, string collectionName);

        Task<bool> CollectionExist(IWorkContext context, string collectionName);

        Task CreateCollection(IWorkContext context, string collectionName);

        Task CreateCappedCollection(IWorkContext context, string collectionName, int maxNumberOfDocuments, long maxSizeInBytes);

        Task DropCollection(IWorkContext context, string collectionName);

        DocumentCollection<TDocument> GetCollection<TDocument>(string collectionName);
    }
}
