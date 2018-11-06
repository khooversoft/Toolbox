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
        private readonly IWorkContext _workContext = WorkContext.Empty;

        public DocumentSearchAdvanceTests()
        {
            Utility.Initialize();
        }

        [Fact]
        public async Task TestSimpleSearchSuccess()
        {
            var pipeline = new Pipeline()
                + (new Query() + (new Field(nameof(TestDocument.Index)) > 3))
                + new OrderBy(DirectionType.Ascending, nameof(TestDocument.Index));

            var findResult = await Utility.Collection.Find(_workContext, pipeline);
            List<TestDocument> resultDocuments = findResult.ToList();
            resultDocuments.Count.Should().Be(6);

            Utility.VerifyDocuments(Utility.TestDocuments.OrderBy(x => x.Index).Skip(4), resultDocuments);
        }

        [Fact]
        public async Task TestSimpleSearchDesendingSuccess()
        {
            var pipeline = new Pipeline()
                + (new Query() + (new Field(nameof(TestDocument.Index)) > 3))
                + new OrderBy(DirectionType.Descending, nameof(TestDocument.Index));

            var findResult = await Utility.Collection.Find(_workContext, pipeline);
            List<TestDocument> resultDocuments = findResult.ToList();
            resultDocuments.Count.Should().Be(6);

            Utility.VerifyDocuments(Utility.TestDocuments.OrderByDescending(x => x.Index).Take(6), resultDocuments, orderData: false);
        }

        [Fact]
        public void TestSimpleSearchMultipleSortFail()
        {
            var pipeline = new Pipeline()
                + (new Query() + (new Field(nameof(TestDocument.Index)) > 3))
                + new OrderBy(DirectionType.Ascending, nameof(TestDocument.Index))
                + new OrderBy(DirectionType.Ascending, nameof(TestDocument.Address1));

            Func<Task<IEnumerable<TestDocument>>> act = async () => await Utility.Collection.Find(_workContext, pipeline);
            act.Should().Throw<InvalidOperationException>();
        }
    }
}
