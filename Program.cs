using BenchMark;
using BenchmarkDotNet.Running;

BenchmarkDotNet.Reports.Summary summary;
var summary = BenchmarkRunner.Run<PairBenchmark>();
summary = BenchmarkRunner.Run<SumBenchmark>();