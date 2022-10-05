using EgrepCpy.RegEx;

namespace EgrepCpy.Automaton;

public class Automata
{
    private List<int> initialStates;
    private List<int> finalStates;
    private List<List<int>> transitions;

    public Automata()
    {
        initialStates = new List<int>();
        finalStates = new List<int>();
        transitions = new List<List<int>>();
    }

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

    public List<MatchResult> Match(string input) => input.Split('\n').SelectMany((line, index) => MatchLine(line, index)).ToList();

    private List<MatchResult> MatchLine(string line, int index)
    {
        var res = new List<MatchResult>();

        var initialState = initialStates[0];

        for (int start = 0; start < line.Length; start++)
        {
            var ends = MatchFrom(line, start, initialState);

            res.AddRange(ends.Select(x => new MatchResult(index, start, x)));
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
