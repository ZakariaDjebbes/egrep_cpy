using EgrepCpy.RegEx;

namespace EgrepCpy.Automaton;

public struct Automata
{
    private List<int> initialStates;
    private List<int> finalStates;
    private List<List<int>> transitions;

    public Automata(List<int> initialStates, List<int> finalStates, List<List<int>> transitions)
    {
        this.initialStates = initialStates;
        this.finalStates = finalStates;
        this.transitions = transitions;
    }

    public List<int> InitialStates { get => initialStates; set => initialStates = value; }
    public List<int> FinalStates { get => finalStates; set => finalStates = value; }
    public List<List<int>> Transitions { get => transitions; set => transitions = value; }

    private string TableToString(IList<int> table)
    {
        if (table.Count == 0)
            return "[]";

        String result = "[";
        result += table[0];

        for (int i = 1; i < table.Count; i++)
        {
            result += $",{table[i]}";
        }

        result += "]";

        return result;
    }

    private string MultiDimArrayToString(List<List<int>> table)
    {
        String result = "[";

        for (int i = 0; i < table.Count; i++)
        {
            result += "[";
            for (int j = 0; j < table[i].Count; j++)
            {
                result += $"{table[i][j]}";

                if (j != table[i].Count - 1)
                    result += ",";
            }
            result += "]";
            if (i != table[i].Count - 1)
                result += ",";
        }

        result += "]";

        return result;
    }

    public List<int> GetUniqueDefaultTransitions()
    {
        var uniqueTransitions = new List<int>();

        foreach (var transition in Transitions)
        {
            if (!uniqueTransitions.Contains(transition[2]) && IsDefaultTransition(transition[2]))
            {
                uniqueTransitions.Add(transition[2]);
            }
        }

        return uniqueTransitions;
    }

    private List<int> MatchFrom(string text, int startChar, int startState)
    {
        var res = new List<int>();

        if (startChar < text.Length)
        {
            var nextState = transitions
                .Where(x => x[0] == startState && x[2] == text[startChar])
                .Select(x => x[1])
                .FirstOrDefault(-1);

            if (nextState != -1)
            {
                res = MatchFrom(text, startChar + 1, nextState);
            }
        }

        if (IsFinalState(startState))
            res.Add(startChar);

        return res;
    }

    public List<MatchResult> Match(string input)
    {
        var texts = input.Split('\n');
        var res = new List<MatchResult>();

        for (int line = 0; line < texts.Length; line++)
        {
            var text = texts[line];
            var initialState = initialStates[0];

            for (int start = 0; start < text.Length; start++)
            {
                var ends = MatchFrom(text, start, initialState);

                res.AddRange(ends.Select(x => new MatchResult(line, start, x)));
            }
        }

        return res;
    }

    public bool IsFinalState(int state) => FinalStates.Contains(state);

    public bool IsInitialState(int state) => InitialStates.Contains(state);

    public override string ToString()
    {
        return $"Initial States: {TableToString(initialStates)} Final States: {TableToString(finalStates)}"
               + $" Transitions: {MultiDimArrayToString(transitions)}";
    }

    private String CodeToString(int code)
    {
        if (code == RegExTree.CONCAT) return ".";
        if (code == RegExTree.REPEAT) return "*";
        if (code == RegExTree.ALTERN) return "|";
        if (code == RegExTree.ANY) return ".";
        if (code == RegExTree.EPSILON) return "ε";

        return Convert.ToString((char)code);
    }

    private bool IsDefaultTransition(int code)
        => code != RegExTree.CONCAT && code != RegExTree.REPEAT && code != RegExTree.ALTERN && code != RegExTree.ANY && code != RegExTree.EPSILON;
}
