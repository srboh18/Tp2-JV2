using UnityEngine;

public static class GameObjectExtensions
{
    public static T AddOrGetComponent<T>(this GameObject gameObject) where T : Component
    {
        return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
    }
}