namespace EGREP_CPY;

public static class NdfaGenerator
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

    public static Automata Generate(RegExTree tree)
    {
        // Automata result;
        Dictionary<RegExTree, Automata> intermediateAutomatas = new Dictionary<RegExTree, Automata>();

        tree.SetupComputationState();
        while (!tree.IsComputed)
        {
            tree.UpdateComputationState();
            List<RegExTree> toBeComputed = GetToBeComputedNodes(tree);
            //TODO MOVE DICTIONNARY HERE ONCE IT WORKS

            foreach (RegExTree node in toBeComputed)
            {
                Automata result;

                switch (node.Root)
                {
                    case Program.CONCAT: // .
                        var leftChild = intermediateAutomatas.Where(x => x.Key == node.SubTrees[0]).First();
                        var rightChild = intermediateAutomatas.Where(x => x.Key == node.SubTrees[1]).First();

                        result = ComputeConcat(node, leftChild.Value, rightChild.Value);

                        intermediateAutomatas.Remove(leftChild.Key);
                        intermediateAutomatas.Remove(rightChild.Key);

                        intermediateAutomatas.Add(node, result);
                        break;
                    case Program.ETOILE: // *
                        var child = intermediateAutomatas.Where(x => x.Key == node.SubTrees[0]).First();

                        result = ComputeEtoile(node, child.Value);

                        intermediateAutomatas.Remove(child.Key);
                        intermediateAutomatas.Add(node, result);
                        break;
                    case Program.ALTERN: // | 
                        var left = intermediateAutomatas.Where(x => x.Key == node.SubTrees[0]).First();
                        var right = intermediateAutomatas.Where(x => x.Key == node.SubTrees[1]).First();

                        result = ComputeAltern(node, left.Value, right.Value);

                        intermediateAutomatas.Remove(left.Key);
                        intermediateAutomatas.Remove(right.Key);
                        intermediateAutomatas.Add(node, result);
                        break;
                    default: //LETTRE
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
            new List<int> { newStart, oldLeftStart, Program.EPSILON },
            new List<int> { newStart, oldRightStart, Program.EPSILON },
            new List<int> { oldLeftEnd, newEnd, Program.EPSILON },
            new List<int> { oldRightEnd, newEnd, Program.EPSILON }
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

        left.Transitions.Add(new List<int> { oldLeftEnd, oldRightStart, Program.EPSILON });
        left.Transitions.AddRange(right.Transitions);

        return new Automata
        {
            InitialStates = new List<int> { newStart },
            FinalStates = new List<int> { newEnd },
            Transitions = left.Transitions
        };
    }

    private static Automata ComputeEtoile(RegExTree node, Automata childAutomata)
    {
        int newStart = StateCounter;
        int newEnd = StateCounter;
        int oldStart = childAutomata.InitialStates[0];
        int oldEnd = childAutomata.FinalStates[0];

        childAutomata.Transitions.AddRange(new List<List<int>>()
        {
            new List<int> { newStart, oldStart, Program.EPSILON },
            new List<int> { oldEnd, newEnd, Program.EPSILON },
            new List<int> { oldEnd, oldStart, Program.EPSILON },
            new List<int> { newStart, newEnd, Program.EPSILON },
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