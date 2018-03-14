using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.Toolbox.Test.Tools
{
    [Trait("Category", "Toolbox")]
    public class StringVectorTests
    {
        [Fact]
        public void StringVectorPrimaryTest()
        {
            var variations = new[]
            {
                new {
                    value = (string)null,
                    count = 0,
                    vectors = new List<string>(),
                    pass = true,
                },
                new {
                    value = "",
                    count = 0,
                    vectors = new List<string>(),
                    pass = false,
                },
                new {
                    value = "first",
                    count = 1,
                    vectors = new List<string> { "first" },
                    pass = true,
                },
                new {
                    value = @"first\second",
                    count = 1,
                    vectors = new List<string> { @"first\second" },
                    pass = true,
                },
                new {
                    value = "first/second",
                    count = 2,
                    vectors = new List<string> { "first", "second" },
                    pass = true,
                },
                new {
                    value = "first/second/third",
                    count = 3,
                    vectors = new List<string> { "first", "second", "third" },
                    pass = true,
                },
            };

            foreach (var item in variations)
            {
                StringVector sv;

                bool pass;
                try
                {
                    if (item.value == null)
                    {
                        sv = new StringVector();
                    }
                    else
                    {
                        sv = new StringVector(item.value);
                    }

                    sv.Count.Should().Be(item.count);
                    sv.Count.Should().Be(item.vectors.Count);
                    sv.Where((x, i) => item.vectors[i] == x).Count().Should().Be(item.count);

                    pass = true;
                }
                catch
                {
                    pass = false;
                }

                pass.Should().Be(item.pass);
            }
        }

        enum ETest
        {
            First,
            Second
        };

        [Fact]
        public void StringVectorParseEnumTest()
        {
            var v = new StringVector("name/First/3");
            v[0].Should().Be("name");
            ETest e = v.Get<ETest>(1);
            v.Get<int>(2).Should().Be(3);
        }
    }
}
