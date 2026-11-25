using BenchmarkDotNet.Attributes;

namespace BenchMark.BenchMark;

[MemoryDiagnoser]
public class ForOrCalcDate
{
    private const int periodDiv = 12;
    private readonly DateTime date1;
    private readonly DateTime date2;

    public ForOrCalcDate()
    {
        date1 = DateTime.Now;
        date2 = date1.AddDays(10);
    }

    [Benchmark]
    public int Calc() => (int) Math.Ceiling((date2 - date1).TotalHours / periodDiv);

    [Benchmark]
    public int For()
    {
        var date = date1;
        var periode = 0;
        while (date < date2)
        {
            date = date.AddHours(periodDiv); // Ajoute la durée d'une période à la date actuelle
            periode++; // Incrémente le compteur de périodes
        }

        return periode;
    }
}
