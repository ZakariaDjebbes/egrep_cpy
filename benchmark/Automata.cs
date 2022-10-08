namespace Benchmark;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using EgrepCpy;
using EgrepCpy.Automaton;
using EgrepCpy.RegEx;

[RPlotExporter, RankColumn]
public class Automata
{
    private class Config : ManualConfig 
    {
        public Config() => AddJob(
                Job.Dry
                .WithLaunchCount(5)
                .WithId("Automata Matching"));
    }
 
    [Params("Sargon", "Sa.?on|Franc?e", "(S|s).+on", "S.*d", "Eg.?p?t?")]
    public string RegEx { get; set; }
    private readonly string text = File.ReadAllText("babylon.txt");

    [Benchmark]
    public List<MatchResult> AutomataBenchmark() => DfaGenerator.Generate(NdfaGenerator.Generate(RegExParser.Parse(RegEx))).Match(text);
}