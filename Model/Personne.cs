using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchMark.Model
{
    public class Personne
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
    }
}
