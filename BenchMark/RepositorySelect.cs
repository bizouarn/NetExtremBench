using System;
using BenchMark.Model;
using BenchmarkDotNet.Attributes;
using Microsoft.Diagnostics.Tracing.StackSources;
using ZLinq;

namespace BenchMark.BenchMark
{
    [MemoryDiagnoser]
    public class RepositorySelect
    {
        private IEnumerable<IPersonne> _ret;

        public IPersonne[] res;

        [Params(10, 1000, 100_000, 200_000)]
        public int N;

        [IterationSetup]
        public void IterationSetup()
        {
            res = Personne.GetAll(N).ToArray();
        }

        private IEnumerable<IPersonne> GetAll()
        {
            return res;
        }

        [Benchmark]
        public void GetPersonesOld()
        {
            var recs = GetAll();

            if (recs != null)
            {
                var mapper = new PersonneMapper();
                var result = new List<IPersonne>();
                recs.ForEach(r => result.Add(mapper.Map(r))).AsParallel();
                _ret = result;
            }
        }

        [Benchmark]
        public void GetPersones()
        {
            var recs = GetAll();

            var mapper = new PersonneMapper();
            _ret = recs.Select(mapper.Map).ToArray();
        }

        [Benchmark]
        public void GetPersonesNewZLinq()
        {
            var recs = GetAll();
            var mapper = new PersonneMapper();
            _ret = recs.AsValueEnumerable().Select(mapper.Map).ToArray();
        }

        [Benchmark]
        public void GetPersonesYield()
        {
            var mapper = new PersonneMapper();
            _ret = Map(GetAll(), mapper).ToArray();
        }

        private IEnumerable<Personne> Map(IEnumerable<IPersonne> list, PersonneMapper mapper)
        {
            foreach (var p in list)
                yield return mapper.Map(p);
        }

        [Benchmark]
        public void GetPersonesFor()
        {
            var recs = GetAll().ToArray();
            var recsLength = recs.Length;
            var mapper = new PersonneMapper();
            var result = new Personne[recsLength];
            for (int i = 0; i < recsLength; i++)
                result[i] = mapper.Map(recs[i]);
            _ret = result;
        }
    }
}
