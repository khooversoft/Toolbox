using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;

namespace Khooversoft.Toolbox.Test.Tools
{
    public class MessageRouterTests
    {
        [Fact]
        public void SimpleMessageTest()
        {
            const int size = 100;
            var list = new List<int>();

            var router = new MessageRouter<int>(WorkContext.Empty);
            router.Register("name", (x, _) => list.Add(x));

            Enumerable.Range(0, size).Run(x => router.Post(x));
            router.Close(true);

            list.Count.Should().Be(size);
        }

        [Fact]
        public void MultiTargetMessageTest()
        {
            const int size = 100;
            var list1 = new List<int>();
            var list2 = new List<int>();

            var router = new MessageRouter<int>(WorkContext.Empty);
            router.Register("name", (x, _) => list1.Add(x));
            router.Register("name2", (x, _) => list2.Add(x));

            Enumerable.Range(0, size).Run(x => router.Post(x));
            router.Close(true);

            list1.Count.Should().Be(size);
            list2.Count.Should().Be(size);
        }

        [Fact]
        public void MultiTargetWaitMessageTest()
        {
            const int size = 100;
            var list1 = new List<int>();
            var list2 = new List<int>();

            var router = new MessageRouter<int>(WorkContext.Empty);
            router.Register("name", (x, _) => list1.Add(x));
            router.Register("name2", (x, _) => list2.Add(x));

            Enumerable.Range(0, size).Run(x => router.Post(x));
            Thread.Sleep(TimeSpan.FromSeconds(1));

            list1.Count.Should().Be(size);
            list2.Count.Should().Be(size);
        }
    }
}
