# Benchmark : Itérateur : branche dans la boucle ou choix de la source

## 🎯 Objectif

Comparer deux écritures d'un itérateur `yield` avec parcours normal ou inversé : duplication des boucles vs choix de la source en amont.

## 🧱 Code testé

Source : [`Collections/EnumerateBench.cs`](../BenchMark/Collections/EnumerateBench.cs)

```csharp
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BenchMark.BenchMark;

[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net80)]
public class EnumerateBench
{
    public List<string> List;

    [Params(10000, 10000000)] public int N;
    [Params(false, true)] public bool Reverse;

    [GlobalSetup]
    public void GlobalSetup()
    {
        List = new List<string>();
        for (var i = 0; i < N; i++) List.Add(Guid.NewGuid().ToString());
    }

    [Benchmark]
    public void Before()
    {
        var ret = string.Join(',', BeforeGetEnumerable());
    }

    [Benchmark]
    public void After()
    {
        var ret = string.Join(',', AfterGetEnumerable());
    }
    
    public IEnumerable<string> BeforeGetEnumerable()
    {
        if (!Reverse)
        {
            foreach (var item in List)
                if (IsMatch(item))
                    yield return item;
        }
        else
        {
            foreach (var item in List.AsEnumerable().Reverse())
                if (IsMatch(item))
                    yield return item;
        }
    }

    public IEnumerable<string> AfterGetEnumerable()
    {
        foreach (var item in Reverse ? List.AsEnumerable().Reverse() : List)
            if (IsMatch(item))
                yield return item;
    }
    
    public bool IsMatch(string item)
    {
        return true;
    }
}
```

## 📊 Résultats observés

> Mesures en mode `--job short` (ShortRun) sur .NET 10 : indicatives, à confirmer par un run complet. Les jobs .NET 6 / 8 sont absents (runtimes non installés).

| Method | Job | Runtime | N | Reverse | Mean |
|---|---|---|---|---|---|
| Before | ShortRun | .NET 10.0 | 10000 | False | 267.8 μs |
| After | ShortRun | .NET 10.0 | 10000 | False | 314.6 μs |
| Before | ShortRun | .NET 10.0 | 10000 | True | 341.8 μs |
| After | ShortRun | .NET 10.0 | 10000 | True | 372.9 μs |
| Before | ShortRun | .NET 10.0 | 10000000 | False | 445,973.4 μs |
| After | ShortRun | .NET 10.0 | 10000000 | False | 407,996.8 μs |
| Before | ShortRun | .NET 10.0 | 10000000 | True | 539,169.6 μs |
| After | ShortRun | .NET 10.0 | 10000000 | True | 514,223.4 μs |

## 🔍 Conclusion

- **Constat** : Aucune différence significative (±10 %, dans le bruit).
- **Interprétation** : Le compilateur génère le même itérateur ; le coût vient de `Join` et des GUID.
- **À retenir** : Choisir la version la plus lisible.
