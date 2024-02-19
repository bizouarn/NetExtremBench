using BenchMark;
using BenchMark.BenchMark;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

Summary summary;
summary = BenchmarkRunner.Run<Pair>();
summary = BenchmarkRunner.Run<Sum>();
summary = BenchmarkRunner.Run<ForOrCalcDate>();
summary = BenchmarkRunner.Run<CalcBrightness>();
summary = BenchmarkRunner.Run<Sort>();