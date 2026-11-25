# Benchmark : Mise à jour d'une clé : TryAdd + indexeur vs `CollectionsMarshal.GetValueRefOrAddDefault`

## 🎯 Objectif

Comparer deux implémentations d'un setter de type « property bag » : double lookup (`TryAdd` puis indexeur) vs un seul lookup via `GetValueRefOrAddDefault`.

## 🧱 Code testé

Source : [`Collections/DictionaryBenchmark.cs`](../BenchMark/Collections/DictionaryBenchmark.cs)

```csharp
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
```

## 📊 Résultats observés

> Mesures en mode `--job short` (ShortRun) sur .NET 10 : indicatives, à confirmer par un run complet. Les jobs .NET 6 / 8 sont absents (runtimes non installés).

| Method | Mean | Ratio | Code Size | Allocated |
|---|---|---|---|---|
| Update_ExistingKey_ValueChanged_Initial | 10.739 ns | 1.00 | 3,746 B | - |
| Update_ExistingKey_ValueChanged_Optimized | 7.482 ns | 0.70 | 1,986 B | - |
| Update_ExistingKey_SameValue_Initial | 10.864 ns | 1.01 | 3,750 B | - |
| Update_ExistingKey_SameValue_Optimized | 7.512 ns | 0.70 | 1,986 B | - |

## 🔍 Conclusion

- **Constat** : `GetValueRefOrAddDefault` ≈ 30 % plus rapide, code JIT 2× plus petit.
- **Interprétation** : Un seul lookup au lieu de deux (`TryAdd` puis indexeur).
- **À retenir** : À utiliser pour les « upsert » sur chemin chaud ; sinon la différence est de quelques ns.
