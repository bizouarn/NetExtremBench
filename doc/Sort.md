# Benchmark : Tri de chaînes : `List.Sort` vs LINQ `OrderBy`

## 🎯 Objectif

Comparer le tri en place d'une `List<string>` à `OrderBy().ToList()` sur 1 M d'éléments.

## 🧱 Code testé

Source : [`Collections/Sort.cs`](../BenchMark/Collections/Sort.cs)

```csharp
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
```

## 📊 Résultats observés

> Mesures en mode `--job short` (ShortRun) sur .NET 10 : indicatives, à confirmer par un run complet. Les jobs .NET 6 / 8 sont absents (runtimes non installés).

| Method | Mean | Allocated |
|---|---|---|
| SortList | 147.0 ms | - |
| OrderByLinq | 196.7 ms | 28000512 B |

## 🔍 Conclusion

- **Constat** : `List.Sort` ≈ 1,3× plus rapide, sans allocation (contre 28 Mo).
- **Interprétation** : Tri en place, sans copie ; `OrderBy` bufferise puis `ToList` recopie.
- **À retenir** : `Sort()` si la modification en place est acceptable.
