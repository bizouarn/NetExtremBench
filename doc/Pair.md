# Benchmark : determiner si un entier est pair

## 🎯 Objectif

Comparer l'efficacité de quatre approches pour déterminer si un entier est pair :
- % 2 == 0 (modulo)
- & 1 == 0 (bitwise)
- En séquentiel
- En parallèle

## 🧱 Code testé

```csharp
[MemoryDiagnoser]
public class Pair
{
    private readonly List<int> randomNumbers;
    private readonly Random rnd = new();

    public Pair()
    {
        randomNumbers = GenerateRandomNumbers(1000);
    }

    [Benchmark]
    public void Modulo()
    {
        foreach (var randomNumber in randomNumbers)
        {
            var result = randomNumber % 2 == 0;
        }
    }

    [Benchmark]
    public void Bit()
    {
        foreach (var randomNumber in randomNumbers)
        {
            var result = (randomNumber & 1) == 0;
        }
    }

    [Benchmark]
    public void ParallelModulo()
    {
        Parallel.ForEach(randomNumbers, randomNumber =>
        {
            var result = randomNumber % 2 == 0;
        });
    }

    [Benchmark]
    public void ParallelBit()
    {
        Parallel.ForEach(randomNumbers, randomNumber =>
        {
            var result = (randomNumber & 1) == 0;
        });
    }

    private List<int> GenerateRandomNumbers(int count)
    {
        var numbers = new List<int>(count);
        for (var i = 0; i < count; i++) numbers.Add(rnd.Next(0, 1001));
        return numbers;
    }
}
```

## 📊 Résultats observés

| Method         | Mean     | Error     | StdDev    | Gen0   | Allocated |
|--------------- |---------:|----------:|----------:|-------:|----------:|
| Modulo         | 1.092 us | 0.0047 us | 0.0042 us |      - |         - |
| Bit            | 1.083 us | 0.0054 us | 0.0051 us |      - |         - |
| ParallelModulo | 7.320 us | 0.1452 us | 0.2505 us | 0.4883 |    3035 B |
| ParallelBit    | 7.510 us | 0.1467 us | 0.2607 us | 0.4807 |    2982 B |

## 🔍 Conclusion

* Utiliser `% 2 == 0` ou `& 1 == 0` : perf identiques, choix stylistique.
* N’utilise pas Parallel.ForEach pour des opérations simples et rapides, dégradation de performance + surcoût mémoire.