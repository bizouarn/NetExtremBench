using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BenchMark.BenchMark
{
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net80)]
    [MemoryDiagnoser]
    public class IndexOf
    {
        private string _guid = System.Guid.NewGuid().ToString() + "#" + System.Guid.NewGuid().ToString();
        private int _res = 0;

        [Benchmark]
        public void CharType()
        {
            _res = _guid.IndexOf('#');
        }

        [Benchmark]
        public void StringType()
        {
            _res = _guid.IndexOf("#");
        }

        [Benchmark]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AgressiveCharType()
        {
            _res = _guid.IndexOf('#');
        }
    }
}
