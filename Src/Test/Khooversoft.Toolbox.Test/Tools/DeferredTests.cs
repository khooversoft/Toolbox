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
    public class DeferredTests
    {
        [Fact]
        public void DeferredTest()
        {
            const string testText = "test";

            var t = new Deferred<string>(() => testText);
            t.Value.Should().Be(testText);

            var t1 = new Deferred<string>(testText);
            t1.Value.Should().Be(testText);

            bool pass;
            try
            {
                t1 = new Deferred<string>(() => throw new ArgumentException());
                var a = t1.Value;
                pass = true;
            }
            catch
            {
                pass = false;
            }

            pass.Should().BeFalse();
        }
    }
}
