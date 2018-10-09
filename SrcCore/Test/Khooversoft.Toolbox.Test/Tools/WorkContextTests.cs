using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Khooversoft.Toolbox.Test.Tools
{
    public class WorkContextTests
    {
        [Fact]
        public void EmptyWorkTest()
        {
            IWorkContext context = WorkContext.Empty;
            TestContextProperties(context);
        }

        [Fact]
        public void WorkWithPropertyTest()
        {
            IWorkContext context = WorkContext.Empty;
            IWorkContext nextContext = context.With("Key", "Value");

            context.Properties.ContainsKey("Key").Should().BeFalse();
            TestContextProperties(context);

            nextContext.Properties.ContainsKey("Key").Should().BeTrue();
            TestContextProperties(nextContext);
        }

        [Fact]
        public void WorkWithPropertyClassTest()
        {
            IWorkContext context = WorkContext.Empty;

            var custom = new Test("Value1");
            IWorkContext nextContext = context.With(custom);
            TestContextProperties(nextContext);

            context.Properties.ContainsKey("Test").Should().BeFalse();
            nextContext.Properties.ContainsKey("Value1");

            var test = nextContext.Properties.Get<Test>();
            test.Should().NotBeNull();
            test.Value.Should().Be(custom.Value);
        }

        private void TestContextProperties(IWorkContext context)
        {
            context.Should().NotBeNull();
            context.Cv.Should().NotBeNull();
            context.Tag.Should().NotBeNull();
            context.Container.Should().BeNull();
            context.EventLog.Should().NotBeNull();
            context.Dimensions.Should().NotBeNull();
        }

        private class Test
        {
            public Test(string value)
            {
                Value = value;
            }

            public string Value { get; }
        }
    }
}
