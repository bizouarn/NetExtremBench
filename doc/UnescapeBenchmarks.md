# Benchmark : Déséchappement de `\n`

## 🎯 Objectif

Comparer `string.Replace` à une version manuelle (comptage puis copie) sur un texte de 100 000 lignes.

## 🧱 Code testé

Source : [`Strings/UnescapeBenchmarks.cs`](../BenchMark/Strings/UnescapeBenchmarks.cs)

```csharp
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BenchMark.BenchMark
{
    [SimpleJob(RuntimeMoniker.Net80)]
    [MemoryDiagnoser]
    [GcServer(true)]
    public class UnescapeBenchmarks
    {
        private string testString;
        private string res;

        [GlobalSetup]
        public void Setup()
        {
            // 100 000 répétitions pour simuler un vrai load
            testString = string.Concat(Enumerable.Repeat("Line1\\nLine2\\nLine3\\n", 100_000));
        }

        [Benchmark]
        public void StringReplace()
        {
            res = testString.Replace("\\n", "\n");
        }

        [Benchmark]
        public void SpanUnescape()
        {
            res = new string(UnescapeChar(testString, '\\', 'n', '\n'));
        }

        private static ReadOnlySpan<char> UnescapeChar(ReadOnlySpan<char> input, char escapeChar1, char escapeChar2, char target)
        {
            var count = 0;
            for (var i = 0; i < input.Length - 1; i++)
            {
                if (input[i] == escapeChar1 && input[i + 1] == escapeChar2)
                {
                    count++;
                    i++;
                }
            }

            if (count == 0) return input;

            var result = new char[input.Length - count];
            var w = 0;
            for (var i = 0; i < input.Length; i++)
            {
                if (i < input.Length - 1 && input[i] == escapeChar1 && input[i + 1] == escapeChar2)
                {
                    result[w++] = target;
                    i++;
                }
                else
                {
                    result[w++] = input[i];
                }
            }

            return result;
        }
    }
}
```

## 📊 Résultats observés

> Mesures en mode `--job short` (ShortRun) sur .NET 10 : indicatives, à confirmer par un run complet. Les jobs .NET 6 / 8 sont absents (runtimes non installés).

| Method | Job | Runtime | Mean | Allocated |
|---|---|---|---|---|
| StringReplace | ShortRun | .NET 10.0 | 4.872 ms | 3.43 MB |
| SpanUnescape | ShortRun | .NET 10.0 | 4.294 ms | 6.87 MB |

## 🔍 Conclusion

- **Constat** : Version manuelle ≈ 12 % plus rapide mais 2× plus d'allocations (6,9 Mo contre 3,4 Mo).
- **Interprétation** : `Replace` est déjà optimisé/vectorisé et alloue une seule `string` ; la version manuelle passe par un `char[]` puis `new string`.
- **À retenir** : Garder `string.Replace` ; optimiser seulement avec `string.Create`.
