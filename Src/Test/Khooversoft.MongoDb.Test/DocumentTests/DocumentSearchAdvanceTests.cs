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
    public class DocumentSearchAdvanceTests
    {
        private const string _dbName = "TestIndexDatabase";
        private const string _collectionName = "TestCollection";
        private const int _maxRowCount = 10;
        private readonly IDocumentServer _documentServer;
        private readonly IWorkContext _workContext = WorkContext.Empty;
        private readonly List<TestDocument> _testDocuments;
        private DateTime _tranData = DateTime.UtcNow;
        private IDocumentCollection<TestDocument> _collection;

        public DocumentSearchAdvanceTests()
        {
            _documentServer = new DocumentServer(Constants.ConnectionString);

            _testDocuments = Enumerable.Range(0, _maxRowCount)
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
            DateTime searchDate = _tranData.AddDays(4);

            //var command = new Command()
            //    + new Query()
            var query = new And()
                + (new Field(nameof(TestDocument.TranDate)) > searchDate)
                + new OrderBy(DirectionType.Ascending, nameof(TestDocument.Index));

            var findResult = await _collection.Find(_workContext, query.ToDocument());
            List<TestDocument> resultDocuments = findResult.ToList();
            resultDocuments.Count.Should().Be(0);
        }

        private TestDocument CreateTestDocument(int index)
        {
            DateTime tt = _tranData.AddDays(index);

            return new TestDocument
            {
                Index = _maxRowCount - index,
                FirstName = $"First_{index}",
                LastName = $"Last_{index}",
                Birthdate = DateTime.UtcNow,
                Address1 = $"Addr1_{index}",
                Address2 = $"Addr2_{index}",
                City = $"City_{index}",
                State = $"State_{index}",
                ZipCode = $"Zip_{index}",
                TranDate = tt,
                GuidKey = Guid.NewGuid(),
                LockedDate = index % 3 == 0 ? tt : (DateTime?)null,
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
                result.GuidKey.Should().Be(test.GuidKey);
                result.LockedDate.Should().Be(test.LockedDate);
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

            public Guid GuidKey { get; set; }

            [BsonDateTimeOptions(Kind = DateTimeKind.Utc, Representation = BsonType.Int64)]
            public DateTime? LockedDate { get; set; }
        }
    }
}
