# Benchmark : Lambda et static Lambda

## 🎯 Objectif

Comparer les performances entre :
- OrderBy(x => x.Id) (lambda "classique")
- OrderBy(static x => x.Id) (lambda statique, sans capture de contexte)

## 🧱 Code testé

```csharp
[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
[GcServer(true)]
public class Lambda
{
    public List<Personne> res;

    [Params(10000, 100000)]
    public int N;

    [IterationSetup]
    public void IterationSetup()
    {
        res = Personne.GetAll(N).ToList();
    }

    [Benchmark]
    public void SortLambda()
    {
        var sorted = res.OrderBy(x => x.Id).ToList();
    }

    [Benchmark]
    public void SortStaticLambda()
    {
        var sorted = res.OrderBy(static x => x.Id).ToList();
    }
}
```

## 📊 Résultats observés

| Method           | Job      | Runtime  | N      | Mean       | Error     | StdDev    | Median     | Allocated  |
|----------------- |--------- |--------- |------- |-----------:|----------:|----------:|-----------:|-----------:|
| SortLambda       | .NET 6.0 | .NET 6.0 | 10000  |   898.9 us |  17.29 us |  16.17 us |   894.3 us |  234.67 KB |
| SortStaticLambda | .NET 6.0 | .NET 6.0 | 10000  |   897.0 us |  17.64 us |  22.94 us |   895.0 us |  234.67 KB |
| SortLambda       | .NET 8.0 | .NET 8.0 | 10000  | 1,120.0 us |  20.53 us |  30.09 us | 1,107.3 us |  234.67 KB |
| SortStaticLambda | .NET 8.0 | .NET 8.0 | 10000  | 1,137.3 us |  22.21 us |  44.35 us | 1,117.1 us |  234.67 KB |
| SortLambda       | .NET 6.0 | .NET 6.0 | 100000 | 9,761.0 us | 114.08 us | 174.21 us | 9,749.5 us | 2344.05 KB |
| SortStaticLambda | .NET 6.0 | .NET 6.0 | 100000 | 9,897.2 us | 193.47 us | 363.39 us | 9,818.5 us | 2344.05 KB |
| SortLambda       | .NET 8.0 | .NET 8.0 | 100000 | 6,732.0 us |  72.42 us | 134.23 us | 6,722.2 us | 2344.05 KB |
| SortStaticLambda | .NET 8.0 | .NET 8.0 | 100000 | 7,287.8 us | 106.36 us | 202.36 us | 7,237.7 us | 2344.05 KB |

## 🔍 Conclusion

* Tu peux utiliser static pour de la clarté ou pour suivre des guidelines de code, mais ne t’attends pas à un gain notable.
* Pour des très gros volumes, .NET 8 apporte un vrai boost de performance sur OrderBy (~25-30%).