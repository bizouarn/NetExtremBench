# Benchmark : `StringBuilder.Append` : char vs string

## 🎯 Objectif

Comparer `Append(char)` et `Append(string)` pour des chaînes d'un seul caractère, selon le nombre d'ajouts.

## 🧱 Code testé

Source : [`Strings/AppendBench.cs`](../BenchMark/Strings/AppendBench.cs)

```csharp
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BenchMark.BenchMark
{
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net80)]
    [MemoryDiagnoser]
    public class AppendBench
    {
        private StringBuilder _sb;

        [Params(10, 100, 10000, 100000)] public int N;

        [GlobalSetup]
        public void Setup()
        {
            _sb = new StringBuilder();
        }

        [Benchmark]
        public void AppendChar()
        {
            _sb.Clear();
            for (int i = 0; i < N; i++)
            {
                _sb.Append(i % 2 == 0 ? 'A' : 'B');
            }
        }

        [Benchmark]
        public void AppendString()
        {
            _sb.Clear();
            for (int i = 0; i < N; i++)
            {
                _sb.Append(i % 2 == 0 ? "A" : "B");
            }
        }
    }
}
```

## 📊 Résultats observés

> Mesures en mode `--job short` (ShortRun) sur .NET 10 : indicatives, à confirmer par un run complet. Les jobs .NET 6 / 8 sont absents (runtimes non installés).

| Method | Job | Runtime | N | Mean | Allocated |
|---|---|---|---|---|---|
| AppendChar | ShortRun | .NET 10.0 | 10 | 13.45 ns | - |
| AppendString | ShortRun | .NET 10.0 | 10 | 17.13 ns | - |
| AppendChar | ShortRun | .NET 10.0 | 100 | 138.20 ns | - |
| AppendString | ShortRun | .NET 10.0 | 100 | 199.39 ns | - |
| AppendChar | ShortRun | .NET 10.0 | 10000 | 11,451.62 ns | - |
| AppendString | ShortRun | .NET 10.0 | 10000 | 19,690.38 ns | - |
| AppendChar | ShortRun | .NET 10.0 | 100000 | 114,770.67 ns | - |
| AppendString | ShortRun | .NET 10.0 | 100000 | 185,798.78 ns | - |

## 🔍 Conclusion

- **Constat** : `Append(char)` ≈ 1,6× plus rapide que `Append(string)`, 0 allocation dans les deux cas.
- **Interprétation** : Une `string` impose une copie via `Span` et des vérifications de longueur.
- **À retenir** : Passer un `char` pour un seul caractère.
