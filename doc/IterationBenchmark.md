# Benchmark : Itération sur différentes structures

## 🎯 Objectif

Comparer les performances d’itération entre différentes structures de données contenant des entiers :
- int[]
- List<int>
- IEnumerable<int> (boxing via list)
- Span<int>

## 🧱 Code testé

```csharp
[MemoryDiagnoser]
public class IterationBenchmark
{
    private int[] array;
    private List<int> list;
    private IEnumerable<int> enumerable;

    [GlobalSetup]
    public void Setup()
    {
        array = new int[1_000_000];
        list = new List<int>(1_000_000);
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = i;
            list.Add(i);
        }

        enumerable = list;
    }

    [Benchmark]
    public long IterateArray()
    {
        long sum = 0;
        for (int i = 0; i < array.Length; i++)
            sum += array[i];
        return sum;
    }

    [Benchmark]
    public long IterateEnumerable()
    {
        long sum = 0;
        foreach (var value in enumerable)
            sum += value;
        return sum;
    }


    [Benchmark]
    public long IterateSpan()
    {
        long sum = 0;
        var span = array.AsSpan();
        for (int i = 0; i < span.Length; i++)
            sum += span[i];
        return sum;
    }

    [Benchmark]
    public long IterateList()
    {
        long sum = 0;
        for (int i = 0; i < list.Count; i++)
            sum += list[i];
        return sum;
    }
}
```

## 📊 Résultats observés

| Method            | Mean       | Error    | StdDev   | Allocated |
|------------------ |-----------:|---------:|---------:|----------:|
| IterateArray      |   463.7 us |  4.39 us |  3.66 us |         - |
| IterateEnumerable | 5,495.7 us | 23.56 us | 22.04 us |      40 B |
| IterateSpan       |   401.6 us |  7.92 us |  8.80 us |         - |
| IterateList       |   891.1 us | 12.72 us | 11.90 us |         - |

## 🔍 Conclusion

- Span<T> est le plus rapide, légèrement devant le tableau brut (array), car il évite certains contrôles de bound-checking tout en étant très proche du tableau.
- Array reste une référence très performante, avec un accès direct aux données.
- List<T> est 2x plus lent qu’un tableau, dû au coût d'accès .Count et à une moins bonne optimisation en boucle.
- IEnumerable<T> est de loin le plus lent (~10× plus lent que array) à cause de l’itération indirecte via l’énumérateur, sans possibilité d’optimisation agressive par le JIT.