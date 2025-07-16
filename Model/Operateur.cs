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
