using BenchmarkDotNet.Attributes;

namespace BenchMark.BenchMark
{
    [MemoryDiagnoser]
    public class IterationBenchmark
    {
        private int[] array;
        private List<int> list;
        private IEnumerable<int> enumerable;

        [GlobalSetup]
        public void Setup()
        {
            array = new int[1_000_000];
            list = new List<int>(1_000_000);
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = i;
                list.Add(i);
            }

            enumerable = list;
        }

        [Benchmark]
        public long IterateArray()
        {
            long sum = 0;
            for (int i = 0; i < array.Length; i++)
                sum += array[i];
            return sum;
        }

        [Benchmark]
        public long IterateEnumerable()
        {
            long sum = 0;
            foreach (var value in enumerable)
                sum += value;
            return sum;
        }


        [Benchmark]
        public long IterateSpan()
        {
            long sum = 0;
            var span = array.AsSpan();
            for (int i = 0; i < span.Length; i++)
                sum += span[i];
            return sum;
        }

        [Benchmark]
        public long IterateList()
        {
            long sum = 0;
            for (int i = 0; i < list.Count; i++)
                sum += list[i];
            return sum;
        }
    }

}
