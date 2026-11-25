using BenchmarkDotNet.Attributes;

namespace BenchMark.BenchMark
{
    public class Item
    {
        public string Code { get; set; }
        public int Number { get; set; }
    }

    public class ToDictionary
    {
        private List<Item> _items;

        [Params(1_000, 100_000, 1_000_000)] // tailles testées
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            _items = Enumerable.Range(1, N)
                .Select(i => new Item
                {
                    Code = "M" + (i % 100),
                    Number = i
                })
                .ToList();
        }

        [Benchmark]
        public Dictionary<Tuple<string, int>, Item> WithValueSelector()
        {
            return _items.ToDictionary(
                static mf => Tuple.Create(mf.Code, mf.Number),
                static mf => mf
            );
        }

        [Benchmark]
        public Dictionary<Tuple<string, int>, Item> WithoutValueSelector()
        {
            return _items.ToDictionary(
                static mf => Tuple.Create(mf.Code, mf.Number)
            );
        }
    }
}
