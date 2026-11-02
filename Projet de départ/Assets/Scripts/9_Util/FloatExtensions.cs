using UnityEngine;

public static class FloatExtensions
{
    public static int RoundToInt(this float value)
    {
        return Mathf.RoundToInt(value);
    }
}