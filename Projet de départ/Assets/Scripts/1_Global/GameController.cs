using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

// Game Controller.
//
// Vous le trouverez dans le GameObject "GameController". Il contient des références vers d'autres objets dans la scène.
// Il agit comme une sorte de répertoire. En temps normal, vous ne devriez pas avoir à y toucher.
//
// Ce controleur est accessible facilement avec le Finder. Par exemple, si vous désirez obtenir toutes les maisons :
// 
//     CityBuilding[] houses = Finder.GameController.CityObjects.HouseBuildings;
//
// Vous n'avez pas à toucher cette classe pour le travail.
public class GameController : MonoBehaviour
{
    [SerializeField] private CityObjects cityObjects = new();
    [SerializeField] private ObjectPools objectPools = new();

    public CityObjects CityObjects => cityObjects;
    public ObjectPools ObjectPools => objectPools;

    #region (Ne pas toucher) Editor related stuff.

#if UNITY_EDITOR
    public void FindObjectsInScene()
    {
        cityObjects.FindObjectsInScene();
    }
#endif

    #endregion
}

[Serializable]
public sealed class CityObjects
{
    [Header("Actors")]
    [SerializeField] private Character[] characters;
    [SerializeField] private CharacterSpawnPoint[] characterSpawnPoints;

    [Header("Locations")]
    [SerializeField] private Building[] houseBuildings;
    [SerializeField] private Building[] workplaceBuildings;
    [SerializeField] private Building[] foodBuildings;
    [SerializeField] private Building[] socialBuildings;

    // Actors
    public Character[] Characters => characters;
    public CharacterSpawnPoint[] CharacterSpawnPoints => characterSpawnPoints;

    // Locations
    public Building[] HouseBuildings => houseBuildings;
    public Building[] WorkplaceBuildings => workplaceBuildings;
    public Building[] FoodBuildings => foodBuildings;
    public Building[] SocialBuildings => socialBuildings;

    #region (Ne pas toucher) Editor related stuff.

#if UNITY_EDITOR
    public void FindObjectsInScene()
    {
        // Find objects in the scene.
        characters = Object.FindObjectsByType<Character>();
        characterSpawnPoints = Object.FindObjectsByType<CharacterSpawnPoint>();

        var buildings = Object.FindObjectsByType<Building>();
        houseBuildings = buildings.Where(it => it.Type == Building.CityBuildingType.House).ToArray();
        workplaceBuildings = buildings.Where(it => it.Type == Building.CityBuildingType.Workplace).ToArray();
        foodBuildings = buildings.Where(it => it.Type == Building.CityBuildingType.Food).ToArray();
        socialBuildings = buildings.Where(it => it.Type == Building.CityBuildingType.Social).ToArray();

        // Sort arrays for easier debugging.
        Array.Sort(characters, (a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
    }
#endif

    #endregion
}

[Serializable]
public sealed class ObjectPools
{
    [SerializeField] private ObjectPool trash;

    public ObjectPool Trash => trash;
}

#region (Ne pas toucher) Editor related stuff.

#if UNITY_EDITOR
[CustomEditor(typeof(GameController))]
public class CityGameControllerEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var inspector = new VisualElement();
        var targetObject = (target as GameController)!;

        // Base inspector.
        InspectorElement.FillDefaultInspector(inspector, serializedObject, this);

        // Callbacks.
        void OnFindBuildingsClick()
        {
            Undo.RecordObject(this, "Find objects in scene");
            targetObject.FindObjectsInScene();
        }

        // Custom inspector.
        var toolsLabel = new Label("Tools");
        toolsLabel.style.marginTop = 12;
        toolsLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        inspector.Add(toolsLabel);

        var findBuildingsButton = new Button();
        findBuildingsButton.text = "Find City Objects in Scene";
        findBuildingsButton.clicked += OnFindBuildingsClick;
        inspector.Add(findBuildingsButton);

        return inspector;
    }
}
#endif

#endregion