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
                Name = Name,
                Age = Age,
                Address = Address
            };
        }

        public static IEnumerable<Personne> GetAll(int N = 10_000)
        {
            var values = new Personne[] {
                new Personne { Name = "John Doe", Age = 30, Address = "123 Main St" },
                new Personne { Name = "Jane Smith", Age = 25, Address = "456 Elm St" },
                new Personne { Name = "Bob Johnson", Age = 40, Address = "789 Pine St" },
                new Personne { Name = "Jack Sparrow", Age = 38, Address = "Black Pearl" },
                new Personne { Name = "Anne Bonny", Age = 29, Address = "Revenge" },
                new Personne { Name = "Edward Teach", Age = 45, Address = "Queen Anne's Revenge" },
                new Personne { Name = "Bartholomew Roberts", Age = 41, Address = "Royal Fortune" },
                new Personne { Name = "Kurt Cobain", Age = 27, Address = "Seattle" },
                new Personne { Name = "Freddie Mercury", Age = 45, Address = "London" },
                new Personne { Name = "Jim Morrison", Age = 27, Address = "Los Angeles" }, 
                new Personne { Name = "Joan Jett", Age = 63, Address = "Rockville" },
                new Personne { Name = "Jimi Hendrix", Age = 27, Address = "Electric Ladyland" },
                new Personne { Name = "John Lennon", Age = 40, Address = "Liverpool" },
                new Personne { Name = "Paul McCartney", Age = 82, Address = "London" },
                new Personne { Name = "George Harrison", Age = 58, Address = "Liverpool" },
                new Personne { Name = "Ringo Starr", Age = 84, Address = "Liverpool" },
                new Personne { Name = "Homer Simpson", Age = 39, Address = "742 Evergreen Terrace" },
                new Personne { Name = "Bugs Bunny", Age = 82, Address = "ToonTown" },
                new Personne { Name = "Rick Sanchez", Age = 70, Address = "C-137 Universe" },
                new Personne { Name = "Scooby-Doo", Age = 7, Address = "Mystery Machine" },
                new Personne { Name = "SpongeBob SquarePants", Age = 20, Address = "Bikini Bottom" }
            };
            for (var i = 0; i < N; i++) 
                yield return values[i % values.Length].Copy();
        }
    }
}
