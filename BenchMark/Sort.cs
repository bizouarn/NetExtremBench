using BenchmarkDotNet.Attributes;

namespace BenchMark.BenchMark;

[MemoryDiagnoser]
public class Sort
{
    public List<string> res;

    [IterationSetup]
    public void IterationSetup()
    {
        res = new List<string>
        {
            "John Doe", "123", "Jane Smith", "456", "Bob Johnson"
        };
        for (var i = 0; i < 1000000; i++) res.Add(res[i % 5]);
    }

    [Benchmark]
    public void SortList()
    {
        res.Sort();
    }

    [Benchmark]
    public void OrderByLinq()
    {
        res = res.OrderBy(x => x).ToList();
    }
}