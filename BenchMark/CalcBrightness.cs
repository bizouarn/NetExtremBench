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