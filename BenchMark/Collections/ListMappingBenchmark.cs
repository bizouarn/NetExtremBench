using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using System.Collections.Concurrent;

namespace BenchMark.BenchMark;

/// <summary>
/// Compare plusieurs façons de mapper une liste d'entités vers une liste de DTO :
/// LINQ (Select + ToList / ToArray), List.ConvertAll, foreach, List.ForEach et Parallel.ForEach.
/// </summary>
[MemoryDiagnoser]
public class ListMappingBenchmark
{
    private List<SourceEntity> _sourceData = null!;

    // Exposé en IEnumerable pour simuler un appelant qui ne connaît pas le type concret.
    private IEnumerable<SourceEntity> SourceData => _sourceData;

    private IEntityMapper _mapper = null!;

    [Params(10, 100_000)]
    public int Count;

    [GlobalSetup]
    public void Setup()
    {
        _sourceData = Enumerable.Range(1, Count)
            .Select(i => new SourceEntity { Id = i, Label = $"Item_{i}" })
            .ToList();
        _mapper = new EntityMapper();
    }

    [Benchmark]
    public List<TargetDto> Select_ToList() => SourceData.Select(_mapper.Map).ToList();

    [Benchmark(Baseline = true)]
    public TargetDto[] Select_ToArray() => SourceData.Select(_mapper.Map).ToArray();

    [Benchmark]
    public List<TargetDto> ConvertAll() => SourceData.AsList().ConvertAll(_mapper.Map);

    [Benchmark]
    public List<TargetDto> Foreach()
    {
        var result = new List<TargetDto>();
        foreach (var item in _sourceData) result.Add(_mapper.Map(item));
        return result;
    }

    [Benchmark]
    public List<TargetDto> Foreach_PreSized()
    {
        var result = new List<TargetDto>(_sourceData.Count);
        foreach (var item in _sourceData) result.Add(_mapper.Map(item));
        return result;
    }

    [Benchmark]
    public List<TargetDto> ListForEach()
    {
        var result = new List<TargetDto>();
        _sourceData.ForEach(item => result.Add(_mapper.Map(item)));
        return result;
    }

    [Benchmark]
    public List<TargetDto> ParallelForEach()
    {
        var result = new ConcurrentBag<TargetDto>();
        Parallel.ForEach(_sourceData, item => result.Add(_mapper.Map(item)));
        return result.ToList();
    }
}

public class SourceEntity
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
}

public class TargetDto
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
}

public interface IEntityMapper
{
    TargetDto Map(SourceEntity src);
}

public class EntityMapper : IEntityMapper
{
    public TargetDto Map(SourceEntity src) => new() { Id = src.Id, Label = src.Label };
}

internal static class ListExt
{
    /// <summary>
    /// Retourne la source telle quelle si c'est déjà une <see cref="List{T}"/>, sinon appelle ToList().
    /// </summary>
    public static List<T> AsList<T>(this IEnumerable<T>? source) => source switch
    {
        null => null!,
        List<T> list => list,
        _ => Enumerable.ToList(source),
    };
}
