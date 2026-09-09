using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    private static readonly System.Random rng = new();

    // Extension method for List<T> to get a random element
    public static T GetRandom<T>(this IList<T> list, bool ignoreEmpty = true)
    {
        if (list == null || list.Count == 0)
        {
            if (!ignoreEmpty)
                Debug.LogError("The list cannot be null or empty.");
            return default;
        }

        int index = UnityEngine.Random.Range(0, list.Count);
        return list[index];
    }

    // Extension method to shuffle a list
    public static void Shuffle<T>(this IList<T> list)
    {
        if (list == null || list.Count <= 1) return;

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]); // Swap (C# 7.0+)
        }
    }

    // Extension method to pop the last element from the list
    public static T Pop<T>(this IList<T> list)
    {
        if (list == null || list.Count == 0)
            throw new System.InvalidOperationException("Cannot pop from an empty list.");

        int lastIndex = list.Count - 1;
        T item = list[lastIndex];
        list.RemoveAt(lastIndex);
        return item;
    }
}