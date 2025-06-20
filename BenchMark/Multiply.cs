using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BenchMark.BenchMark
{
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net80)]
    [MemoryDiagnoser]
    public class Multiply
    {
        [Benchmark]
        public void sbyteMult()
        {
            var ret = 5 * (sbyte) 1;
        }

        [Benchmark]
        public void shortMult()
        {
            var ret = 5 * (short) 1;
        }

        [Benchmark]
        public void intMult()
        {
            var ret = 5 * (int) 1;
        }
    }
}
