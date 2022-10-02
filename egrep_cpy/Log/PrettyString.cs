namespace Egrep_Cpy.Log;

public struct PrettyString
{
    public string Text { get; set; }
    public ConsoleColor Color { get; set; }
    
    public PrettyString(string text, ConsoleColor color)
    {
        Text = text;
        Color = color;
    }

    public PrettyString(string text)
    {
        Text = text;
        Color = ConsoleColor.White;
    }
}