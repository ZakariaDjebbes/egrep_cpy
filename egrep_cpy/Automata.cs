using System.Numerics;
namespace EGREP_CPY;

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

    private int TryChar(char c, int from)
    {
        List<int> transition = Transitions.Find(t => t[0] == from && t[2] == c)
                               ?? new List<int>();


        var count = Transitions.FindAll(t => t[0] == from && t[2] == c).Count;

        if (count > 1) // Should never happen to be asked about
            throw new InvalidDataException("There is more than one transition with the same input");

        if (transition.Count == 0)
        {
            return -1;
        }
        else
        {
            return transition[1];
        }
    }

    public Dictionary<Vector2, string> MatchBruteForce(string text)
    {
        var res = new Dictionary<Vector2, string>();
        var initialState = initialStates[0];
        var matchStart = -1;

        for (int i = 0; i < text.Length; i++)
        {
            var c = text[i];
            var currentState = TryChar(c, initialState);

            if (currentState != -1)
            {
                initialState = currentState;

                if (matchStart == -1)
                    matchStart = i;

                if (IsFinalState(currentState))
                {
                    res.Add(new Vector2(matchStart, i), text.SubStr(matchStart, i));
                    initialState = initialStates[0];
                    matchStart = -1;
                }
            }
            else 
            {
                initialState = initialStates[0];
                matchStart = -1;
            }
        }

        return res;
    }

    public bool IsFinalState(int state) => FinalStates.Contains(state);

    public bool IsInitialStates(int state) => InitialStates.Contains(state);

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
