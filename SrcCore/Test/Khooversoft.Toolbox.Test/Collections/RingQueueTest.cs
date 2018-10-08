using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.Toolbox.Test.Collections
{
    [Trait("Category", "Toolbox")]
    public class RingQueueTests
    {
        [Fact]
        public void RingQueueTest()
        {
            const int size = 3;
            var ring = new RingQueue<int>(10);

            ring.IsEmpty.Should().BeTrue();
            ring.IsFull.Should().BeFalse();

            for (int i = 0; i < size; i++)
            {
                ring.Enqueue(i);
                ring.Count.Should().Be(i + 1);
            }
            ring.Count.Should().Be(size);
            ring.IsFull.Should().BeFalse();
            ring.LostCount.Should().Be(0);

            for (int i = 0; i < size; i++)
            {
                ring.Count.Should().Be(size - i);
                int value = ring.Dequeue();
                value.Should().Be(i);
            }

            ring.IsEmpty.Should().BeTrue();
            ring.IsFull.Should().BeFalse();
        }

        [Fact]
        public void RingQueueMaxTest()
        {
            const int maxCount = 10;
            var ring = new RingQueue<int>(maxCount);

            ring.IsEmpty.Should().BeTrue();
            ring.IsFull.Should().BeFalse();

            for (int i = 0; i < maxCount; i++)
            {
                ring.Enqueue(i);
                ring.Count.Should().Be(i + 1);
            }
            ring.Count.Should().Be(maxCount);
            ring.IsFull.Should().BeTrue();
            ring.LostCount.Should().Be(0);

            for (int i = 0; i < maxCount; i++)
            {
                int value = ring.Dequeue();
                value.Should().Be(i);
            }

            ring.IsEmpty.Should().BeTrue();
            ring.IsFull.Should().BeFalse();
        }

        [Fact]
        public void RingQueueLostTest()
        {
            const int maxCount = 20;
            const int size = 18;
            var ring = new RingQueue<int>(size);

            ring.IsEmpty.Should().BeTrue();
            ring.IsFull.Should().BeFalse();

            for (int i = 0; i < maxCount; i++)
            {
                ring.Enqueue(i);
                ring.Count.Should().Be(Math.Min(size, i + 1));
            }
            ring.Count.Should().Be(size);
            ring.IsFull.Should().BeTrue();
            ring.LostCount.Should().Be(maxCount - size);

            for (int i = maxCount - size; i < maxCount; i++)
            {
                int value = ring.Dequeue();
                value.Should().Be(i);
            }

            ring.IsEmpty.Should().BeTrue();
            ring.IsFull.Should().BeFalse();
        }

        [Fact]
        public void RingQueueTryDequeueTest()
        {
            const int maxCount = 2000;
            const int size = 20;
            var ring = new RingQueue<int>(size);

            ring.IsEmpty.Should().BeTrue();
            ring.IsFull.Should().BeFalse();

            for (int i = 0; i < maxCount; i++)
            {
                ring.Enqueue(i);
                ring.Count.Should().Be(Math.Min(size, i + 1));
            }
            ring.Count.Should().Be(size);
            ring.IsFull.Should().BeTrue();
            ring.LostCount.Should().Be(maxCount - size);

            for (int i = maxCount - size; i < maxCount; i++)
            {
                (bool success, int value) rtn = ring.TryDequeue();

                rtn.value.Should().Be(i);
            }

            ring.IsEmpty.Should().BeTrue();
            ring.IsFull.Should().BeFalse();
        }

        [Fact]
        public void RingQueueTryPeekTest()
        {
            const int maxCount = 2000;
            const int size = 20;
            var ring = new RingQueue<int>(size);

            ring.IsEmpty.Should().BeTrue();
            ring.IsFull.Should().BeFalse();

            for (int i = 0; i < maxCount; i++)
            {
                ring.Enqueue(i);
                ring.Count.Should().Be(Math.Min(size, i + 1));
            }
            ring.Count.Should().Be(size);
            ring.IsFull.Should().BeTrue();
            ring.LostCount.Should().Be(maxCount - size);

            for (int i = maxCount - size; i < maxCount; i++)
            {
                (bool success, int peekValue) rtn = ring.TryPeek();
                rtn.success.Should().BeTrue();
                int value = ring.Dequeue();

                value.Should().Be(rtn.peekValue);
                value.Should().Be(i);
            }

            ring.IsEmpty.Should().BeTrue();
            ring.IsFull.Should().BeFalse();
        }

        [Fact]
        public void RingQueueGetListTest()
        {
            const int loopCount = 10;
            const int size = 1000;
            const int batchSize = 250;
            var ring = new RingQueue<int>(size);

            ring.IsEmpty.Should().BeTrue();
            ring.IsFull.Should().BeFalse();

            int index = 0;
            for (int i = 0; i < loopCount; i++)
            {
                for (int j = 0; j < batchSize; j++)
                {
                    ring.Enqueue(index);
                    ring.Count.Should().Be(Math.Min(size, index + 1));
                    index++;
                }

                IList<int> list = ring.ToList();
                list.Count.Should().Be(Math.Min(size, batchSize * (i + 1)));
            }
        }
    }
}
