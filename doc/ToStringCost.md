# Benchmark : `.Syntaxe` vs `.ToString()` sur la classe `Operateur`

## 🎯 Objectif

Comparer les performances entre :

- L'accès direct à une propriété métier : `operateur.Syntaxe`
- L'appel de la méthode virtuelle `operateur.ToString()` (qui retourne `Syntaxe`)

## 🧱 Code testé

### Classe `Operateur`
```csharp
public class Operateur
{
    public Operateur(string syntaxe)
    {
        Syntaxe = syntaxe;
    }

    public string Syntaxe { get; set; }

    public override string ToString() => Syntaxe;
}
````

### Benchmark avec BenchmarkDotNet

```csharp
[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
[GcServer(true)]
public class ToStringCost
{
    private Operateur _operateur = new("T > 0 && T < 10");

    [Benchmark]
    public string AccessOperateur() => _operateur.Syntaxe;

    [Benchmark]
    public string AccessToString() => _operateur.ToString();
}
```

## 📊 Résultats observés

| Method          | Job      | Runtime  | Mean      | Error     | StdDev    | Allocated |
|---------------- |--------- |--------- |----------:|----------:|----------:|----------:|
| AccessOperateur | .NET 6.0 | .NET 6.0 | 0.2452 ns | 0.0223 ns | 0.0198 ns |         - |
| AccessToString  | .NET 6.0 | .NET 6.0 | 0.4063 ns | 0.0203 ns | 0.0190 ns |         - |
| AccessOperateur | .NET 8.0 | .NET 8.0 | 0.0961 ns | 0.0224 ns | 0.0199 ns |         - |
| AccessToString  | .NET 8.0 | .NET 8.0 | 0.2203 ns | 0.0075 ns | 0.0067 ns |         - |

## 🔍 Conclusion

* Utiliser **`Syntaxe`** quand tu veux accéder à la donnée métier.
* Réserver **`ToString()`** pour les besoins d'affichage, debug ou log.
* Éviter de dépendre de `ToString()` pour des traitements métier.

> **Accès direct à `.Syntaxe` = plus rapide, plus explicite, plus fiable !**

```csharp
// ✅ BON - Logique métier
if (operateur.Syntaxe.Contains("&&")) { ... }

// ✅ BON - Affichage/debug
Console.WriteLine(operateur.ToString());
logger.LogInfo($"Opérateur : {operateur}");

// ❌ ÉVITER - Logique métier via ToString()
if (operateur.ToString().Contains("&&")) { ... }
```