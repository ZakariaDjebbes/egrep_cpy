namespace Benchmark;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using EgrepCpy;
using EgrepCpy.Algorithms;

[RPlotExporter, RankColumn]
public class Kmp
{
    private class Config : ManualConfig 
    {
        public Config() => AddJob(
                Job.Dry
                .WithLaunchCount(5)
                .WithId("Knuth-Morris-Pratt"));
    }

    [Params("Sargon", "Babylon", "Egypt", "Another", "Paris", "clear")]
    public string RegEx { get; set; }
    private readonly string text = File.ReadAllText("babylon.txt");

    [Benchmark]
    public List<MatchResult> KmpBenchmark() => new KnuthMorrisPratt(RegEx).Search(text);
}
