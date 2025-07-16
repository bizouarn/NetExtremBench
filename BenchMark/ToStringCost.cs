using BenchMark.Model;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BenchMark.BenchMark
{
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net80)]
    [MemoryDiagnoser]
    [GcServer(true)]
    public class ToStringCost
    {
        private Operateur _operateur = new ("T > 0 && T < 10");

        [Benchmark]
        public string AccessOperateur() => _operateur.Syntaxe;

        [Benchmark]
        public string AccessToString() => _operateur.ToString();
    }
}
