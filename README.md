# NetExtremBench

🧪 **Lab personnel de benchmarks .NET - Notes et expérimentations en cours**

## 📋 Description

NetExtremBench est mon laboratoire personnel de benchmarks pour explorer les performances et comportements de .NET. Il s'agit d'une **collection de notes et d'expérimentations en évolution constante**, pas d'une suite complète ou finalisée.

⚠️ **IMPORTANT** : Ce repository contient des benchmarks **incomplets, experimentaux et en cours de développement** et qui réponde a un besoin défois partiel. Les résultats peuvent être partiels, non-optimisés ou en cours de validation.

## 🎯 Objectifs

- **Exploration personnelle** : Tester des hypothèses et curiosités sur les performances .NET
- **Lab évolutif** : Ajouter des tests selon mes besoins et questionnements
- **Apprentissage empirique** : Valider ou invalider des hypothèses par la pratique

## 🔧 Technologies utilisées

- **BenchmarkDotNet** - Framework de benchmarking de référence
- **.NET 10** - Le projet cible `net10.0` ; certains benchs gardent des `SimpleJob` .NET 6 / 8 pour comparer les runtimes (nécessite ces runtimes installés)
- **Métriques avancées** - Mesure de la mémoire, du GC, des allocations

## 📚 Ressources

- [Documentation BenchmarkDotNet](https://benchmarkdotnet.org/)
- [Guide des performances .NET](https://learn.microsoft.com/en-us/aspnet/core/performance/overview)
- [Blog de performance .NET](https://devblogs.microsoft.com/dotnet/category/performance/)

## 🗂️ Organisation

| Dossier | Contenu |
|---|---|
| `BenchMark/Collections` | Dictionnaires, listes, itération, mapping, recherche |
| `BenchMark/Strings` | `StringBuilder`, spans, formatage, recherche de sous-chaînes |
| `BenchMark/Numeric` | Sommes, calculs, SIMD |
| `BenchMark/Misc` | Lambdas, enums, hash |
| `Model/` | Classes de test partagées |
| [`doc/`](doc/README.md) | Un document par benchmark (objectif, code, résultats, conclusion) |

## ▶️ Exécution

```powershell
dotnet run -c Release                          # menu interactif
dotnet run -c Release -- --filter *Sum*        # un benchmark précis
dotnet run -c Release -- --list flat           # lister tous les benchmarks
```
