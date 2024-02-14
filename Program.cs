using BenchMark;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

Summary summary;
summary = BenchmarkRunner.Run<PairBenchmark>();
summary = BenchmarkRunner.Run<SumBenchmark>();
summary = BenchmarkRunner.Run<ForOrCalcDate>();
summary = BenchmarkRunner.Run<CalcBrightness>();