# Benchmark : `IsChanged` : `Dictionary.ContainsValue` vs `HashSet.Count`

## 🎯 Objectif

Évaluer le coût d'un test « y a-t-il au moins une propriété modifiée ? » : parcours de toutes les valeurs d'un dictionnaire vs `Count` d'un `HashSet` ne contenant que les propriétés modifiées.

## 🧱 Code testé

Source : [`Collections/PropStatesBenchmark.cs`](../BenchMark/Collections/PropStatesBenchmark.cs)

```csharp
using BenchmarkDotNet.Attributes;

namespace BenchMark.BenchMark
{
    [MemoryDiagnoser]
    public class PropStatesBenchmark
    {
        // Nombre total de propriétés trackées sur le ViewModel (une application riche peut
        // facilement avoir plusieurs milliers de propriétés bindées sur un
        // écran complexe : grille, formulaire multi-onglets, etc.)
        [Params(1000, 5000)]
        public int TotalProperties;

        // Nombre de propriétés réellement modifiées par l'utilisateur à un
        // instant T (dans la vraie vie : quelques champs, pas tout l'écran)
        [Params(5, 50)]
        public int ChangedCount;

        // Nombre de fois où IsChanged est interrogé pendant la session
        // (ex : réévaluation de CanExecute à chaque frappe clavier sur un
        // formulaire, ou binding IsEnabled réévalué à chaque notification
        // de PropertyChanged d'une propriété quelconque du VM)
        private const int IsChangedCalls = 200_000;

        private Dictionary<string, bool> _dictBefore = new();
        private HashSet<string> _setAfter = new();

        [GlobalSetup]
        public void Setup()
        {
            _dictBefore = new Dictionary<string, bool>(TotalProperties);
            _setAfter = new HashSet<string>();

            for (int i = 0; i < TotalProperties; i++)
            {
                var propName = $"Property_{i}";
                _dictBefore[propName] = false; // toutes enregistrées, comme dans la version "avant"
            }

            // On marque ChangedCount propriétés comme modifiées, réparties
            // au hasard dans l'ensemble (pas juste au début, pour ne pas
            // avantager artificiellement le scan)
            var rnd = new Random(42);
            var indices = Enumerable.Range(0, TotalProperties)
                                     .OrderBy(_ => rnd.Next())
                                     .Take(ChangedCount);

            foreach (var i in indices)
            {
                var propName = $"Property_{i}";
                _dictBefore[propName] = true;
                _setAfter.Add(propName); // seules les propriétés changées existent dans le HashSet
            }
        }

        [Benchmark(Baseline = true)]
        public int Before_Dictionary_ContainsValue()
        {
            int changedCount = 0;
            for (int i = 0; i < IsChangedCalls; i++)
            {
                if (_dictBefore.ContainsValue(true))
                    changedCount++;
            }
            return changedCount;
        }

        [Benchmark]
        public int After_HashSet_Count()
        {
            int changedCount = 0;
            for (int i = 0; i < IsChangedCalls; i++)
            {
                if (_setAfter.Count > 0)
                    changedCount++;
            }
            return changedCount;
        }
    }

}
```

## 📊 Résultats observés

> Mesures en mode `--job short` (ShortRun) sur .NET 10 : indicatives, à confirmer par un run complet. Les jobs .NET 6 / 8 sont absents (runtimes non installés).

| Method | TotalProperties | ChangedCount | Mean | Ratio | Allocated |
|---|---|---|---|---|---|
| Before_Dictionary_ContainsValue | 1000 | 5 | 26,255.67 μs | 1.000 | - |
| After_HashSet_Count | 1000 | 5 | 86.52 μs | 0.003 | - |
| Before_Dictionary_ContainsValue | 1000 | 50 | 2,370.91 μs | 1.00 | - |
| After_HashSet_Count | 1000 | 50 | 85.64 μs | 0.04 | - |
| Before_Dictionary_ContainsValue | 5000 | 5 | 137,689.37 μs | 1.000 | - |
| After_HashSet_Count | 5000 | 5 | 87.51 μs | 0.001 | - |
| Before_Dictionary_ContainsValue | 5000 | 50 | 6,731.34 μs | 1.00 | - |
| After_HashSet_Count | 5000 | 50 | 87.65 μs | 0.01 | - |

## 🔍 Conclusion

- **Constat** : `HashSet.Count` : ~86 µs constant ; `ContainsValue` : 2 à 138 ms, ×27 à ×1 500.
- **Interprétation** : `ContainsValue` parcourt toutes les entrées ; `Count` est une lecture de champ.
- **À retenir** : Stocker seulement les éléments modifiés et tester `Count`.
