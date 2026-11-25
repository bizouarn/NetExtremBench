using BenchmarkDotNet.Attributes;

namespace BenchMark.BenchMark;

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