using FluentAssertions;
using Khooversoft.Toolbox;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.MongoDb.Test.InstructionTests
{
    [Trait("Category", "MongoDB")]
    //[Collection("DB tests")]
    public class ProjectionTests
    {
        private readonly IWorkContext _workContext = WorkContext.Empty;

        public ProjectionTests()
        {
            Utility.Initialize();
        }

        [Fact]
        public async Task SimpleProjectionTest()
        {
            string lookupName = $"NotFirst_{1}";

            var query = new Pipeline()
                + new Projection(nameof(TestDocument.FirstName).ToEnumerable());

            var findResult = await Utility.Collection.Find(_workContext, query);
            List<TestDocument> resultDocuments = findResult.ToList();
            resultDocuments.Should().NotBeNull();
            resultDocuments.Count.Should().Be(Utility.Count);

            resultDocuments
                .All(x => x.LastName == null)
                .Should().BeTrue();
        }

        [Fact]
        public async Task ChangeTypeProjectionTest()
        {
            string lookupName = $"NotFirst_{1}";

            var query = new Pipeline()
                + new Projection(nameof(TestDocument.FirstName).ToEnumerable());

            var findResult = await Utility.Database.GetCollection<ReducedTestDocument>(Utility.CollectionName).Find(_workContext, query);
            List<ReducedTestDocument> resultDocuments = findResult.ToList();
            resultDocuments.Should().NotBeNull();
            resultDocuments.Count.Should().Be(Utility.Count);

            resultDocuments
                .All(x => x.LastName == null)
                .Should().BeTrue();
        }

        [Fact]
        public async Task BsonDocumentTypeProjectionTest()
        {
            string lookupName = $"NotFirst_{1}";

            var query = new Pipeline()
                + new Projection(new string[] { nameof(TestDocument.FirstName), nameof(TestDocument.LastName) })
                + new OrderBy(DirectionType.Descending, nameof(TestDocument.FirstName));

            var findResult = await Utility.Database.GetCollection<BsonDocument>(Utility.CollectionName).Find(_workContext, query);
            List<BsonDocument> resultDocuments = findResult.ToList();
            resultDocuments.Should().NotBeNull();
            resultDocuments.Count.Should().Be(Utility.Count);

            int index = 0;
            foreach(var item in Utility.TestDocuments.OrderByDescending(x => x.Index))
            {
                BsonValue value = resultDocuments[index++][nameof(TestDocument.FirstName)];
                value.Should().NotBeNull();
                value.AsString.Should().Be(item.FirstName);
            }
        }

        private class ReducedTestDocument
        {
            public ObjectId _id { get; set; }

            public int Index { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }
        }
    }
}
