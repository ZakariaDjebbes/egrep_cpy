#nullable disable
#define RELEASE

using System.Diagnostics;
using System.Text;
using CommandLine;
using EgrepCpy.Log;
using EgrepCpy.Automaton;
using EgrepCpy.RegEx;
using EgrepCpy.Algorithms;

namespace EgrepCpy;

public class Program
{
    public static void Main(String[] args)
    {
        Parser.Default.ParseArguments<CommandLineOptions>(args)
            .WithParsed(opts =>
            {
                Run(opts);
            });
    }

    private static void Run(CommandLineOptions opts)
    {
        try
        {
            var text = ToASCII(File.ReadAllText(opts.File));
            var regEx = opts.RegEx;
            List<MatchResult> matches = null;
            var useKMP = RegExParser.ShouldUseKMP(regEx);

            Logger.Log($"Finding matches of RegEx [{regEx}] on text [{opts.File}]\n");

            TimeSpan time;

            if (useKMP)
                time = UseKMP(opts, text, regEx, ref matches);
            else
                time = UseAutomata(opts, text, regEx, ref matches);


            var lines = text.Split('\n');
            List<PrettyString> prettyStrings = new();
            
            if (opts.PrettyPrint)
            {
                for (int line = 0; line < lines.Length; line++)
                {
                    var prettyString = new PrettyString($"{lines[line]}");
                    var lineMatches = matches.AsParallel().Where(x => x.Line == line);

                    foreach (var match in lineMatches)
                    {
                        for (int i = match.Start; i < match.End; i++)
                        {
                            prettyString.Colors[i] = ConsoleColor.Green;
                        }
                    }

                    prettyStrings.Add(prettyString);
                }
            }
            else
            {
                for (int line = 0; line < lines.Length; line++)
                {
                    var prettyString = new PrettyString($"{lines[line]}");

                    if (matches.AsParallel().Any(x => x.Line == line))
                    {
                        var lineMatches = matches.AsParallel().Where(x => x.Line == line);

                        foreach (var match in lineMatches)
                        {
                            for (int i = match.Start; i < match.End; i++)
                            {
                                prettyString.Colors[i] = ConsoleColor.Green;
                            }
                        }

                        prettyString.Push($"Line {line + 1} : ", ConsoleColor.Blue);
                        prettyStrings.Add(prettyString);
                    }
                }
            }

            Logger.PrettyLog(prettyStrings);

            if (opts.PrintTime)
            {
                Logger.PrettyLog(new List<PrettyString> {
                    new PrettyString($"\nTime elapsed for parsing and matching the regex : "),
                    new PrettyString($"{time.Minutes}m {time.Seconds}s {time.Milliseconds}ms \n", ConsoleColor.Blue)});
            }

            if (opts.PrintCount)
            {
                Logger.LogSuccess($"Found {matches.Count} matche(s).\n");
            }
        }
        catch (Exception e)
        {
            Logger.LogError("  >> ERROR: " + e.Message);
            Logger.LogError("  >> StackTrace: " + e.StackTrace);
            Logger.LogError("  >> InnerException: " + e.InnerException);
            Logger.LogError("  >> Source: " + e.Source);
            Logger.LogError("  >> TargetSite: " + e.TargetSite);
        }
        finally
        {
            Console.WriteLine();
        }
    }

    private static TimeSpan UseKMP(CommandLineOptions opts, string text, string regEx, ref List<MatchResult> matches)
    {
        var kmp = new KnuthMorrisPratt(regEx);
        var watch = Stopwatch.StartNew();
        matches = kmp.Search(text);
        watch.Stop();

        if (opts.PrintDetails)
        {
            Logger.Log($"\tUsing the KMP algorithm, current carry over table:");
            for (int i = 0; i < kmp.CarryOver.Length; i++)
            {
                Logger.Log($"\t\t[{i}]: {kmp.CarryOver[i]}");
            }
            Console.WriteLine();
        }

        return watch.Elapsed;
    }

    private static TimeSpan UseAutomata(CommandLineOptions opts, string text, string regEx, ref List<MatchResult> matches)
    {
        RegExTree ret = RegExParser.Parse(regEx);
        Automata ndfa = NdfaGenerator.Generate(ret);
        var watch = System.Diagnostics.Stopwatch.StartNew();
        Automata dfa = DfaGenerator.Generate(ndfa);
        matches = dfa.Match(text);
        watch.Stop();

        if (opts.PrintDetails)
        {
            Logger.Log($"\tResulting RegExTree :");
            Logger.LogInfo($"\t\t{ret.ToString()}");
            Logger.Log("\tResulting Non Deterministic Automata : ");
            Logger.LogInfo($"\t\t{ndfa.ToString()}");
            Logger.Log("\tResulting Deterministic Automata : ");
            Logger.LogInfo($"\t\t{dfa.ToString()}");
            Console.WriteLine();
        }

        return watch.Elapsed;
    }

    public static string ToASCII(string text) => new ASCIIEncoding().GetString(Encoding.ASCII.GetBytes(text));
}