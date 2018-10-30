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
                + new Projection("FirstName".ToEnumerable());

            var findResult = await Utility.Collection.Find(_workContext, query);
            List<TestDocument> resultDocuments = findResult.ToList();
            resultDocuments.Should().NotBeNull();
            resultDocuments.Count.Should().Be(Utility.Count);

            resultDocuments
                .All(x => x.LastName == null)
                .Should().BeTrue();
        }
    }
}
