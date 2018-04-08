using FluentAssertions;
using Khooversoft.MongoDb.Models.V1;
using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.MongoDb.Test.DatabaseTests
{
    [Trait("Category", "Toolbox")]
    public class CreateRemoveDatabaseTests
    {
        private readonly IDocumentServer _documentServer;
        private readonly IWorkContext _workContext = WorkContext.Empty;

        public CreateRemoveDatabaseTests()
        {
            _documentServer = new DocumentServer(Constants.ConnectionString);
        }

        [Fact]
        public async Task CreateAndRemoveDatabaseTest()
        {
            const int count = 10;

            IEnumerable<DatabaseDetailV1> result = await _documentServer.ListDatabases(_workContext);
            result.Should().NotBeNull();
            result.Count().Should().BeGreaterThan(1);

            IList<string> dbList = Enumerable.Range(0, count).Select(x => $"Database_{x}").ToList();
            IEnumerable<string> dbToDelete = result.Where(x => dbList.Contains(x.Name)).Select(x => x.Name).ToList();

            foreach (var dbName in dbToDelete)
            {
                await _documentServer.DropDatabase(_workContext, dbName);
            }

            IEnumerable<DatabaseDetailV1> baseResult = await _documentServer.ListDatabases(_workContext);

            foreach (var dbName in dbList)
            {
                IDocumentDatabase db = _documentServer.GetDatabase(_workContext, dbName);
                await db.CreateCollection(_workContext, "TestCollection");
            }

            IEnumerable<DatabaseDetailV1> afterGetDatbase = await _documentServer.ListDatabases(_workContext);

            IEnumerable<DatabaseDetailV1> afterFilter = afterGetDatbase.Where(x => !baseResult.Any(y => y.Name.Equals(x.Name, StringComparison.OrdinalIgnoreCase)));
            afterFilter.Count().Should().Be(count);

            afterFilter
                .OrderBy(x => x.Name)
                .Zip(dbList.OrderBy(x => x), (f, s) => new { F = f, S = s })
                .All(x => x.F.Name == x.S)
                .Should().BeTrue();

            foreach (var dbName in dbList)
            {
                await _documentServer.DropDatabase(_workContext, dbName);
            }

            IEnumerable<DatabaseDetailV1> afterDelete = await _documentServer.ListDatabases(_workContext);
            afterDelete.Count().Should().Be(baseResult.Count());
        }

        [Fact]
        public async Task CreateAndRemoveCollectionsTest()
        {
            const int count = 10;
            const string dbName = "TestDatabase_01";

            IList<string> collectionList = Enumerable.Range(0, count).Select(x => $"Collection_{x}").ToList();

            await _documentServer.DropDatabase(_workContext, dbName);
            IDocumentDatabase db = _documentServer.GetDatabase(_workContext, dbName);

            IEnumerable<CollectionDetailV1> collections = await db.ListCollections(_workContext);
            collections.Should().NotBeNull();
            collections.Count().Should().Be(0);

            foreach (var collectionName in collectionList)
            {
                await db.CreateCollection(_workContext, collectionName);
            }

            collections = await db.ListCollections(_workContext);
            collections.Should().NotBeNull();
            collections.Count().Should().Be(count);

            collections
                .OrderBy(x => x.Name)
                .Zip(collectionList.OrderBy(x => x), (f, s) => new { F = f, S = s })
                .All(x => x.F.Name == x.S)
                .Should().BeTrue();

            foreach (var collectionName in collectionList)
            {
                await db.DropCollection(_workContext, collectionName);
            }

            collections = await db.ListCollections(_workContext);
            collections.Should().NotBeNull();
            collections.Count().Should().Be(0);

            await _documentServer.DropDatabase(_workContext, dbName);
        }
    }
}
