public struct Automata
{
    private int[] initialStates;
    private int[] finalStates;
    private int[,] transitions;

    public Automata(int[] initialStates, int[] finalStates, int[,] transitions)
    {
        this.initialStates = initialStates;
        this.finalStates = finalStates;
        this.transitions = transitions;
    }

    public int[] InitialStates { get => initialStates; set => initialStates = value; }
    public int[] FinalStates { get => finalStates; set => finalStates = value; }
    public int[,] Transitions { get => transitions; set => transitions = value; }
}


public static class NFAGenerator
{
}