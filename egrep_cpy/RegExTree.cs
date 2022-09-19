public class RegExTree
{

    public int Root { get; set; }
    public List<RegExTree> SubTrees { get => subTrees; set => subTrees = value; }

    private List<RegExTree> subTrees;

    public RegExTree(int root, List<RegExTree> subTrees)
    {
        Root = root;
        this.subTrees = subTrees;
    }

    //FROM TREE TO PARENTHESIS
    public String toString()
    {
        if (!subTrees.Any())
        {
            return rootToString();
        }

        String result = rootToString() + "(" + subTrees[0].toString();

        for (int i = 1; i < subTrees.Count; i++)
        {
            result += "," + subTrees[i].toString();
        }

        return result + ")";
    }
    private String rootToString()
    {
        if (Root == Program.CONCAT) return ".";
        if (Root == Program.ETOILE) return "*";
        if (Root == Program.ALTERN) return "|";
        if (Root == Program.DOT) return ".";
        return Convert.ToString((char)Root);
    }
}