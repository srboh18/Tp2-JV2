using UnityEngine;

public static class RandomUtils
{
    public static bool Chance(float chances)
    {
        return Random.Range(0, 100) <= chances;
    }
}