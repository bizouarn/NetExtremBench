using BenchmarkDotNet.Attributes;

namespace BenchMark.BenchMark;

[MemoryDiagnoser]
public class AddDict
{
    public int keyToAdd = 999999999;
    public Dictionary<int, string> res;
    public string valueToAdd = "John Week";

    [IterationSetup]
    public void IterationSetup()
    {
        var values = new List<string>
        {
            "John Doe", "123", "Jane Smith", "456", "Bob Johnson"
        };
        res = new Dictionary<int, string>();
        for (var i = 0; i < 10000000; i++) res.Add(i, values[i % 5]);
    }

    [Benchmark]
    public void LinkContainKey()
    {
        var exist = res.Keys.Any(x => x == keyToAdd);
    }

    [Benchmark]
    public void ContainKey()
    {
        var exist = res.ContainsKey(keyToAdd);
    }

    [Benchmark]
    public void AddNotContains()
    {
        if (res.ContainsKey(keyToAdd))
            res[keyToAdd] = valueToAdd;
        else
            res.Add(keyToAdd, valueToAdd);
    }

    [Benchmark]
    public void Add()
    {
        res.Add(keyToAdd, valueToAdd);
    }

    [Benchmark]
    public void TryAdd()
    {
        res.TryAdd(keyToAdd, valueToAdd);
    }
}