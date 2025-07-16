using BenchMark.BenchMark;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

Summary summary;
summary = BenchmarkRunner.Run<ToStringCost>();