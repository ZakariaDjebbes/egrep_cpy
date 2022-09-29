#nullable disable
namespace EGREP_CPY;

public class Program
{
    public const int CONCAT = 0xC04CA7;
    public const int ETOILE = 0xE7011E;
    public const int ALTERN = 0xA17E54;
    public const int PROTECTION = 0xBADDAD;
    public const int PARENTHESEOUVRANT = 0x16641664;
    public const int PARENTHESEFERMANT = 0x51515151;
    public const int ANY = 0x414E59;
    public const int EPSILON = 0x52525252;

    private static string regEx = "a(b|c)*";

    public static void Main(String[] args)
    {
    start:

        if (args.Length != 0)
        {
            regEx = args[0];
        }
        else
        {
            Logger.LogInfo("  >> Please enter a regEx: ", false);
            regEx = Console.ReadLine();
        }

        Logger.LogWarning($"  >> Parsing RegEx {regEx}");

        if (regEx.Length < 1)
        {
            Logger.LogError("  >> ERROR: empty regEx.");
        }
        else
        {
            // Logger.LogSuccess("     >> ASCII codes: [" + (int)regEx[0], false);

            // for (int i = 1; i < regEx.Length; i++)
            // {
            //     Logger.LogSuccess("," + (int)regEx[i], false);
            // }

            // Logger.LogSuccess("].");

            try
            {
                RegExTree ret = RegExParser.Parse(regEx);
                // Logger.LogSuccess("     >> Tree result: " + ret.ToString() + ".");
                // Logger.LogWarning("  >> Parsing over.");
                // Logger.LogWarning("  >> Creating the Non Deterministic Finite Automaton (NFA) from the syntax tree.");
                Automata ndfa = NdfaGenerator.Generate(ret);
                // Logger.LogSuccess("    >> Resulting Automata : ");
                // Logger.LogSuccess("    >> " + ndfa.ToString());
                // Logger.LogWarning("  >> NDFA Generation over.");
                // Logger.LogWarning("  >> Creating the Deterministic Finite Automaton (DFA) from the previously generated NDFA.");
                Automata dfa = DfaGenerator.Generate(ndfa);
                // Logger.LogSuccess("    >> Resulting Automata : ");
                // Logger.LogSuccess("    >> " + dfa.ToString());
                // Logger.LogWarning("  >> DFA Generation over.");
                // Logger.LogWarning("  >> ...");
                string text;
                do
                {
                    Logger.LogInfo("  >> Please enter some text or -1 to exit : ", false);
                    text = Console.ReadLine();

                    if (text == "-2")
                    {
                        goto start;
                    }
                    else if (text == "-1")
                    {
                        break;
                    }

                    if (dfa.TryText(text))
                        Logger.LogSuccess("    >> accepted");
                    else
                        Logger.LogError("    >> rejected");
                } while (true);


                Logger.LogWarning("  >> Exiting...");
            }
            catch (Exception e)
            {
                Logger.LogError("  >> ERROR: " + e.Message);
                Logger.LogError("  >> TRACE: " + e.StackTrace);
            }
        }

    }

}