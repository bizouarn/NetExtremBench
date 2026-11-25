# Benchmark : Calcul de luminosité d'une couleur

## 🎯 Objectif

Comparer `Color.GetBrightness()` au calcul direct `(min + max) / 510` sur les composantes RGB.

## 🧱 Code testé

Source : [`Numeric/CalcBrightness.cs`](../BenchMark/Numeric/CalcBrightness.cs)

```csharp
using System.Drawing;
using BenchmarkDotNet.Attributes;

namespace BenchMark.BenchMark;

[MemoryDiagnoser]
public class CalcBrightness
{
    private readonly Color[] colorList;

    public CalcBrightness()
    {
        colorList = [Color.Red, Color.Aqua, Color.Green, Color.White];
    }


    [Benchmark]
    public void Auto()
    {
        foreach (var color in colorList)
        {
            var colorR = Color.FromArgb(color.A, color.R, color.G, color.B);
            var brightness = colorR.GetBrightness();
            var result = brightness >= 0.5;
        }
    }

    [Benchmark]
    public void Calc()
    {
        foreach (var color in colorList)
        {
            MinMaxRgb(out var min, out var max, color.R, color.G, color.B);
            var brightness = (min + max) / (byte.MaxValue * 2f);
            var result = brightness >= 0.5;
        }
    }

    private static void MinMaxRgb(out short min, out short max, short r, short g, short b)
    {
        if (r > g)
        {
            max = r;
            min = g;
        }
        else
        {
            max = g;
            min = r;
        }

        if (b > max)
            max = b;
        else if (b < min) min = b;
    }
}
```

## 📊 Résultats observés

> Mesures en mode `--job short` (ShortRun) sur .NET 10 : indicatives, à confirmer par un run complet. Les jobs .NET 6 / 8 sont absents (runtimes non installés).

| Method | Mean | Allocated |
|---|---|---|
| Auto | 22.38 ns | - |
| Calc | 12.36 ns | - |

## 🔍 Conclusion

- **Constat** : Calcul direct ≈ 1,8× plus rapide (12 ns contre 22 ns).
- **Interprétation** : `FromArgb` reconstruit une `Color` avant `GetBrightness`.
- **À retenir** : Sans importance hors boucle serrée (traitement d'image) ; sinon préférer la lisibilité.
