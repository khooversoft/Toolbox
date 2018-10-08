using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Khooversoft.Toolbox.Test.Tools
{
    public class ConvertToExtensionsTest
    {
        [Fact]
        public void ToKeyValuesTest()
        {
            var test = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("Name", "Hello"),
                new KeyValuePair<string, object>("Age", 10),
                new KeyValuePair<string, object>("BirthDate", new DateTime(12, 1, 10)),
            };

            IEnumerable<KeyValuePair<string, object>> list = new { Name = "Hello", Age = 10, BirthDate = new DateTime(12, 1, 10) }
                .ToKeyValues();

            list.OrderBy(x => x.Key)
                .Zip(test.OrderBy(x => x.Key), (o, i) => new { o, i })
                .All(x => x.o.Key == x.i.Key && x.o.Value.Equals(x.i.Value))
                .Should().BeTrue();
        }

        [Fact]
        public void EmptyKeyValusTest()
        {
            IEnumerable<KeyValuePair<string, object>> list = new { }
                .ToKeyValues();

            list.Should().NotBeNull();
            list.Count().Should().Be(0);
        }

        [Fact]
        public void MartializedTests()
        {
            var variations = new[]
            {
                new
                {
                    Test = new List<KeyValuePair<string, string>>
                    {
                    },
                    ShouldBe = new List<KeyValuePair<string, string>>
                    {
                    },
                },
                new
                {
                    Test = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("First", "first_1"),
                        new KeyValuePair<string, string>("First", "first_1"),
                        new KeyValuePair<string, string>("First", "first_1"),
                    },
                    ShouldBe = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("First", "first_1"),
                    },
                },
                new
                {
                    Test = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("First", "first_1"),
                        new KeyValuePair<string, string>("Second", "second_1"),
                        new KeyValuePair<string, string>("Third", "third_1"),
                    },
                    ShouldBe = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("First", "first_1"),
                        new KeyValuePair<string, string>("Second", "second_1"),
                        new KeyValuePair<string, string>("Third", "third_1"),
                    },
                },
                new
                {
                    Test = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("First", "first_1"),
                        new KeyValuePair<string, string>("Second", "second_1"),
                        new KeyValuePair<string, string>("Third", "third_1"),
                        new KeyValuePair<string, string>("Second", "second_2"),
                    },
                    ShouldBe = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("First", "first_1"),
                        new KeyValuePair<string, string>("Second", "second_2"),
                        new KeyValuePair<string, string>("Third", "third_1"),
                    },
                },
            };

            int testNumber = 0;
            foreach(var item in variations)
            {
                IEnumerable<KeyValuePair<string, string>> result = item.Test.Materialized();

                result
                    .Zip(item.ShouldBe, (o, i) => new { o, i })
                    .All(x => x.o.Key == x.i.Key && x.o.Value == x.i.Value)
                    .Should().BeTrue($"testNumber={testNumber}");

                testNumber++;
            }
        }
    }
}
