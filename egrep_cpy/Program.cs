#nullable disable
#define RELEASE

using System.Text;
using CommandLine;
using Egrep_Cpy.Log;
using Egrep_Cpy.Generation;
using Egrep_Cpy.RegEx;

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
        string text = null, regEx = null;

        try
        {
            regEx = opts.RegEx;
            text = ToASCII(File.ReadAllText(opts.File));

            Logger.Log($"Finding matches of RegEx [{regEx}] on text [{opts.File}]\n");
            RegExTree ret = RegExParser.Parse(regEx);
            Automata ndfa = NdfaGenerator.Generate(ret);
            Automata dfa = DfaGenerator.Generate(ndfa);

            if (opts.PrintDetails)
            {
                Logger.Log($"\tResulting RegExTree :");
                Logger.LogInfo($"\t\t{ret.ToString()}.");
                Logger.Log("\tResulting Non Deterministic Automata : ");
                Logger.LogInfo($"\t\t{ndfa.ToString()}");
                Logger.Log("\tResulting Deterministic Automata : ");
                Logger.LogInfo($"\t\t{dfa.ToString()}");
            }

            var matches = dfa.MatchBruteForce(text);

            if (opts.PrintCount)
                Logger.LogSuccess($"Found {matches.Count} matches.\n");

            if (opts.PrettyPrint)
            {
                var prettyText = new PrettyString[text.Length];

                for (int i = 0; i < text.Length; i++)
                {
                    if(matches.Find(x => x.Item1 <= i && x.Item2 > i) != null)
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
                    Logger.LogSuccess($"\t{(opts.PrintLine ? $"Line 1 - " : "")}{(opts.PrintRange ? $"Column ({item.Item1} - {item.Item2})" : "")} > {text.SubStr(item.Item1, item.Item2 - 1)}");
                }
            }

        }
        catch (Exception e)
        {
            Logger.LogError("  >> ERROR: " + e.Message);
        }
    }

    public static string ToASCII(string text) => new ASCIIEncoding().GetString(Encoding.ASCII.GetBytes(text));
}