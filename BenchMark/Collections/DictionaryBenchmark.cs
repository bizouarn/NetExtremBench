using BenchmarkDotNet.Attributes;
using System.Runtime.InteropServices;

namespace BenchMark.BenchMark;

[MemoryDiagnoser]
[DisassemblyDiagnoser]
public class DictionaryBenchmark
{
    private readonly Dictionary<string, object?> _dataInitial = new(StringComparer.Ordinal);
    private readonly Dictionary<string, object?> _dataOptimized = new(StringComparer.Ordinal);

    private const string ExistingKey = "FirstName";
    private const string NewKey = "LastName";
    private const string Value1 = "Alice";
    private const string Value2 = "Bob";

    [GlobalSetup]
    public void Setup()
    {
        _dataInitial[ExistingKey] = Value1;
        _dataOptimized[ExistingKey] = Value1;
    }

    // --- Version Initiale ---
    public void SetInitial(string columnName, object? value)
    {
        if (_dataInitial.TryAdd(columnName, value))
        {
            // Simule OnPropertyChanged
        }
        else if (_dataInitial[columnName] != value)
        {
            _dataInitial[columnName] = value;
            // Simule OnPropertyChanged
        }
    }

    // --- Version Optimisée (.NET 10) ---
    public void SetOptimized(string columnName, object? value)
    {
        ref var currentVal = ref CollectionsMarshal.GetValueRefOrAddDefault(_dataOptimized, columnName, out bool isNew);

        if (isNew || !Equals(currentVal, value))
        {
            currentVal = value;
            // Simule OnPropertyChanged
        }
    }

    // --- Scénarios ---

    [Benchmark(Baseline = true)]
    public void Update_ExistingKey_ValueChanged_Initial()
        => SetInitial(ExistingKey, Value2);

    [Benchmark]
    public void Update_ExistingKey_ValueChanged_Optimized()
        => SetOptimized(ExistingKey, Value2);

    [Benchmark]
    public void Update_ExistingKey_SameValue_Initial()
        => SetInitial(ExistingKey, Value1);

    [Benchmark]
    public void Update_ExistingKey_SameValue_Optimized()
        => SetOptimized(ExistingKey, Value1);
}