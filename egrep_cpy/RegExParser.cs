public static class RegExParser
{
    //FROM REGEX TO SYNTAX TREE
    public static RegExTree parse(String regEx)
    {
        List<RegExTree> result = new List<RegExTree>();

        for (int i = 0; i < regEx.Length; i++)
        {
            result.Add(new RegExTree(charToRoot(regEx[i]), new List<RegExTree>()));
        }

        return parse(result);
    }

    private static int charToRoot(char c)
    {
        if (c == '.') return Program.DOT;
        if (c == '*') return Program.ETOILE;
        if (c == '|') return Program.ALTERN;
        if (c == '(') return Program.PARENTHESEOUVRANT;
        if (c == ')') return Program.PARENTHESEFERMANT;

        return (int)c;
    }

    private static RegExTree parse(List<RegExTree> result)
    {
        while (containParenthese(result)) result = processParenthese(result);
        while (containEtoile(result)) result = processEtoile(result);
        while (containConcat(result)) result = processConcat(result);
        while (containAltern(result)) result = processAltern(result);

        if (result.Count > 1) throw new Exception();

        return removeProtection(result[0]);
    }
    private static bool containParenthese(List<RegExTree> trees)
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

    private static List<RegExTree> processParenthese(List<RegExTree> trees)
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
                subTrees.Add(parse(content));
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

    private static bool containEtoile(List<RegExTree> trees)
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

    private static List<RegExTree> processEtoile(List<RegExTree> trees)
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
    
    private static bool containConcat(List<RegExTree> trees)
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

    private static List<RegExTree> processConcat(List<RegExTree> trees)
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


    private static bool containAltern(List<RegExTree> trees)
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

    private static List<RegExTree> processAltern(List<RegExTree> trees)
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

    private static RegExTree removeProtection(RegExTree tree)
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
            return removeProtection(tree.SubTrees[0]);
        }

        List<RegExTree> subTrees = new List<RegExTree>();

        foreach (RegExTree t in tree.SubTrees)
        {
            subTrees.Add(removeProtection(t));
        }

        return new RegExTree(tree.Root, subTrees);
    }
}