using BenchmarkDotNet.Attributes;
using System.Collections.Concurrent;

namespace BenchMark.BenchMark;

[MemoryDiagnoser]
public class Mapper
{
    private readonly SimpleMapper _mapper = new();
    private List<EntBureauFiltre> _recs;

    [GlobalSetup]
    public void GlobalSetup()
    {
        // Initialisation de la liste de 10 000 éléments
        _recs = new List<EntBureauFiltre>();
        for (var i = 0; i < 10000; i++) _recs.Add(new EntBureauFiltre { Id = i, Nom = $"Nom {i}" });
    }

    [Benchmark]
    public void GetBureauFiltreBoAllWithForEach()
    {
        var recs = _recs;
        if (recs != null)
        {
            var mapper = _mapper;
            var result = new List<IBureauFiltreBo>();
            recs.ForEach(r => result.Add(mapper.Map(r)));
        }
    }

    [Benchmark]
    public void GetBureauFiltreBoAllWithFoLoop()
    {
        var recs = _recs;
        if (recs != null)
        {
            var mapper = _mapper;
            var result = new List<IBureauFiltreBo>();
            foreach (var r in _recs) result.Add(mapper.Map(r));
        }
    }

    [Benchmark]
    public void GetBureauFiltreBoAllWithParallel()
    {
        var recs = _recs;
        if (recs != null)
        {
            var mapper = _mapper;
            var result = new ConcurrentBag<IBureauFiltreBo>();
            Parallel.ForEach(recs, r => result.Add(mapper.Map(r)));
            var resultList = result.ToList();
        }
    }

    [Benchmark]
    public void GetBureauFiltreBoAllWithSelect()
    {
        var recs = _recs;
        if (recs != null)
        {
            var mapper = _mapper;
            recs.Select(r => mapper.Map(r)).ToList();
        }
    }

    public class EntBureauFiltre
    {
        public int Id { get; set; }
        public string Nom { get; set; }
    }

    public interface IBureauFiltreBo
    {
        int Id { get; }
        string Nom { get; }
    }

    public class SimpleMapper
    {
        public IBureauFiltreBo Map(EntBureauFiltre source)
        {
            return new BureauFiltreBo { Id = source.Id, Nom = source.Nom };
        }
    }

    public class BureauFiltreBo : IBureauFiltreBo
    {
        public int Id { get; set; }
        public string Nom { get; set; }
    }
}