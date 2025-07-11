namespace BenchMark.Model
{
    public class PersonneMapper
    {
        public Personne Map(IPersonne source)
        {
            return new Personne { Name = source.Name, Age = source.Age, Address = source.Address };
        }
    }
}
