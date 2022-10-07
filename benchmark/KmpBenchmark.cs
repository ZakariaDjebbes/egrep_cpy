using BenchmarkDotNet.Attributes;
using EgrepCpy;
using EgrepCpy.Algorithms;

[RPlotExporter, RankColumn]
public class KmpBenchmark
{
    [Params("Sargon", "Babylon", "Egypt", "Another", "Paris", "clear")]
    public string RegEx { get; set; }
    private readonly string text = File.ReadAllText("babylon.txt");

    [Benchmark]
    public IEnumerable<MatchResult> Kmp() => new KnuthMorrisPratt(RegEx).Search(text);
}
