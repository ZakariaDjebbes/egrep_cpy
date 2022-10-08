namespace Benchmark;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using EgrepCpy;
using EgrepCpy.Algorithms;
using EgrepCpy.Automaton;
using EgrepCpy.RegEx;

[RPlotExporter, RankColumn]
public class Compare
{
    private class Config : ManualConfig 
    {
        public Config() => AddJob(
                Job.Dry
                .WithLaunchCount(5)
                .WithId("Compare KMP and Automaton"));
    }
 
    [Params("Sargon", "Egypt", "difference", "aaabc", "A", "a", "Dest")]
    public string RegEx { get; set; }
    private readonly string text = File.ReadAllText("babylon.txt");

    [Benchmark]
    public List<MatchResult> KMP() => new KnuthMorrisPratt(RegEx).Search(text);
    
    [Benchmark]
    public List<MatchResult> Automata() => DfaGenerator.Generate(NdfaGenerator.Generate(RegExParser.Parse(RegEx))).Match(text);
}