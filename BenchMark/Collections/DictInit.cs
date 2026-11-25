using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BenchMark.BenchMark
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net80)]
    public class DictInit
    {
        [Benchmark]
        public Dictionary<int, int> InitDirect()
        {
            var d = new Dictionary<int, int>(10);
            for (int i = 0; i < 10; i++)
                d[i] = i;
            return d;
        }

        [Benchmark]
        public Dictionary<int, int> InitProgressive()
        {
            var d = new Dictionary<int, int>();
            for (int i = 0; i < 10; i++)
                d[i] = i;
            return d;
        }
    }

}
