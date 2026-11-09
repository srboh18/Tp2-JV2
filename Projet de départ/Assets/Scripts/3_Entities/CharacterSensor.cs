using System.Linq;
using UnityEngine;

public class CharacterSensor : MonoBehaviour
{
    private Character character;
    public CharacterBlackboard Blackboard { get; private set; }

    private void Awake()
    {
        Blackboard = GetComponent<CharacterBlackboard>();
        character = GetComponentInParent<Character>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Détection des déchets.
        Trash trash = other.GetComponentInParent<Trash>();

        if (trash != null)
        {
            if (character.TrashBehaviour == CityCharacterTrashBehaviour.PickUp)
            {
                Blackboard.LastTrashSeen = trash;
            }

            return;
        }

        // Détection des amis.
        Character friend = other.GetComponentInParent<Character>();

        if (friend != null && friend != character)
        {
            if (Blackboard.Friends != null &&
                Blackboard.Friends.Contains(friend))
            {
                Blackboard.LastFriendSeen = friend;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Le déchet n'est plus détecté.
        Trash trash = other.GetComponentInParent<Trash>();

        if (trash != null && Blackboard.LastTrashSeen == trash)
        {
            Blackboard.LastTrashSeen = null;
            return;
        }

        // L'ami n'est plus détecté.
        Character friend = other.GetComponentInParent<Character>();

        if (friend != null && Blackboard.LastFriendSeen == friend)
        {
            Blackboard.LastFriendSeen = null;
        }
    }
}