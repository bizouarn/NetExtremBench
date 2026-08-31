using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BenchMark.BenchMark
{
    [SimpleJob(RuntimeMoniker.Net80)]
    [MemoryDiagnoser]
    [GcServer(true)]
    public class UnescapeBenchmarks
    {
        private string testString;
        private string res;

        [GlobalSetup]
        public void Setup()
        {
            // 100 000 répétitions pour simuler un vrai load
            testString = string.Concat(Enumerable.Repeat("Line1\\nLine2\\nLine3\\n", 100_000));
        }

        [Benchmark]
        public void StringReplace()
        {
            res = testString.Replace("\\n", "\n");
        }

        [Benchmark]
        public void SpanUnescape()
        {
            res = new string(UnescapeChar(testString, '\\', 'n', '\n'));
        }

        private static ReadOnlySpan<char> UnescapeChar(ReadOnlySpan<char> input, char escapeChar1, char escapeChar2, char target)
        {
            var count = 0;
            for (var i = 0; i < input.Length - 1; i++)
            {
                if (input[i] == escapeChar1 && input[i + 1] == escapeChar2)
                    count++;
            }

            if (count == 0) return input;

            var result = new char[input.Length - count].AsSpan();
            input.CopyTo(result);


            return result;
        }
    }
}
