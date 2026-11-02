using UnityEngine;

// Batiment.
//
// Il existe quatre sortes de bâtiments (voir l'énumération CityBuildingType). Certains bâtiments répondent à un besoin
// des personnages (voir classe CharacterVitals). Voici la liste des batiments possibles :
//
//      * Workplace - Travail. Ne répond à aucun besoin.
//      * Food - Épicerie, pour manger. Baisse "Hunger".
//      * Social - Cafés, pour socialiser. Baisse "Loneliness".
//      * House - Maison, pour dormir. Baisse "Sleepiness".
//
// Les batiments ont une entrée. Les personnages doivent se rendre à cette position pour entrer dans le batiment. Il est
// bien important de comprendre que les personnages n'entrent pas réellement dans le bâtiment : ils deviennent
// invisibles pour "simuler" le fait qu'ils y entrent.
//
// Cette classe implémente IDestination (voir les commentaires de cette interface pour les détails). Aussi, vous n'avez
// pas à toucher cette classe pour le travail.
public class Building : MonoBehaviour, IDestination
{
    [SerializeField] private CityBuildingType type = CityBuildingType.House;
    [SerializeField] private GameObject entrance;

    public CityBuildingType Type => type;
    public Vector3 Position => entrance.transform.position;
    public bool IsAvailable => isActiveAndEnabled;

    public enum CityBuildingType
    {
        House,
        Workplace,
        Food,
        Social
    }
}