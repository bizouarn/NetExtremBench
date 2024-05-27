using BenchmarkDotNet.Attributes;
namespace BenchMark.BenchMark
{
    [MemoryDiagnoser]
    public class GetTraduction
    {
        private const char separator = '|';
        private string id = "ID_TEST|Save";

        [Benchmark]
        public void GetTraductionSpan()
        {
            GetTraductionSpan(id);
        }

        [Benchmark]
        public void GetTraductionStr()
        {
            GetTraductionStr(id);
        }

        public string GetTraductionSpan(ReadOnlySpan<char> id)
        {
            var separatorIndex = id.IndexOf(separator);
            if (separatorIndex == -1 || separatorIndex + 1 >= id.Length) return id.ToString();
            var group = id[..separatorIndex];
            var traductionId = id[(separatorIndex + 1)..];
            return GetTraductionFake(group.ToString(), traductionId.ToString());
        }

        public string GetTraductionStr(string id)
        {
            if (!id.Contains(separator)) return id;
            var split = id.Split(separator);
            return GetTraductionFake(split[0], split[1]);
        }

        public string GetTraductionFake(ReadOnlySpan<char> group, ReadOnlySpan<char> id)
        {
            return string.Concat(group, id);
        }
    }
}
