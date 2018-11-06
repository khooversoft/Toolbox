using FluentAssertions;
using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.MongoDb.Test.AggregateTests
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

            var aggregate = new Aggregate()
                + (new Match() + (new Field(nameof(TestDocument.Index)) > 3));

            var findResult = await Utility.Collection.Aggregate(_workContext, aggregate);
            List<TestDocument> resultDocuments = findResult.ToList();
            resultDocuments.Count.Should().Be(6);

            Utility.VerifyDocuments(Utility.TestDocuments.OrderBy(x => x.Index).Skip(4), resultDocuments);
        }

        [Fact]
        public async Task TestSimpleSearchAndSort()
        {
            string lookupName = $"NotFirst_{1}";

            var aggregate = new Aggregate()
                + (new Match() + (new Field(nameof(TestDocument.Index)) > 3))
                + (new Sort() + new OrderBy(DirectionType.Ascending, nameof(TestDocument.Index)));

            var findResult = await Utility.Collection.Aggregate(_workContext, aggregate);
            List<TestDocument> resultDocuments = findResult.ToList();
            resultDocuments.Count.Should().Be(6);

            Utility.VerifyDocuments(Utility.TestDocuments.OrderBy(x => x.Index).Skip(4), resultDocuments);
        }

        [Fact]
        public async Task TestSimpleSearchAndDecending()
        {
            string lookupName = $"NotFirst_{1}";

            var aggregate = new Aggregate()
                + (new Match() + (new Field(nameof(TestDocument.Index)) > 3))
                + (new Sort() + new OrderBy(DirectionType.Descending, nameof(TestDocument.Index)));

            var findResult = await Utility.Collection.Aggregate(_workContext, aggregate);
            List<TestDocument> resultDocuments = findResult.ToList();
            resultDocuments.Count.Should().Be(6);

            Utility.VerifyDocuments(Utility.TestDocuments.OrderByDescending(x => x.Index).Take(6), resultDocuments);
        }

        [Fact]
        public async Task TestSimpleSearchAndMultipleSort()
        {
            string lookupName = $"NotFirst_{1}";

            var aggregate = new Aggregate()
                + (new Match() + (new Field(nameof(TestDocument.Index)) > 3))
                + (new Sort()
                    + new OrderBy(DirectionType.Ascending, nameof(TestDocument.Index))
                    + new OrderBy(DirectionType.Descending, nameof(TestDocument.Address1))
                    );

            var findResult = await Utility.Collection.Aggregate(_workContext, aggregate);
            List<TestDocument> resultDocuments = findResult.ToList();
            resultDocuments.Count.Should().Be(6);

            Utility.VerifyDocuments(Utility.TestDocuments.OrderBy(x => x.Index).Skip(4), resultDocuments);
        }
    }
}
