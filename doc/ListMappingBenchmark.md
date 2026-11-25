# Benchmark : Mapping d'une liste d'entités vers des DTO

## 🎯 Objectif

Comparer `Select().ToList()`, `Select().ToArray()`, `List.ConvertAll`, `foreach`, `List.ForEach` et `Parallel.ForEach`.

## 🧱 Code testé

Source : [`Collections/ListMappingBenchmark.cs`](../BenchMark/Collections/ListMappingBenchmark.cs)

```csharp
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
```

## 📊 Résultats observés

> Mesures en mode `--job short` (ShortRun) sur .NET 10 : indicatives, à confirmer par un run complet. Les jobs .NET 6 / 8 sont absents (runtimes non installés).

| Method | Count | Mean | Ratio | Allocated |
|---|---|---|---|---|
| Select_ToList | 10 | 122.45 ns | 0.99 | 592 B |
| Select_ToArray | 10 | 123.28 ns | 1.00 | 560 B |
| ConvertAll | 10 | 96.44 ns | 0.78 | 456 B |
| Foreach | 10 | 128.63 ns | 1.04 | 648 B |
| Foreach_PreSized | 10 | 88.80 ns | 0.72 | 456 B |
| ListForEach | 10 | 164.83 ns | 1.34 | 680 B |
| ParallelForEach | 10 | 4,838.41 ns | 39.25 | 4149 B |
| Select_ToList | 100000 | 6,390,452.34 ns | 0.97 | 4000245 B |
| Select_ToArray | 100000 | 6,577,353.65 ns | 1.00 | 4000213 B |
| ConvertAll | 100000 | 6,104,317.71 ns | 0.93 | 4000110 B |
| Foreach | 100000 | 8,406,612.50 ns | 1.28 | 5297728 B |
| Foreach_PreSized | 100000 | 6,514,088.54 ns | 0.99 | 4000110 B |
| ListForEach | 100000 | 8,493,388.54 ns | 1.29 | 5297794 B |
| ParallelForEach | 100000 | 17,651,078.12 ns | 2.68 | 8429229 B |

## 🔍 Conclusion

- **Constat** : `ConvertAll` / `foreach` pré-dimensionné les plus rapides ; `Parallel.ForEach` 2,7× à 39× plus lent.
- **Interprétation** : La taille connue évite les redimensionnements (–25 % d'allocations) ; la parallélisation coûte plus cher que le mapping trivial et perd l'ordre.
- **À retenir** : `ConvertAll` ou `new List<T>(capacité)` ; pas de `Parallel` pour un mapping simple.
