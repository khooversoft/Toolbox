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
    public class CollectionIndexTests
    {
        private static readonly string _dbName = $"TestIndexDatabase_{nameof(CollectionIndexTests)}";
        private const string _collectionName = "TestCollection";
        private readonly IDocumentServer _documentServer;
        private readonly IWorkContext _workContext = WorkContext.Empty;

        public CollectionIndexTests()
        {
            _documentServer = new DocumentServer(Constants.ConnectionString);
        }

        [Fact]
        public async Task CreateSingleIndexTest()
        {
            await _documentServer.DropDatabase(_workContext, _dbName);
            IDocumentDatabase db = _documentServer.GetDatabase(_workContext, _dbName);
            await db.CreateCollection(_workContext, _collectionName);

            IDocumentCollection<TestDocument> collection = db.GetCollection<TestDocument>(_collectionName);

            IEnumerable<IndexDetailV1> list = await collection.Index.ListIndexes(_workContext);
            list.Should().NotBeNull();
            list.Count().Should().Be(1);

            list.First().Name.Should().Be("_id_");

            var model = new CollectionIndex
            {
                Name = "TestIndex_1",
                Unique = true,
                Sparse = false,
                Keys = new List<IndexKey>
                {
                    new IndexKey { FieldName = "FirstName" },
                    new IndexKey { FieldName = "Last Name" },
                }
            };

            await collection.Index.CreateIndex(_workContext, model);

            list = await collection.Index.ListIndexes(_workContext);
            list.Should().NotBeNull();
            list.Count().Should().Be(2);

            list.First().Name.Should().Be("_id_");

            IndexDetailV1 detail = list.Skip(1).First();
            detail.Name.Should().Be(model.Name);
            detail.Unique.Should().Be(model.Unique);
            detail.Sparse.Should().Be(model.Sparse);
            detail.Keys.Count.Should().Be(model.Keys.Count);
            detail.Keys[0].FieldName.Should().Be(model.Keys[0].FieldName);
            detail.Keys[0].Descending.Should().Be(model.Keys[0].Descending);
            detail.Keys[1].FieldName.Should().Be(model.Keys[1].FieldName);
            detail.Keys[1].Descending.Should().Be(model.Keys[1].Descending);

            await db.DropCollection(_workContext, _collectionName);
            await _documentServer.DropDatabase(_workContext, _dbName);
        }

        [Fact]
        public async Task RemoveSingleIndexTest()
        {
            await _documentServer.DropDatabase(_workContext, _dbName);
            IDocumentDatabase db = _documentServer.GetDatabase(_workContext, _dbName);
            await db.CreateCollection(_workContext, _collectionName);

            IDocumentCollection<TestDocument> collection = db.GetCollection<TestDocument>(_collectionName);

            IEnumerable<IndexDetailV1> list = await collection.Index.ListIndexes(_workContext);
            list.Should().NotBeNull();
            list.Count().Should().Be(1);

            list.First().Name.Should().Be("_id_");

            var model = new CollectionIndex
            {
                Name = "TestIndex_1",
                Unique = true,
                Sparse = false,
                Keys = new List<IndexKey>
                {
                    new IndexKey { FieldName = "FirstName" },
                    new IndexKey { FieldName = "Last Name" },
                }
            };

            await collection.Index.CreateIndex(_workContext, model);

            list = await collection.Index.ListIndexes(_workContext);
            list.Should().NotBeNull();
            list.Count().Should().Be(2);

            list.First().Name.Should().Be("_id_");
            list.Skip(1).First().Name.Should().Be(model.Name);

            await collection.Index.DropIndex(_workContext, model.Name);

            list = await collection.Index.ListIndexes(_workContext);
            list.Should().NotBeNull();
            list.Count().Should().Be(1);

            list.First().Name.Should().Be("_id_");

            await db.DropCollection(_workContext, _collectionName);
            await _documentServer.DropDatabase(_workContext, _dbName);
        }

        private class TestDocument
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }

            public DateTime Birthdate { get; set; }

            public string Address1 { get; set; }

            public string Address2 { get; set; }

            public string City { get; set; }

            public string State { get; set; }

            public string ZipCode { get; set; }
        }
    }
}
