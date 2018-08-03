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

        public T this[int index]
        {
            get { return _list[index]; }
            set { _list[index] = value; }
        }

        public int Count => _list.Count;

        public bool EndOfList => _list.Count == 0 || _cursor >= _list.Count;

        public int Cursor
        {
            get { return _cursor; }
            set { _cursor = Math.Min(_list.Count, value); }
        }

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

        public IEnumerable<T> FromCursor { get { return _list.Skip(Cursor); } }

        public void Clear()
        {
            _list.Clear();
        }

        public void Add(T value)
        {
            _list.Add(value);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public IEnumerable<T> Next(int count)
        {
            while (!EndOfList && count-- != 0)
            {
                yield return Next();
            }
        }

        public T Next()
        {
            if (_cursor >= _list.Count)
            {
                throw new InvalidOperationException("End of list has been reached");
            }

            return _list[_cursor++];
        }

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

        public void SaveCursor()
        {
            _cursorStack.Push(_cursor);
        }

        public void RestoreCursor()
        {
            if (_cursorStack.Count == 0)
            {
                throw new InvalidOperationException("No cursor(s) to restore");
            }

            _cursor = _cursorStack.Pop();
        }

        public void AbandonSavedCursor()
        {
            if (_cursorStack.Count == 0)
            {
                throw new InvalidOperationException("No cursor(s) to restore");
            }

            _cursorStack.Pop();
        }

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
