using FluentAssertions;
using Khooversoft.Toolbox;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.MongoDb.Test.DocumentTests
{
    [Trait("Category", "MongoDB")]
    public class DocumentSearchTests
    {
        private const string _dbName = "TestIndexDatabase";
        private const string _collectionName = "TestCollection";
        private readonly IDocumentServer _documentServer;
        private readonly IWorkContext _workContext = WorkContext.Empty;
        private readonly List<TestDocument> _testDocuments;
        private DateTime _tranData = DateTime.UtcNow;
        private IDocumentCollection<TestDocument> _collection;

        public DocumentSearchTests()
        {
            _documentServer = new DocumentServer(Constants.ConnectionString);

            _testDocuments = Enumerable.Range(0, 10)
                .Select(x => CreateTestDocument(x))
                .OrderBy(x => x._id)
                .ToList();

            IDocumentDatabase db = _documentServer.GetDatabase(_workContext, _dbName);
            _collection = db.GetCollection<TestDocument>(_collectionName);

            ResetDatabase()
                .GetAwaiter()
                .GetResult();
        }

        private async Task ResetDatabase()
        {
            await _documentServer.DropDatabase(_workContext, _dbName);
            await _testDocuments.RunAsync(async x => await _collection.Insert(_workContext, x));
        }

        [Fact]
        public async Task TestSimpleSearchFail()
        {
            string lookupName = $"NotFirst_{1}";

            var query = new And()
                + new Compare(CompareType.Equal, "FirstName", lookupName);

            var findResult = await _collection.Find(_workContext, query.ToDocument());
            List<TestDocument> resultDocuments = findResult.ToList();
            resultDocuments.Count.Should().Be(0);
        }

        [Fact]
        public async Task TestSimpleSearch()
        {
            string lookupName = $"First_{1}";

            var query = new And()
                + new Compare(CompareType.Equal, "FirstName", lookupName);

            var findResult = await _collection.Find(_workContext, query.ToDocument());
            List<TestDocument> resultDocuments = findResult.ToList();
            VerifyDocuments(_testDocuments.Where(x => x.Index == 1), resultDocuments);
        }

        [Fact]
        public async Task Test_1_SimpleSearch()
        {
            var query = new And()
                + (new Field(nameof(TestDocument.FirstName)) == $"First_{1}");

            var findResult = await _collection.Find(_workContext, query.ToDocument());
            List<TestDocument> resultDocuments = findResult.ToList();
            VerifyDocuments(_testDocuments.Where(x => x.Index == 1), resultDocuments);
        }

        [Fact]
        public async Task TestEqualSymbol_1_SimpleSearch()
        {
            var query = new And()
                + (new Field(nameof(TestDocument.Index)) > 5)
                + (new Field(nameof(TestDocument.Index)) < 8);

            var findResult = await _collection.Find(_workContext, query.ToDocument());
            List<TestDocument> resultDocuments = findResult.ToList();
            VerifyDocuments(_testDocuments.Where(x => x.Index > 5 && x.Index < 8), resultDocuments);
        }

        [Fact]
        public async Task TestEqualSymbol_2_SimpleSearch()
        {
            var query = new And()
                + (new Field(nameof(TestDocument.Index)) >= 5 )
                + (new Field(nameof(TestDocument.Index)) <= 8);

            var findResult = await _collection.Find(_workContext, query.ToDocument());
            List<TestDocument> resultDocuments = findResult.ToList();
            VerifyDocuments(_testDocuments.Where(x => x.Index >= 5 && x.Index <= 8), resultDocuments);
        }

        [Fact]
        public async Task TestEqualSymbol_3_SimpleSearch()
        {
            var query = new And()
                + (new Field(nameof(TestDocument.Index)) >= 5 <= 8);

            var findResult = await _collection.Find(_workContext, query.ToDocument());
            List<TestDocument> resultDocuments = findResult.ToList();
            VerifyDocuments(_testDocuments.Where(x => x.Index >= 5 && x.Index <= 8), resultDocuments);
        }

        [Fact]
        public async Task TestDate_1_SimpleSearch()
        {
            var query = new And()
                + new Compare(CompareType.Equal, nameof(TestDocument.TranDate), _tranData);

            var findResult = await _collection.Find(_workContext, query.ToDocument());
            List<TestDocument> resultDocuments = findResult.ToList();
            VerifyDocuments(_testDocuments.Where(x => x.TranDate == _tranData), resultDocuments);
        }

        [Fact]
        public async Task TestDate_2_SimpleSearch()
        {
            var query = new And()
                + (new Field(nameof(TestDocument.TranDate)) == _tranData);

            var findResult = await _collection.Find(_workContext, query.ToDocument());
            List<TestDocument> resultDocuments = findResult.ToList();
            VerifyDocuments(_testDocuments.Where(x => x.TranDate == _tranData), resultDocuments);
        }

        [Fact]
        public async Task AndSimpleSearch()
        {
            string lookupName = $"First_{1}";
            string secondName = $"Last_{1}";

            var query = new And()
                + new Compare(CompareType.Equal, "FirstName", lookupName)
                + new Compare(CompareType.Equal, "LastName", secondName);

            var findResult = await _collection.Find(_workContext, query.ToDocument());
            List<TestDocument> resultDocuments = findResult.ToList();
            VerifyDocuments(_testDocuments.Where(x => x.Index == 1), resultDocuments);
        }

        [Fact]
        public async Task OrSimpleSearch()
        {
            var query = new Or()
                + (new And() + new Compare(CompareType.Equal, "FirstName", $"First_{1}"))
                + (new And() + new Compare(CompareType.Equal, "Address1", $"Addr1_{2}"));

            BsonDocument queryBson = query.ToDocument();

            var findResult = await _collection.Find(_workContext, queryBson);
            List<TestDocument> resultDocuments = findResult.ToList();
            VerifyDocuments(_testDocuments.Where(x => x.Index == 1 || x.Index == 2), resultDocuments);
        }

        [Fact]
        public async Task GreaterThanSimpleSearch()
        {
            var query = new And()
                + new Compare(CompareType.GreaterThen, nameof(TestDocument.Index), 5);

            BsonDocument queryBson = query.ToDocument();

            var findResult = await _collection.Find(_workContext, queryBson);
            List<TestDocument> resultDocuments = findResult.ToList();
            VerifyDocuments(_testDocuments.Where(x => x.Index > 5), resultDocuments);
        }

        [Fact]
        public async Task GreaterThanEqualSimpleSearch()
        {
            var query = new And()
                + new Compare(CompareType.GreaterThenEqual, nameof(TestDocument.Index), 5);

            BsonDocument queryBson = query.ToDocument();

            var findResult = await _collection.Find(_workContext, queryBson);
            List<TestDocument> resultDocuments = findResult.ToList();
            VerifyDocuments(_testDocuments.Where(x => x.Index >= 5), resultDocuments);
        }

        [Fact]
        public async Task LessThanSimpleSearch()
        {
            var query = new And()
                + new Compare(CompareType.LessThen, nameof(TestDocument.Index), 4);

            BsonDocument queryBson = query.ToDocument();

            var findResult = await _collection.Find(_workContext, queryBson);
            List<TestDocument> resultDocuments = findResult.ToList();
            VerifyDocuments(_testDocuments.Where(x => x.Index < 4), resultDocuments);
        }

        [Fact]
        public async Task LessThanEqualSimpleSearch()
        {
            var query = new And()
                + new Compare(CompareType.LessThenEqual, nameof(TestDocument.Index), 4);

            BsonDocument queryBson = query.ToDocument();

            var findResult = await _collection.Find(_workContext, queryBson);
            List<TestDocument> resultDocuments = findResult.ToList();
            VerifyDocuments(_testDocuments.Where(x => x.Index <= 4), resultDocuments);
        }

        private TestDocument CreateTestDocument(int index)
        {
            DateTime tt = _tranData.AddDays(index);

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
                TranDate = tt,
            };
        }

        private void VerifyDocuments(IEnumerable<TestDocument> sourceDocuments, IEnumerable<TestDocument> resultDocuments)
        {
            int docIndex = 0;
            var sourceList = sourceDocuments.OrderBy(x => x.Index).ToList();
            var resultList = resultDocuments.OrderBy(x => x.Index).ToList();
            sourceList.Count.Should().Be(resultList.Count);

            foreach (var result in resultDocuments)
            {
                var test = sourceList[docIndex++];

                result.FirstName.Should().Be(test.FirstName);
                result.LastName.Should().Be(test.LastName);
                result.Birthdate.ToString("s").Should().Be(test.Birthdate.ToString("s"));
                result.Address1.Should().Be(test.Address1);
                result.Address2.Should().Be(test.Address2);
                result.City.Should().Be(test.City);
                result.State.Should().Be(test.State);
                result.ZipCode.Should().Be(test.ZipCode);
                result.TranDate.Should().Be(test.TranDate);
            }
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

            [BsonDateTimeOptions(Kind = DateTimeKind.Utc, Representation = BsonType.Int64)]
            public DateTime TranDate { get; set; }
        }
    }
}
