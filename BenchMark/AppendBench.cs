using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BenchMark.BenchMark
{
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net80)]
    [MemoryDiagnoser]
    public class AppendBench
    {
        private StringBuilder _sb;

        [Params(10, 100, 10000, 100000)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            _sb = new StringBuilder();
        }

        [Benchmark]
        public void AppendChar()
        {
            _sb.Clear();
            for (int i = 0; i < N; i++)
            {
                _sb.Append(i % 2 == 0 ? 'A' : 'B');
            }
        }

        [Benchmark]
        public void AppendString()
        {
            _sb.Clear();
            for (int i = 0; i < N; i++)
            {
                _sb.Append(i % 2 == 0 ? "A" : "B");
            }
        }
    }
}
