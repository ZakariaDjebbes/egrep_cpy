#nullable disable

public class Program
{
    public const int CONCAT = 0xC04CA7;
    public const int ETOILE = 0xE7011E;
    public const int ALTERN = 0xA17E54;
    public const int PROTECTION = 0xBADDAD;
    public const int PARENTHESEOUVRANT = 0x16641664;
    public const int PARENTHESEFERMANT = 0x51515151;
    public const int DOT = 0xD07;

    private static string regEx = "a(b|c)*";

    public static void Main(String[] args)
    {
        Console.WriteLine("Started egrep_cpy...");

        if (args.Length != 0)
        {
            regEx = args[0];
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("  >> Please enter a regEx: ");
            regEx = Console.ReadLine();
        }

        Console.ResetColor();
        Console.WriteLine($"  >> Parsing RegEx {regEx}");
        RegExTree ret;

        if (regEx.Length < 1)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  >> ERROR: empty regEx.");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("     >> ASCII codes: [" + (int)regEx[0]);
            
            for (int i = 1; i < regEx.Length; i++)
            {
                Console.Write("," + (int)regEx[i]);
            }
            
            Console.WriteLine("].");

            try
            {
                ret = RegExParser.parse(regEx);
                Console.WriteLine("     >> Tree result: " + ret.toString() + ".");
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  >> ERROR: syntax error for regEx \"" + regEx + "\".");
                Console.WriteLine("  >> ERROR: " + e.Message);
            }
        }

        Console.ResetColor();
        Console.WriteLine("  >> Parsing over.");
        Console.WriteLine("  >> Creating the Non Deterministic Finite Automaton (NFA) from the syntax tree.");
    }

}