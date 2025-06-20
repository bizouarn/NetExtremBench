using BenchMark.Model;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BenchMark.BenchMark
{
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net80)]
    [MemoryDiagnoser]
    public class EnumUtils
    {
        private List<string> _types;

        public EnumUtils()
        {
            _types = new List<string>();
            foreach (var enumValue in Enum.GetValues(typeof(TypeCompteEnum)))
            {
                _types.Add(enumValue.ToString());
            }
        }

        [Benchmark]
        public void Old()
        {
            var res = false;
            foreach (var str in _types)
            {
                foreach (var enumValue in Enum.GetValues(typeof(TypeCompteEnum)))
                {
                    if (str == enumValue.ToString())
                    {
                        res = true;
                    }
                }
            }
        }

        [Benchmark]
        public void New()
        {
            var res = false;
            foreach (var str in _types)
            {
                Enum.TryParse(typeof(TypeCompteEnum), str, true, out var result);
                if (str == result.ToString())
                {
                    res = true;
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        [Benchmark]
        public void NewSpan()
        {
            var res = false;
            foreach (ReadOnlySpan<char> str in _types)
            {
                var success = Enum.TryParse(typeof(TypeCompteEnum), str, true, out var result);
                if (success)
                {
                    res = true;
                }
                else
                {
                    throw new Exception();
                }
            }
        }
    }
}
