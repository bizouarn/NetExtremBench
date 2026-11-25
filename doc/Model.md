# Modèle de test (`Model/`)

Petites classes partagées par certains benchmarks. Elles ne sont pas des benchmarks elles-mêmes.

| Type | Rôle | Utilisé par |
|------|------|-------------|
| `Personne` / `IPersonne` | Entité de test avec `Id` aléatoire ; `Personne.GetAll(n)` génère `n` copies à partir d'un jeu fixe de 21 personnes | [Lambda](Lambda.md) |
| `Operateur` | Objet portant une `Syntaxe`, dont `ToString()` renvoie la valeur | [ToStringCost](ToStringCost.md) |
| `TypeCompteEnum` | Enum de test | [EnumUtils](EnumUtils.md) |
| `PersonneMapper` | Mapper d'exemple (actuellement inutilisé) | — |
