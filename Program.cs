using BenchMark.BenchMark;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

Summary summary;
summary = BenchmarkRunner.Run<ToStringCost>();
//summary = BenchmarkRunner.Run<Pair>();
//summary = BenchmarkRunner.Run<Multiply>();