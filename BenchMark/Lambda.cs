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
        var values = new List<Personne>
        {
            new Personne { Name = "John Doe", Age = 30, Address = "123 Main St" },
            new Personne { Name = "Jane Smith", Age = 25, Address = "456 Elm St" },
            new Personne { Name = "Bob Johnson", Age = 40, Address = "789 Pine St" }
        };

        res = new List<Personne>();
        for (var i = 0; i < N; i++) res.Add(values[i % 3].Copy());
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