using Egrep_Cpy.RegEx;

namespace Egrep_Cpy.Generation;

public static class NdfaGenerator
{
    private const int UPPER_ASCII_BOUND = 128; // ASCII NON EXTENDED
    private const int LOWER_ASCII_BOUND = 0;
    private static int StateCounter
    {
        get
        {
            stateCounter++;
            return stateCounter - 1;
        }
    }
    private static int stateCounter = 0;

    public static Automata Generate(RegExTree tree)
    {
        Dictionary<RegExTree, Automata> intermediateAutomatas = new Dictionary<RegExTree, Automata>();

        tree.SetupComputationState();
        while (!tree.IsComputed)
        {
            tree.UpdateComputationState();
            List<RegExTree> toBeComputed = GetToBeComputedNodes(tree);

            foreach (RegExTree node in toBeComputed)
            {
                Automata result;

                switch (node.Root)
                {
                    case RegExTree.CONCAT: // .
                        var leftChild = intermediateAutomatas.Where(x => x.Key == node.SubTrees[0]).First();
                        var rightChild = intermediateAutomatas.Where(x => x.Key == node.SubTrees[1]).First();

                        result = ComputeConcat(node, leftChild.Value, rightChild.Value);

                        intermediateAutomatas.Remove(leftChild.Key);
                        intermediateAutomatas.Remove(rightChild.Key);

                        intermediateAutomatas.Add(node, result);
                        break;
                    case RegExTree.REPEAT: // *
                        var child = intermediateAutomatas.Where(x => x.Key == node.SubTrees[0]).First();

                        result = ComputeRepeat(node, child.Value);

                        intermediateAutomatas.Remove(child.Key);
                        intermediateAutomatas.Add(node, result);
                        break;
                    case RegExTree.ALTERN: // | 
                        var left = intermediateAutomatas.Where(x => x.Key == node.SubTrees[0]).First();
                        var right = intermediateAutomatas.Where(x => x.Key == node.SubTrees[1]).First();

                        result = ComputeAltern(node, left.Value, right.Value);

                        intermediateAutomatas.Remove(left.Key);
                        intermediateAutomatas.Remove(right.Key);
                        intermediateAutomatas.Add(node, result);
                        break;
                    case RegExTree.ANY: // any
                        intermediateAutomatas.Add(node, ComputeAny(node));
                        break;
                    default: // ANY CHARACTER EXCEPT ANY (.)
                        intermediateAutomatas.Add(node, ComputeDefaultNodes(node));
                        break;
                }
            }
        }

        if (intermediateAutomatas.Count != 1)
        {
            throw new Exception("Something went wrong");
        }

        return intermediateAutomatas.First().Value;
    }

    private static Automata ComputeAny(RegExTree node)
    {
        var initialState = StateCounter;
        var finalState = StateCounter;

        Automata result = new Automata
        {
            InitialStates = new List<int> { initialState },
            FinalStates = new List<int> { finalState },
            Transitions = new List<List<int>>()
        };

        for (int i = LOWER_ASCII_BOUND; i < UPPER_ASCII_BOUND; i++)
        {
            if ((char)i != '\n') //????
                result.Transitions.Add(new List<int> { initialState, finalState, i });
        }

        return result;
    }

    private static Automata ComputeAltern(RegExTree node, Automata left, Automata right)
    {
        int newStart = StateCounter;
        int newEnd = StateCounter;
        int oldLeftStart = left.InitialStates[0];
        int oldLeftEnd = left.FinalStates[0];
        int oldRightStart = right.InitialStates[0];
        int oldRightEnd = right.FinalStates[0];


        left.Transitions.AddRange(new List<List<int>>()
        {
            new List<int> { newStart, oldLeftStart, RegExTree.EPSILON },
            new List<int> { newStart, oldRightStart, RegExTree.EPSILON },
            new List<int> { oldLeftEnd, newEnd, RegExTree.EPSILON },
            new List<int> { oldRightEnd, newEnd, RegExTree.EPSILON }
        });

        left.Transitions.AddRange(right.Transitions);

        return new Automata
        {
            InitialStates = new List<int> { newStart },
            FinalStates = new List<int> { newEnd },
            Transitions = left.Transitions
        };
    }

    private static Automata ComputeConcat(RegExTree node, Automata left, Automata right)
    {
        int newStart = left.InitialStates[0];
        int newEnd = right.FinalStates[0];
        int oldLeftEnd = left.FinalStates[0];
        int oldRightStart = right.InitialStates[0];

        left.Transitions.Add(new List<int> { oldLeftEnd, oldRightStart, RegExTree.EPSILON });
        left.Transitions.AddRange(right.Transitions);

        return new Automata
        {
            InitialStates = new List<int> { newStart },
            FinalStates = new List<int> { newEnd },
            Transitions = left.Transitions
        };
    }

    private static Automata ComputeRepeat(RegExTree node, Automata childAutomata)
    {
        int newStart = StateCounter;
        int newEnd = StateCounter;
        int oldStart = childAutomata.InitialStates[0];
        int oldEnd = childAutomata.FinalStates[0];

        childAutomata.Transitions.AddRange(new List<List<int>>()
        {
            new List<int> { newStart, oldStart, RegExTree.EPSILON },
            new List<int> { oldEnd, newEnd, RegExTree.EPSILON },
            new List<int> { oldEnd, oldStart, RegExTree.EPSILON },
            new List<int> { newStart, newEnd, RegExTree.EPSILON },
        });

        return new Automata
        {
            InitialStates = new List<int> { newStart },
            FinalStates = new List<int> { newEnd },
            Transitions = childAutomata.Transitions
        };
    }


    private static Automata ComputeDefaultNodes(RegExTree node)
    {
        Automata result;
        int state1 = StateCounter;
        int state2 = StateCounter;
        List<List<int>> transitions = new List<List<int>>()
        {
            new List<int> { state1, state2, node.Root }
        };

        result = new Automata(new List<int> { state1 },
                              new List<int> { state2 },
                              transitions
                              );
        return result;
    }

    private static List<RegExTree> GetToBeComputedNodes(RegExTree tree)
    {
        List<RegExTree> result = new List<RegExTree>();

        if (tree.ToBeComputed)
        {
            tree.ToBeComputed = false;
            result.Add(tree);
        }

        foreach (RegExTree subTree in tree.SubTrees)
        {
            result.AddRange(GetToBeComputedNodes(subTree));
        }

        return result;
    }
}