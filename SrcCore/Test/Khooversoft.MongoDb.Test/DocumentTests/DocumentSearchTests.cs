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
    //[Collection("DB tests")]
    public class DocumentSearchTests
    {
        private readonly IWorkContext _workContext = WorkContext.Empty;

        public DocumentSearchTests()
        {
            Utility.Initialize();
        }

        [Fact]
        public async Task TestSimpleSearchFail()
        {
            string lookupName = $"NotFirst_{1}";

            var query = new And()
                + new Compare(CompareType.Equal, "FirstName", lookupName);

            var findResult = await Utility.Collection.Find(_workContext, query.ToDocument());
            List<TestDocument> resultDocuments = findResult.ToList();
            resultDocuments.Count.Should().Be(0);
        }

        [Fact]
        public async Task TestSimpleSearch()
        {
            string lookupName = $"First_{1}";

            var query = new And()
                + new Compare(CompareType.Equal, "FirstName", lookupName);

            var findResult = await Utility.Collection.Find(_workContext, query.ToDocument());
            List<TestDocument> resultDocuments = findResult.ToList();
            Utility.VerifyDocuments(Utility.TestDocuments.Where(x => x.Index == 1), resultDocuments);
        }

        [Fact]
        public async Task Test_1_SimpleSearch()
        {
            var query = new And()
                + (new Field(nameof(TestDocument.FirstName)) == $"First_{1}");

            var findResult = await Utility.Collection.Find(_workContext, query.ToDocument());
            List<TestDocument> resultDocuments = findResult.ToList();
            Utility.VerifyDocuments(Utility.TestDocuments.Where(x => x.Index == 1), resultDocuments);
        }

        [Fact]
        public async Task TestEqualSymbol_1_SimpleSearch()
        {
            var query = new And()
                + (new Field(nameof(TestDocument.Index)) > 5)
                + (new Field(nameof(TestDocument.Index)) < 8);

            var findResult = await Utility.Collection.Find(_workContext, query.ToDocument());
            List<TestDocument> resultDocuments = findResult.ToList();
            Utility.VerifyDocuments(Utility.TestDocuments.Where(x => x.Index > 5 && x.Index < 8), resultDocuments);
        }

        [Fact]
        public async Task TestEqualSymbol_2_SimpleSearch()
        {
            var query = new And()
                + (new Field(nameof(TestDocument.Index)) >= 5 )
                + (new Field(nameof(TestDocument.Index)) <= 8);

            var findResult = await Utility.Collection.Find(_workContext, query.ToDocument());
            List<TestDocument> resultDocuments = findResult.ToList();
            Utility.VerifyDocuments(Utility.TestDocuments.Where(x => x.Index >= 5 && x.Index <= 8), resultDocuments);
        }

        [Fact]
        public async Task TestEqualSymbol_3_SimpleSearch()
        {
            var query = new And()
                + (new Field(nameof(TestDocument.Index)) >= 5 <= 8);

            var findResult = await Utility.Collection.Find(_workContext, query.ToDocument());
            List<TestDocument> resultDocuments = findResult.ToList();
            Utility.VerifyDocuments(Utility.TestDocuments.Where(x => x.Index >= 5 && x.Index <= 8), resultDocuments);
        }

        [Fact]
        public async Task TestDate_1_SimpleSearch()
        {
            var query = new And()
                + new Compare(CompareType.Equal, nameof(TestDocument.TranDate), Utility.TranData);

            var findResult = await Utility.Collection.Find(_workContext, query.ToDocument());
            List<TestDocument> resultDocuments = findResult.ToList();
            Utility.VerifyDocuments(Utility.TestDocuments.Where(x => x.TranDate == Utility.TranData), resultDocuments);
        }

        [Fact]
        public async Task TestDate_2_SimpleSearch()
        {
            var query = new And()
                + (new Field(nameof(TestDocument.TranDate)) == Utility.TranData);

            var findResult = await Utility.Collection.Find(_workContext, query.ToDocument());
            List<TestDocument> resultDocuments = findResult.ToList();
            Utility.VerifyDocuments(Utility.TestDocuments.Where(x => x.TranDate == Utility.TranData), resultDocuments);
        }

        [Fact]
        public async Task TestDate_3_SimpleSearch()
        {
            DateTime searchDate = Utility.TranData.AddDays(4);

            var query = new And()
                + (new Field(nameof(TestDocument.TranDate)) <= searchDate);

            var findResult = await Utility.Collection.Find(_workContext, query.ToDocument());
            List<TestDocument> resultDocuments = findResult.ToList();
            Utility.VerifyDocuments(Utility.TestDocuments.Where(x => x.TranDate <= searchDate), resultDocuments);
        }

        [Fact]
        public async Task TestDate_4_SimpleSearch()
        {
            DateTime searchDate = Utility.TranData.AddDays(4);

            var query = new And()
                + (new Field(nameof(TestDocument.TranDate)) > searchDate);

            var findResult = await Utility.Collection.Find(_workContext, query.ToDocument());
            List<TestDocument> resultDocuments = findResult.ToList();
            Utility.VerifyDocuments(Utility.TestDocuments.Where(x => x.TranDate > searchDate), resultDocuments);
        }

        [Fact]
        public async Task AndSimpleSearch()
        {
            string lookupName = $"First_{1}";
            string secondName = $"Last_{1}";

            var query = new And()
                + new Compare(CompareType.Equal, "FirstName", lookupName)
                + new Compare(CompareType.Equal, "LastName", secondName);

            var findResult = await Utility.Collection.Find(_workContext, query.ToDocument());
            List<TestDocument> resultDocuments = findResult.ToList();
            Utility.VerifyDocuments(Utility.TestDocuments.Where(x => x.Index == 1), resultDocuments);
        }

        [Fact]
        public async Task OrSimpleSearch()
        {
            var query = new Or()
                + (new And() + new Compare(CompareType.Equal, "FirstName", $"First_{1}"))
                + (new And() + new Compare(CompareType.Equal, "Address1", $"Addr1_{2}"));

            BsonDocument queryBson = query.ToDocument();

            var findResult = await Utility.Collection.Find(_workContext, queryBson);
            List<TestDocument> resultDocuments = findResult.ToList();
            Utility.VerifyDocuments(Utility.TestDocuments.Where(x => x.Index == 1 || x.Index == 2), resultDocuments);
        }

        [Fact]
        public async Task GreaterThanSimpleSearch()
        {
            var query = new And()
                + new Compare(CompareType.GreaterThen, nameof(TestDocument.Index), 5);

            BsonDocument queryBson = query.ToDocument();

            var findResult = await Utility.Collection.Find(_workContext, queryBson);
            List<TestDocument> resultDocuments = findResult.ToList();
            Utility.VerifyDocuments(Utility.TestDocuments.Where(x => x.Index > 5), resultDocuments);
        }

        [Fact]
        public async Task GreaterThanEqualSimpleSearch()
        {
            var query = new And()
                + new Compare(CompareType.GreaterThenEqual, nameof(TestDocument.Index), 5);

            BsonDocument queryBson = query.ToDocument();

            var findResult = await Utility.Collection.Find(_workContext, queryBson);
            List<TestDocument> resultDocuments = findResult.ToList();
            Utility.VerifyDocuments(Utility.TestDocuments.Where(x => x.Index >= 5), resultDocuments);
        }

        [Fact]
        public async Task LessThanSimpleSearch()
        {
            var query = new And()
                + new Compare(CompareType.LessThen, nameof(TestDocument.Index), 4);

            BsonDocument queryBson = query.ToDocument();

            var findResult = await Utility.Collection.Find(_workContext, queryBson);
            List<TestDocument> resultDocuments = findResult.ToList();
            Utility.VerifyDocuments(Utility.TestDocuments.Where(x => x.Index < 4), resultDocuments);
        }

        [Fact]
        public async Task LessThanEqualSimpleSearch()
        {
            var query = new And()
                + new Compare(CompareType.LessThenEqual, nameof(TestDocument.Index), 4);

            BsonDocument queryBson = query.ToDocument();

            var findResult = await Utility.Collection.Find(_workContext, queryBson);
            List<TestDocument> resultDocuments = findResult.ToList();
            Utility.VerifyDocuments(Utility.TestDocuments.Where(x => x.Index <= 4), resultDocuments);
        }

        [Fact]
        public async Task SearchFor_1_GuidSearch()
        {
            Guid searchForKey = Utility.TestDocuments.Skip(2).First().GuidKey;

            var query = new And()
                + new Compare(CompareType.Equal, nameof(TestDocument.GuidKey), searchForKey);

            BsonDocument queryBson = query.ToDocument();

            var findResult = await Utility.Collection.Find(_workContext, queryBson);
            List<TestDocument> resultDocuments = findResult.ToList();
            Utility.VerifyDocuments(Utility.TestDocuments.Where(x => x.GuidKey == searchForKey), resultDocuments);
        }

        [Fact]
        public async Task SearchFor_2_GuidSearch()
        {
            Guid searchForKey = Utility.TestDocuments.Skip(4).First().GuidKey;

            var query = new And()
                + (new Field(nameof(TestDocument.GuidKey)) == searchForKey);

            BsonDocument queryBson = query.ToDocument();

            var findResult = await Utility.Collection.Find(_workContext, queryBson);
            List<TestDocument> resultDocuments = findResult.ToList();
            Utility.VerifyDocuments(Utility.TestDocuments.Where(x => x.GuidKey == searchForKey), resultDocuments);
        }

        [Fact]
        public async Task LockedDate_IsNull_GuidSearch()
        {
            var query = new And()
                + (new Field(nameof(TestDocument.LockedDate)) == null);

            BsonDocument queryBson = query.ToDocument();

            var findResult = await Utility.Collection.Find(_workContext, queryBson);
            List<TestDocument> resultDocuments = findResult.ToList();
            Utility.VerifyDocuments(Utility.TestDocuments.Where(x => x.LockedDate == null), resultDocuments);
        }

        [Fact]
        public async Task LockedDate_IsNotNull_GuidSearch()
        {
            var query = new And()
                + (new Field(nameof(TestDocument.LockedDate)) != null);

            BsonDocument queryBson = query.ToDocument();

            var findResult = await Utility.Collection.Find(_workContext, queryBson);
            List<TestDocument> resultDocuments = findResult.ToList();
            Utility.VerifyDocuments(Utility.TestDocuments.Where(x => x.LockedDate != null), resultDocuments);
        }

        [Fact]
        public async Task LockedDate_IsNotNull_AndEqual_GuidSearch()
        {
            DateTime tt = Utility.TranData.AddDays(3);

            var query = new And()
                + (new Field(nameof(TestDocument.LockedDate)) == tt);

            BsonDocument queryBson = query.ToDocument();

            var findResult = await Utility.Collection.Find(_workContext, queryBson);
            List<TestDocument> resultDocuments = findResult.ToList();
            Utility.VerifyDocuments(Utility.TestDocuments.Where(x => x.LockedDate == tt), resultDocuments);
        }
    }
}
