using BenchmarkDotNet.Attributes;

namespace BenchMark
{
    [MemoryDiagnoser]
    public class PairBenchmark
    {
        private readonly Random rnd = new Random();
        private readonly List<int> randomNumbers;

        public PairBenchmark()
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
            for (int i = 0; i < count; i++)
            {
                numbers.Add(rnd.Next(0, 1001));
            }
            return numbers;
        }
    }
}
