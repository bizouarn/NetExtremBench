using BenchmarkDotNet.Attributes;

namespace BenchMark.BenchMark;

[MemoryDiagnoser]
public class ForOrCalcDate
{
    private readonly int periodDiv = 12;
    private DateTime date1;
    private DateTime date2;

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