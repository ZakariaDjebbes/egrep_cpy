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
                if (j == 2)
                    result += $"{CodeToString(table[i][j])}";
                else
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

    public bool TryText(string text)
    {
        var currentState = InitialStates[0];
        var currentChar = 0;

        while (currentChar < text.Length)
        {
            List<int> transition = Transitions.Find(t => t[0] == currentState && t[2] == text[currentChar])
                                   ?? new List<int>();

            if (transition.Count == 0)
                return false;

            if (IsFinalState(transition[1]))
                return true;

            currentState = transition[1];
            currentChar++;
        }

        return FinalStates.Contains(currentState);
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
        if (code == Program.CONCAT) return ".";
        if (code == Program.ETOILE) return "*";
        if (code == Program.ALTERN) return "|";
        if (code == Program.DOT) return ".";
        return Convert.ToString((char)code);
    }

    private bool IsDefaultTransition(int code)
        => code != Program.CONCAT && code != Program.ETOILE && code != Program.ALTERN && code != Program.DOT && code != Program.EPSILON;
}
