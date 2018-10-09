using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.Toolbox.Services.Test.Graph
{
    [Trait("Category", "Toolbox")]
    public class EdgeIdTests
    {
        [Fact]
        public void ConversionTest()
        {
            var e1 = new EdgeId(0, 1);
            e1.Value.Should().Be(1);

            var e2 = new EdgeId(1, 0);
            e2.Value.Should().Be(4294967296);

            var e3 = new EdgeId(1, 1);
            e3.Value.Should().Be(4294967297);

            var e4 = new EdgeId(2, 2);
            var e5 = new EdgeId(2, 2);

            (e1 == e2).Should().BeFalse();
            (e1 != e2).Should().BeTrue();

            (e1 == e3).Should().BeFalse();
            (e4 == e5).Should().BeTrue();
            (e1 != e3).Should().BeTrue();
            (e4 != e5).Should().BeFalse();

            (e1 < e2).Should().BeTrue();
            (e1 > e2).Should().BeFalse();
            (e4 > e3).Should().BeTrue();

            e1.CompareTo(e2).Should().Be(-1);
            e2.CompareTo(e1).Should().Be(1);
            e4.CompareTo(e5).Should().Be(0);
        }
    }
}
