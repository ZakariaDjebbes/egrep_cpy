#nullable disable //null //null another time null
#define RELEASE

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
        var text = ToASCII(File.ReadAllText(opts.File));
        var regEx = opts.RegEx;
        List<MatchResult> matches = new();
        var useKMP = RegExParser.ShouldUseKMP(regEx);

        try
        {
            Logger.Log($"Finding matches of RegEx [{regEx}] on text [{opts.File}]\n");

            var watch = System.Diagnostics.Stopwatch.StartNew();
            if (useKMP)
                UseKMP(opts, text, regEx, ref matches);
            else
                UseAutomata(opts, text, regEx, ref matches);

            matches = matches.Where(x => (matches.Where(y => y.Line == x.Line && y.Start == x.Start).Max(x => x.End) == x.End) && matches.Where(y => y.Line == x.Line && y.End == x.End).Min(x => x.Start) == x.Start).ToList();
            watch.Stop();


            if (opts.PrintTime)
            {
                Logger.PrettyLog(new List<PrettyString> {
                    new PrettyString($"\nTime elapsed for parsing and matching the regex : "),
                    new PrettyString($"{watch.Elapsed.Minutes}m {watch.Elapsed.Seconds}s {watch.Elapsed.Milliseconds}ms \n", ConsoleColor.Blue)});
            }

            if (opts.PrintCount)
                Logger.LogSuccess($"Found {matches.Count} matche(s).\n");

            var lines = text.Split('\n');
            var prettyText = new List<PrettyString>();

            if (opts.PrettyPrint)
            {
                for (int l = 0; l < lines.Length; l++)
                {
                    var prettyString = new PrettyString($"{lines[l]}");
                    var lineMatches = matches.Where(x => x.Line == l);

                    foreach (var match in lineMatches)
                    {
                        for (int i = match.Start; i < match.End; i++)
                        {
                            prettyString.Colors[i] = ConsoleColor.Green;
                        }
                    }

                    prettyText.Add(prettyString);
                }

                Logger.PrettyLog(prettyText);
            }
            else
            {
                foreach (var match in matches)
                {
                    var line = lines[match.Line];
                    // {(item.Start == item.End ? $"{line.SubStr(item.Start, item.End)}" : $"{line.SubStr(item.Start, item.End - 1)}")}
                    Logger.Log($"Line {match.Line + 1} Column ({match.Start} - {match.End}) : {line}");

                }

                Logger.PrettyLog(prettyText);
            }
        }
        // catch (InvalidRegExException e)
        // {
        //     var msg = "  >> Invalid RegEx: ";

        //     Logger.LogError("  >> ERROR: " + e.Message);
        //     Logger.LogError(msg + regEx);
        //     Logger.LogError($"{new StringBuilder().Append(' ', msg.Length + e.Failure)}^" );
        // }
        catch (Exception e)
        {
            Logger.LogError("  >> ERROR: " + e.Message);
            Logger.LogError("  >> StackTrace: " + e.StackTrace);
            Logger.LogError("  >> InnerException: " + e.InnerException);
        }
        finally
        {
            Console.WriteLine();
        }
    }

    private static void UseKMP(CommandLineOptions opts, string text, string regEx, ref List<MatchResult> matches)
    {
        var kmp = new KnuthMorrisPratt(regEx);
        matches = kmp.Search(text);

        if (opts.PrintDetails)
        {
            Logger.Log($"\tUsing the KMP algorithm, current carry over table:");
            for (int i = 0; i < kmp.CarryOver.Length; i++)
            {
                Logger.Log($"\t\t[{i}]: {kmp.CarryOver[i]}");
            }
            Console.WriteLine();
        }
    }

    private static void UseAutomata(CommandLineOptions opts, string text, string regEx, ref List<MatchResult> matches)
    {
        RegExTree ret = RegExParser.Parse(regEx);
        Automata ndfa = NdfaGenerator.Generate(ret);
        Automata dfa = DfaGenerator.Generate(ndfa);
        matches = dfa.Match(text);

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
    }

    public static string ToASCII(string text) => new ASCIIEncoding().GetString(Encoding.ASCII.GetBytes(text));
}