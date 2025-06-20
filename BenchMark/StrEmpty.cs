using BenchmarkDotNet.Attributes;

namespace BenchMark.BenchMark
{
    [MemoryDiagnoser]
    public class StrEmpty
    {
        private string[] strings = ["Aymeric", null, "", "            "];
        private string[] _array;
        private static bool res;

        [GlobalSetup]
        public void Setup()
        {
            _array = Enumerable.Range(0, short.MaxValue / 2)
                .Select(x => strings[x % strings.Length]).ToArray();
        }

        [Benchmark]
        public void StrIsNullOrEmpty()
        {
            foreach (var val in _array)
            {
                res = string.IsNullOrEmpty(val);
            }
        }

        [Benchmark]
        public void StrOpIsNullOrEmpty()
        {
            foreach (var val in _array)
            {
                res = strOp.IsNullOrEmpty(val);
            }
        }

        [Benchmark]
        public void StrIsNullOrWhiteSpace()
        {
            foreach (var val in _array)
            {
                res = string.IsNullOrWhiteSpace(val);
            }
        }

        [Benchmark]
        public void StrOpIsNullOrWhiteSpace()
        {
            foreach (var val in _array)
            {
                res = strOp.IsNullOrWhiteSpace(val);
            }
        }

    }

    public static class strOp
    {
        public static bool IsNullOrEmpty(ReadOnlySpan<char> value)
        {
            return value.IsEmpty;
        }
        
        public static bool IsNullOrWhiteSpace(ReadOnlySpan<char> value)
        {
            return IsNullOrEmpty(value) || value.IsWhiteSpace();
        }
    }
}
