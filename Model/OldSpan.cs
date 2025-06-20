namespace BenchMark.Model
{
    internal class OldSpan<T>
    {
        protected bool Equals(OldSpan<T> other)
        {
            return Equals(_array, other._array);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((OldSpan<T>)obj);
        }

        public override int GetHashCode()
        {
            return _array != null ? _array.GetHashCode() : 0;
        }

        public static readonly T[] _fakeRef = Array.Empty<T>();
        private readonly T[] _array;

        public OldSpan()
        {
            _array = _fakeRef;
        }

        public OldSpan(ref T[] array)
        {
            _array = array ?? _fakeRef;
        }

        public OldSpan(ref OldSpan<T> oldSpan)
        {
            _array = oldSpan._array;
        }

        public OldSpan(T[] array, int start, int length)
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

        public static OldSpan<T> Empty => default;

        public static bool operator !=(OldSpan<T> left, OldSpan<T> right)
        {
            return !(left == right);
        }

        public static implicit operator OldSpan<T>(T[] array)
        {
            return new OldSpan<T>(ref array);
        }

        public static implicit operator OldSpan<T>(ArraySegment<T> segment)
        {
            return new OldSpan<T>(segment.Array, segment.Offset, segment.Count);
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

        public static bool operator ==(OldSpan<T> left, OldSpan<T> right)
        {
            return right?._array == left?._array;
        }

        public override string ToString()
        {
            if (_array is char[] charArray) return new string(charArray);
            return $"System.OldSpan<{typeof(T).Name}>[{_array.Length}]";
        }

        public OldSpan<T> Slice(int start)
        {
            var ret = new T[_array.Length - start];
            Array.Copy(_array, start, ret, 0, _array.Length - start);
            return ret;
        }

        public OldSpan<T> Trim()
        {
            var start = 0;
            var end = Length - 1;
            if (this is OldSpan<char> spanChar)
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

        public OldSpan<T> Slice(int start, int length)
        {
            var ret = new T[length];
            Array.Copy(_array, start, ret, 0, length);
            return ret;
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
            var ret = new T[_array.Length];
            _array.CopyTo(ret, 0);
            return ret;
        }
    }
}
