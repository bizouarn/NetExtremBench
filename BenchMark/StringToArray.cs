using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BenchMark.BenchMark
{
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net80)]
    [MemoryDiagnoser]
    public class StringToArray
    {
        private string _guid = System.Guid.NewGuid().ToString() + "#" + System.Guid.NewGuid().ToString();
        private char[] _res;

        [Benchmark]
        public void For()
        {
            var array = new char[_guid.Length];
            for (var i = 0; i < _guid.Length; i++)
                array[i] = _guid[i];
            _res = array;
        }

        [Benchmark]
        public void ToCharArray()
        {
            _res = _guid.ToCharArray();
        }

        [Benchmark]
        public void AsSpan()
        {
            _res = _guid.AsSpan().ToArray();
        }
    }
}
