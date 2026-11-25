# Benchmark : AddRange sur une `ObservableCollection`

## 🎯 Objectif

Mesurer le gain d'un `AddRange` qui s'appuie sur `List<T>.AddRange` (quand `Items` est une `List<T>`) par rapport à une boucle `foreach` + `Items.Add`.

## 🧱 Code testé

Source : [`Collections/AddRangeBenchmark.cs`](../BenchMark/Collections/AddRangeBenchmark.cs)

```csharp
using BenchmarkDotNet.Attributes;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace BenchMark.BenchMark;

// ---------------------------------------------------------------
// Version AVANT optimisation (boucle foreach + Items.Add)
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
// Version APRÈS optimisation (List<T>.AddRange)
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
```

## 📊 Résultats observés

> Mesures en mode `--job short` (ShortRun) sur .NET 10 : indicatives, à confirmer par un run complet. Les jobs .NET 6 / 8 sont absents (runtimes non installés).

| Method | Job | N | Mean | Ratio | Allocated |
|---|---|---|---|---|---|
| Before_Array | ShortRun | 100 | 344.79 ns | 1.00 | 1336 B |
| After_Array | ShortRun | 100 | 53.59 ns | 0.16 | 608 B |
| Before_Array | ShortRun | 1000 | 2,363.22 ns | 1.00 | 8576 B |
| After_Array | ShortRun | 1000 | 308.62 ns | 0.13 | 4208 B |
| Before_Array | ShortRun | 10000 | 26,748.87 ns | 1.00 | 131552 B |
| After_Array | ShortRun | 10000 | 2,520.83 ns | 0.09 | 40208 B |
| Before_Array | ShortRun | 100000 | 290,919.36 ns | 1.00 | 1049424 B |
| After_Array | ShortRun | 100000 | 116,346.32 ns | 0.40 | 400250 B |
| Before_Lazy | ShortRun | 100 | 344.44 ns | 1.00 | 1336 B |
| After_Lazy | ShortRun | 100 | 360.12 ns | 1.05 | 1376 B |
| Before_Lazy | ShortRun | 1000 | 2,811.15 ns | 1.00 | 8576 B |
| After_Lazy | ShortRun | 1000 | 2,941.20 ns | 1.05 | 8616 B |
| Before_Lazy | ShortRun | 10000 | 28,971.47 ns | 1.00 | 131552 B |
| After_Lazy | ShortRun | 10000 | 25,205.81 ns | 0.87 | 131592 B |
| Before_Lazy | ShortRun | 100000 | 304,839.68 ns | 1.00 | 1049399 B |
| After_Lazy | ShortRun | 100000 | 263,715.82 ns | 0.87 | 1049367 B |
| Before_List | ShortRun | 100 | 369.60 ns | 1.00 | 1336 B |
| After_List | ShortRun | 100 | 51.88 ns | 0.14 | 608 B |
| Before_List | ShortRun | 1000 | 2,420.31 ns | 1.00 | 8576 B |
| After_List | ShortRun | 1000 | 309.18 ns | 0.13 | 4208 B |
| Before_List | ShortRun | 10000 | 24,140.56 ns | 1.00 | 131552 B |
| After_List | ShortRun | 10000 | 2,143.13 ns | 0.09 | 40208 B |
| Before_List | ShortRun | 100000 | 264,409.93 ns | 1.00 | 1049614 B |
| After_List | ShortRun | 100000 | 118,554.47 ns | 0.45 | 400250 B |

## 🔍 Conclusion

- **Constat** : `List<T>.AddRange` : 6 à 10× plus rapide et ~2,5× moins d'allocations ; aucun gain avec une source paresseuse.
- **Interprétation** : `AddRange` connaît la taille, redimensionne une fois et copie en bloc au lieu d'une `Add` par élément ; avec LINQ, le coût est l'énumération.
- **À retenir** : Utiliser `List.AddRange` quand `Items` est une `List<T>` ; sinon ne rien attendre.
