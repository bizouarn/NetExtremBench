using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BenchMark.BenchMark
{
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net80)]
    [MemoryDiagnoser]
    public class Multiply
    {
        // Valeurs lues depuis des champs pour empêcher le repliement de constantes par le compilateur.
        private sbyte _sbyteValue = 1;
        private short _shortValue = 1;
        private int _intValue = 1;

        [Benchmark]
        public int sbyteMult() => 5 * _sbyteValue;

        [Benchmark]
        public int shortMult() => 5 * _shortValue;

        [Benchmark]
        public int intMult() => 5 * _intValue;
    }
}
