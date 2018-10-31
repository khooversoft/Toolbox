using FluentAssertions;
using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.MongoDb.Test.Aggregate
{
    [Trait("Category", "MongoDB")]
    public class AggregateTests
    {
        private readonly IWorkContext _workContext = WorkContext.Empty;

        public AggregateTests()
        {
            Utility.Initialize();
        }

        [Fact]
        public async Task TestSimpleSearch()
        {
            string lookupName = $"NotFirst_{1}";

            var pipeline = new Pipeline()
                + (new Query() + (new Field(nameof(TestDocument.Index)) > 3))
                + new OrderBy(DirectionType.Ascending, nameof(TestDocument.Index));

            var findResult = await Utility.Collection.Find(_workContext, pipeline);
            List<TestDocument> resultDocuments = findResult.ToList();
            resultDocuments.Count.Should().Be(6);

            Utility.VerifyDocuments(Utility.TestDocuments.OrderBy(x => x.Index).Skip(4), resultDocuments);
        }
    }
}
