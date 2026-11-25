# Benchmark : Recherche d'un élément (array, List, HashSet)

## 🎯 Objectif

Comparer `Enumerable.Contains`, `Array.IndexOf`, `Span.Contains`, `List.Contains` et `HashSet.Contains` sur 100 000 entiers (élément absent : pire cas).

## 🧱 Code testé

Source : [`Collections/Contains.cs`](../BenchMark/Collections/Contains.cs)

```csharp
using BenchmarkDotNet.Attributes;

namespace BenchMark.BenchMark;

[ShortRunJob]
[HtmlExporter]
[MemoryDiagnoser]
public class Contains
{
    private const int _max = 100000;
    private readonly int[] _array = Enumerable.Range(0, _max).ToArray();
    private readonly List<int> _list = Enumerable.Range(0, _max).ToList();
    private readonly HashSet<int> _hashSet = Enumerable.Range(0, _max).ToHashSet();
    private bool exist;

    [Benchmark]
    public void LinkContain()
    {
        exist = LinkContain(_array, _max);
    }

    public bool LinkContain(int[] array, int value)
    {
        return array.Contains(value);
    }

    [Benchmark]
    public void IndexContain()
    {
        exist = IndexContain(_array, _max);
    }

    public bool IndexContain(int[] array, int value)
    {
        return Array.IndexOf(array, value) > -1;
    }

    [Benchmark]
    public void SpanContain()
    {
        SpanContain(_array, _max);
    }

    public bool SpanContain(int[] array, int value)
    {
        return ((ReadOnlySpan<int>) array).Contains(value);
    }

    [Benchmark]
    public void ListContain()
    {
        exist = ListContain(_list, _max);
    }

    public bool ListContain(List<int> list, int value)
    {
        return list.Contains(value);
    }


    [Benchmark]
    public void HashSetContain()
    {
        exist = HashSetContain(_hashSet, _max);
    }

    public bool HashSetContain(HashSet<int> list, int value)
    {
        return list.Contains(value);
    }
}
```

## 📊 Résultats observés

> Mesures en mode `--job short` (ShortRun) sur .NET 10 : indicatives, à confirmer par un run complet. Les jobs .NET 6 / 8 sont absents (runtimes non installés).

| Method | Mean | Allocated |
|---|---|---|
| LinkContain | 9,957.917 ns | - |
| IndexContain | 9,940.679 ns | - |
| SpanContain | 9,816.922 ns | - |
| ListContain | 9,550.726 ns | - |
| HashSetContain | 2.624 ns | - |

## 🔍 Conclusion

- **Constat** : `HashSet` : 2,6 ns contre ~10 µs pour les autres (×3 800) ; array, `List`, span et LINQ sont équivalents.
- **Interprétation** : Les parcours linéaires sont tous vectorisés par le runtime et donc identiques ; le hash évite le parcours.
- **À retenir** : `HashSet` dès que l'on cherche plusieurs fois dans la même collection.
