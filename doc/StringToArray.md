# Benchmark : Méthode pour convertir une string en char[]

## 🎯 Objectif

Comparer les performances entre `boucle for`,  `_guid.ToCharArray()` et `string.ToArray()`.

## 🧱 Code testé

```csharp
 [SimpleJob(RuntimeMoniker.Net60)]
 [SimpleJob(RuntimeMoniker.Net80)]
 [MemoryDiagnoser]
 public class StringToArray
 {
    private string _guid = System.Guid.NewGuid().ToString() + "#" + System.Guid.NewGuid().ToString();
    private char[] _res;

    [Benchmark]
    public void For()
    {
        var array = new char[_guid.Length];
        for (var i = 0; i < _guid.Length; i++)
            array[i] = _guid[i];
        _res = array;
    }

    [Benchmark]
    public void ToCharArray()
    {
        _res = _guid.ToCharArray();
    }

    [Benchmark]
    public void ToArray()
    {
        _res = _guid.ToArray();
    }
}
```

## 📊 Résultats observés

| Method      | Job      | Runtime  | Mean      | Error    | StdDev   | Gen0   | Allocated |
|------------ |--------- |--------- |----------:|---------:|---------:|-------:|----------:|
| For         | .NET 6.0 | .NET 6.0 |  68.52 ns | 0.791 ns | 0.740 ns | 0.0280 |     176 B |
| ToCharArray | .NET 6.0 | .NET 6.0 |  15.06 ns | 0.326 ns | 0.446 ns | 0.0280 |     176 B |
| ToArray     | .NET 6.0 | .NET 6.0 | 483.43 ns | 7.109 ns | 6.649 ns | 0.1068 |     672 B |
| For         | .NET 8.0 | .NET 8.0 |  63.98 ns | 1.234 ns | 1.320 ns | 0.0280 |     176 B |
| ToCharArray | .NET 8.0 | .NET 8.0 |  15.01 ns | 0.278 ns | 0.285 ns | 0.0280 |     176 B |
| ToArray     | .NET 8.0 | .NET 8.0 | 292.74 ns | 4.392 ns | 3.893 ns | 0.1068 |     672 B |

## 🔍 Conclusion

* Utiliser `.ToCharArray()` pour convertir une string en char[].