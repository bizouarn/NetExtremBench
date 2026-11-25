# Benchmark : Multiplication sbyte / short / int

## 🎯 Objectif

Vérifier si le type entier utilisé dans une multiplication a un impact (les constantes sont probablement repliées par le compilateur — à lire avec prudence).

## 🧱 Code testé

Source : [`Numeric/Multiply.cs`](../BenchMark/Numeric/Multiply.cs)

```csharp
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BenchMark.BenchMark
{
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net80)]
    [MemoryDiagnoser]
    public class Multiply
    {
        [Benchmark]
        public void sbyteMult()
        {
            var ret = 5 * (sbyte) 1;
        }

        [Benchmark]
        public void shortMult()
        {
            var ret = 5 * (short) 1;
        }

        [Benchmark]
        public void intMult()
        {
            var ret = 5 * (int) 1;
        }
    }
}
```

## 📊 Résultats observés

> Mesures en mode `--job short` (ShortRun) sur .NET 10 : indicatives, à confirmer par un run complet. Les jobs .NET 6 / 8 sont absents (runtimes non installés).

| Method | Job | Runtime | Mean | Allocated |
|---|---|---|---|---|
| sbyteMult | ShortRun | .NET 10.0 | 0.0013 ns | - |
| shortMult | ShortRun | .NET 10.0 | 0.0052 ns | - |
| intMult | ShortRun | .NET 10.0 | 0.0000 ns | - |

## 🔍 Conclusion

- **Constat** : Aucune différence mesurable (≈ 0 ns).
- **Interprétation** : Le JIT réduit ces multiplications à une instruction ; la mesure est sous la résolution.
- **À retenir** : Choisir le type pour sa sémantique, jamais pour la vitesse.
