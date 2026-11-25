# Benchmark : Conversion string → enum

## 🎯 Objectif

Comparer une recherche manuelle dans `Enum.GetValues` à `Enum.TryParse` (string et span).

## 🧱 Code testé

Source : [`Misc/EnumUtils.cs`](../BenchMark/Misc/EnumUtils.cs)

```csharp
using BenchMark.Model;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BenchMark.BenchMark
{
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net80)]
    [MemoryDiagnoser]
    public class EnumUtils
    {
        private List<string> _types;

        public EnumUtils()
        {
            _types = new List<string>();
            foreach (var enumValue in Enum.GetValues(typeof(TypeCompteEnum)))
            {
                _types.Add(enumValue.ToString());
            }
        }

        [Benchmark]
        public void Old()
        {
            var res = false;
            foreach (var str in _types)
            {
                foreach (var enumValue in Enum.GetValues(typeof(TypeCompteEnum)))
                {
                    if (str == enumValue.ToString())
                    {
                        res = true;
                    }
                }
            }
        }

        [Benchmark]
        public void New()
        {
            var res = false;
            foreach (var str in _types)
            {
                Enum.TryParse(typeof(TypeCompteEnum), str, true, out var result);
                if (str == result.ToString())
                {
                    res = true;
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        [Benchmark]
        public void NewSpan()
        {
            var res = false;
            foreach (ReadOnlySpan<char> str in _types)
            {
                var success = Enum.TryParse(typeof(TypeCompteEnum), str, true, out var result);
                if (success)
                {
                    res = true;
                }
                else
                {
                    throw new Exception();
                }
            }
        }
    }
}
```

## 📊 Résultats observés

> Mesures en mode `--job short` (ShortRun) sur .NET 10 : indicatives, à confirmer par un run complet. Les jobs .NET 6 / 8 sont absents (runtimes non installés).

| Method | Job | Runtime | Mean | Allocated |
|---|---|---|---|---|
| Old | ShortRun | .NET 10.0 | 429.55 ns | 432 B |
| New | ShortRun | .NET 10.0 | 112.04 ns | 72 B |
| NewSpan | ShortRun | .NET 10.0 | 66.76 ns | 72 B |

## 🔍 Conclusion

- **Constat** : `TryParse` ≈ 4× plus rapide, la surcharge span ≈ 6,4× avec 6× moins d'allocations.
- **Interprétation** : `GetValues` alloue un tableau et chaque `ToString` une `string` ; `TryParse` n'alloue rien d'autre.
- **À retenir** : Ne jamais parcourir `Enum.GetValues` pour valider ; `Enum.TryParse` (span si possible).
