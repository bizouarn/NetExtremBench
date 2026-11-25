# Benchmark : `ToDictionary` avec clé `Tuple`

## 🎯 Objectif

Comparer `ToDictionary(keySelector, valueSelector)` à `ToDictionary(keySelector)` quand la valeur est l'élément lui-même (les `Tuple` référence allouent à chaque clé).

## 🧱 Code testé

Source : [`Collections/ToDictionary.cs`](../BenchMark/Collections/ToDictionary.cs)

```csharp
using BenchmarkDotNet.Attributes;

namespace BenchMark.BenchMark
{
    public class Item
    {
        public string Code { get; set; }
        public int Number { get; set; }
    }

    public class ToDictionary
    {
        private List<Item> _items;

        [Params(1_000, 100_000, 1_000_000)] // tailles testées
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            _items = Enumerable.Range(1, N)
                .Select(i => new Item
                {
                    Code = "M" + (i % 100),
                    Number = i
                })
                .ToList();
        }

        [Benchmark]
        public Dictionary<Tuple<string, int>, Item> WithValueSelector()
        {
            return _items.ToDictionary(
                static mf => Tuple.Create(mf.Code, mf.Number),
                static mf => mf
            );
        }

        [Benchmark]
        public Dictionary<Tuple<string, int>, Item> WithoutValueSelector()
        {
            return _items.ToDictionary(
                static mf => Tuple.Create(mf.Code, mf.Number)
            );
        }
    }
}
```

## 📊 Résultats observés

> Mesures en mode `--job short` (ShortRun) sur .NET 10 : indicatives, à confirmer par un run complet. Les jobs .NET 6 / 8 sont absents (runtimes non installés).

| Method | N | Mean |
|---|---|---|
| WithValueSelector | 1000 | 32.08 μs |
| WithoutValueSelector | 1000 | 31.13 μs |
| WithValueSelector | 100000 | 23,292.08 μs |
| WithoutValueSelector | 100000 | 23,406.72 μs |
| WithValueSelector | 1000000 | 272,695.35 μs |
| WithoutValueSelector | 1000000 | 265,073.95 μs |

## 🔍 Conclusion

- **Constat** : Aucune différence (~1 %).
- **Interprétation** : Le coût est dominé par l'allocation des `Tuple` et le hachage de la clé.
- **À retenir** : Chercher à réduire le coût de la clé (`ValueTuple`, `struct`), pas le sélecteur de valeur.
