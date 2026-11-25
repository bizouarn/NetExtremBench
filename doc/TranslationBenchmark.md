# Benchmark : Recherche dans un dictionnaire avec un span (`AlternateLookup`)

## 🎯 Objectif

Mesurer l'intérêt de `Dictionary<string,…>.GetAlternateLookup<ReadOnlySpan<char>>()` (.NET 9+) pour éviter d'allouer une `string` lors d'une recherche.

## 🧱 Code testé

Source : [`Strings/TranslationBenchmark.cs`](../BenchMark/Strings/TranslationBenchmark.cs)

```csharp
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BenchMark.BenchMark
{
    //[SimpleJob(RuntimeMoniker.Net80)]
    [SimpleJob(RuntimeMoniker.Net10_0)]
    [MemoryDiagnoser] // Très important pour voir les allocations !
    public class TranslationBenchmark
    {
        private Dictionary<string, string> _translations = new();
        private List<string> _keys = new();
        private string _longKey = "UI_MENU_MAIN_SETTINGS_BUTTON_LABEL_LONG";
        private Dictionary<string, string>.AlternateLookup<ReadOnlySpan<char>> _lookup;

        [GlobalSetup]
        public void Setup()
        {
            for (int i = 0; i < 100_000; i++)
            {
                var key = $"Key_{i}_{_longKey}";
                _translations.Add(key, $"Traduction {i}");
                _keys.Add(key);
            }

            _lookup = _translations.GetAlternateLookup<ReadOnlySpan<char>>();
        }

        // --- SCÉNARIO 1 : CLASSIQUE (NET 8/10) ---
        [Benchmark(Baseline = true)]
        public string? GetWithString()
        {
            // On simule une entrée string (déjà existante en mémoire)
            string search = _keys[50];
            _translations.TryGetValue(search, out var val);
            return val;
        }

        // --- SCÉNARIO 2 : SPAN SUR .NET 8 (Simulation du fallback) ---
        [Benchmark]
        public string? GetWithSpan_Net8_Fallback()
        {
            // On simule un Span (ex: provenant d'un buffer de lecture)
            ReadOnlySpan<char> spanKey = _keys[50].AsSpan();

            // Simule ton extension en .NET 8 : appelle .ToString()
            return _translations.TryGetValue(spanKey.ToString(), out var val) ? val : null;
        }
#if NET9_0_OR_GREATER
        [Benchmark]
        public string? GetWithSpan_Net10_Optimized()
        {
            ReadOnlySpan<char> spanKey = _keys[50].AsSpan();
            
            // Utilise l'AlternateLookup sans allocation
            return _lookup.TryGetValue(spanKey, out var val) ? val : null;
        }
#endif
    }
}
```

## 📊 Résultats observés

> Mesures en mode `--job short` (ShortRun) sur .NET 10 : indicatives, à confirmer par un run complet. Les jobs .NET 6 / 8 sont absents (runtimes non installés).

| Method | Job | Mean | Ratio | Allocated |
|---|---|---|---|---|
| GetWithString | .NET 10.0 | 15.74 ns | 1.00 | - |
| GetWithSpan_Net8_Fallback | .NET 10.0 | 32.48 ns | 2.06 | 120 B |
| GetWithSpan_Net10_Optimized | .NET 10.0 | 17.73 ns | 1.13 | - |
| GetWithString | ShortRun | 15.97 ns | 1.00 | - |
| GetWithSpan_Net8_Fallback | ShortRun | 32.36 ns | 2.03 | 120 B |
| GetWithSpan_Net10_Optimized | ShortRun | 17.68 ns | 1.11 | - |

## 🔍 Conclusion

- **Constat** : `AlternateLookup` : 0 allocation, ≈ 1,8× plus rapide que `span.ToString()`, +10 % seulement face à une `string` existante.
- **Interprétation** : Le hash est calculé directement sur le span, sans créer de `string`.
- **À retenir** : Utiliser `GetAlternateLookup<ReadOnlySpan<char>>` (.NET 9+) dès qu'on cherche à partir d'un span.
