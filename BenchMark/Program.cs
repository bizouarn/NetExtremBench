using BenchmarkDotNet.Running;

namespace BenchMark.BenchMark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<AppendBench>();
        }
    }
}
