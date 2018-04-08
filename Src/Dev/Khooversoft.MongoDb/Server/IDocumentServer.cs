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
    public interface IDocumentServer
    {
        string ConnectionString { get; }

        MongoClient Client { get; }

        Task<IEnumerable<DatabaseDetailV1>> ListDatabases(IWorkContext context);

        Task<bool> DatabaseExist(IWorkContext context, string dbName);

        Task DropDatabase(IWorkContext context, string dbName);

        DocumentDatabase GetDatabase(IWorkContext context, string dbName);

        Task<DatabaseDetailV1> GetDatabaseDetail(IWorkContext context, string dbName);
    }
}
