using EgrepCpy.Automaton;

namespace EgrepCpy.Algorithms;

public class KnuthMorrisPratt
{
    public string Pattern
    {
        get => pattern;

        set
        {
            pattern = value;
            carryOver = GetCarryOver(pattern);
        }
    }

    public int[] CarryOver => carryOver;

    private int[] carryOver = new int[0];
    private string pattern = String.Empty;

    public KnuthMorrisPratt(string _pattern)
    {
        Pattern = _pattern;
        carryOver = GetCarryOver(_pattern);
    }

    public List<MatchResult> Search(string input)
    {
        var matches = new List<MatchResult>();

        var texts = input.Split('\n');

        for (int line = 0; line < texts.Length; line++)
        {
            var text = texts[line];
            var j = 0;
            var k = 0;

            while (j < text.Length)
            {
                if (text[j] == pattern[k])
                {
                    j++;
                    k++;

                    if (k == pattern.Length)
                    {
                        var start = j - k;
                        matches.Add(new MatchResult(line, start, start + pattern.Length));
                        k = carryOver[k - 1];
                    }
                }
                else
                {
                    k = carryOver[k];
                    if (k < 0)
                    {
                        j++;
                        k++;
                    }
                }
            }
        }

        return matches;
    }

    private int[] GetCarryOver(string pattern)
    {
        var carryOver = new int[pattern.Length + 1];
        carryOver[0] = -1;
        carryOver[carryOver.Length - 1] = 0;

        for (int i = 1; i < pattern.Length; i++)
        {
            var max = GetMaxPrefixSuffix(pattern.SubStr(0, i - 1));
            carryOver[i] = max;
        }

        for (int i = 1; i < pattern.Length; i++)
        {
            if (carryOver[i] != -1 && pattern[carryOver[i]] == pattern[i] && carryOver[carryOver[i]] == -1)
            {
                carryOver[i] = -1;
            }
        }

        for (int i = 0; i < pattern.Length; i++)
        {
            if (carryOver[i] != -1 && pattern[carryOver[i]] == pattern[i] && carryOver[carryOver[i]] != -1)
            {
                carryOver[i] = carryOver[carryOver[i]];
            }
        }

        return carryOver;
    }

    private static int GetMaxPrefixSuffix(string pattern)
    {
        var n = pattern.Length;
        var lps = new int[n];
        var length = 0;
        var i = 1;

        lps[0] = 0;

        while (i < n)
        {
            if (pattern[i] == pattern[length])
            {
                length++;
                lps[i] = length;
                i++;
            }
            else
            {
                if (length != 0)
                {
                    length = lps[length - 1];
                }
                else
                {
                    lps[i] = 0;
                    i++;
                }
            }
        }

        int res = lps[n - 1];

        return (res > n / 2) ? n / 2 : res;
    }
}