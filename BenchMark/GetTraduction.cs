using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            if (!id.Contains(separator)) return id.ToString();
            var split = id.Slice(0, id.IndexOf(separator));
            var secondPart = id.Slice(id.IndexOf(separator) + 1);
            return GetTraductionFake(split.ToString(), secondPart.ToString());
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
