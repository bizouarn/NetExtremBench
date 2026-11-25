# Benchmark : Hash d'un dictionnaire : LINQ vs ArrayPool + Span.Sort

## 🎯 Objectif

Calculer un hash stable des entrées d'un dictionnaire : `Where().Order()` (comparateur culturel) vs `ArrayPool` + `Span.Sort(StringComparer.Ordinal)`.

## 🧱 Code testé

Source : [`Misc/GetHashCodeBenchmark.cs`](../BenchMark/Misc/GetHashCodeBenchmark.cs)

```csharp
using BenchmarkDotNet.Attributes;
using System.Buffers;

namespace BenchMark.BenchMark;

[MemoryDiagnoser]
[DisassemblyDiagnoser]
public class GetHashCodeBenchmark
{
    // Nombre de colonnes dans la ligne simulée.
    // Ajustez cette liste pour tester plusieurs tailles de dataset.
    [Params(5, 20, 100, 200, 500)]
    public int ColumnCount;

    // Proportion de clés exclues par IsNotExcluded (ex: colonnes techniques)
    private const int ExcludedEvery = 7;

    private Dictionary<string, string> _data = null!;

    [GlobalSetup]
    public void Setup()
    {
        _data = new Dictionary<string, string>(ColumnCount);
        for (int i = 0; i < ColumnCount; i++)
        {
            // Mélange volontaire majuscules/minuscules pour exposer
            // la différence Ordinal vs culture-sensible si elle existe.
            string key = (i % 2 == 0 ? "Col_" : "col_") + i;
            _data[key] = $"Value{i}";
        }
    }

    private bool IsNotExcluded(string key)
    {
        // Filtre simplifié
        return !key.EndsWith(ExcludedEvery.ToString());
    }

    [Benchmark(Baseline = true)]
    public int Old_LinqWhereOrder()
    {
        var hash = new HashCode();

        var relevantColumns = _data.Keys
            .Where(IsNotExcluded)
            .Order(); // comparateur par défaut (sensible à la culture)

        foreach (var column in relevantColumns)
        {
            hash.Add(column);
            hash.Add(_data[column]);
        }

        return hash.ToHashCode();
    }

    [Benchmark]
    public int New_ArrayPoolSpanSort()
    {
        var hash = new HashCode();
        string[] rentedArray = ArrayPool<string>.Shared.Rent(_data.Keys.Count);
        try
        {
            int count = 0;
            foreach (var key in _data.Keys)
            {
                if (IsNotExcluded(key))
                {
                    rentedArray[count++] = key;
                }
            }

            Span<string> activeKeys = rentedArray.AsSpan(0, count);
            activeKeys.Sort(StringComparer.Ordinal);

            foreach (var key in activeKeys)
            {
                hash.Add(key);
                hash.Add(_data[key]);
            }
        }
        finally
        {
            ArrayPool<string>.Shared.Return(rentedArray);
        }

        return hash.ToHashCode();
    }

    [Benchmark]
    public int New_ArraySpanSort_NoPool()
    {
        var hash = new HashCode();

        var relevantColumns = _data.Keys.ToArray();
        Array.Sort(relevantColumns, StringComparer.Ordinal);

        foreach (var column in relevantColumns)
        {
            if (IsNotExcluded(column))
            {
                hash.Add(column);
                hash.Add(_data[column]);
            }
        }

        return hash.ToHashCode();
    }
}
```

## 📊 Résultats observés

> Mesures en mode `--job short` (ShortRun) sur .NET 10 : indicatives, à confirmer par un run complet. Les jobs .NET 6 / 8 sont absents (runtimes non installés).

| Method | ColumnCount | Mean | Ratio | Code Size | Allocated |
|---|---|---|---|---|---|
| Old_LinqWhereOrder | 5 | 3.226 μs | 1.00 | 7,291 B | 512 B |
| New_ArrayPoolSpanSort | 5 | 3.002 μs | 0.93 | 6,588 B | 64 B |
| New_ArraySpanSort_NoPool | 5 | 3.069 μs | 0.95 | 4,963 B | 128 B |
| Old_LinqWhereOrder | 20 | 14.437 μs | 1.00 | 10,252 B | 664 B |
| New_ArrayPoolSpanSort | 20 | 12.132 μs | 0.84 | 6,617 B | 64 B |
| New_ArraySpanSort_NoPool | 20 | 12.447 μs | 0.86 | 5,554 B | 248 B |
| Old_LinqWhereOrder | 100 | 81.306 μs | 1.00 | 11,186 B | 1528 B |
| New_ArrayPoolSpanSort | 100 | 62.829 μs | 0.77 | 6,914 B | 64 B |
| New_ArraySpanSort_NoPool | 100 | 63.033 μs | 0.78 | 5,217 B | 888 B |
| Old_LinqWhereOrder | 200 | 173.919 μs | 1.00 | 11,371 B | 2608 B |
| New_ArrayPoolSpanSort | 200 | 126.575 μs | 0.73 | 7,082 B | 64 B |
| New_ArraySpanSort_NoPool | 200 | 127.821 μs | 0.73 | 5,470 B | 1688 B |
| Old_LinqWhereOrder | 500 | 458.274 μs | 1.00 | 11,369 B | 5848 B |
| New_ArrayPoolSpanSort | 500 | 324.143 μs | 0.71 | 7,098 B | 64 B |
| New_ArraySpanSort_NoPool | 500 | 332.057 μs | 0.72 | 5,423 B | 4088 B |

## 🔍 Conclusion

- **Constat** : Gain de 7 % (5 colonnes) à 29 % (500) ; allocations réduites à 64 B constants.
- **Interprétation** : Pas d'itérateurs/`Order()` culturel ; tri `Ordinal` sur un span.
- **À retenir** : Adopter pour les grosses lignes, en sachant que l'ordre de tri change (hash différent d'avant).
