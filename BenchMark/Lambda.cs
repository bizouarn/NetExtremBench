using BenchmarkDotNet.Attributes;

namespace BenchMark.BenchMark;

[MemoryDiagnoser]
public class Lambda
{
    public List<Person> res;

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }

        public Person()
        {
            Random rnd = new Random();
            Id = rnd.Next(int.MinValue, int.MaxValue); 
        }

        public Person Copy()
        {
            return new Person
            {
                Name = this.Name,
                Age = this.Age,
                Address = this.Address
            };
        }
    }

    [IterationSetup]
    public void IterationSetup()
    {
        var values = new List<Person>
        {
            new Person { Name = "John Doe", Age = 30, Address = "123 Main St" },
            new Person { Name = "Jane Smith", Age = 25, Address = "456 Elm St" },
            new Person { Name = "Bob Johnson", Age = 40, Address = "789 Pine St" }
        };

        res = new List<Person>();
        for (var i = 0; i < 10000000; i++) res.Add(values[i % 3].Copy());
    }

    [Benchmark]
    public void SortLambda()
    {
        var sorted = res.OrderBy(static x => x.Id).ToList(); // Materialiser l'opération
    }

    [Benchmark]
    public void SortStaticLambda()
    {
        var sorted = res.OrderBy(static x => x.Id).ToList(); // Materialiser l'opération
    }
}