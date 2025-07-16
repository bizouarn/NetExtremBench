using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchMark.Model
{
    public class Operateur
    {
        public Operateur(string syntaxe)
        {
            Syntaxe = syntaxe;
        }
        public string Syntaxe { get; set; }

        public override string ToString() => Syntaxe;
    }
}
