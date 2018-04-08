using Khooversoft.MongoDb.Models.V1;
using Khooversoft.Toolbox;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    public class DocumentServer : IDocumentServer
    {
        public DocumentServer(string connectionString)
        {
            Verify.IsNotEmpty(nameof(connectionString), connectionString);

            ConnectionString = connectionString;
            Client = new MongoClient(ConnectionString);
        }

        public string ConnectionString { get; }

        public MongoClient Client { get; private set; }

        public async Task<IEnumerable<DatabaseDetailV1>> ListDatabases(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);

            using (IAsyncCursor<BsonDocument> cursor = await Client.ListDatabasesAsync(context.CancellationToken))
            {
                var resultList = cursor.ToList();

                return resultList
                    .Select(x => BsonSerializer.Deserialize<InternalDatabaseDetail>(x))
                    .Select(x => x.ConvertTo())
                    .ToList();
            }
        }

        public async Task<bool> DatabaseExist(IWorkContext context, string dbName)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotEmpty(nameof(dbName), dbName);

            return (await ListDatabases(context))
                .Any(x => x.Name == dbName);
        }

        public async Task DropDatabase(IWorkContext context, string dbName)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotEmpty(nameof(dbName), dbName);

            await Client.DropDatabaseAsync(dbName, context.CancellationToken);
        }

        public DocumentDatabase GetDatabase(IWorkContext context, string dbName)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotEmpty(nameof(dbName), dbName);

            IMongoDatabase mongoDatabase = Client.GetDatabase(dbName);
            Verify.IsNotNull(nameof(mongoDatabase), mongoDatabase);

            return new DocumentDatabase(this, mongoDatabase);
        }

        public async Task<DatabaseDetailV1> GetDatabaseDetail(IWorkContext context, string dbName)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotEmpty(nameof(dbName), dbName);

            IEnumerable<DatabaseDetailV1> databaseList = await ListDatabases(context);
            return databaseList.FirstOrDefault(x => x.Name.Equals(dbName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
