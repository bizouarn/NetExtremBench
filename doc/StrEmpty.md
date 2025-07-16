# Benchmark : string.IsNullOrEmpty vs string.IsNullOrWhiteSpace vs Span custom

## 🎯 Objectif

Comparer les performances des méthodes standard string.IsNullOrEmpty / string.IsNullOrWhiteSpace avec des versions custom basées sur ReadOnlySpan<char>, afin d’évaluer :
- L’impact d’une implémentation bas-niveau (Span) sans allocation.
- Le coût relatif de la détection de WhiteSpace.

## 🧱 Code testé

```csharp
[MemoryDiagnoser]
public class StrEmpty
{
    private string[] strings = ["Aymeric", null, "", "            "];
    private string[] _array;
    private static bool res;

    [GlobalSetup]
    public void Setup()
    {
        _array = Enumerable.Range(0, short.MaxValue / 2)
            .Select(x => strings[x % strings.Length]).ToArray();
    }

    [Benchmark]
    public void StrIsNullOrEmpty()
    {
        foreach (var val in _array)
        {
            res = string.IsNullOrEmpty(val);
        }
    }

    [Benchmark]
    public void StrOpIsNullOrEmpty()
    {
        foreach (var val in _array)
        {
            res = strOp.IsNullOrEmpty(val);
        }
    }

    [Benchmark]
    public void StrIsNullOrWhiteSpace()
    {
        foreach (var val in _array)
        {
            res = string.IsNullOrWhiteSpace(val);
        }
    }

    [Benchmark]
    public void StrOpIsNullOrWhiteSpace()
    {
        foreach (var val in _array)
        {
            res = strOp.IsNullOrWhiteSpace(val);
        }
    }

}

public static class strOp
{
    public static bool IsNullOrEmpty(ReadOnlySpan<char> value)
    {
        return value.IsEmpty;
    }
    
    public static bool IsNullOrWhiteSpace(ReadOnlySpan<char> value)
    {
        return IsNullOrEmpty(value) || value.IsWhiteSpace();
    }
}
```

## 📊 Résultats observés

| Method                  | Mean       | Error     | StdDev    | Allocated |
|------------------------ |-----------:|----------:|----------:|----------:|
| StrIsNullOrEmpty        |  14.352 us | 0.0918 us | 0.0813 us |         - |
| StrOpIsNullOrEmpty      |   9.640 us | 0.0556 us | 0.0520 us |         - |
| StrIsNullOrWhiteSpace   | 100.168 us | 1.6822 us | 1.4047 us |         - |
| StrOpIsNullOrWhiteSpace |  94.260 us | 1.7918 us | 3.1381 us |         - |

## 🔍 Conclusion

- Les versions Span<char> sont plus rapides, notamment pour IsNullOrEmpty (~30% de gain).
- IsNullOrWhiteSpace est beaucoup plus coûteux (~10× plus lent que IsNullOrEmpty) : la détection de blancs implique une itération caractère par caractère.
- Les versions Span restent bénéfiques même dans ce cas, mais le gain est marginal (~5-6%) sur les whitespaces.

### Recommandation :

- Pour du code ultra-performant ou dans des hot-paths, préfère un check Span si tu maîtrises les appels et nullability.
- Sinon, string.IsNullOrEmpty reste très rapide, lisible, et adaptée.