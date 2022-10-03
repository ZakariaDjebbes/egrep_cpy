#nullable disable
#define RELEASE

using System.Text;
using CommandLine;
using Egrep_Cpy.Log;
using Egrep_Cpy.Automata;
using Egrep_Cpy.RegEx;
using Egrep_Cpy.Algorithms;

namespace Egrep_Cpy;

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
        var useKMP = true;

        try
        {
            Logger.Log($"Finding matches of RegEx [{regEx}] on text [{opts.File}]\n");

            if (useKMP)
                UseKMP(opts, text, regEx);
            else
                UseAutomata(opts, text, regEx);
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

    private static void UseKMP(CommandLineOptions opts, string text, string regEx)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        var kmp = new KnuthMorrisPratt(regEx);
        var matches = kmp.Search(text);
        watch.Stop();

        if (opts.PrintTime)
        {
            Logger.PrettyLog(new PrettyString($"\nTime elapsed for parsing and matching the regex : ",
                                                ConsoleColor.White),
                            new PrettyString($"{watch.Elapsed.Minutes}m {watch.Elapsed.Seconds}s {watch.Elapsed.Milliseconds}ms \n",
                                                ConsoleColor.Blue));
        }

        if (opts.PrintDetails)
        {
            Logger.Log($"\tUsing the KMP algorithm, current carry over table:");
            for (int i = 0; i < kmp.CarryOver.Length; i++)
            {
                Logger.Log($"\t\t[{i}]: {kmp.CarryOver[i]}");
            }
            Console.WriteLine();
        }

        if (opts.PrintCount)
            Logger.LogSuccess($"Found {matches.Count} matches.\n");

        if (opts.PrettyPrint)
        {
            var prettyText = new PrettyString[text.Length];

            for (int i = 0; i < text.Length; i++)
            {
                var match = matches[i];
                var start = match;
                var end = match + regEx.Length;

                if (matches.FindAll(x => (start <= i && end > i) || (start == i && end == i)).Count != 0)
                {
                    prettyText[i] = new PrettyString(text[i].ToString(), ConsoleColor.Green);
                }
                else
                {
                    prettyText[i] = new PrettyString(text[i].ToString(), ConsoleColor.White);
                }
            }

            Logger.PrettyLog(prettyText);
        }
        else
        {
            for (int i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                var start = match;
                var end = match + regEx.Length;
                var matchText = text.Substring(start, end - start);

                Logger.LogSuccess($"Match {i + 1} : {matchText}");
            }
        }
    }

    private static void UseAutomata(CommandLineOptions opts, string text, string regEx)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        RegExTree ret = RegExParser.Parse(regEx);
        Automata.Automata ndfa = NdfaGenerator.Generate(ret);
        Automata.Automata dfa = DfaGenerator.Generate(ndfa);
        var matches = dfa.MatchBruteForce(text);
        watch.Stop();

        if (opts.PrintTime)
        {
            Logger.PrettyLog(new PrettyString($"\nTime elapsed for parsing and matching the regex : ",
                                                ConsoleColor.White),
                            new PrettyString($"{watch.Elapsed.Minutes}m {watch.Elapsed.Seconds}s {watch.Elapsed.Milliseconds}ms \n",
                                                ConsoleColor.Blue));
        }

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


        if (opts.PrintCount)
            Logger.LogSuccess($"Found {matches.Count} matches.\n");

        if (opts.PrettyPrint)
        {
            var prettyText = new PrettyString[text.Length];

            for (int i = 0; i < text.Length; i++)
            {
                if (matches.FindAll(x => (x.Start <= i && x.End > i) || (x.Start == i && x.End == i)).Count != 0)
                {
                    prettyText[i] = new PrettyString(text[i].ToString(), ConsoleColor.Green);
                }
                else
                {
                    prettyText[i] = new PrettyString(text[i].ToString(), ConsoleColor.White);
                }
            }

            Logger.PrettyLog(prettyText);
        }
        else
        {
            foreach (var item in matches)
            {
                Logger.LogSuccess($"\t{(opts.PrintLine ? $"Line {item.Line} " : "")}{(opts.PrintRange ? $"Column ({item.Start} - {item.End})" : "")} > {(item.Start == item.End ? $"{text.SubStr(item.Start, item.End)}" : $"{text.SubStr(item.Start, item.End - 1)}")}");
            }
        }
    }

    public static string ToASCII(string text) => new ASCIIEncoding().GetString(Encoding.ASCII.GetBytes(text));
}