using BenchmarkDotNet.Attributes;

namespace BenchMark;

[MemoryDiagnoser]
public class SumBenchmark
{
    private readonly int[] intArray;
    private Stack<int> intStack;
    private int tmpRes;

    public SumBenchmark()
    {
        var size = 100;
        intArray = new int[size];
        for (var i = 0; i < intArray.Length; i++) intArray[i] = i;
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
}