using BenchmarkDotNet.Attributes;
using System.Text;

namespace BenchMark.BenchMark
{
    public interface IWorker
    {
        string Id { get; }
        string Name { get; }
    }

    public record Worker(string Id, string Name) : IWorker;

    public record Assignment(string WorkerId, DateTime Start, DateTime End);
    
    [MemoryDiagnoser] // Mesure la mémoire allouée et le garbage collection
    public class DistinctTooltipBenchmark
    {
        private readonly List<Assignment> _assignments = new();
        private readonly Dictionary<string, IWorker> _cache = new();

        private readonly DateTime _start = new(2026, 1, 1, 8, 0, 0);
        private readonly DateTime _end = new(2026, 1, 1, 17, 0, 0);

        [Params(0, 2, 10, 100)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            _assignments.Clear();
            _cache.Clear();

            var rand = new Random(42);

            // Simulation d'un jeu de données avec doublons
            for (int i = 0; i < N; i++)
            {
                string id = (i % Math.Max(1, N / 5)).ToString(); // 5 affectations par worker en moyenne

                if (!_cache.ContainsKey(id))
                {
                    _cache[id] = new Worker(id, $"Worker {id}");
                }

                _assignments.Add(new Assignment(
                    WorkerId: id,
                    Start: _start.AddHours(rand.Next(-2, 2)),
                    End: _end.AddHours(rand.Next(-2, 2))
                ));
            }
        }

        [Benchmark(Baseline = true)]
        public string Original()
        {
            var result = new StringBuilder();
            var seenWorkers = new List<IWorker?>();

            foreach (var c in _assignments)
            {
                if (c.End >= _start && c.Start <= _end)
                {
                    var worker = GetWorker(c.WorkerId);
                    if (worker != null && !seenWorkers.Contains(worker))
                    {
                        seenWorkers.Add(worker);
                        BuildTooltipLine(worker, ref result);
                    }
                }
            }

            return result.ToString();
        }

        [Benchmark]
        public string Optimized()
        {
            if (_assignments.Count > 0)
            {
                var result = new StringBuilder();
                var processedWorkerIds = new HashSet<string>();

                foreach (var c in _assignments)
                {
                    if (c.End >= _start
                        && c.Start <= _end
                        && processedWorkerIds.Add(c.WorkerId))
                    {
                        var worker = GetWorker(c.WorkerId);
                        if (worker != null)
                        {
                            BuildTooltipLine(worker, ref result);
                        }
                    }
                }

                return result.ToString();
            }

            return string.Empty;
        }

        private IWorker? GetWorker(string id) => _cache.TryGetValue(id, out var worker) ? worker : null;

        private static void BuildTooltipLine(IWorker worker, ref StringBuilder sb)
        {
            sb.Append("- ").Append(worker.Name).AppendLine();
        }
    }
}
