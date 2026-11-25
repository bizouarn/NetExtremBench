# Benchmark : Initialisation d'un `Dictionary` avec ou sans capacité

## 🎯 Objectif

Mesurer l'intérêt de passer la capacité initiale au constructeur quand le nombre d'éléments est connu.

## 🧱 Code testé

Source : [`Collections/DictInit.cs`](../BenchMark/Collections/DictInit.cs)

```csharp
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BenchMark.BenchMark
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net80)]
    public class DictInit
    {
        [Benchmark]
        public Dictionary<int, int> InitDirect()
        {
            var d = new Dictionary<int, int>(10);
            for (int i = 0; i < 10; i++)
                d[i] = i;
            return d;
        }

        [Benchmark]
        public Dictionary<int, int> InitProgressive()
        {
            var d = new Dictionary<int, int>();
            for (int i = 0; i < 10; i++)
                d[i] = i;
            return d;
        }
    }

}
```

## 📊 Résultats observés

> Mesures en mode `--job short` (ShortRun) sur .NET 10 : indicatives, à confirmer par un run complet. Les jobs .NET 6 / 8 sont absents (runtimes non installés).

| Method | Job | Runtime | Mean | Allocated |
|---|---|---|---|---|
| InitDirect | ShortRun | .NET 10.0 | 63.89 ns | 352 B |
| InitProgressive | ShortRun | .NET 10.0 | 124.66 ns | 776 B |

## 🔍 Conclusion

- **Constat** : Avec capacité : 2× plus rapide, 2,2× moins d'allocations.
- **Interprétation** : Sans capacité, le dictionnaire se redimensionne et re-hache plusieurs fois.
- **À retenir** : Toujours passer la capacité si elle est connue.
