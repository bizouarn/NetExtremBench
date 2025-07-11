using BenchMark.Model;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BenchMark.BenchMark;

[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
[GcServer(true)]
public class Lambda
{
    public List<Personne> res;

    [Params(10000, 100000)]
    public int N;

    [IterationSetup]
    public void IterationSetup()
    {
        res = Personne.GetAll(N).ToList();
    }

    [Benchmark]
    public void SortLambda()
    {
        var sorted = res.OrderBy(x => x.Id).ToList();
    }

    [Benchmark]
    public void SortStaticLambda()
    {
        var sorted = res.OrderBy(static x => x.Id).ToList();
    }
}