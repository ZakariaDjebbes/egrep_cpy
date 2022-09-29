namespace EGREP_CPY;

public static class RegExParser
{
    //FROM REGEX TO SYNTAX TREE
    public static RegExTree Parse(String regEx)
    {
        List<RegExTree> result = new List<RegExTree>();

        for (int i = 0; i < regEx.Length; i++)
        {
            result.Add(new RegExTree(CharToRoot(regEx[i]), new List<RegExTree>()));
        }

        return Parse(result);
    }

    private static int CharToRoot(char c)
    {
        if (c == '.') return Program.ANY;
        if (c == '*') return Program.ETOILE;
        if (c == '|') return Program.ALTERN;
        if (c == '(') return Program.PARENTHESEOUVRANT;
        if (c == ')') return Program.PARENTHESEFERMANT;

        return (int)c;
    }

    private static RegExTree Parse(List<RegExTree> result)
    {
        while (ContainParenthese(result)) result = ProcessParenthese(result);
        while (ContainEtoile(result)) result = ProcessEtoile(result);
        while (ContainConcat(result)) result = ProcessConcat(result);
        while (ContainAltern(result)) result = ProcessAltern(result);

        if (result.Count > 1) throw new Exception();

        return RemoveProtection(result[0]);
    }

    private static bool ContainParenthese(List<RegExTree> trees)
    {
        foreach (RegExTree t in trees)
        {
            if (t.Root == Program.PARENTHESEFERMANT || t.Root == Program.PARENTHESEOUVRANT)
            {
                return true;
            }
        }
        return false;
    }

    private static List<RegExTree> ProcessParenthese(List<RegExTree> trees)
    {
        List<RegExTree> result = new List<RegExTree>();
        bool found = false;

        foreach (RegExTree t in trees)
        {
            if (!found && t.Root == Program.PARENTHESEFERMANT)
            {
                bool done = false;
                List<RegExTree> content = new List<RegExTree>();

                while (!done && result.Any())
                {
                    if (result[result.Count - 1].Root == Program.PARENTHESEOUVRANT)
                    {
                        done = true;
                        result.RemoveAt(result.Count - 1);
                    }
                    else
                    {
                        RegExTree last = result[result.Count - 1];

                        result.RemoveAt(result.Count - 1);
                        content.Insert(0, last);
                    }
                }

                if (!done)
                {
                    throw new Exception();
                }

                List<RegExTree> subTrees = new List<RegExTree>();

                found = true;
                subTrees.Add(Parse(content));
                result.Add(new RegExTree(Program.PROTECTION, subTrees));
            }
            else
            {
                result.Add(t);
            }
        }
        if (!found) throw new Exception();
        return result;
    }

    private static bool ContainEtoile(List<RegExTree> trees)
    {
        foreach (RegExTree t in trees)
        {
            if (t.Root == Program.ETOILE && !t.SubTrees.Any())
            {
                return true;
            }
        }

        return false;
    }

    private static List<RegExTree> ProcessEtoile(List<RegExTree> trees)
    {
        List<RegExTree> result = new List<RegExTree>();
        bool found = false;

        foreach (RegExTree t in trees)
        {
            if (!found && t.Root == Program.ETOILE && !t.SubTrees.Any())
            {
                if (!result.Any())
                {
                    throw new Exception();
                }

                RegExTree last = result[result.Count - 1];
                List<RegExTree> subTrees = new List<RegExTree>();

                found = true;
                result.RemoveAt(result.Count - 1);
                subTrees.Add(last);
                result.Add(new RegExTree(Program.ETOILE, subTrees));
            }
            else
            {
                result.Add(t);
            }
        }
        return result;
    }
    
    private static bool ContainConcat(List<RegExTree> trees)
    {
        bool firstFound = false;

        foreach (RegExTree t in trees)
        {
            if (!firstFound && t.Root != Program.ALTERN)
            {
                firstFound = true; continue;
            }
            if (firstFound)
            {
                if (t.Root != Program.ALTERN)
                {
                    return true;
                }
                else
                {
                    firstFound = false;
                }
            }
        }
        return false;
    }

    private static List<RegExTree> ProcessConcat(List<RegExTree> trees)
    {
        List<RegExTree> result = new List<RegExTree>();
        bool found = false;
        bool firstFound = false;

        foreach (RegExTree t in trees)
        {
            if (!found && !firstFound && t.Root != Program.ALTERN)
            {
                firstFound = true;
                result.Add(t);
                continue;
            }

            if (!found && firstFound && t.Root == Program.ALTERN)
            {
                firstFound = false;
                result.Add(t);
                continue;
            }

            if (!found && firstFound && t.Root != Program.ALTERN)
            {
                List<RegExTree> subTrees = new List<RegExTree>();
                RegExTree last = result[result.Count - 1];

                found = true;
                result.RemoveAt(result.Count - 1);
                subTrees.Add(last);
                subTrees.Add(t);
                result.Add(new RegExTree(Program.CONCAT, subTrees));
            }
            else
            {
                result.Add(t);
            }
        }

        return result;
    }


    private static bool ContainAltern(List<RegExTree> trees)
    {
        foreach (RegExTree t in trees)
        {
            if (t.Root == Program.ALTERN && !t.SubTrees.Any())
            {
                return true;
            }
        }

        return false;
    }

    private static List<RegExTree> ProcessAltern(List<RegExTree> trees)
    {
        List<RegExTree> result = new List<RegExTree>();
        bool found = false;
        RegExTree? gauche = null;
        bool done = false;

        foreach (RegExTree t in trees)
        {
            if (!found && t.Root == Program.ALTERN && !t.SubTrees.Any())
            {
                if (!result.Any())
                {
                    throw new Exception();
                }

                found = true;
                gauche = result[result.Count - 1];
                result.RemoveAt(result.Count - 1);
                continue;
            }

            if (found && !done)
            {
                if (gauche == null)
                {
                    throw new Exception();
                }

                List<RegExTree> subTrees = new List<RegExTree>();

                done = true;
                subTrees.Add(gauche);
                subTrees.Add(t);
                result.Add(new RegExTree(Program.ALTERN, subTrees));
            }
            else
            {
                result.Add(t);
            }
        }

        return result;
    }

    private static RegExTree RemoveProtection(RegExTree tree)
    {
        if (tree.Root == Program.PROTECTION && tree.SubTrees.Count != 1)
        {
            throw new Exception();
        }

        if (!tree.SubTrees.Any())
        {
            return tree;
        }

        if (tree.Root == Program.PROTECTION)
        {
            return RemoveProtection(tree.SubTrees[0]);
        }

        List<RegExTree> subTrees = new List<RegExTree>();

        foreach (RegExTree t in tree.SubTrees)
        {
            subTrees.Add(RemoveProtection(t));
        }

        return new RegExTree(tree.Root, subTrees);
    }
}