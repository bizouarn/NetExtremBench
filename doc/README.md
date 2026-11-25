# Index des benchmarks

Un document par benchmark : objectif, code testé, résultats, conclusion.

## Collections

| Benchmark | Doc | Résultats |
|---|---|---|
| `AddDict` | [Ajout dans un `Dictionary` (ContainsKey, TryAdd, Add)](AddDict.md) | ✅ |
| `AddRangeBenchmark` | [AddRange sur une `ObservableCollection`](AddRangeBenchmark.md) | ✅ |
| `Contains` | [Recherche d'un élément (array, List, HashSet)](Contains.md) | ✅ |
| `DictInit` | [Initialisation d'un `Dictionary` avec ou sans capacité](DictInit.md) | ✅ |
| `DictionaryBenchmark` | [Mise à jour d'une clé : TryAdd + indexeur vs `CollectionsMarshal.GetValueRefOrAddDefault`](DictionaryBenchmark.md) | ✅ |
| `DistinctTooltipBenchmark` | [Dédoublonnage : `List.Contains` vs `HashSet.Add`](DistinctTooltipBenchmark.md) | ✅ |
| `EnumerateBench` | [Itérateur : branche dans la boucle ou choix de la source](EnumerateBench.md) | ✅ |
| `IterationBenchmark` | [Itération sur différentes structures](IterationBenchmark.md) | ✅ |
| `ListMappingBenchmark` | [Mapping d'une liste d'entités vers des DTO](ListMappingBenchmark.md) | ✅ |
| `PropStatesBenchmark` | [`IsChanged` : `Dictionary.ContainsValue` vs `HashSet.Count`](PropStatesBenchmark.md) | ✅ |
| `Sort` | [Tri de chaînes : `List.Sort` vs LINQ `OrderBy`](Sort.md) | ✅ |
| `ToDictionary` | [`ToDictionary` avec clé `Tuple`](ToDictionary.md) | ✅ |
| `TriggersBenchmark` | [Recherche d'un type dans une liste d'objets](TriggersBenchmark.md) | ✅ |

## Strings

| Benchmark | Doc | Résultats |
|---|---|---|
| `AppendBench` | [`StringBuilder.Append` : char vs string](AppendBench.md) | ✅ |
| `Capitalize` | [Mettre en majuscule la première lettre (string vs span)](Capitalize.md) | ✅ |
| `DateFormattingBenchmark` | [Formatage massif de dates (ToString vs TryFormat + StringPool)](DateFormattingBenchmark.md) | ✅ |
| `GetTraduction` | [Découpage d'un identifiant `groupe|id` (span vs Split)](GetTraduction.md) | ✅ |
| `IndexOf` | [IndexOf char vs string vs AggressiveInlining](IndexOf.md) | ✅ |
| `NormalizationBenchmark` | [Échappement de caractères interdits](NormalizationBenchmark.md) | ✅ |
| `StrEmpty` | [string.IsNullOrEmpty vs string.IsNullOrWhiteSpace vs Span custom](StrEmpty.md) | ✅ |
| `StringBuilderResetBenchmark` | [Réutiliser un `StringBuilder` : `Clear()` vs `new`](StringBuilderResetBenchmark.md) | ✅ |
| `StringToArray` | [Méthode pour convertir une string en char[]](StringToArray.md) | ✅ |
| `ToStringCost` | [`.Syntaxe` vs `.ToString()` sur la classe `Operateur`](ToStringCost.md) | ✅ |
| `TranslationBenchmark` | [Recherche dans un dictionnaire avec un span (`AlternateLookup`)](TranslationBenchmark.md) | ✅ |
| `UnescapeBenchmarks` | [Déséchappement de `\n`](UnescapeBenchmarks.md) | ✅ |

## Numeric

| Benchmark | Doc | Résultats |
|---|---|---|
| `CalcBrightness` | [Calcul de luminosité d'une couleur](CalcBrightness.md) | ✅ |
| `ForOrCalcDate` | [Nombre de périodes entre deux dates : boucle vs calcul](ForOrCalcDate.md) | ✅ |
| `Multiply` | [Multiplication sbyte / short / int](Multiply.md) | ✅ |
| `Pair` | [determiner si un entier est pair](Pair.md) | ✅ |
| `Sum` | [## 🎯 Benchmark : Méthodes de calcul de somme sur tableau d'entiers](Sum.md) | ✅ |
| `SumFloat` | [Somme d'une `List<float>`](SumFloat.md) | ✅ |

## Misc

| Benchmark | Doc | Résultats |
|---|---|---|
| `EnumUtils` | [Conversion string → enum](EnumUtils.md) | ✅ |
| `GetHashCodeBenchmark` | [Hash d'un dictionnaire : LINQ vs ArrayPool + Span.Sort](GetHashCodeBenchmark.md) | ✅ |
| `Lambda` | [Lambda et static Lambda](Lambda.md) | ✅ |

## Autres

- [Model](Model.md) — classes de test partagées
