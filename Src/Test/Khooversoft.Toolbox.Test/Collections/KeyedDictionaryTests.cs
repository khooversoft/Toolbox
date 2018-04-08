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
    public class KeyedDictionaryTests
    {
        [Fact]
        public void SimpleSetTest()
        {
            const int count = 100;
            var store = new KeyedDictionary<string, Record>(x => x.Key);
            store.GetKey.Should().NotBeNull();

            foreach (var index in Enumerable.Range(0, count))
            {
                store.Set(new Record { Key = $"Key_{index}", Value = index });
            }

            store.Count.Should().Be(count);

            int recordIndex = 0;
            foreach (var record in store.OrderBy(x => x.Value))
            {
                record.Key.Should().Be($"Key_{recordIndex}");
                record.Value.Should().Be(recordIndex);
                recordIndex++;
            }
        }

        [Fact]
        public void SimpleSetRemoveTest()
        {
            const int count = 100;
            var store = new KeyedDictionary<string, Record>(x => x.Key);

            foreach (var index in Enumerable.Range(0, count))
            {
                store.Set(new Record { Key = $"Key_{index}", Value = index });
            }

            store.Count.Should().Be(count);
            store.ContainsKey("xxx").Should().BeFalse();

            int recordIndex = 0;
            foreach (var record in store.OrderBy(x => x.Value))
            {
                string key = $"Key_{recordIndex}";
                record.Key.Should().Be(key);
                record.Value.Should().Be(recordIndex);

                store.ContainsKey(key).Should().BeTrue();
                Record readRecord;
                store.TryGetValue(key, out readRecord).Should().BeTrue();
                readRecord.Should().NotBeNull();
                readRecord.Key.Should().Be(key);

                readRecord = store[key];
                readRecord.Should().NotBeNull();
                readRecord.Key.Should().Be(key);
                readRecord.Value.Should().Be(recordIndex);

                recordIndex++;
            }

            store.Remove("xxxx").Should().BeFalse();
            store.Invoking(x => { var r = x["notKey"]; }).ShouldThrow<KeyNotFoundException>();

            var list = new List<string>(store.Select(x => store.GetKey(x)));
            list.Count.Should().Be(count);
            foreach (var item in list)
            {
                store.Remove(item).Should().BeTrue();
            }

            store.Count.Should().Be(0);
        }

        [Fact]
        public void SimpleSetRemoveCaseTest()
        {
            const int count = 100;
            var store = new KeyedDictionary<string, Record>(x => x.Key, StringComparer.OrdinalIgnoreCase);

            foreach (var index in Enumerable.Range(0, count))
            {
                store.Set(new Record { Key = $"Key_{index}", Value = index });
            }

            store.Count.Should().Be(count);
            store.ContainsKey("xxx").Should().BeFalse();

            int recordIndex = 0;
            foreach (var record in store.OrderBy(x => x.Value))
            {
                string key = $"Key_{recordIndex}";
                record.Key.Should().Be(key);
                record.Value.Should().Be(recordIndex);
                recordIndex++;

                string newKey = key.ToLower();
                store.ContainsKey(newKey).Should().BeTrue();
                Record readRecord;
                store.TryGetValue(newKey, out readRecord).Should().BeTrue();
                readRecord.Should().NotBeNull();
                readRecord.Key.Should().Be(key);
            }

            store.Remove("xxxx").Should().BeFalse();

            var list = new List<string>(store.Select(x => store.GetKey(x)));
            list.Count.Should().Be(count);
            foreach (var item in list)
            {
                store.Remove(item.ToLower()).Should().BeTrue();
            }

            store.Count.Should().Be(0);
        }

        [Fact]
        public void SimpleSetRemoveWithActionTest()
        {
            const int count = 100;
            int addCount = 0;
            int removeCount = 0;
            var store = new KeyedDictionary<string, Record>(x => x.Key, onBeforeAdd: x => { addCount++; return x; }, onAfterRemove: x => removeCount++);

            foreach (var index in Enumerable.Range(0, count))
            {
                store.Set(new Record { Key = $"Key_{index}", Value = index });
            }

            store.Count.Should().Be(count);
            addCount.Should().Be(count);

            int recordIndex = 0;
            foreach (var record in store.OrderBy(x => x.Value))
            {
                record.Key.Should().Be($"Key_{recordIndex}");
                record.Value.Should().Be(recordIndex);
                recordIndex++;
            }

            var list = new List<string>(store.Select(x => store.GetKey(x)));
            list.Count.Should().Be(count);
            foreach (var item in list)
            {
                store.Remove(item).Should().BeTrue();
            }

            store.Count.Should().Be(0);
            removeCount.Should().Be(count);
        }

        [Fact]
        public void ConstructionTest()
        {
            const int count = 10;
            int addCount = 0;
            int removeCount = 0;
            var store = new KeyedDictionary<string, Record>(x => x.Key, onBeforeAdd: x => { addCount++; return x; }, onAfterRemove: x => removeCount++)
            {
                new Record { Key = $"Key_0", Value = 0 },
                new Record { Key = $"Key_1", Value = 1 },
                new Record { Key = $"Key_2", Value = 2 },
                new Record { Key = $"Key_3", Value = 3 },
                new Record { Key = $"Key_4", Value = 4 },
                new Record { Key = $"Key_5", Value = 5 },
                new Record { Key = $"Key_6", Value = 6 },
                new Record { Key = $"Key_7", Value = 7 },
                new Record { Key = $"Key_8", Value = 8 },
                new Record { Key = $"Key_9", Value = 9 },
            };

            store.Count.Should().Be(count);
            addCount.Should().Be(count);

            int recordIndex = 0;
            foreach (var record in store.OrderBy(x => x.Value))
            {
                record.Key.Should().Be($"Key_{recordIndex}");
                record.Value.Should().Be(recordIndex);
                recordIndex++;
            }

            var list = new List<string>(store.Select(x => store.GetKey(x)));
            list.Count.Should().Be(count);
            foreach (var item in list)
            {
                store.Remove(item).Should().BeTrue();
            }

            store.Count.Should().Be(0);
            removeCount.Should().Be(count);
        }

        [Fact]
        public void ConstructionDuplicateKeyTest()
        {
            Action test = () =>
            {
                var store = new KeyedDictionary<string, Record>(x => x.Key)
                {
                    new Record { Key = $"Key_0", Value = 0 },
                    new Record { Key = $"Key_1", Value = 1 },
                    new Record { Key = $"Key_2", Value = 2 },
                    new Record { Key = $"Key_3", Value = 3 },
                    new Record { Key = $"Key_4", Value = 4 },
                    new Record { Key = $"Key_4", Value = 4 },
                };
            };

            test.ShouldThrow<ArgumentException>();
        }

        private class Record
        {
            public string Key { get; set; }

            public int Value { get; set; }
        }
    }
}
