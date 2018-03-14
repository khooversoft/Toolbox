using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.Toolbox.Test.Extensions
{
    [Trait("Category", "Toolbox")]
    public class StringExtensionTests
    {
        [Fact]
        public void SafeSubStringTest()
        {
            var variations = new[]
            {
                new
                {
                    main = (string)null,
                    start = 0,
                    size = 0,
                    rtn = string.Empty,
                },
                new
                {
                    main = (string)null,
                    start = 0,
                    size = 5,
                    rtn = string.Empty,
                },
                new
                {
                    main = "01234567890123456789",
                    start = 0,
                    size = 20,
                    rtn = "01234567890123456789",
                },
                new
                {
                    main = "012345678901234567890",
                    start = 0,
                    size = 2,
                    rtn = "01",
                },
                new
                {
                    main = "012345678901234567890",
                    start = 0,
                    size = 10,
                    rtn = "0123456789",
                },
                new
                {
                    main = "012345678901234567890",
                    start = 4,
                    size = 10,
                    rtn = "4567890123",
                },
                new
                {
                    main = "012345678901234567890",
                    start = 5,
                    size = 40,
                    rtn = "5678901234567890",
                },
            };

            foreach (var test in variations)
            {
                string returnValue = test.main.SafeSubstring(test.start, test.size);
                returnValue.Should().Be(test.rtn);
            }
        }
    }
}
