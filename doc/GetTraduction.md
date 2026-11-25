# Benchmark : Découpage d'un identifiant `groupe|id` (span vs Split)

## 🎯 Objectif

Comparer `Contains` + `Split` à un découpage par `ReadOnlySpan<char>`.

## 🧱 Code testé

Source : [`Strings/GetTraduction.cs`](../BenchMark/Strings/GetTraduction.cs)

```csharp
using BenchmarkDotNet.Attributes;

namespace BenchMark.BenchMark;

[MemoryDiagnoser]
public class GetTraduction
{
    private const char separator = '|';
    private const string id = "ID_TEST|Save";

    [Benchmark]
    public void GetTraductionSpan()
    {
        GetTraductionSpan(id);
    }

    [Benchmark]
    public void GetTraductionStr()
    {
        GetTraductionStr(id);
    }

    public string GetTraductionSpan(ReadOnlySpan<char> id)
    {
        var separatorIndex = id.IndexOf(separator);
        if (separatorIndex == -1 || separatorIndex + 1 >= id.Length) return id.ToString();
        var group = id[..separatorIndex];
        var traductionId = id[(separatorIndex + 1)..];
        return GetTraductionFake(group.ToString(), traductionId.ToString());
    }

    public string GetTraductionStr(string id)
    {
        if (!id.Contains(separator)) return id;
        var split = id.Split(separator);
        return GetTraductionFake(split[0], split[1]);
    }

    public string GetTraductionFake(ReadOnlySpan<char> group, ReadOnlySpan<char> id)
    {
        return string.Concat(group, id);
    }
}
```

## 📊 Résultats observés

> Mesures en mode `--job short` (ShortRun) sur .NET 10 : indicatives, à confirmer par un run complet. Les jobs .NET 6 / 8 sont absents (runtimes non installés).

| Method | Mean | Allocated |
|---|---|---|
| GetTraductionSpan | 23.41 ns | 120 B |
| GetTraductionStr | 46.91 ns | 160 B |

## 🔍 Conclusion

- **Constat** : Span 2× plus rapide, 25 % d'allocations en moins.
- **Interprétation** : Pas de `Split` (tableau + sous-chaînes) ni de double parcours.
- **À retenir** : Préférer le span pour découper un identifiant sur un chemin chaud.
