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
        var text = ToASCII(File.ReadAllText(opts.File));
        var regEx = opts.RegEx;
        List<MatchResult> matches = new();
        var useKMP = RegExParser.ShouldUseKMP(regEx);

        try
        {
            Logger.Log($"Finding matches of RegEx [{regEx}] on text [{opts.File}]\n");

            TimeSpan time;

            if (useKMP)
                time = UseKMP(opts, text, regEx, ref matches);
            else
                time = UseAutomata(opts, text, regEx, ref matches);


            var lines = text.Split('\n');

            if (opts.PrettyPrint)
            {
                for (int line = 0; line < lines.Length; line++)
                {
                    var prettyString = new PrettyString($"{lines[line]}");
                    var lineMatches = matches.Where(x => x.Line == line);

                    foreach (var match in lineMatches)
                    {
                        for (int i = match.Start; i < match.End; i++)
                        {
                            prettyString.Colors[i] = ConsoleColor.Green;
                        }
                    }

                    Logger.Log(prettyString);
                }
            }
            else
            {
                for (int line = 0; line < lines.Length; line++)
                {
                    var prettyString = new PrettyString($"{lines[line]}");

                    if (matches.Any(x => x.Line == line))
                    {
                        var lineMatches = matches.Where(x => x.Line == line);

                        foreach (var match in lineMatches)
                        {
                            for (int i = match.Start; i < match.End; i++)
                            {
                                prettyString.Colors[i] = ConsoleColor.Green;
                            }
                        }

                        prettyString.Push($"Line {line} : ", ConsoleColor.Blue);
                        Logger.PrettyLog(prettyString);
                    }
                }
            }

            if (opts.PrintTime)
            {
                Logger.PrettyLog(new List<PrettyString> {
                    new PrettyString($"\nTime elapsed for parsing and matching the regex : "),
                    new PrettyString($"{time.Minutes}m {time.Seconds}s {time.Milliseconds}ms \n", ConsoleColor.Blue)});
            }

            if (opts.PrintCount)
                Logger.LogSuccess($"Found {matches.Count} matche(s).\n");


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
        watch.Stop();
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

        return watch.Elapsed;
    }

    public static string ToASCII(string text) => new ASCIIEncoding().GetString(Encoding.ASCII.GetBytes(text));
}