# Benchmark : Ajout dans un `Dictionary` (ContainsKey, TryAdd, Add)

## 🎯 Objectif

Comparer les façons de tester/ajouter une clé dans un très gros dictionnaire (10 M d'entrées) : `Keys.Any` (LINQ, O(n)) vs `ContainsKey` vs `TryAdd` vs `Add`.

## 🧱 Code testé

Source : [`Collections/AddDict.cs`](../BenchMark/Collections/AddDict.cs)

```csharp
using BenchmarkDotNet.Attributes;

namespace BenchMark.BenchMark;

[MemoryDiagnoser]
public class AddDict
{
    public int keyToAdd = 999999999;
    public Dictionary<int, string> res;
    public string valueToAdd = "John Week";

    [IterationSetup]
    public void IterationSetup()
    {
        var values = new List<string>
        {
            "John Doe", "123", "Jane Smith", "456", "Bob Johnson"
        };
        res = new Dictionary<int, string>();
        for (var i = 0; i < 10000000; i++) res.Add(i, values[i % 5]);
    }

    [Benchmark]
    public void LinkContainKey()
    {
        var exist = res.Keys.Any(x => x == keyToAdd);
    }

    [Benchmark]
    public void ContainKey()
    {
        var exist = res.ContainsKey(keyToAdd);
    }

    [Benchmark]
    public void AddNotContains()
    {
        if (res.ContainsKey(keyToAdd))
            res[keyToAdd] = valueToAdd;
        else
            res.Add(keyToAdd, valueToAdd);
    }

    [Benchmark]
    public void Add()
    {
        res.Add(keyToAdd, valueToAdd);
    }

    [Benchmark]
    public void TryAdd()
    {
        res.TryAdd(keyToAdd, valueToAdd);
    }
}
```

## 📊 Résultats observés

> Mesures en mode `--job short` (ShortRun) sur .NET 10 : indicatives, à confirmer par un run complet. Les jobs .NET 6 / 8 sont absents (runtimes non installés).

| Method | Mean | Allocated |
|---|---|---|
| LinkContainKey | 35,980,333.3 ns | 128 B |
| ContainKey | 2,066.7 ns | - |
| AddNotContains | 2,500.0 ns | - |
| Add | 866.7 ns | - |
| TryAdd | 1,933.3 ns | - |

## 🔍 Conclusion

- **Constat** : `Keys.Any` ≈ 36 ms contre quelques µs pour `ContainsKey` ; `Add`/`TryAdd`/`ContainsKey` sont indiscernables (bruit de mesure).
- **Interprétation** : `Keys.Any` énumère toutes les clés (O(n)), `ContainsKey` fait un accès par hash (O(1)).
- **À retenir** : Ne jamais chercher une clé via LINQ ; `ContainsKey`/`TryAdd`.
