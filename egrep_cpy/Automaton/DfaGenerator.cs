using EgrepCpy.RegEx;

namespace EgrepCpy.Automaton;

public static class DfaGenerator
{
    private static int StateCounter
    {
        get
        {
            stateCounter++;
            return stateCounter - 1;
        }
    }
    private static int stateCounter = 0;

    public static Automata Generate(Automata ndfa)
    {
        Automata result = new Automata()
        {
            InitialStates = new List<int>(),
            FinalStates = new List<int>(),
            Transitions = new List<List<int>>()
        };

        Dictionary<int, HashSet<int>> uniqueCharsTransitions;
        List<List<int>> treatedStates = new List<List<int>>();
        List<List<int>> oldStates = new List<List<int>>();

        oldStates.Add(new List<int>(ndfa.InitialStates));
        oldStates[0].AddRange(GetOldStates(ndfa, oldStates[0]));

        for (int i = 0; i < oldStates.Count; i++)
        {
            while (!treatedStates.ContainsList(oldStates[i]))
            {
                treatedStates.Add(oldStates[i]);
                uniqueCharsTransitions = GetUniqueCharTransitions(ndfa, oldStates[i]);
                var newState = oldStates.GetIndexOfList(oldStates[i]) == -1 ? stateCounter : oldStates.GetIndexOfList(oldStates[i]);

                if (IsOldStatesInitial(oldStates[i], ndfa))
                {
                    result.InitialStates.Add(newState);
                }

                if (IsOldStateFinal(oldStates[i], ndfa))
                {
                    result.FinalStates.Add(newState);
                }

                var transitions = new List<List<int>>();

                foreach (var uniqueChar in uniqueCharsTransitions.Keys)
                {
                    if (uniqueCharsTransitions[uniqueChar].Count > 0)
                    {
                        oldStates.Add(uniqueCharsTransitions[uniqueChar].ToList());
                        transitions.Add(new List<int> { i, oldStates.GetIndexOfList(uniqueCharsTransitions[uniqueChar].ToList()), uniqueChar });
                    }
                }

                result.Transitions.AddRange(transitions);
            }

        }

        return result;
    }

    private static bool IsOldStateFinal(List<int> oldStates, Automata ndfa)
    {
        foreach (var state in oldStates)
        {
            if (ndfa.FinalStates.Contains(state))
                return true;
        }

        return false;
    }

    private static bool IsOldStatesInitial(List<int> oldStates, Automata ndfa)
    {
        foreach (var state in oldStates)
        {
            if (ndfa.InitialStates.Contains(state))
                return true;
        }

        return false;
    }

    private static Dictionary<int, HashSet<int>> GetUniqueCharTransitions(Automata ndfa, List<int> oldStates)
    {
        var uniqueCharsTransitions = new Dictionary<int, HashSet<int>>();
        uniqueCharsTransitions.AddKeys(ndfa.GetUniqueDefaultTransitions());

        foreach (var uniqueChar in uniqueCharsTransitions.Keys)
        {
            foreach (var oldState in oldStates)
            {
                var ts = GetPassingTransitions(ndfa, oldState, uniqueChar);
                if (ts.Count > 0)
                {
                    for (int i = 0; i < ts.Count; i++)
                    {
                        ts.AddRange(GetFollowingEpsilonStates(ndfa, ts[i]));
                    }

                    foreach (var t in ts)
                    {
                        uniqueCharsTransitions[uniqueChar].Add(t);
                    }
                }
            }
        }

        return uniqueCharsTransitions;
    }

    private static List<int> GetOldStates(Automata ndfa, IList<int> olderStates)
    {
        var result = new List<int>();

        foreach (var state in olderStates)
        {
            foreach (var transition in ndfa.Transitions)
            {
                if (transition[0] == state)
                {
                    if (transition[2] == RegExTree.EPSILON)
                    {
                        result.Add(transition[1]);
                        result.AddRange(GetFollowingEpsilonStates(ndfa, transition[1]));
                    }
                }
            }
        }

        return result;
    }

    private static List<int> GetPassingTransitions(Automata ndfa, int state, int uniqueChar)
    {
        var result = new List<int>();

        foreach (var transition in ndfa.Transitions)
        {
            if (transition[0] == state && transition[2] == uniqueChar)
            {
                result.Add(transition[1]);
            }
        }


        return result;
    }

    private static IList<int> GetFollowingEpsilonStates(Automata ndfa, int state)
    {
        var result = new List<int>();

        foreach (var transition in ndfa.Transitions)
        {
            if (transition[0] == state)
            {
                if (transition[2] == RegExTree.EPSILON)
                {
                    result.Add(transition[1]);
                    result.AddRange(GetFollowingEpsilonStates(ndfa, transition[1]));
                }
            }
        }

        return result;
    }

}