using System;
using UnityEngine;

public static class ArrayExtensions
{
    public static T Random<T>(this T[] elements)
    {
        return elements[UnityEngine.Random.Range(0, elements.Length)];
    }

    public static T[] Shuffle<T>(this T[] elements)
    {
        for (var i = 0; i < elements.Length - 1; i++)
        {
            var j = UnityEngine.Random.Range(i + 1, elements.Length);
            (elements[i], elements[j]) = (elements[j], elements[i]);
        }
        return elements;
    }

    public static T[] Slice<T>(this T[] elements, int length)
    {
        return elements[..length];
    }
}