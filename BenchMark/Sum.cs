using BenchmarkDotNet.Attributes;

namespace BenchMark.BenchMark;

[MemoryDiagnoser]
public class Sum
{
    private readonly int[] intArray;
    private Stack<int> intStack;
    private int tmpRes;

    public Sum()
    {
        var size = 1000;
        var rand = new Random();
        intArray = new int[size];
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
    public void Link()
    {
        var res = intArray.Sum();
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