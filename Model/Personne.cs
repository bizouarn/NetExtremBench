namespace BenchMark.Model
{
    public class Personne : IPersonne
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }

        public Personne()
        {
            Random rnd = new Random();
            Id = rnd.Next(int.MinValue, int.MaxValue);
        }

        public Personne Copy()
        {
            return new Personne
            {
                Name = this.Name,
                Age = this.Age,
                Address = this.Address
            };
        }

        public static IEnumerable<Personne> GetAll(int N = 10_000)
        {
            var values = new Personne[] {
                new Personne { Name = "John Doe", Age = 30, Address = "123 Main St" },
                new Personne { Name = "Jane Smith", Age = 25, Address = "456 Elm St" },
                new Personne { Name = "Bob Johnson", Age = 40, Address = "789 Pine St" }
            };
            for (var i = 0; i < N; i++) 
                yield return  values[i % values.Length];
        }
    }
}
