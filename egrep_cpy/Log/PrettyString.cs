namespace EgrepCpy.Log;

public struct PrettyString
{
    public string Text { get; set; }
    public List<ConsoleColor> Colors { get; set; }

    public PrettyString(string text, List<ConsoleColor> colors)
    {
        Text = text;
        Colors = colors;
    }

    public PrettyString(string text, ConsoleColor color)
    {
        Text = text;
        Colors = new List<ConsoleColor>( Enumerable.Repeat(color, text.Length));
    }

    public PrettyString(string text)
    {
        Text = text;
        Colors = new List<ConsoleColor>(Enumerable.Repeat(Console.ForegroundColor, text.Length));
    }
}