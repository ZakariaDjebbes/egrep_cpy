namespace EGREP_CPY;

public class RegExTree
{
    public int Root { get; set; }
    public List<RegExTree> SubTrees { get; set; }
    public RegExTree? Parent { get; set; }
    public bool ToBeComputed
    {
        get => toBeComputed;
        set
        {
            toBeComputed = value;
            IsComputed = !value;
        }
    }
    public bool IsComputed { get; private set; }
    public bool IsLeaf { get => SubTrees.Count == 0; }

    private bool toBeComputed;

    public RegExTree(int root, List<RegExTree> subTrees)
    {
        Root = root;
        SubTrees = subTrees;
    }

    //FROM TREE TO PARENTHESIS
    public override String ToString()
    {
        if (!SubTrees.Any())
        {
            return RootToString();
        }

        String result = RootToString() + "(" + SubTrees[0].ToString();

        for (int i = 1; i < SubTrees.Count; i++)
        {
            result += "," + SubTrees[i].ToString();
        }

        return result + ")";
    }

    public String RootToString()
    {
        if (Root == Program.CONCAT) return ".";
        if (Root == Program.ETOILE) return "*";
        if (Root == Program.ALTERN) return "|";
        if (Root == Program.ANY) return "any";
        if (Root == Program.EPSILON) return "ε";

        return Convert.ToString((char)Root);
    }

    public void SetupComputationState()
    {
        SetParents();

        if (IsLeaf)
        {
            ToBeComputed = true;
        }

        foreach (RegExTree subTree in SubTrees)
        {
            subTree.SetupComputationState();
        }
    }

    public void UpdateComputationState()
    {
        if (!IsLeaf && AllSubTreesComputed() && !(Parent?.ToBeComputed).GetValueOrDefault() && !IsComputed)
        {
            ToBeComputed = true;
        }

        foreach (RegExTree subTree in SubTrees)
        {
            subTree.UpdateComputationState();
        }
    }

    private bool AllSubTreesComputed()
    {
        foreach (RegExTree subTree in SubTrees)
        {
            if (!subTree.IsComputed)
            {
                return false;
            }
        }

        return true;
    }

    private void SetParents()
    {
        foreach (RegExTree subTree in SubTrees)
        {
            subTree.Parent = this;
            subTree.SetParents();
        }
    }
}