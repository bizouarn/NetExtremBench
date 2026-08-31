using BenchmarkDotNet.Attributes;

namespace BenchMark.BenchMark
{
    public class ModuleFonction
    {
        public string CodeModule { get; set; }
        public int NumFonction { get; set; }
    }

    public class ToDictionary
    {
        private List<ModuleFonction> _moduleFonctions;

        [Params(1_000, 100_000, 1_000_000)] // tailles testées
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            _moduleFonctions = Enumerable.Range(1, N)
                .Select(i => new ModuleFonction
                {
                    CodeModule = "M" + (i % 100),
                    NumFonction = i
                })
                .ToList();
        }

        [Benchmark]
        public Dictionary<Tuple<string, int>, ModuleFonction> WithValueSelector()
        {
            return _moduleFonctions.ToDictionary(
                static mf => Tuple.Create(mf.CodeModule, mf.NumFonction),
                static mf => mf
            );
        }

        [Benchmark]
        public Dictionary<Tuple<string, int>, ModuleFonction> WithoutValueSelector()
        {
            return _moduleFonctions.ToDictionary(
                static mf => Tuple.Create(mf.CodeModule, mf.NumFonction)
            );
        }
    }
}
