using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using ZLinq;

namespace BenchMark.BenchMark;

[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
[GcServer(true)]
public class Sum
{
    private int[] intArray;
    private Stack<int> intStack;

    [Params(1_000, 10_000, 50_000)] public int N;
    private int tmpRes;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var rand = new Random();
        intArray = new int[N];
        for (var i = 0; i < intArray.Length; i++) intArray[i] = rand.Next(0, 1000);
        intStack = new Stack<int>(intArray);
    }

    [IterationSetup]
    public void IterationSetup()
    {
        intStack = new Stack<int>(intArray);
    }

    [Benchmark]
    public void Foreach()
    {
        var res = 0;
        foreach (var value in intArray) res += value;
        tmpRes = res;
    }

    [Benchmark]
    public void For()
    {
        var res = 0;
        for (var i = 0; i < intArray.Length; i++)
            res += intArray[i];
        tmpRes = res;
    }

    [Benchmark]
    public void Stack()
    {
        var res = 0;
        while (intStack.Count > 0) res += intStack.Pop();
        tmpRes = res;
    }

    [Benchmark]
    public void Linq()
    {
        tmpRes = intArray.Sum();
    }

    [Benchmark]
    public void ZLinq()
    {
        tmpRes = intArray.AsValueEnumerable().Sum();
    }

    [Benchmark]
    public void SpanFor()
    {
        var res = 0;
        var span = intArray.AsSpan();
        for (var i = 0; i < span.Length; i++) res += span[i];
        tmpRes = res;
    }

    [Benchmark]
    public void SpanForeach()
    {
        var res = 0;
        var span = intArray.AsSpan();
        foreach (var i in span) res += i;
        tmpRes = res;
    }
}