using FluentAssertions;
using Khooversoft.Toolbox;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.MongoDb.Test.DatabaseTests
{
    [Trait("Category", "Toolbox")]
    public class DocumentReadWriteTests
    {
        private const string _dbName = "TestIndexDatabase";
        private const string _collectionName = "TestCollection";
        private readonly IDocumentServer _documentServer;
        private readonly IWorkContext _workContext = WorkContext.Empty;

        public DocumentReadWriteTests()
        {
            _documentServer = new DocumentServer(Constants.ConnectionString);

            ResetDatabase()
                .GetAwaiter()
                .GetResult();
        }

        private async Task ResetDatabase()
        {
            await _documentServer.DropDatabase(_workContext, _dbName);
        }

        [Fact]
        public async Task CreateReadTest()
        {
            DocumentDatabase db = _documentServer.GetDatabase(_workContext, _dbName);
            IDocumentCollection<TestDocument> collection = db.GetCollection<TestDocument>(_collectionName);

            TestDocument test = CreateTestDocument(0);

            await collection.Insert(_workContext, test);

            IEnumerable<TestDocument> results = await collection.Find(_workContext, new BsonDocument());
            results.Should().NotBeNull();
            results.Count().Should().Be(1);

            TestDocument result = results.First();
            result.Should().NotBeNull();
            result.Index.Should().Be(test.Index);
            result.FirstName.Should().Be(test.FirstName);
            result.LastName.Should().Be(test.LastName);
            result.Birthdate.ToString("s").Should().Be(test.Birthdate.ToString("s"));
            result.Address1.Should().Be(test.Address1);
            result.Address2.Should().Be(test.Address2);
            result.City.Should().Be(test.City);
            result.State.Should().Be(test.State);
            result.ZipCode.Should().Be(test.ZipCode);
        }

        [Fact]
        public async Task CreateReadDeleteTest()
        {
            DocumentDatabase db = _documentServer.GetDatabase(_workContext, _dbName);
            IDocumentCollection<TestDocument> collection = db.GetCollection<TestDocument>(_collectionName);

            TestDocument test = CreateTestDocument(0);

            await collection.Insert(_workContext, test);

            IEnumerable<TestDocument> results = await collection.Find(_workContext, new BsonDocument());
            results.Should().NotBeNull();
            results.Count().Should().Be(1);

            TestDocument result = results.First();
            result.Should().NotBeNull();
            result.Index.Should().Be(test.Index);
            result.FirstName.Should().Be(test.FirstName);
            result.LastName.Should().Be(test.LastName);
            result.Birthdate.ToString("s").Should().Be(test.Birthdate.ToString("s"));
            result.Address1.Should().Be(test.Address1);
            result.Address2.Should().Be(test.Address2);
            result.City.Should().Be(test.City);
            result.State.Should().Be(test.State);
            result.ZipCode.Should().Be(test.ZipCode);

            var builder = Builders<TestDocument>.Filter;
            var filter = builder.Eq("_id", result._id);

            await collection.Delete(_workContext, filter);

            List<TestDocument> currentList = collection.MongoCollection
                .AsQueryable()
                .Where(x => x._id == result._id)
                .ToList();

            currentList.Count.Should().Be(0);
        }

        [Fact]
        public async Task CreateMultipleReadTest()
        {
            const int count = 10;

            DocumentDatabase db = _documentServer.GetDatabase(_workContext, _dbName);
            IDocumentCollection<TestDocument> collection = db.GetCollection<TestDocument>(_collectionName);

            var documentList = new List<TestDocument>();
            foreach (var index in Enumerable.Range(0, count))
            {
                TestDocument test = CreateTestDocument(index);
                await collection.Insert(_workContext, test);

                documentList.Add(test);
            }

            IEnumerable<TestDocument> results = await collection.Find(_workContext, new BsonDocument());
            results.Should().NotBeNull();
            results.Count().Should().Be(count);

            int docIndex = 0;
            foreach (var result in results.OrderBy(x => x.Index))
            {
                var test = documentList[docIndex++];

                result.FirstName.Should().Be(test.FirstName);
                result.LastName.Should().Be(test.LastName);
                result.Birthdate.ToString("s").Should().Be(test.Birthdate.ToString("s"));
                result.Address1.Should().Be(test.Address1);
                result.Address2.Should().Be(test.Address2);
                result.City.Should().Be(test.City);
                result.State.Should().Be(test.State);
                result.ZipCode.Should().Be(test.ZipCode);
            }
        }

        [Fact]
        public async Task CreateMultipleReadDeleteTest()
        {
            const int count = 10;

            DocumentDatabase db = _documentServer.GetDatabase(_workContext, _dbName);
            IDocumentCollection<TestDocument> collection = db.GetCollection<TestDocument>(_collectionName);

            var documentList = new List<TestDocument>();
            foreach (var index in Enumerable.Range(0, count))
            {
                TestDocument test = CreateTestDocument(index);
                await collection.Insert(_workContext, test);

                documentList.Add(test);
            }

            IEnumerable<TestDocument> results = await collection.Find(_workContext, new BsonDocument());
            results.Should().NotBeNull();
            results.Count().Should().Be(count);

            // Create filter to delete odd records
            var builder = Builders<TestDocument>.Filter;
            var filter = builder.In("Index", new int[] { 1, 3, 5, 7, 9 });

            await collection.Delete(_workContext, filter);

            results = await collection.Find(_workContext, new BsonDocument());
            results.Should().NotBeNull();
            results.Count().Should().Be(count - 5);

            filter = builder.Gte("Index", 0);
            await collection.Delete(_workContext, filter);

            results = await collection.Find(_workContext, new BsonDocument());
            results.Should().NotBeNull();
            results.Count().Should().Be(0);
        }

        private TestDocument CreateTestDocument(int index)
        {
            return new TestDocument
            {
                Index = index,
                FirstName = $"First_{index}",
                LastName = $"Last_{index}",
                Birthdate = DateTime.UtcNow,
                Address1 = $"Addr1_{index}",
                Address2 = $"Addr2_{index}",
                City = $"City_{index}",
                State = $"State_{index}",
                ZipCode = $"Zip_{index}",
            };
        }

        private class TestDocument
        {
            public ObjectId _id { get; set; }

            public int Index { get; set; }

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
