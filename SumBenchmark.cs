using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BenchMark
{
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.NativeAot70)]
    public class SumBenchmark
    {
        private readonly int[] intArray;
        private readonly Stack<int> intStack;
        private readonly Stack<int> intStackRef;
        private int tmpRes;

        public SumBenchmark()
        {
            var size = 100;
            intArray = new int[size];
            intStackRef = new Stack<int>();
            for (int i = 0; i < intArray.Length; i++){
                intArray[i] = i;
                intStackRef.Push(i);
            }
            intStack = new Stack<int>(intArray);
        }
        
        [Benchmark]
        public void Foreach()
        {
            var res = 0;
            foreach (var value in intArray)
            {
                res += value;
            }
            tmpRes = res;
        }

        [Benchmark]
        public void For()
        {
            var res = 0;
            for (int i = 0; i < intArray.Length; i++)
                res += intArray[i];
            tmpRes = res;
        }

        [Benchmark]
        public void Stack()
        {
            var res = 0;
            while(intStack.Count > 0) {
                res += intStack.Pop();
            }
            tmpRes = res;
        }

        [Benchmark]
        public void StackWithRef()
        {
            var res = 0;
            while(intStack.Count > 0) {
                res += intStackRef.Pop();
            }
            tmpRes = res;
        }
    }
}
