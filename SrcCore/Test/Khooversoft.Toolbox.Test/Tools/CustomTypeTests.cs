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
    public class CustomTypeTests
    {
        [Fact]
        public void ValueStringNullFailure()
        {
            var variations = new[]
            {
                new {
                    value = (string)null,
                    pass = false
                },
                new {
                    value = "",
                    pass = false
                },
                new {
                    value = "test",
                    pass = true
                },
            };

            foreach (var item in variations)
            {
                bool pass;
                ValueString t1 = null;
                try
                {
                    t1 = new ValueString(item.value);
                    pass = true;
                }
                catch
                {
                    pass = false;
                }

                pass.Should().Be(item.pass, $"value={item.value}");

                if (t1 != null)
                {
                    t1.Should().NotBeNull();
                    t1.IsValueValid().Should().Be(item.pass);
                }
            }
        }

        private class ValueString : StringType
        {
            public ValueString(string value)
                : base(value)
            {
            }
        }
    }
}
