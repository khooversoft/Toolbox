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
    public class EnumerableExtensionTests
    {
        public EnumerableExtensionTests()
        {
        }

        [Fact]
        public void PartitionPassTests()
        {
            var variations = new[]
            {
                new {
                    List = new int[0],
                    Result = new int[][] { },
                    Chunk = 2,
                },
                new {
                    List = new int[] { 0, 1, 2, 3, 4, 5 },
                    Result = new int[][] { new int[] { 0 }, new int[] { 1 }, new int[] { 2 }, new int[] { 3 }, new int[] { 4 }, new int[] { 5 } },
                    Chunk = 1,
                },
                new {
                    List = new int[] { 0, 1, 2, 3, 4, 5 },
                    Result = new int[][] { new int[] { 0, 1 }, new int[] { 2, 3 }, new int[] { 4, 5 } },
                    Chunk = 2,
                },
                new {
                    List = new int[] { 0, 1, 2, 3, 4 },
                    Result = new int[][] { new int[] { 0, 1 }, new int[] { 2, 3 }, new int[] { 4 } },
                    Chunk = 2,
                },
                new {
                    List = new int[] { 0, 1, 2, 3, 4, 5 },
                    Result = new int[][] { new int[] { 0, 1, 2 }, new int[] { 3, 4, 5 } },
                    Chunk = 3,
                },
                new {
                    List = new int[] { 0, 1, 2, 3, 4, 5, 6 },
                    Result = new int[][] { new int[] { 0, 1, 2 }, new int[] { 3, 4, 5 }, new[] { 6 } },
                    Chunk = 3,
                },
            };

            foreach (var test in variations)
            {
                IEnumerable<IEnumerable<int>> result = test.List.Partition(test.Chunk);
                result.Should().NotBeNull();

                test.Result.Length.Should().Be(result.Count());


                int index = 0;
                foreach (var o in result)
                {
                    var array = o.ToArray();
                    array.Length.Should().Be(test.Result[index].Length);

                    var innerArray = test.Result[index].ToArray();
                    array.SequenceEqual(innerArray).Should().BeTrue();

                    index++;
                }
            }
        }

        [Fact]
        public void PartitionFailTests()
        {
            var variations = new[]
            {
                new {
                    List = (int[])null,
                    Result = (int[,])null,
                    Chunk = 2,
                },
            };

            foreach (var test in variations)
            {
                bool pass = false;
                try
                {
                    IEnumerable<IEnumerable<int>> result = test.List.Partition(test.Chunk);
                    result.Should().NotBeNull();
                    result.Count();
                    pass = true;
                }
                catch
                {
                    pass = false;
                }

                pass.Should().BeFalse();
            }
        }

        [Fact]
        public void PartitionSignalTest()
        {
            var variations = new[]
            {
                new {
                    List = new int[] { 0, 1, 2, 3, 4, 5 },
                    Result = new int[][] { new int[] { 0 }, new int[] { 1 }, new int[] { 2 }, new int[] { 3 }, new int[] { 4 }, new int[] { 5 } },
                    Chunk = 1,
                },
                new {
                    List = new int[] { 0, 1, 2, 3, 4, 5 },
                    Result = new int[][] { new int[] { 0, 1 }, new int[] { 2, 3 }, new int[] { 4, 5 } },
                    Chunk = 2,
                },
                new {
                    List = new int[] { 0, 1, 2, 3, 4 },
                    Result = new int[][] { new int[] { 0, 1 }, new int[] { 2, 3 }, new int[] { 4 } },
                    Chunk = 2,
                },
                new {
                    List = new int[] { 0, 1, 2, 3, 4, 5 },
                    Result = new int[][] { new int[] { 0, 1, 2 }, new int[] { 3, 4, 5 } },
                    Chunk = 3,
                },
                new {
                    List = new int[] { 0, 1, 2, 3, 4, 5, 6 },
                    Result = new int[][] { new int[] { 0, 1, 2 }, new int[] { 3, 4, 5 }, new[] { 6 } },
                    Chunk = 3,
                },
            };

            foreach (var test in variations)
            {
                IEnumerable<IEnumerable<int>> result = test.List.Partition((l, x) => l.Count == test.Chunk);
                result.Should().NotBeNull();
                result.Count();

                test.Result.Length.Should().Be(result.Count());


                int index = 0;
                foreach (var o in result)
                {
                    var array = o.ToArray();
                    array.Length.Should().Be(test.Result[index].Length);

                    var innerArray = test.Result[index].ToArray();
                    array.SequenceEqual(innerArray).Should().BeTrue();

                    index++;
                }
            }
        }

        [Fact]
        public void PartitionSignalTest_LeftOptional()
        {
            var variations = new[]
            {
                new {
                    List = new int[] { 0, 1, 2, 3, 4, 5 },
                    Result = new int[][] { new int[] { 0, 1, 2, 3, 4, 5 } },
                    Marker = 9,
                },
                new {
                    List = new int[] { 0, 1, 0, 1, 0, 1 },
                    Result = new int[][] { new int[] { 0, 1 }, new int[] { 0, 1 }, new int[] { 0, 1 } },
                    Marker = 1,
                },
                new {
                    List = new int[] { 1, 1, 1 },
                    Result = new int[][] { new int[] { 1 }, new int[] { 1 }, new int[] { 1 } },
                    Marker = 1,
                },
                new {
                    List = new int[] { 0, 1, 2, 1, 2, 1 },
                    Result = new int[][] { new int[] { 0, 1, 2 }, new int[] { 1, 2 }, new int[] { 1 } },
                    Marker = 2,
                },
                new {
                    List = new int[] { 1, 0, 1, 1 },
                    Result = new int[][] { new int[] { 1 }, new int[] { 0, 1 }, new int[] { 1 } },
                    Marker = 1,
                },
            };

            foreach (var test in variations)
            {
                IEnumerable<IEnumerable<int>> result = test.List.Partition((l, x) => l.LastOrDefault() == test.Marker);
                result.Should().NotBeNull();
                result.Count();

                test.Result.Length.Should().Be(result.Count());


                int index = 0;
                foreach (var o in result)
                {
                    var array = o.ToArray();
                    array.Length.Should().Be(test.Result[index].Length);

                    var innerArray = test.Result[index].ToArray();
                    array.SequenceEqual(innerArray).Should().BeTrue();

                    index++;
                }
            }
        }

        [Fact]
        public void PartitionSignalTest_RightOptional()
        {
            var variations = new[]
            {
                new {
                    List = new int[] { 0, 1, 2, 3, 4, 5 },
                    Result = new int[][] { new int[] { 0 }, new int[] { 1 }, new int[] { 2 }, new int[] { 3 }, new int[] { 4 }, new int[] { 5 } },
                    Marker = 9,
                },
                new {
                    List = new int[] { 0, 1, 0, 0, 2, 1 },
                    Result = new int[][] { new int[] { 0, 1 }, new int[] { 0 }, new int[] { 0 }, new int[] { 2, 1 } },
                    Marker = 1,
                },
                new {
                    List = new int[] { 0, 1, 0, 1, 2, 1 },
                    Result = new int[][] { new int[] { 0, 1 }, new int[] { 0, 1 }, new int[] { 2, 1 } },
                    Marker = 1,
                },
            };

            foreach (var test in variations)
            {
                IEnumerable<IEnumerable<int>> result = test.List.Partition((l, x) => x != test.Marker);

                result.Should().NotBeNull();
                result.Count();

                test.Result.Length.Should().Be(result.Count());


                int index = 0;
                foreach (var o in result)
                {
                    var array = o.ToArray();
                    array.Length.Should().Be(test.Result[index].Length);

                    var innerArray = test.Result[index].ToArray();
                    array.SequenceEqual(innerArray).Should().BeTrue();

                    index++;
                }
            }
        }
    }
}
