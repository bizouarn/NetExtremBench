using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace BenchMark.BenchMark;

[MemoryDiagnoser]
public class ListMappingBenchmark
{
    private List<EntHoraireTranche> _sourceData = null!;

    private IEnumerable<EntHoraireTranche> SourceData
    {
        get { return _sourceData; }
        set { _ = value; }
    }

    private IHoraireTrancheBoMapper _mapper = null!;
    private static readonly Consumer Consumer = new Consumer();

    [Params(10, 100_000)]
    public int Count;

    [GlobalSetup]
    public void Setup()
    {
        _sourceData = Enumerable.Range(1, Count)
            .Select(i => new EntHoraireTranche { Id = i, Libelle = $"Tranche_{i}" })
            .ToList();

        // Remplace par la création de ton vrai mapper via _superFactory si nécessaire
        _mapper = new HoraireTrancheBoMapper();
    }

    [Benchmark]
    public List<BoHoraireTranche> Select_ToList()
    {
        // Equivalent de ta version LINQ + Select (évalué ici via ToList)
        return SourceData.Select(_mapper.Map).ToList();
    }

    [Benchmark(Baseline = true)]
    public BoHoraireTranche[] Select_ToArray()
    {
        // Equivalent de ta version LINQ + Select (évalué ici via ToList)
        return SourceData.Select(_mapper.Map).ToArray();
    }

    [Benchmark]
    public List<BoHoraireTranche> ConvertAll()
    {
        // Version optimisée directe sur List<T>
        return SourceData.AsList().ConvertAll(_mapper.Map);
    }
}

public class EntHoraireTranche
{
    public int Id { get; set; }
    public string Libelle { get; set; } = string.Empty;
}

public class BoHoraireTranche
{
    public int Id { get; set; }
    public string Libelle { get; set; } = string.Empty;
}

public interface IHoraireTrancheBoMapper
{
    BoHoraireTranche Map(EntHoraireTranche src);
}

public class HoraireTrancheBoMapper : IHoraireTrancheBoMapper
{
    public BoHoraireTranche Map(EntHoraireTranche src)
        => new() { Id = src.Id, Libelle = src.Libelle };
}

internal static class SqlExt
{
    /// <summary>
    /// Obtains the data as a list; if it is *already* a list, the original object is returned without
    /// any duplication; otherwise, ToList() is invoked.
    /// </summary>
    /// <typeparam name="T">The type of element in the list.</typeparam>
    /// <param name="source">The enumerable to return as a list.</param>
    public static List<T> AsList<T>(this IEnumerable<T>? source) => source switch
    {
        null => null!,
        List<T> list => list,
        _ => Enumerable.ToList(source),
    };
}