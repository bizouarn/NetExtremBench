using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BenchMark.BenchMark;

[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net80)]
public class EnumerateBench
{
    public List<string> List;

    [Params(10000, 10000000)] public int N;
    [Params(false, true)] public bool Reverse;

    [GlobalSetup]
    public void GlobalSetup()
    {
        List = new List<string>();
        for (var i = 0; i < N; i++) List.Add(Guid.NewGuid().ToString());
    }

    [Benchmark]
    public void Before()
    {
        var ret = string.Join(',', BeforeGetEnumerable());
    }

    [Benchmark]
    public void After()
    {
        var ret = string.Join(',', AfterGetEnumerable());
    }
    
    public IEnumerable<string> BeforeGetEnumerable()
    {
        if (!Reverse)
        {
            foreach (var item in List)
                if (IsMatch(item))
                    yield return item;
        }
        else
        {
            foreach (var item in List.AsEnumerable().Reverse())
                if (IsMatch(item))
                    yield return item;
        }
    }

    public IEnumerable<string> AfterGetEnumerable()
    {
        foreach (var item in Reverse ? List.AsEnumerable().Reverse() : List)
            if (IsMatch(item))
                yield return item;
    }
    
    public bool IsMatch(string item)
    {
        return true;
    }
}