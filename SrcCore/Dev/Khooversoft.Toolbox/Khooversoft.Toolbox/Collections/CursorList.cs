using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    /// <summary>
    /// Cursor list, implements a minimum required for list with a cursor as an index
    /// Cursor can be saved and restored, or abandon
    /// </summary>
    /// <typeparam name="T">list type</typeparam>
    public class CursorList<T> : IEnumerable<T>
    {
        private const string _endOfListMessage = "End of list has been reached";
        private readonly List<T> _list = new List<T>();
        private readonly Stack<int> _cursorStack = new Stack<int>();
        private int _cursor = 0;

        public CursorList()
        {
        }

        public CursorList(IEnumerable<T> list)
        {
            _list = new List<T>(list);
        }

        /// <summary>
        /// Return value at index
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>value</returns>
        public T this[int index]
        {
            get { return _list[index]; }
            set { _list[index] = value; }
        }

        /// <summary>
        /// Count in list
        /// </summary>
        public int Count => _list.Count;

        /// <summary>
        /// Test end of list, true if cursor is at the end
        /// </summary>
        public bool EndOfList => _list.Count == 0 || _cursor >= _list.Count;

        /// <summary>
        /// Set / Get cursor position
        /// </summary>
        public int Cursor
        {
            get { return _cursor; }
            set { _cursor = Math.Min(_list.Count, Math.Max(0, value)); }
        }

        /// <summary>
        /// Get value at cursor
        /// </summary>
        public T Current
        {
            get
            {
                if (EndOfList)
                {
                    throw new InvalidOperationException("End of list has been reached");
                }

                return _list[_cursor];
            }
        }

        /// <summary>
        /// Return values from cursor
        /// </summary>
        public IEnumerable<T> FromCursor { get { return _list.Skip(Cursor); } }

        /// <summary>
        /// Clear list
        /// </summary>
        public void Clear()
        {
            _list.Clear();
        }

        /// <summary>
        /// Add value to list
        /// </summary>
        /// <param name="value"></param>
        public void Add(T value)
        {
            _list.Add(value);
        }

        /// <summary>
        /// Remove value at index
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        /// <summary>
        /// Like "Next" return list of values
        /// </summary>
        /// <param name="count">number of next instructions</param>
        /// <returns></returns>
        public IEnumerable<T> Next(int count)
        {
            while (!EndOfList && count-- != 0)
            {
                yield return Next();
            }
        }

        /// <summary>
        /// Get next value and advance cursor
        /// </summary>
        /// <returns>value</returns>
        public T Next()
        {
            if (_cursor >= _list.Count)
            {
                throw new InvalidOperationException("End of list has been reached");
            }

            return _list[_cursor++];
        }

        /// <summary>
        /// Try to get the next value and if available
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryNext(out T value)
        {
            value = default(T);

            if (_cursor >= _list.Count)
            {
                return false;
            }

            value = _list[_cursor++];
            return true;
        }

        /// <summary>
        /// Save cursor in cursor stack
        /// </summary>
        public void SaveCursor()
        {
            _cursorStack.Push(_cursor);
        }

        /// <summary>
        /// Restore cursor from save stack
        /// </summary>
        public void RestoreCursor()
        {
            if (_cursorStack.Count == 0)
            {
                throw new InvalidOperationException("No cursor(s) to restore");
            }

            _cursor = _cursorStack.Pop();
        }

        /// <summary>
        /// Abandon a saved cursor position
        /// </summary>
        public void AbandonSavedCursor()
        {
            if (_cursorStack.Count == 0)
            {
                throw new InvalidOperationException("No cursor(s) to restore");
            }

            _cursorStack.Pop();
        }

        /// <summary>
        /// Return debug details
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var list = new List<string>()
            {
                $"{nameof(Cursor)}={Cursor}",
                $"{nameof(Current)}={Current}",
                $"{nameof(Count)}={Count}",
                $"{nameof(EndOfList)}={EndOfList}",
                $"FromCursor(5): ({string.Join(", ", FromCursor.Take(5).Select(x => x.ToString()))})",
            };

            return string.Join(", ", list);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
