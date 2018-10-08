using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.Toolbox.Test.Collections
{
    [Trait("Category", "Toolbox")]
    public class LruCacheTests
    {
        private readonly Random _random = new Random();

        [Fact]
        public void LruSimpleTest()
        {
            const int count = 10;

            var cache = new LruCache<int, int>(count);

            Enumerable.Range(0, count).Run(x => cache.Set(x, x));
            cache.Count.Should().Be(count);

            Enumerable.Range(0, count).All(x => cache.Remove(x)).Should().BeTrue();
            cache.Count.Should().Be(0);
        }

        [Fact]
        public void LruLookupTest()
        {
            const int count = 10;

            var cache = new LruCache<int, int>(count);

            for (int i = 0; i < count; i++)
            {
                cache.Set(i, i);
                cache.GetCacheDetails(i).Should().NotBeNull();
                cache.Count.Should().Be(i + 1);
            }

            Enumerable.Range(0, count).Reverse().All(x => cache.Remove(x)).Should().BeTrue();
            cache.Count.Should().Be(0);
        }

        [Fact]
        public void LruRemoveTest()
        {
            const int count = 10;

            var cache = new LruCache<int, int>(count);
            Enumerable.Range(0, count).Run(x => cache.Set(x, x));

            int index = _random.Next(0, count);
            cache.GetCacheDetails(index).Should().NotBeNull();
            cache.Remove(index).Should().BeTrue();
            cache.GetCacheDetails(index).Should().BeNull();
            cache.Remove(index).Should().BeFalse();
        }

        [Fact]
        public void LruCapacityTest()
        {
            var variations = new[]
            {
                new
                {
                    Count = 10,
                    Capacity = 10,
                    ResultCount = 10,
                },
                new
                {
                    Count = 10,
                    Capacity = 20,
                    ResultCount = 10,
                },
                new
                {
                    Count = 10,
                    Capacity = 5,
                    ResultCount = 5,
                },
            };

            foreach (var test in variations)
            {
                int removeCount = 0;
                var cache = new LruCache<int, int>(test.Capacity);
                cache.CacheItemRemoved += x => removeCount++;

                Enumerable.Range(0, test.Count).Run(x => cache.Set(x, x));

                cache.Count.Should().Be(test.ResultCount);

                cache
                    .Select(x => x.Value)
                    .OrderBy(x => x)
                    .Zip(Enumerable.Range(0, test.Count).Skip(test.Capacity), (a, b) => new Tuple<int, int>(a, b))
                    .All(x => x.Item1 == x.Item2)
                    .Should().BeTrue();

                removeCount.Should().Be(test.Count - test.ResultCount);
            }
        }

        [Fact]
        public void LruLinkListTest()
        {
            const int capacity = 10;
            var cache = new LruCache<int, int>(capacity);

            Enumerable.Range(0, capacity).Run(x => cache.Set(x, x));

            cache
                .Select(x => x.Value)
                .OrderBy(x => x)
                .Zip(Enumerable.Range(0, capacity), (a, b) => new Tuple<int, int>(a, b))
                .All(x => x.Item1 == x.Item2)
                .Should().BeTrue();

            IEnumerable<int> newList = Enumerable.Range(0, capacity / 2).ToList();
            newList.Run(x => cache.TryGetValue(x, out int value));

            cache
                .Skip(newList.Count())
                .Select(x => x.Value)
                .OrderBy(x => x)
                .Zip(newList, (a, b) => new Tuple<int, int>(a, b))
                .All(x => x.Item1 == x.Item2)
                .Should().BeTrue();
        }

        [Fact]
        public async Task MultiThreadTest()
        {
            const int taskCount = 5;
            var tasks = new List<Task>();
            var tokenSource = new CancellationTokenSource();
            var cache = new LruCache<int, int>(1000);

            Enumerable.Range(0, taskCount)
                .Select(x => Task.Run(() => WriteTask(cache, tokenSource.Token)))
                .Run(x => tasks.Add(x));

            Enumerable.Range(0, taskCount)
                .Select(x => Task.Run(() => ReadTask(cache, tokenSource.Token)))
                .Run(x => tasks.Add(x));

            await Task.Delay(TimeSpan.FromSeconds(2));
            tokenSource.Cancel();
            Task.WaitAll(tasks.ToArray());
        }

        private Task WriteTask(LruCache<int, int> cache, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                int index = _random.Next(0, 1000);
                cache.Set(index, index);
            }

            return Task.FromResult(0);
        }

        private Task ReadTask(LruCache<int, int> cache, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                int index = _random.Next(0, 1000);
                LruCache<int, int>.CacheItem cacheDetail = cache.GetCacheDetails(index);
                if (cacheDetail != null)
                {
                    cache.Remove(index);
                }
            }

            return Task.FromResult(0);
        }
    }
}
