using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

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

        // --- V1 : Ta première logique (Corrigée avec le "!") ---
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

        // --- V2 : Ta deuxième logique (Directement les Replace) ---
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
