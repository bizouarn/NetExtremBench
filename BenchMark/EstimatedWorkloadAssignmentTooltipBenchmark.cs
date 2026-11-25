using BenchmarkDotNet.Attributes;
using System.Text;

namespace BenchMark.BenchMark
{
    public interface ISalarie
    {
        string Id { get; }
        string Nom { get; }
    }

    public record Salarie(string Id, string Nom) : ISalarie;

    public record AffectationChantier(string PersonnelId, DateTime DateHeureDebut, DateTime DateHeureFin);
    
    [MemoryDiagnoser] // Mesure la mémoire allouée et le garbage collection
    public class EstimatedWorkloadAssignmentTooltipBenchmark
    {
        private readonly List<AffectationChantier> _affectationChantiersList = new();
        private readonly Dictionary<string, ISalarie> _cache = new();

        private readonly DateTime _dateHeureDebut = new(2026, 1, 1, 8, 0, 0);
        private readonly DateTime _dateHeureFin = new(2026, 1, 1, 17, 0, 0);

        [Params(0, 2, 10, 100)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            _affectationChantiersList.Clear();
            _cache.Clear();

            var rand = new Random(42);

            // Simulation d'un jeu de données avec doublons
            for (int i = 0; i < N; i++)
            {
                string id = (i % (N / 5)).ToString(); // 5 affectations par salarié en moyenne

                if (!_cache.ContainsKey(id))
                {
                    _cache[id] = new Salarie(id, $"Salarié {id}");
                }

                _affectationChantiersList.Add(new AffectationChantier(
                    PersonnelId: id,
                    DateHeureDebut: _dateHeureDebut.AddHours(rand.Next(-2, 2)),
                    DateHeureFin: _dateHeureFin.AddHours(rand.Next(-2, 2))
                ));
            }
        }

        [Benchmark(Baseline = true)]
        public string Original()
        {
            var result = new StringBuilder();
            var employeeList = new List<ISalarie?>();

            foreach (var c in _affectationChantiersList)
            {
                if (c.DateHeureFin >= _dateHeureDebut && c.DateHeureDebut <= _dateHeureFin)
                {
                    var sal = GetSalarie(c.PersonnelId);
                    if (sal != null && !employeeList.Contains(sal))
                    {
                        employeeList.Add(sal);
                        BuildTooltipLine(sal, ref result);
                    }
                }
            }

            return result.ToString();
        }

        [Benchmark]
        public string Optimized()
        {
            if (_affectationChantiersList.Count > 0)
            {
                var result = new StringBuilder();
                var processedPersonnelIds = new HashSet<string>();

                foreach (var c in _affectationChantiersList)
                {
                    if (c.DateHeureFin >= _dateHeureDebut
                        && c.DateHeureDebut <= _dateHeureFin
                        && processedPersonnelIds.Add(c.PersonnelId))
                    {
                        var sal = GetSalarie(c.PersonnelId);
                        if (sal != null)
                        {
                            BuildTooltipLine(sal, ref result);
                        }
                    }
                }

                return result.ToString();
            }

            return string.Empty;
        }

        private ISalarie? GetSalarie(string id) => _cache.TryGetValue(id, out var sal) ? sal : null;

        private static void BuildTooltipLine(ISalarie sal, ref StringBuilder sb)
        {
            sb.Append("- ").Append(sal.Nom).AppendLine();
        }
    }
}
