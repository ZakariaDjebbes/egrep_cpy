using BenchmarkDotNet.Attributes;
using EgrepCpy;
using EgrepCpy.Automaton;
using EgrepCpy.RegEx;

[RPlotExporter, RankColumn]
public class AutomataBenchmark
{
    [Params("Sargon", "Sa.?on|Franc?e", "(S|s).+on", "S.*d", "Eg.?p?t?")]
    public string RegEx { get; set; }
    private readonly string text = File.ReadAllText("babylon.txt");

    [Benchmark]
    public IEnumerable<MatchResult> AutomataMatch() => DfaGenerator.Generate(NdfaGenerator.Generate(RegExParser.Parse(RegEx))).Match(text);
}