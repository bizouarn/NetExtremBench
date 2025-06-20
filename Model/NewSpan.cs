namespace BenchMark.Model
{
    internal class NewSpan<T>
    {
        protected bool Equals(NewSpan<T> other)
        {
            return Equals(_array, other._array);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((NewSpan<T>)obj);
        }

        public override int GetHashCode()
        {
            return _array != null ? _array.GetHashCode() : 0;
        }

        public static readonly T[] _fakeRef = Array.Empty<T>();
        private readonly T[] _array;

        public NewSpan()
        {
            _array = _fakeRef;
        }

        public NewSpan(ref T[] array)
        {
            _array = array ?? _fakeRef;
        }

        public NewSpan(ref NewSpan<T> newSpan)
        {
            _array = newSpan._array;
        }

        public NewSpan(T[] array, int start, int length)
        {
            _array = length > 0 ? new T[length] : _fakeRef;
            Array.Copy(array, start, _array, 0, length);
        }

        public virtual T this[int index]
        {
            get => _array[index];
            set => _array[index] = value;
        }

        public int Length => _array.Length;

        public bool IsEmpty => _array.Length == 0;

        public static NewSpan<T> Empty => default;

        public static bool operator !=(NewSpan<T> left, NewSpan<T> right)
        {
            return !(left == right);
        }

        public static implicit operator NewSpan<T>(T[] array)
        {
            return new NewSpan<T>(ref array);
        }

        public static implicit operator NewSpan<T>(ArraySegment<T> segment)
        {
            return new NewSpan<T>(segment.Array, segment.Offset, segment.Count);
        }

        public ref readonly T GetPinnableReference()
        {
            if (_array.Length != 0)
                return ref _array[0];
            return ref _fakeRef[0];
        }

        public int IndexOf(T v)
        {
            return Array.IndexOf(_array, v);
        }

        public void CopyTo(ref T[] destination)
        {
            Array.Copy(_array, destination, _array.Length);
        }

        public bool TryCopyTo(T[] destination)
        {
            try
            {
                Array.Copy(_array, destination, _array.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool operator ==(NewSpan<T> left, NewSpan<T> right)
        {
            return right?._array == left?._array;
        }

        public override string ToString()
        {
            if (_array is char[] charArray) return new string(charArray);
            return $"System.NewSpan<{typeof(T).Name}>[{_array.Length}]";
        }

        public NewSpan<T> Slice(int start)
        {
            return new NewSpan<T>(_array, start, _array.Length - start);
        }

        public NewSpan<T> Trim()
        {
            var start = 0;
            var end = Length - 1;
            if (this is NewSpan<char> spanChar)
            {
                for (; start < Length; start++)
                    if (!char.IsWhiteSpace(spanChar[start]))
                        break;

                for (; end > start; end--)
                    if (!char.IsWhiteSpace(spanChar[end]))
                        break;
            }

            return Slice(start, end - start + 1);
        }

        public NewSpan<T> Slice(int start, int length)
        {
            return new NewSpan<T>(_array, start, length);
        }

        public int Count(T value)
        {
            var i = 0;
            foreach (var item in _array)
                if (item.Equals(value))
                    i++;
            return i;
        }

        public T[] ToArray()
        {
            return (T[])_array.Clone();
        }
    }
}
