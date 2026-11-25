# Benchmark : Formatage massif de dates (ToString vs TryFormat + StringPool)

## 🎯 Objectif

Évaluer le coût du formatage d'une grille de dates : `ToString(format)` vs `TryFormat` sur buffer de pile, avec ou sans `StringPool` (CommunityToolkit.HighPerformance) pour dédupliquer les chaînes.

## 🧱 Code testé

Source : [`Strings/DateFormattingBenchmark.cs`](../BenchMark/Strings/DateFormattingBenchmark.cs)

```csharp
using System.Globalization;
using BenchmarkDotNet.Attributes;
using CommunityToolkit.HighPerformance.Buffers;

namespace BenchMark.BenchMark;

/*[ShortRunJob]*/
[MemoryDiagnoser]
[DisassemblyDiagnoser]
public class DateFormattingBenchmark
{
    // Profil réaliste observé : une grille de 600 lignes, avec 20 colonnes
    // de type date par ligne (donc 600 * 20 = 12 000 formatages par passage)
    [Params(/*100, 500,*/ 1000/*, 10_000, 100_000*/)]
    public int RowCount = 600;
    
    [Params(5/*, 10*/)]
    public int ColumnCount = 5;
    
    private const int DistinctValues = 70;

    // Format à tester : "D" (dd/MM/yyyy) ou "H" (HH:mm)
    [Params(/*"D", "H",*/ "DH")] public string FormatType = "D";

    // _data[ligne][colonne]
    private DateTime[][] _data = [];

    private ReadOnlySpan<char> Format => FormatType switch
    {
        "D" => "dd/MM/yyyy",
        "H" => "HH:mm",
        "DH" => "dd/MM/yyyy HH:mm",
        _ => throw new NotSupportedException()
    };

    [GlobalSetup]
    public void GlobalSetup()
    {
        var rng = new Random(42);
        var baseDate = new DateTime(2024, 1, 1);

        // Pool de valeurs distinctes partagé entre toutes les colonnes,
        // pour rester proche de ~50 valeurs distinctes au total
        var distinct = new DateTime[DistinctValues];
        for (var i = 0; i < DistinctValues; i++)
            distinct[i] = baseDate.AddMinutes(rng.Next(0, 60 * 24 * 365));

        _data = new DateTime[RowCount][];
        for (var row = 0; row < RowCount; row++)
        {
            _data[row] = new DateTime[ColumnCount];
            for (var col = 0; col < ColumnCount; col++)
                _data[row][col] = distinct[rng.Next(DistinctValues)];
        }
    }

    // --- Baseline sans pooling : ToString(format) à chaque cellule ---
    [Benchmark(Baseline = true)]
    public string?[,] Old_ToString()
    {
        var result = new string?[RowCount, ColumnCount];
        var format = FormatType switch
        {
            "D" => "dd/MM/yyyy",
            "H" => "HH:mm",
            _ => "dd/MM/yyyy"
        };

        for (var row = 0; row < RowCount; row++)
            for (var col = 0; col < ColumnCount; col++)
                result[row, col] = _data[row][col].ToString(format, CultureInfo.InvariantCulture);

        return result;
    }

    // --- Version intermédiaire : ToString(format) puis pool sur string ---
    /*[Benchmark]
    public string?[,] Old_ToStringThenPool()
    {
        var pool = new StringPool();
        var result = new string?[RowCount, ColumnCount];
        var format = FormatType switch
        {
            "D" => "dd/MM/yyyy",
            "H" => "HH:mm",
            _ => "dd/MM/yyyy"
        };

        for (var row = 0; row < RowCount; row++)
            for (var col = 0; col < ColumnCount; col++)
            {
                var valueString = _data[row][col].ToString(format, CultureInfo.InvariantCulture);
                result[row, col] = pool.GetOrAdd(valueString);
            }

        return result;
    }*/

    // --- Nouvelle version : TryFormat dans un stackalloc puis pool sur span ---
    [Benchmark]
    public string?[,] New_TryFormatThenPool()
    {
        var pool = new StringPool();
        var result = new string?[RowCount, ColumnCount];
        Span<char> strValue = stackalloc char[16];
        var format = Format;

        for (var row = 0; row < RowCount; row++)
            for (var col = 0; col < ColumnCount; col++)
                if (_data[row][col].TryFormat(strValue, out var charsWritten, format, CultureInfo.InvariantCulture))
                    result[row, col] = pool.GetOrAdd(strValue[..charsWritten]);

        return result;
    }

    [Benchmark]
    public string?[,] New_TryFormat()
    {
        var result = new string?[RowCount, ColumnCount];
        Span<char> strValue = stackalloc char[16];
        var format = Format;

        for (var row = 0; row < RowCount; row++)
        for (var col = 0; col < ColumnCount; col++)
            if (_data[row][col].TryFormat(strValue, out var charsWritten, format, CultureInfo.InvariantCulture))
                result[row, col] = strValue[..charsWritten].ToString();

        return result;
    }
}
```

## 📊 Résultats observés

> Mesures en mode `--job short` (ShortRun) sur .NET 10 : indicatives, à confirmer par un run complet. Les jobs .NET 6 / 8 sont absents (runtimes non installés).

| Method | RowCount | ColumnCount | FormatType | Mean | Ratio | Code Size | Allocated |
|---|---|---|---|---|---|---|---|
| Old_ToString | 1000 | 5 | DH | 270.7 μs | 1.00 | 1,921 B | 273.48 KB |
| New_TryFormatThenPool | 1000 | 5 | DH | 521.0 μs | 1.92 | 3,124 B | 118.23 KB |
| New_TryFormat | 1000 | 5 | DH | 370.9 μs | 1.37 | 2,052 B | 312.54 KB |

## 🔍 Conclusion

- **Constat** : `ToString` le plus rapide (271 µs) ; `TryFormat` +37 % ; `StringPool` ×1,9 mais –57 % d'allocations.
- **Interprétation** : Le pool paie un hachage par cellule pour dédupliquer ; avec peu de valeurs distinctes, il économise la mémoire, pas le CPU.
- **À retenir** : CPU prioritaire : `ToString`. Mémoire prioritaire (grille longue durée de vie) : `TryFormat` + `StringPool`.
