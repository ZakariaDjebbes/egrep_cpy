#nullable disable
using System.Collections;
using System.Collections.Generic;

namespace EgrepCpy;

public static class Extensions
{
    public static void AddKeys<K, V>(this IDictionary<K, V> dictionary, IEnumerable<K> items)
    {
        foreach (var item in items)
        {
            dictionary.Add(item, (V)Activator.CreateInstance(typeof(V)));
        }
    }

    public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
    {
        if (list is null)
        {
            throw new ArgumentNullException(nameof(list));
        }

        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        if (list is List<T> asList)
        {
            asList.AddRange(items);
        }
        else
        {
            foreach (var item in items)
            {
                list.Add(item);
            }
        }
    }

    public static bool ContainsList(this List<List<int>> me, List<int> other)
    {
        if (me is null)
        {
            throw new ArgumentNullException(nameof(me));
        }

        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        if (me.Count == 0)
        {
            return false;
        }

        foreach (var list in me)
        {
            if(list.OrderBy(x => x).SequenceEqual(other.OrderBy(x => x)))
                return true;
        }

        return false;
    }

    public static int GetIndexOfList(this List<List<int>> me, List<int> other)
    {
        if (me is null)
        {
            throw new ArgumentNullException(nameof(me));
        }

        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        if (me.Count == 0)
        {
            return -1;
        }

        for (int i = 0; i < me.Count; i++)
        {
            if (me[i].OrderBy(x => x).SequenceEqual(other.OrderBy(x => x)))
                return i;
        }

        return -1;
    }

    public static String ListToString<T>(this IList<T> list)
    {
        if (list is null)
        {
            throw new ArgumentNullException(nameof(list));
        }

        String result = "[";

        for (int i = 0; i < list.Count; i++)
        {
            result += list[i];
            if (i != list.Count - 1)
            {
                result += ",";
            }
        }

        result += "]";

        return result;
    }

    public static String DictionaryToString<K, V>(this IDictionary<K, V> dictionary, Type T = null)
    {
        if (dictionary is null)
        {
            throw new ArgumentNullException(nameof(dictionary));
        }

        String result = "{";

        foreach (var key in dictionary.Keys)
        {
            result += $"{key} : {dictionary[key]}";

            if (!key.Equals(dictionary.Keys.Last()))
            {
                result += ",";
            }
        }

        result += "}";

        return result;
    }

    public static string SubStr(this string str, int start, int end) => str.Substring(start, end - start + 1);

}