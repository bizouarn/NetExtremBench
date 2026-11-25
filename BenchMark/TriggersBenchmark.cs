using BenchmarkDotNet.Attributes;

namespace BenchMark.BenchMark
{
    [MemoryDiagnoser]
    public class TriggersBenchmark
    {
        private List<object> _triggersList = null!;
        private object[] _triggersArray = null!;

        [Params(10, 100)]
        public int Size { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            // On remplit avec des objets quelconques, le filtre est à la fin pour tester le pire cas
            var list = new List<object>();
            for (int i = 0; i < Size - 1; i++)
            {
                list.Add(new MyOtherAttribute());
            }
            list.Add(new IgnoreLogAttribute());

            _triggersList = list;
            _triggersArray = list.ToArray();
        }

        [Benchmark(Baseline = true)]
        public object? Linq_OfType_Any()
        {
            if (_triggersList.OfType<IgnoreLogAttribute>().Any()) return null!;
            return new object();
        }

        [Benchmark]
        public object? Foreach_Is()
        {
            foreach (var trigger in _triggersList)
            {
                if (trigger is IgnoreLogAttribute) return null!;
            }
            return new object();
        }

        [Benchmark]
        public object? For_List_Is()
        {
            for (int i = 0; i < _triggersList.Count; i++)
            {
                if (_triggersList[i] is IgnoreLogAttribute) return null!;
            }
            return new object();
        }

        [Benchmark]
        public object? For_Array_Is()
        {
            for (int i = 0; i < _triggersArray.Length; i++)
            {
                if (_triggersArray[i] is IgnoreLogAttribute) return null!;
            }
            return new object();
        }

        [Benchmark]
        public object? Span_Is()
        {
            ReadOnlySpan<object> span = _triggersArray;
            foreach (var trigger in span)
            {
                if (trigger is IgnoreLogAttribute) return null!;
            }
            return new object();
        }
    }

    public class IgnoreLogAttribute : Attribute { }
    public class MyOtherAttribute : Attribute { }
}
