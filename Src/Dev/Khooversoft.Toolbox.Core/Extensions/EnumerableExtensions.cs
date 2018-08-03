using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Partition an collection based on a partition size
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="self">enumerable</param>
        /// <param name="partitionSize">partition size</param>
        /// <returns>collection of partitions</returns>
        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> self, int partitionSize)
        {
            Verify.IsNotNull(nameof(self), self);
            Verify.Assert(partitionSize > 0, $"{nameof(partitionSize)} must be greater then 0");
            List<T> list = null;

            foreach (var item in self)
            {
                list = list ?? new List<T>(partitionSize);
                list.Add(item);

                if (list.Count >= partitionSize)
                {
                    var returnList = list;
                    list = null;

                    yield return returnList;
                }
            }

            if (list?.Count > 0)
            {
                yield return list;
            }
        }

        /// <summary>
        /// Partition based on signal based on the next element.  The signaled element will be placed in the next partition
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="self">collection</param>
        /// <param name="partitionSignal">partition signal, true signals current collection is a partition</param>
        /// <returns>collection of partitions</returns>
        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> self, Func<IReadOnlyList<T>, T, bool> partitionSignal)
        {
            Verify.IsNotNull(nameof(self), self);
            List<T> list = null;

            foreach (var item in self)
            {
                if (list != null && partitionSignal(list, item))
                {
                    var returnList = list;
                    list = new List<T> { item };

                    yield return returnList;
                }
                else
                {
                    list = list ?? new List<T>();
                    list.Add(item);
                }
            }

            if (list?.Count > 0)
            {
                yield return list;
            }
        }

        /// <summary>
        /// Convert enumerable to hash set
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="self">enumerable</param>
        /// <returns>hash set</returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> self)
        {
            if( self == null )
            {
                return null;
            }

            return new HashSet<T>(self);
        }
    }
}
