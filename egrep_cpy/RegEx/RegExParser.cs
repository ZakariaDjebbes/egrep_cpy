namespace Egrep_Cpy.RegEx;

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
        if (c == '.') return RegExTree.ANY;
        if (c == '*') return RegExTree.REPEAT;
        if (c == '|') return RegExTree.ALTERN;
        if (c == '(') return RegExTree.OPENING_PARENTHESIS;
        if (c == ')') return RegExTree.CLOSING_PARENTHESIS;

        return (int)c;
    }

    private static RegExTree Parse(List<RegExTree> result)
    {
        while (ContainParenthesis(result)) result = ProcessParenthesis(result);
        while (ContainRepeat(result)) result = ProcessRepeat(result);
        while (ContainConcat(result)) result = ProcessConcat(result);
        while (ContainAltern(result)) result = ProcessAltern(result);

        if (result.Count > 1) throw new Exception();

        return RemoveProtection(result[0]);
    }

    private static bool ContainParenthesis(List<RegExTree> trees)
    {
        foreach (RegExTree t in trees)
        {
            if (t.Root == RegExTree.CLOSING_PARENTHESIS || t.Root == RegExTree.OPENING_PARENTHESIS)
            {
                return true;
            }
        }
        return false;
    }

    private static List<RegExTree> ProcessParenthesis(List<RegExTree> trees)
    {
        List<RegExTree> result = new List<RegExTree>();
        bool found = false;

        foreach (RegExTree t in trees)
        {
            if (!found && t.Root == RegExTree.CLOSING_PARENTHESIS)
            {
                bool done = false;
                List<RegExTree> content = new List<RegExTree>();

                while (!done && result.Any())
                {
                    if (result[result.Count - 1].Root == RegExTree.OPENING_PARENTHESIS)
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
                result.Add(new RegExTree(RegExTree.PROTECTION, subTrees));
            }
            else
            {
                result.Add(t);
            }
        }
        if (!found) throw new Exception();
        return result;
    }

    private static bool ContainRepeat(List<RegExTree> trees)
    {
        foreach (RegExTree t in trees)
        {
            if (t.Root == RegExTree.REPEAT && !t.SubTrees.Any())
            {
                return true;
            }
        }

        return false;
    }

    private static List<RegExTree> ProcessRepeat(List<RegExTree> trees)
    {
        List<RegExTree> result = new List<RegExTree>();
        bool found = false;

        foreach (RegExTree t in trees)
        {
            if (!found && t.Root == RegExTree.REPEAT && !t.SubTrees.Any())
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
                result.Add(new RegExTree(RegExTree.REPEAT, subTrees));
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
            if (!firstFound && t.Root != RegExTree.ALTERN)
            {
                firstFound = true; continue;
            }
            if (firstFound)
            {
                if (t.Root != RegExTree.ALTERN)
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
            if (!found && !firstFound && t.Root != RegExTree.ALTERN)
            {
                firstFound = true;
                result.Add(t);
                continue;
            }

            if (!found && firstFound && t.Root == RegExTree.ALTERN)
            {
                firstFound = false;
                result.Add(t);
                continue;
            }

            if (!found && firstFound && t.Root != RegExTree.ALTERN)
            {
                List<RegExTree> subTrees = new List<RegExTree>();
                RegExTree last = result[result.Count - 1];

                found = true;
                result.RemoveAt(result.Count - 1);
                subTrees.Add(last);
                subTrees.Add(t);
                result.Add(new RegExTree(RegExTree.CONCAT, subTrees));
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
            if (t.Root == RegExTree.ALTERN && !t.SubTrees.Any())
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
            if (!found && t.Root == RegExTree.ALTERN && !t.SubTrees.Any())
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
                result.Add(new RegExTree(RegExTree.ALTERN, subTrees));
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
        if (tree.Root == RegExTree.PROTECTION && tree.SubTrees.Count != 1)
        {
            throw new Exception();
        }

        if (!tree.SubTrees.Any())
        {
            return tree;
        }

        if (tree.Root == RegExTree.PROTECTION)
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