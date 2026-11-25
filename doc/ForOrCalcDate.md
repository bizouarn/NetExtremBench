# Benchmark : Nombre de périodes entre deux dates : boucle vs calcul

## 🎯 Objectif

Comparer une boucle `AddHours` à un calcul direct `Math.Ceiling(TotalHours / période)`.

## 🧱 Code testé

Source : [`Numeric/ForOrCalcDate.cs`](../BenchMark/Numeric/ForOrCalcDate.cs)

```csharp
using BenchmarkDotNet.Attributes;

namespace BenchMark.BenchMark;

[MemoryDiagnoser]
public class ForOrCalcDate
{
    private const int periodDiv = 12;
    private DateTime date1;
    private readonly DateTime date2;

    public ForOrCalcDate()
    {
        date1 = DateTime.Now;
        date2 = DateTime.Now.AddDays(10);
    }

    [Benchmark]
    public void Calc()
    {
        var periode = (int) Math.Ceiling((date2 - date1).TotalHours / periodDiv);
    }

    [Benchmark]
    public void For()
    {
        var periode = 0;
        while (date1 < date2)
        {
            date1 = date1.AddHours(periodDiv); // Ajoute la durée d'une période à la date actuelle
            periode++; // Incrémente le compteur de périodes
        }
    }
}
```

## 📊 Résultats observés

> Mesures en mode `--job short` (ShortRun) sur .NET 10 : indicatives, à confirmer par un run complet. Les jobs .NET 6 / 8 sont absents (runtimes non installés).

| Method | Mean | Allocated |
|---|---|---|
| Calc | 1.477 ns | - |
| For | 26.276 ns | - |

## 🔍 Conclusion

- **Constat** : Calcul direct 18× plus rapide (1,5 ns contre 26 ns pour 20 périodes).
- **Interprétation** : La boucle est O(périodes) ; la formule est O(1).
- **À retenir** : Calculer plutôt qu'itérer (attention à l'arrondi aux bornes).
