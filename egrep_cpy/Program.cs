#nullable disable
using System.Text;

namespace EGREP_CPY;

public class Program
{
    public static void Main(String[] args)
    {
        var regEx = "e.";
        var text = ToASCII("Je suis d'accord avec votre avis cher monsieur le president. Pourriez-vous me donner votre avis sur le sujet ?");

        if (args.Length != 0)
        {
            regEx = args[0];
        }

        Logger.LogWarning($"  >> Parsing RegEx {regEx}");
        Logger.LogSuccess("     >> ASCII codes: [" + (int)regEx[0], false);

        for (int i = 1; i < regEx.Length; i++)
        {
            Logger.LogSuccess("," + (int)regEx[i], false);
        }

        Logger.LogSuccess("].");

        try
        {
            // Création du regex tree
            RegExTree ret = RegExParser.Parse(regEx);
            Logger.LogSuccess("     >> Tree result: " + ret.ToString() + ".");
            Logger.LogWarning("  >> Parsing over.");
            Logger.LogWarning("  >> Creating the Non Deterministic Finite Automaton (NFA) from the syntax tree.");

            // Création de l'automate non déterministe
            Automata ndfa = NdfaGenerator.Generate(ret);
            Logger.LogSuccess("    >> Resulting Automata : ");
            Logger.LogSuccess("    >> " + ndfa.ToString());
            Logger.LogWarning("  >> NDFA Generation over.");
            Logger.LogWarning("  >> Creating the Deterministic Finite Automaton (DFA) from the previously generated NDFA.");

            // Génération de l'automate déterministe
            Automata dfa = DfaGenerator.Generate(ndfa);
            Logger.LogSuccess("    >> Resulting Automata : ");
            Logger.LogSuccess("    >> " + dfa.ToString());
            Logger.LogWarning("  >> DFA Generation over.");
            Logger.LogWarning("  >> ...");

            // Recherche des matches

            Logger.LogWarning(message: "  >> Searching for matches in the text (Converted to ASCII) : " + text, timestamp: true);

            var matches = dfa.MatchBruteForce(text);

            Logger.LogSuccess(message: $"    >> Found {matches.Count} matches : ", timestamp: true);

            foreach (var item in matches.Keys)
            {
                Logger.LogSuccess($"    >> ({item.X} - {item.Y}) : {matches[item]}");
            }

            Logger.LogWarning("  >> Exiting...");
        }
        catch (Exception e)
        {
            Logger.LogError("  >> ERROR: " + e.Message);
            Logger.LogError($"  >> Type: {e.GetType()}");
            Logger.LogError("  >> TRACE: " + e.StackTrace);
        }
    }

    public static string ToASCII(string text) => new ASCIIEncoding().GetString(Encoding.ASCII.GetBytes(text));
}