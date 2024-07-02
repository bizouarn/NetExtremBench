using BenchmarkDotNet.Attributes;

namespace BenchMark.BenchMark;

[MemoryDiagnoser]
public class Contains
{
    private const int _max = 100000;
    private readonly int[] _array = Enumerable.Range(0, _max).ToArray();
    private readonly List<int> _list = Enumerable.Range(0, _max).ToList();
    private bool exist;

    [Benchmark]
    public void LinkContain()
    {
        exist = LinkContain(_array, _max);
    }

    public bool LinkContain(int[] array, int value)
    {
        return array.Contains(value);
    }

    [Benchmark]
    public void IndexContain()
    {
        exist = IndexContain(_array, _max);
    }

    public bool IndexContain(int[] array, int value)
    {
        return Array.IndexOf(array, value) > -1;
    }

    [Benchmark]
    public void SpanContain()
    {
        SpanContain(_array, _max);
    }

    public bool SpanContain(int[] array, int value)
    {
        return ((ReadOnlySpan<int>) array).Contains(value);
    }

    [Benchmark]
    public void ListContain()
    {
        exist = ListContain(_list, _max);
    }

    public bool ListContain(List<int> list, int value)
    {
        return list.Contains(value);
    }
}