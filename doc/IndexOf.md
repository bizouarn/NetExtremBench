# Benchmark : IndexOf char vs string vs AggressiveInlining

## 🎯 Objectif

Comparer les performances des différentes surcharges de string.IndexOf(...) :
- IndexOf(char) — recherche d’un caractère.
- IndexOf(string) — recherche d’une chaîne de caractères.
- IndexOf(char) avec [MethodImpl(MethodImplOptions.AggressiveInlining)].

## 🧱 Code testé

```csharp
 [SimpleJob(RuntimeMoniker.Net60)]
 [SimpleJob(RuntimeMoniker.Net80)]
 [MemoryDiagnoser]
 public class IndexOf
 {
     private string _guid = Guid.NewGuid() + "#" + Guid.NewGuid();
     private int _res = 0;

     [Benchmark]
     public void CharType()
     {
         _res = _guid.IndexOf('#');
     }

     [Benchmark]
     public void StringType()
     {
         _res = _guid.IndexOf("#");
     }

     [Benchmark]
     [MethodImpl(MethodImplOptions.AggressiveInlining)]
     public void AgressiveCharType()
     {
         _res = _guid.IndexOf('#');
     }
 }
```

## 📊 Résultats observés

| Method            | Job      | Runtime  | Mean         | Error      | StdDev     | Allocated |
|------------------ |--------- |--------- |-------------:|-----------:|-----------:|----------:|
| CharType          | .NET 6.0 | .NET 6.0 |     5.249 ns |  0.0248 ns |  0.0207 ns |         - |
| StringType        | .NET 6.0 | .NET 6.0 | 1,793.216 ns |  3.9440 ns |  3.2935 ns |         - |
| AgressiveCharType | .NET 6.0 | .NET 6.0 |     4.770 ns |  0.0214 ns |  0.0189 ns |         - |
| CharType          | .NET 8.0 | .NET 8.0 |     2.855 ns |  0.0185 ns |  0.0173 ns |         - |
| StringType        | .NET 8.0 | .NET 8.0 | 1,827.091 ns | 14.6115 ns | 13.6677 ns |         - |
| AgressiveCharType | .NET 8.0 | .NET 8.0 |     2.887 ns |  0.0086 ns |  0.0071 ns |         - |

## 🔍 Conclusion

- IndexOf(char) est extrêmement rapide : 2–5 ns.
- IndexOf(string) est ~600× plus lent, même pour un seul caractère. Le coût du parsing, du matching et de la gestion d’Ordinal entre en jeu.
- AggressiveInlining apporte un gain minime (0.5 ns max) mais améliore la régularité (StdDev plus bas).
- .NET 8 optimise davantage l’appel à IndexOf(char) : ~45% de gain par rapport à .NET 6.

### Recommandation :

- Utilise toujours IndexOf(char) quand tu peux.
- Ne passe jamais une string "c" au lieu de 'c' par habitude, c’est un piège.
- AggressiveInlining est marginal ici, à réserver aux hot-paths très sensibles.