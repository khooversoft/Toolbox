using System;
using System.Collections;
using System.Collections.Generic;

namespace Khooversoft.Toolbox
{
    /// <summary>
    /// Ring queue, FIFO performance queue.
    /// Queue is a fixed size ring
    /// Items in the ring can be overwritten if reads are not as fast as writes.
    /// 
    /// Note: Additional performance can be achieved for reference class when its members are updated and not replaced with
    /// a new instance.
    /// 
    /// This is thread safe
    /// </summary>
    /// <typeparam name="T">Queue of T</typeparam>
    public class RingQueue<T> : IEnumerable<T>
    {
        private Queue<T> _queue;
        private readonly object _lock = new object();

        /// <summary>
        /// Construct ring of a specific size
        /// </summary>
        /// <param name="size"></param>
        public RingQueue(int size)
        {
            Verify.Assert(size >= 0, "Size must be greater then 0");

            Size = size;
            Clear();
        }

        /// <summary>
        /// Is queue empty?
        /// </summary>
        public bool IsEmpty { get { return _queue.Count == 0; } }

        /// <summary>
        /// Is queue full
        /// </summary>
        public bool IsFull { get { return _queue.Count == Size; } }

        /// <summary>
        /// Number of records lost because of overwrite
        /// </summary>
        public int LostCount { get; private set; }

        /// <summary>
        /// Current queue count
        /// </summary>
        public int Count { get { return _queue.Count; } }

        /// <summary>
        /// Size of queue
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Clear queue
        /// </summary>
        /// <returns>this</returns>
        public RingQueue<T> Clear()
        {
            lock (_lock)
            {
                _queue = new Queue<T>(Size);
                LostCount = 0;
            }

            return this;
        }

        /// <summary>
        /// Enqueue new item
        /// </summary>
        /// <param name="item">item</param>
        public void Enqueue(T item)
        {
            lock (_lock)
            {
                if (IsFull)
                {
                    _queue.Dequeue();
                    LostCount++;
                }

                _queue.Enqueue(item);
            }
        }

        /// <summary>
        /// Dequeue from the queue
        /// </summary>
        /// <returns>T</returns>
        /// <exception cref="IndexOutOfRangeException">if queue is empty</exception>
        public T Dequeue()
        {
            lock (_lock)
            {
                Verify.Assert<IndexOutOfRangeException>(!IsEmpty, "Queue is empty");
                return _queue.Dequeue();
            }
        }

        /// <summary>
        /// Try to dequeue value
        /// </summary>
        /// <returns>true if value returned, false if not</returns>
        public (bool Success, T value) TryDequeue()
        {
            lock (_lock)
            {
                if (IsEmpty)
                {
                    return (false, default(T));
                }

                return (true, Dequeue());
            }
        }

        /// <summary>
        /// Try to peek at value
        /// </summary>
        /// <param name="value">value returned</param>
        /// <returns>true if value exists, false if queue is empty</returns>
        public (bool Success, T value) TryPeek()
        {
            lock (_lock)
            {
                if (IsEmpty)
                {
                    return (false, default(T));
                }

                return (true, _queue.Peek());
            }
        }

        /// <summary>
        /// Create new list of items
        /// </summary>
        /// <returns>new IList(T)</returns>
        public IList<T> ToList()
        {
            lock (_lock)
            {
                return new List<T>(_queue);
            }
        }

        /// <summary>
        /// Makes a copy of the ring before enumerator is returned
        /// </summary>
        /// <returns>enumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
