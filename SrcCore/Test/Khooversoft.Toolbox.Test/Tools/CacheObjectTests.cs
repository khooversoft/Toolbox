using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.Toolbox.Test.Tools
{
    [Trait("Category", "Toolbox")]
    public class CacheObjectTests
    {
        [Fact]
        public void EmptyTest()
        {
            var cache = new CacheObject<string>(TimeSpan.FromSeconds(10));

            cache.IsValid().Should().BeFalse();
            cache.IsRefresh().Should().BeFalse();
            cache.TryGetValue(out string value).Should().BeFalse();
            value.Should().BeNullOrEmpty();
        }

        [Fact]
        public void EmptyExtensionTest()
        {
            string item = null;
            var cache = item.ToCacheObject(TimeSpan.FromSeconds(10));

            cache.IsValid().Should().BeTrue();
            cache.IsRefresh().Should().BeFalse();
            cache.TryGetValue(out string value).Should().BeTrue();
            value.Should().BeNullOrEmpty();

            string item2 = "this is the item";
            cache.Set(item2);
            cache.TryGetValue(out string value2).Should().BeTrue();
            value2.Should().Be(item2);
            cache.IsValid().Should().BeTrue();
        }

        [Fact]
        public void ResetTest()
        {
            string item = "Item to be cached";
            var cache = new CacheObject<string>(TimeSpan.FromMilliseconds(100)).Set(item);

            cache.TryGetValue(out string value).Should().BeTrue();
            value.Should().NotBeNullOrEmpty();
            value.Should().Be(item);
            cache.IsValid().Should().BeTrue();
            cache.IsRefresh().Should().BeFalse();

            cache.Clear();
            cache.IsValid().Should().BeFalse();
            cache.IsRefresh().Should().BeFalse();
        }

        [Fact]
        public void ResetWithRefreshTest()
        {
            string item = "Item to be cached";
            var cache = new CacheObject<string>(TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(10)).Set(item);

            cache.TryGetValue(out string value).Should().BeTrue();
            value.Should().NotBeNullOrEmpty();
            value.Should().Be(item);
            cache.IsValid().Should().BeTrue();

            Thread.Sleep(TimeSpan.FromMilliseconds(100));
            cache.IsRefresh().Should().BeTrue();

            cache.Clear();
            cache.IsValid().Should().BeFalse();
            cache.IsRefresh().Should().BeFalse();
        }

        [Fact]
        public void StoreTest()
        {
            string item = "Item to be cached";
            var cache = new CacheObject<string>(TimeSpan.FromSeconds(100)).Set(item);

            cache.IsValid().Should().BeTrue();
            cache.TryGetValue(out string value).Should().BeTrue();
            value.Should().NotBeNullOrEmpty();
            value.Should().Be(item);
        }

        [Fact]
        public void StoreExtensionTest()
        {
            string item = "Item to be cached";
            CacheObject<string> cache = item.ToCacheObject(TimeSpan.FromSeconds(100));

            cache.IsValid().Should().BeTrue();
            cache.TryGetValue(out string value).Should().BeTrue();
            value.Should().NotBeNullOrEmpty();
            value.Should().Be(item);
        }

        [Fact]
        public void ExpireTest()
        {
            string item = "Item to be cached";
            var cache = new CacheObject<string>(TimeSpan.FromMilliseconds(100)).Set(item);

            cache.TryGetValue(out string value).Should().BeTrue();
            value.Should().NotBeNullOrEmpty();
            value.Should().Be(item);

            Thread.Sleep(TimeSpan.FromMilliseconds(200));
            cache.TryGetValue(out value).Should().BeFalse();
            value.Should().BeNullOrEmpty();
        }

        [Fact]
        public void TryTest()
        {
            string item = "Item to be cached";
            var cache = new CacheObject<string>(TimeSpan.FromMilliseconds(100)).Set(item);

            cache.TryGetValue(out string value).Should().Be(true);
            value.Should().NotBeNullOrEmpty();
            value.Should().Be(item);

            Thread.Sleep(TimeSpan.FromMilliseconds(200));
            cache.TryGetValue(out value).Should().BeFalse();
            value.Should().BeNullOrEmpty();
        }

        [Fact]
        public void RefreshTest()
        {
            string item = "Item to be cached";
            var cache = new CacheObject<string>(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(10)).Set(item);

            cache.TryGetValue(out string value).Should().BeTrue();
            value.Should().NotBeNullOrEmpty();
            value.Should().Be(item);

            cache.IsRefresh().Should().Be(false);
            Thread.Sleep(TimeSpan.FromMilliseconds(20));

            cache.IsRefresh().Should().Be(true);
        }
    }
}
