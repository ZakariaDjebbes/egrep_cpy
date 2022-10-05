namespace EgrepCpy;

public struct MatchResult 
{
    public int Line { get; set; }
    public int Start { get; set; }
    public int End { get; set; }

    public MatchResult(int line, int start, int end)
    {
        Line = line;
        Start = start;
        End = end;
    }

    public override string ToString() => $"Line {Line} : {Start} - {End}";
}