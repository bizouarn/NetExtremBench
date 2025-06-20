using BenchMark.Model;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BenchMark.BenchMark
{
    [SimpleJob(RuntimeMoniker.Net60)]
    [MemoryDiagnoser]
    public class SpanBench
    {
        private string _guid = Guid.NewGuid() + "#" + System.Guid.NewGuid().ToString();
        private char[] array;
        private object _res;

        [GlobalSetup]
        public void GlobalSetup()
        {
            array = _guid.ToCharArray();
        }


        [Benchmark]
        public void Old()
        {
            var span = new OldSpan<char>(ref array);
            _res = span.Slice(1);
            _res = span.Slice(1, 6);
            //_res = span.ToArray();
        }
        
        [Benchmark]
        public void New()
        {
            var span = new NewSpan<char>(ref array);
            _res = span.Slice(1);
            _res = span.Slice(1, 6);
            //_res = span.ToArray();
        }

        /*[Benchmark]
        public void SystemSpan()
        {
            var span = array.AsSpan();
            var res = span.Slice(1);
            res = span.Slice(1);
            //res = span.Slice(1, 6);
            //res = span.ToArray();
        }*/
    }
}
