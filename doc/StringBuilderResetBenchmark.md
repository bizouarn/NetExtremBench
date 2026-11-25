# Benchmark : Réutiliser un `StringBuilder` : `Clear()` vs `new`

## 🎯 Objectif

Mesurer les allocations évitées en réutilisant le buffer d'un `StringBuilder` plutôt qu'en en créant un nouveau.

## 🧱 Code testé

Source : [`Strings/StringBuilderResetBenchmark.cs`](../BenchMark/Strings/StringBuilderResetBenchmark.cs)

```csharp
using BenchmarkDotNet.Attributes;
using System.Text;

namespace BenchMark.BenchMark
{
    [MemoryDiagnoser]
    public class StringBuilderResetBenchmark
    {
        // Simule le contenu typique d'un message de log (thread, indentation, méthode, params...)
        private const string Chunk = "[Thread:12] MaMethode(param1=valeur1, param2=valeur2, param3=valeur3) -> "; // ~75 car.
        private const string Chunk2 = "duration=12.345ms result=OK correlationId=8f14e45f-ceea-467e-b5a6-0d1234567890"; // complète jusqu'à ~200

        [Params(100_000)]
        public int Iterations;

        private StringBuilder _sbForClearScenario = new StringBuilder();

        [GlobalSetup]
        public void Setup()
        {
            // On force le buffer à une capacité "chauffée" comme en situation réelle
            // (l'application tourne déjà depuis un moment)
            _sbForClearScenario = new StringBuilder();
            Fill(_sbForClearScenario);
            _sbForClearScenario.Clear();
        }

        private static void Fill(StringBuilder sb)
        {
            sb.Append(Chunk).Append(Chunk2);
        }

        // --- Scénario AVANT : sb = new StringBuilder() ---
        [Benchmark(Baseline = true)]
        public int Before_NewStringBuilder()
        {
            int totalLength = 0;
            var sb = new StringBuilder();

            for (int i = 0; i < Iterations; i++)
            {
                Fill(sb);                 // 1er remplissage (~200 car.)
                totalLength += sb.Length;

                sb = new StringBuilder(); // <-- remplacé par Clear() dans la version "après"

                Fill(sb);                 // 2e remplissage (~200 car.)
                totalLength += sb.Length;
            }

            return totalLength;
        }

        // --- Scénario APRÈS : sb.Clear() ---
        [Benchmark]
        public int After_Clear()
        {
            int totalLength = 0;
            var sb = new StringBuilder();

            for (int i = 0; i < Iterations; i++)
            {
                Fill(sb);
                totalLength += sb.Length;

                sb.Clear(); // <-- réutilisation du buffer

                Fill(sb);
                totalLength += sb.Length;
            }

            return totalLength;
        }
    }

}
```

## 📊 Résultats observés

> Mesures en mode `--job short` (ShortRun) sur .NET 10 : indicatives, à confirmer par un run complet. Les jobs .NET 6 / 8 sont absents (runtimes non installés).

| Method | Iterations | Mean | Ratio | Allocated |
|---|---|---|---|---|
| Before_NewStringBuilder | 100000 | 9.503 ms | 1.00 | 88281.35 KB |
| After_Clear | 100000 | 1.832 ms | 0.19 | 1.82 KB |

## 🔍 Conclusion

- **Constat** : `Clear()` ≈ 5× plus rapide, 88 Mo → 1,8 Ko alloués.
- **Interprétation** : La capacité du buffer est conservée, plus de nouvelle allocation ni de GC.
- **À retenir** : Réutiliser le `StringBuilder` (`Clear()`) dans les boucles.
