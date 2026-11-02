using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class GameObjectUtils
{
#if UNITY_EDITOR

    [MenuItem("GameObject/Sort Children by Position", priority = 200)]
    public static void SortChildrenByPosition()
    {
        var gameObject = Selection.activeGameObject;
        var transform = gameObject.transform;
        Undo.RecordObject(gameObject, $"Sort {gameObject.name} children by position.");

        // Quick and dirty bubble sort.
        var childCount = transform.childCount;
        for (var k = childCount - 1; k > 0; k--)
        {
            var swapped = false;
            for (var i = 0; i < k; i++)
            {
                var lhs = transform.GetChild(i);
                var rhs = transform.GetChild(i + 1);

                if (lhs.position.CompareTo(rhs.position) > 0)
                {
                    lhs.SetSiblingIndex(i + 1);
                    rhs.SetSiblingIndex(i);
                    swapped = true;
                }
            }
            if (!swapped) break;
        }
    }

    [MenuItem("GameObject/Sort Children by Position", isValidateFunction: true)]
    public static bool ValidateSortChildrenByPosition()
    {
        return Selection.activeGameObject != null;
    }
    
    [MenuItem("GameObject/Rename Using Prefab Name", priority = 201)]
    public static void RenameUsingPrefabName()
    {
        var gameObjects = Selection.gameObjects;
        Undo.RecordObjects(gameObjects, "Rename Using Prefab Name.");

        // Object names and the current count for each.
        var names = new Dictionary<string, int>();
        
        for (var i = 0; i < gameObjects.Length; i++)
        {
            var gameObject = gameObjects[i];
            var prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(gameObject);
            if (prefab != null)
            {
                // Get index for name (incrementing if found).
                var name = prefab.name;
                var index = names.ContainsKey(name) ? ++names[name] : names[name] = 1;
                
                // Create new name.
                gameObject.name = $"{name} ({index})";
            }
        }
    }
    
    [MenuItem("GameObject/Rename Using Prefab Name", isValidateFunction: true)]
    public static bool ValidateRenameUsingPrefabName()
    {
        var gameObjects = Selection.gameObjects;
        for (var i = 0; i < gameObjects.Length; i++)
        {
            if (PrefabUtility.GetPrefabInstanceHandle(gameObjects[i]) != null)
            {
                return true;
            }
        }
        return false;
    }
    
    [MenuItem("GameObject/Rename Children by Sequence", priority = 202)]
    public static void RenameChildrenBySequence()
    {
        var gameObject = Selection.activeGameObject;
        var transform = gameObject.transform;
        Undo.RecordObject(gameObject, $"Rename {gameObject.name} children.");

        // Object names and the current count for each.
        var names = new Dictionary<string, int>();

        // Rename children.
        var childCount = transform.childCount;
        for (var i = 0; i < childCount; i++)
        {
            // Get child.
            var child = transform.GetChild(i);
            
            // Get name (without any indexes).
            var name = child.name;
            var endPosition = name.IndexOf('(');
            if (endPosition >= 0) name = name[..endPosition];
            name = name.Trim();
            
            // Get index for name (incrementing if found).
            var index = names.ContainsKey(name) ? ++names[name] : names[name] = 1;

            // Create new name.
            child.name = $"{name} ({index})";
        }
    }

    [MenuItem("GameObject/Rename Children by Sequence", isValidateFunction: true)]
    public static bool ValidateRenameChildrenBySequence()
    {
        var gameObject = Selection.activeGameObject;

        if (gameObject == null) return false;
        if (gameObject.transform.childCount == 0) return false;

        return true;
    }
#endif
}