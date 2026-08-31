using BenchmarkDotNet.Attributes;
using System.Text;

namespace BenchMark.BenchMark
{
    [MemoryDiagnoser]
    public class StringBuilderResetBenchmark
    {
        // Simule le contenu réellement loggé par les aspects (thread, indentation, méthode, params...)
        private const string Chunk = "[Thread:12] MaMethode(param1=valeur1, param2=valeur2, param3=valeur3) -> "; // ~75 car.
        private const string Chunk2 = "duration=12.345ms result=OK correlationId=8f14e45f-ceea-467e-b5a6-0d1234567890"; // complète jusqu'à ~200

        [Params(100_000)]
        public int Iterations;

        private StringBuilder _sbForClearScenario = new StringBuilder();

        [GlobalSetup]
        public void Setup()
        {
            // On force le buffer à une capacité "chauffée" comme dans le vrai code
            // (l'aspect tourne déjà depuis un moment quand ce chemin est emprunté)
            _sbForClearScenario = new StringBuilder();
            Fill(_sbForClearScenario);
            _sbForClearScenario.Clear();
        }

        private static void Fill(StringBuilder sb)
        {
            sb.Append(Chunk).Append(Chunk2);
        }

        // --- Scénario AVANT le patch : sb = new StringBuilder() ---
        [Benchmark(Baseline = true)]
        public int Baseline_NewStringBuilder()
        {
            int totalLength = 0;
            var sb = new StringBuilder();

            for (int i = 0; i < Iterations; i++)
            {
                Fill(sb);                 // 1er remplissage (~200 car.)
                totalLength += sb.Length;

                sb = new StringBuilder(); // <-- ligne remplacée par le patch

                Fill(sb);                 // 2e remplissage (~200 car.)
                totalLength += sb.Length;
            }

            return totalLength;
        }

        // --- Scénario APRÈS le patch : sb.Clear() ---
        [Benchmark]
        public int Patched_Clear()
        {
            int totalLength = 0;
            var sb = new StringBuilder();

            for (int i = 0; i < Iterations; i++)
            {
                Fill(sb);
                totalLength += sb.Length;

                sb.Clear();                // <-- le patch

                Fill(sb);
                totalLength += sb.Length;
            }

            return totalLength;
        }
    }

}
