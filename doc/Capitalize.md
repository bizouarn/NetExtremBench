# Benchmark : Mettre en majuscule la première lettre (string vs span)

## 🎯 Objectif

Comparer `char.ToUpper(s[0]) + s[1..].ToLower()` à une version `ReadOnlySpan<char>` avec `stackalloc` sur un texte long.

## 🧱 Code testé

Source : [`Strings/Capitalize.cs`](../BenchMark/Strings/Capitalize.cs)

```csharp
using System.Globalization;
using BenchmarkDotNet.Attributes;

namespace BenchMark.BenchMark;

[MemoryDiagnoser]
public class Capitalize
{
    private const string stringTest = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum massa magna,  /* … texte tronqué … */

    [Benchmark]
    public void GetCapitalizeSpan()
    {
        CapitalizeSpan(stringTest);
    }

    [Benchmark]
    public void GetCapitalizeStr()
    {
        CapitalizeStr(stringTest);
    }

    public static string CapitalizeStr(string str)
    {
        if (string.IsNullOrEmpty(str)) return string.Empty;
        return char.ToUpper(str[0]) + str[1..].ToLower();
    }

    public static string CapitalizeSpan(ReadOnlySpan<char> str)
    {
        if (str.IsEmpty) return string.Empty;
        var result = str.Length <= 256 ? stackalloc char[str.Length] : new char[str.Length];
        str.ToLower(result, CultureInfo.InvariantCulture);
        result[0] = char.ToUpperInvariant(str[0]);
        return result.ToString();
    }
}
```

## 📊 Résultats observés

> Mesures en mode `--job short` (ShortRun) sur .NET 10 : indicatives, à confirmer par un run complet. Les jobs .NET 6 / 8 sont absents (runtimes non installés).

| Method | Mean | Allocated |
|---|---|---|
| GetCapitalizeSpan | 1.086 μs | 14.09 KB |
| GetCapitalizeStr | 1.423 μs | 21.12 KB |

## 🔍 Conclusion

- **Constat** : Version span ≈ 1,3× plus rapide, ~33 % d'allocations en moins.
- **Interprétation** : Un seul `ToLower` sur buffer de pile au lieu d'un `Substring` + `ToLower` + concaténation.
- **À retenir** : Gain réel mais faible : à réserver aux chemins chauds.
