using UnityEngine;

// Destination.
//
// Une destination a une position et une disponibilité. Une destination peut être non disponible pour plusieurs raisons.
// Par exemple, un déchet ayant été ramassé n'est plus disponible. Aussi, un personnage ayant entré dans un batiment
// devient lui aussi indisponible. C'est à prendre en considération dans votre machine à état, puisque votre personnage
// pourrait devoir annuler l'action qu'il est en train de faire.
//
// Vous n'avez pas à toucher cette classe pour le travail.
public interface IDestination
{
    public Vector3 Position { get; }
    public bool IsAvailable { get; }
}