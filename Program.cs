using BenchMark;
using BenchmarkDotNet.Running;

BenchmarkDotNet.Reports.Summary summary;
summary = BenchmarkRunner.Run<PairBenchmark>();
summary = BenchmarkRunner.Run<SumBenchmark>();