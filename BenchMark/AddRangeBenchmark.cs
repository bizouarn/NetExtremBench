using BenchmarkDotNet.Attributes;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace BenchMark.BenchMark;

// ---------------------------------------------------------------
// Version AVANT le patch (boucle foreach + Items.Add)
// ---------------------------------------------------------------
public class RangeCollectionBefore<T> : ObservableCollection<T>
{
    private bool _suppressNotifications;

    public void AddRange(IEnumerable<T> items)
    {
        CheckReentrancy();
        _suppressNotifications = true;
        try
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
        }
        finally
        {
            _suppressNotifications = false;
        }

        OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}

// ---------------------------------------------------------------
// Version APRÈS le patch (List<T>.AddRange)
// ---------------------------------------------------------------
public class RangeCollectionAfter<T> : ObservableCollection<T>
{
    private bool _suppressNotifications;

    public void AddRange(IEnumerable<T> items)
    {
        CheckReentrancy();
        _suppressNotifications = true;
        try
        {
            if (items != null)
            {
                if (Items is List<T> list)
                {
                    list.AddRange(items);
                }
                else
                {
                    foreach (var item in items)
                    {
                        Items.Add(item);
                    }
                }
            }
        }
        finally
        {
            _suppressNotifications = false;
        }

        OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}


[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByCategory)]
[SimpleJob(launchCount: 1, warmupCount: 2, iterationCount: 3)]
public class AddRangeBenchmark
{
    [Params(100, 1_000, 10_000, 100_000)]
    public int N;

    private List<int> _listSource = null!;
    private int[] _arraySource = null!;

    [GlobalSetup]
    public void Setup()
    {
        _listSource = Enumerable.Range(0, N).ToList();
        _arraySource = _listSource.ToArray();
    }

    // Source paresseuse : la taille n'est pas connue à l'avance
    private IEnumerable<int> Lazy()
    {
        for (int i = 0; i < N; i++) yield return i;
    }

    // ----- Source List<int> -----
    [BenchmarkCategory("List"), Benchmark(Baseline = true)]
    public int Before_List()
    {
        var c = new RangeCollectionBefore<int>();
        c.AddRange(_listSource);
        return c.Count;
    }

    [BenchmarkCategory("List"), Benchmark]
    public int After_List()
    {
        var c = new RangeCollectionAfter<int>();
        c.AddRange(_listSource);
        return c.Count;
    }

    // ----- Source int[] -----
    [BenchmarkCategory("Array"), Benchmark(Baseline = true)]
    public int Before_Array()
    {
        var c = new RangeCollectionBefore<int>();
        c.AddRange(_arraySource);
        return c.Count;
    }

    [BenchmarkCategory("Array"), Benchmark]
    public int After_Array()
    {
        var c = new RangeCollectionAfter<int>();
        c.AddRange(_arraySource);
        return c.Count;
    }

    // ----- Source IEnumerable paresseuse -----
    [BenchmarkCategory("Lazy"), Benchmark(Baseline = true)]
    public int Before_Lazy()
    {
        var c = new RangeCollectionBefore<int>();
        c.AddRange(Lazy());
        return c.Count;
    }

    [BenchmarkCategory("Lazy"), Benchmark]
    public int After_Lazy()
    {
        var c = new RangeCollectionAfter<int>();
        c.AddRange(Lazy());
        return c.Count;
    }
}
