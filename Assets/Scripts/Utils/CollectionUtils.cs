using System;
using System.Collections;

public static class CollectionUtils
{
    public static bool IsNullOrEmpty(this ICollection collection)
    {
        return collection == null || collection.Count <= 0;
    }

    public static bool HasValues(this ICollection collection)
    {
        return !collection.IsNullOrEmpty();
    }

    public static void ForEach<T>(this System.Collections.Generic.IEnumerable<T> enumerables, Action<T> action)
    {
        foreach (T enumerable in enumerables)
        {
            action(enumerable);
        }
    }
}