using BenchmarkDotNet.Attributes;
using System.Buffers;

namespace BenchMark.BenchMark;

[MemoryDiagnoser]
[DisassemblyDiagnoser]
public class GetHashCodeBenchmark
{
    // Nombre de colonnes dans la ligne simulée.
    // Ajustez cette liste pour tester plusieurs tailles de dataset.
    [Params(5, 20, 100, 200, 500)]
    public int ColumnCount;

    // Proportion de clés exclues par IsNotExcluded (ex: colonnes techniques)
    private const int ExcludedEvery = 7;

    private Dictionary<string, string> _data = null!;

    [GlobalSetup]
    public void Setup()
    {
        _data = new Dictionary<string, string>(ColumnCount);
        for (int i = 0; i < ColumnCount; i++)
        {
            // Mélange volontaire majuscules/minuscules pour exposer
            // la différence Ordinal vs culture-sensible si elle existe.
            string key = (i % 2 == 0 ? "Col_" : "col_") + i;
            _data[key] = $"Value{i}";
        }
    }

    private bool IsNotExcluded(string key)
    {
        // Filtre simplifié
        return !key.EndsWith(ExcludedEvery.ToString());
    }

    [Benchmark(Baseline = true)]
    public int Old_LinqWhereOrder()
    {
        var hash = new HashCode();

        var relevantColumns = _data.Keys
            .Where(IsNotExcluded)
            .Order(); // comparateur par défaut (sensible à la culture)

        foreach (var column in relevantColumns)
        {
            hash.Add(column);
            hash.Add(_data[column]);
        }

        return hash.ToHashCode();
    }

    [Benchmark]
    public int New_ArrayPoolSpanSort()
    {
        var hash = new HashCode();
        string[] rentedArray = ArrayPool<string>.Shared.Rent(_data.Keys.Count);
        try
        {
            int count = 0;
            foreach (var key in _data.Keys)
            {
                if (IsNotExcluded(key))
                {
                    rentedArray[count++] = key;
                }
            }

            Span<string> activeKeys = rentedArray.AsSpan(0, count);
            activeKeys.Sort(StringComparer.Ordinal);

            foreach (var key in activeKeys)
            {
                hash.Add(key);
                hash.Add(_data[key]);
            }
        }
        finally
        {
            ArrayPool<string>.Shared.Return(rentedArray);
        }

        return hash.ToHashCode();
    }

    [Benchmark]
    public int New_ArraySpanSort_NoPool()
    {
        var hash = new HashCode();

        var relevantColumns = _data.Keys.ToArray();
        Array.Sort(relevantColumns, StringComparer.Ordinal);

        foreach (var column in relevantColumns)
        {
            if (IsNotExcluded(column))
            {
                hash.Add(column);
                hash.Add(_data[column]);
            }
        }

        return hash.ToHashCode();
    }
}