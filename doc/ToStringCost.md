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

## 📊 Résultats observés (exemples)

| Method         | Job      | Runtime  | Mean      | Error     | StdDev    | Allocated |
|--------------- |--------- |--------- |----------:|----------:|----------:|----------:|
| AccessOperateur| .NET 6.0 | .NET 6.0 | 0.0000 ns | 0.0000 ns | 0.0000 ns |         - |
| AccessToString | .NET 6.0 | .NET 6.0 | 0.4209 ns | 0.0053 ns | 0.0047 ns |         - |
| AccessOperateur| .NET 8.0 | .NET 8.0 | 0.0979 ns | 0.0066 ns | 0.0055 ns |         - |
| AccessToString | .NET 8.0 | .NET 8.0 | 0.2335 ns | 0.0107 ns | 0.0100 ns |         - |

## Conclusion

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