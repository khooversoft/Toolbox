using Khooversoft.MongoDb.Models.V1;
using Khooversoft.Toolbox;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    public class DocumentDatabase : IDocumentDatabase
    {
        private readonly Toolbox.Tag _tag = new Toolbox.Tag(nameof(DocumentDatabase));

        public DocumentDatabase(IDocumentServer documentServer, IMongoDatabase mongoDatabase)
        {
            Verify.IsNotNull(nameof(documentServer), documentServer);
            Verify.IsNotNull(nameof(mongoDatabase), mongoDatabase);

            DocumentServer = documentServer;
            MongoDatabase = mongoDatabase;
        }

        public DocumentDatabase(string connectionString)
        {
            Verify.IsNotNull(nameof(connectionString), connectionString);

            var configuration = new DatabaseConfigurationBuilder(connectionString);

            DocumentServer = new DocumentServer(configuration.Url);
            MongoDatabase = DocumentServer.Client.GetDatabase(configuration.DatabaseName);
        }

        public string ConnectionString { get; }

        public IDocumentServer DocumentServer { get; }

        public IMongoDatabase MongoDatabase { get; }

        public async Task<IEnumerable<CollectionDetailV1>> ListCollections(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);

            using (IAsyncCursor<BsonDocument> cursor = await MongoDatabase.ListCollectionsAsync(options: null, cancellationToken: context.CancellationToken))
            {
                var resultList = cursor.ToList();

                return resultList
                    .Select(x => BsonSerializer.Deserialize<InternalCollectionDetail>(x))
                    .Select(x => x.ConvertTo())
                    .ToList();
            }
        }

        public async Task<CollectionDetailV1> GetCollectionDetail(IWorkContext context, string collectionName)
        {
            Verify.IsNotEmpty(nameof(collectionName), collectionName);

            return (await ListCollections(context))
                .FirstOrDefault(x => x.Name == collectionName);
        }

        public async Task<bool> CollectionExist(IWorkContext context, string collectionName)
        {
            Verify.IsNotEmpty(nameof(collectionName), collectionName);

            return (await ListCollections(context))
                .Any(x => x.Name == collectionName);
        }

        public async Task CreateCollection(IWorkContext context, string collectionName)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotEmpty(nameof(collectionName), collectionName);
            context = context.WithTag(_tag);

            MongoDbEventSource.Log.Info(context, $"Creating collection {collectionName}");
            await MongoDatabase.CreateCollectionAsync(collectionName, options: null, cancellationToken: context.CancellationToken);
        }

        public async Task CreateCappedCollection(IWorkContext context, string collectionName, int maxNumberOfDocuments, long maxSizeInBytes)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotEmpty(nameof(collectionName), collectionName);
            Verify.Assert<ArgumentOutOfRangeException>(maxNumberOfDocuments > 0, nameof(maxNumberOfDocuments));
            Verify.Assert<ArgumentOutOfRangeException>(maxSizeInBytes > 0, nameof(maxSizeInBytes));

            context = context.WithTag(_tag);

            var options = new CreateCollectionOptions
            {
                Capped = true,
                MaxDocuments = maxNumberOfDocuments,
                MaxSize = maxSizeInBytes
            };

            MongoDbEventSource.Log.Info(context, $"Creating capped collection {collectionName}");
            await MongoDatabase.CreateCollectionAsync(collectionName, options: options, cancellationToken: context.CancellationToken);
        }

        public async Task DropCollection(IWorkContext context, string collectionName)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotEmpty(nameof(collectionName), collectionName);
            context = context.WithTag(_tag);

            MongoDbEventSource.Log.Info(context, $"Dropping collection {collectionName}");
            await MongoDatabase.DropCollectionAsync(collectionName, context.CancellationToken);
        }

        public DocumentCollection<TDocument> GetCollection<TDocument>(string collectionName)
        {
            return new DocumentCollection<TDocument>(MongoDatabase.GetCollection<TDocument>(collectionName), collectionName);
        }
    }
}
