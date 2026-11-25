# Benchmark : Recherche d'un type dans une liste d'objets

## 🎯 Objectif

Comparer `OfType<T>().Any()`, `foreach` + `is`, `for` sur List/array et `Span` pour détecter la présence d'un marqueur (pire cas : à la fin).

## 🧱 Code testé

Source : [`Collections/TriggersBenchmark.cs`](../BenchMark/Collections/TriggersBenchmark.cs)

```csharp
using BenchmarkDotNet.Attributes;

namespace BenchMark.BenchMark
{
    [MemoryDiagnoser]
    public class TriggersBenchmark
    {
        private List<object> _triggersList = null!;
        private object[] _triggersArray = null!;

        [Params(10, 100)]
        public int Size { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            // On remplit avec des objets quelconques, le filtre est à la fin pour tester le pire cas
            var list = new List<object>();
            for (int i = 0; i < Size - 1; i++)
            {
                list.Add(new OtherMarkerAttribute());
            }
            list.Add(new SkipMarkerAttribute());

            _triggersList = list;
            _triggersArray = list.ToArray();
        }

        [Benchmark(Baseline = true)]
        public object? Linq_OfType_Any()
        {
            if (_triggersList.OfType<SkipMarkerAttribute>().Any()) return null!;
            return new object();
        }

        [Benchmark]
        public object? Foreach_Is()
        {
            foreach (var trigger in _triggersList)
            {
                if (trigger is SkipMarkerAttribute) return null!;
            }
            return new object();
        }

        [Benchmark]
        public object? For_List_Is()
        {
            for (int i = 0; i < _triggersList.Count; i++)
            {
                if (_triggersList[i] is SkipMarkerAttribute) return null!;
            }
            return new object();
        }

        [Benchmark]
        public object? For_Array_Is()
        {
            for (int i = 0; i < _triggersArray.Length; i++)
            {
                if (_triggersArray[i] is SkipMarkerAttribute) return null!;
            }
            return new object();
        }

        [Benchmark]
        public object? Span_Is()
        {
            ReadOnlySpan<object> span = _triggersArray;
            foreach (var trigger in span)
            {
                if (trigger is SkipMarkerAttribute) return null!;
            }
            return new object();
        }
    }

    public class SkipMarkerAttribute : Attribute { }
    public class OtherMarkerAttribute : Attribute { }
}
```

## 📊 Résultats observés

> Mesures en mode `--job short` (ShortRun) sur .NET 10 : indicatives, à confirmer par un run complet. Les jobs .NET 6 / 8 sont absents (runtimes non installés).

| Method | Size | Mean | Ratio | Allocated |
|---|---|---|---|---|
| Linq_OfType_Any | 10 | 52.891 ns | 1.00 | 88 B |
| Foreach_Is | 10 | 13.218 ns | 0.25 | - |
| For_List_Is | 10 | 13.873 ns | 0.26 | - |
| For_Array_Is | 10 | 10.067 ns | 0.19 | - |
| Span_Is | 10 | 8.072 ns | 0.15 | - |
| Linq_OfType_Any | 100 | 433.046 ns | 1.00 | 88 B |
| Foreach_Is | 100 | 117.286 ns | 0.27 | - |
| For_List_Is | 100 | 139.291 ns | 0.32 | - |
| For_Array_Is | 100 | 136.497 ns | 0.32 | - |
| Span_Is | 100 | 73.269 ns | 0.17 | - |

## 🔍 Conclusion

- **Constat** : `is` en boucle ≈ 4× plus rapide que `OfType().Any()` ; span ≈ 7×.
- **Interprétation** : LINQ alloue un itérateur et passe par des délégués ; la boucle est inlinée.
- **À retenir** : Pour un simple test d'existence sur un chemin chaud, boucle ou span.
