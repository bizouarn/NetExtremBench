# Benchmark : Échappement de caractères interdits

## 🎯 Objectif

Comparer deux versions d'une normalisation de nom (`IndexOfAny` sur un tableau statique vs reconstruit à chaque appel) sur chaîne propre et chaîne à corriger.

## 🧱 Code testé

Source : [`Strings/NormalizationBenchmark.cs`](../BenchMark/Strings/NormalizationBenchmark.cs)

```csharp
using BenchmarkDotNet.Attributes;

namespace BenchMark.BenchMark
{
    [MemoryDiagnoser] // Permet de voir l'impact sur la mémoire (GC)
    public class NormalizationBenchmark
    {
        private static readonly Dictionary<string, string> ForbiddenCharacter = new Dictionary<string, string>
        {
            {".", "&sdot;"},
            {" ", "&space;"},
            {"\t", "&tab;"},
            {"/", "&slash;"}
        };

        private static readonly char[] ForbiddenCharsSearch = ['.', ' ', '\t', '/'];

        // Jeu de données pour le test
        private const string SafeString = "NomDeMembreParfaitementValide";
        private const string UnsafeString = "Mon.Nom / De\tMembre";

        // --- V1 : Vérifie d'abord IndexOfAny, puis remplace ---
        [Benchmark]
        public string V1_WithContainsCheck_Unsafe()
        {
            // On teste avec une chaîne qui a besoin de remplacements
            return NormaliseV1(UnsafeString);
        }

        [Benchmark]
        public string V1_WithContainsCheck_SafeString()
        {
            // On teste avec une chaîne propre
            return NormaliseV1(SafeString);
        }

        // --- V2 : Reconstruit le tableau de caractères à chaque appel ---
        [Benchmark]
        public string V2_DirectReplace_Unsafe() => NormaliseV2(UnsafeString);

        [Benchmark]
        public string V2_DirectReplace_Safe() => NormaliseV2(SafeString);


        private static string NormaliseV1(string member)
        {
            if (member.IndexOfAny(ForbiddenCharsSearch) == -1) return member;
            foreach (var @char in ForbiddenCharacter) member = member.Replace(@char.Key, @char.Value);
            return member;
        }

        private static string NormaliseV2(string member)
        {
            if (member.IndexOfAny(ForbiddenCharacter.Keys.Select(x => x[0]).ToArray()) == -1) return member;
            foreach (var @char in ForbiddenCharacter) member = member.Replace(@char.Key, @char.Value);
            return member;
        }
    }
}
```

## 📊 Résultats observés

> Mesures en mode `--job short` (ShortRun) sur .NET 10 : indicatives, à confirmer par un run complet. Les jobs .NET 6 / 8 sont absents (runtimes non installés).

| Method | Mean | Allocated |
|---|---|---|
| V1_WithContainsCheck_Unsafe | 127.478 ns | 392 B |
| V1_WithContainsCheck_SafeString | 5.221 ns | - |
| V2_DirectReplace_Unsafe | 160.168 ns | 480 B |
| V2_DirectReplace_Safe | 36.577 ns | 88 B |

## 🔍 Conclusion

- **Constat** : Le test préalable `IndexOfAny` rend le cas « chaîne propre » ~7× plus rapide sans allocation ; V2 alloue même sans rien à remplacer.
- **Interprétation** : V2 reconstruit un tableau à chaque appel ; V1 réutilise un tableau statique.
- **À retenir** : Court-circuiter par un test rapide avec des données statiques.
