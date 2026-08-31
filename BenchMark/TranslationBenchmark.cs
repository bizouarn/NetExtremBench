using System.Collections.Concurrent;
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
