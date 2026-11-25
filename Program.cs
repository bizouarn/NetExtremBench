using BenchmarkDotNet.Running;

// Usage :
//   dotnet run -c Release                           -> menu interactif
//   dotnet run -c Release -- --filter *Sum*         -> lance les benchs dont le nom contient "Sum"
//   dotnet run -c Release -- --list flat            -> liste tous les benchs
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
