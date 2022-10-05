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

    public void Append(string text, List<ConsoleColor> colors)
    {
        Text += text;
        Colors.AddRange(colors);
    }

    public void Append(string text, ConsoleColor color)
    {
        Text += text;
        Colors.AddRange(Enumerable.Repeat(color, text.Length));
    }

    public void Push(string text, List<ConsoleColor> colors)
    {
        Text = text + Text;
        Colors.InsertRange(0, colors);
    }

    public void Push(string text, ConsoleColor color)
    {
        Text = text + Text;
        Colors.InsertRange(0, Enumerable.Repeat(color, text.Length));
    }
}