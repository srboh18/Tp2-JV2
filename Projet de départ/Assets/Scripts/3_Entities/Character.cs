using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Personnage.
//
// Vous allez appeler des fonctions sur cette classe à partir des états de votre "State Machine". En d'autres termes,
// cette classe est là pour vous rendre service.
//
// Voici les fonctions qui vous seront utiles :
//
//      * NavigateTo : Cette fonction prend une destination en paramètre et déplace le personnage vers cette
//        destination. Vous pouvez vérifier si le personnage est proche d'une destination avec la fonction IsCloseTo.
//        Vous pouvez également annuler un déplacement avec la fonction CancelNavigate.
//      * PickUpTrash : Cette fonction prends un déchet en paramètre et démarre une routine pendant laquelle le
//        personnage ramasse ce déchet. Vous pouvez vérifier si le personnage est en train de ramasser un déchet avec
//        la fonction IsPickingTrash.
//      * ThrowTrash : Cette fonction démarre une routine où le personnage lance un déchet. Vous pouvoir vérifier si le
//        personnage est en train de lancer un déchet avec la fonction IsThrowingTrash.
//      * PickNextThrowTrashTime : Cette fonction choisie aléatoirement quand le personnage va lancer un déchet. Cette
//        information est conservée dans l'attribut nextThrowTrashTime. Pour vérifier si le temps est venu pour le
//        personnage de lancer un déchet, utiliser la propriété ShouldThrowTrash.
//      * GreetCharacter : Cette fonction prends un personnage en paramètre et démarre une routine pendant laquelle le
//        personnage actuel salue l'autre personnage (avec un signe de la main). Vous pouvez vérifier si le personnage
//        est en train d'en saluer un autre avec la fonction IsGreetingCharacter.
//      * MakeVisible : Cette fonction rends le personnage visible. Lorsqu'il est visible, ses MeshRenderer et ses
//        colliders sont activés. Le personnage est donc affiché à l'écran et il peut être détecté via les fonctions
//        OnCollisionEnter et OnTriggerEnter.
//      * MakeInvisible : Cette fonction rends le personnage invisible. Lorsqu'il est invisible, ses MeshRenderer et ses
//        colliders sont désactivés. Le personnage est n'est donc plus affiché à l'écran et il peut pas être détecté via
//        les fonctions OnCollisionEnter et OnTriggerEnter.
//
// Voici les propriétés qui vous seront utiles :
//
//      * Position : Position du personnage dans le monde.
//      * Vitals : Signes vitaux du personnage.
//      * IsAvailable : Est-ce que le personnage peut être dérangé (pour, par exemple, le saluer) ?
//      * ShouldThrowTrash : Est-ce que le personnage devrait lancer un déchet maintenant ?
//      * TrashBehaviour : Est-ce que le personnage lance ou ramasse les déchets ?
//
// Cette classe implémente IDestination (voir les commentaires de cette interface pour les détails). Aussi, vous aurez à
// modifier cette classe pour le travail.
public class Character : MonoBehaviour, IDestination
{
    [Header("Identity")]
    [field: SerializeField] public string FullName { get; private set; } = "Character";
    [field: SerializeField] public Sprite Avatar { get; private set; }
    [SerializeField] private GameObject pointer;
    
    [Header("Behaviour")]
    [SerializeField] private CityCharacterTrashBehaviour trashBehaviour = CityCharacterTrashBehaviour.Ignore;
    [SerializeField, Min(0)] private float minThrowDelay = 5f;
    [SerializeField, Min(0)] private float maxThrowDelay = 60f;
    
    [Header("Movement")]
    [SerializeField, Min(0)] private float destinationReachedDistance = 0.5f;
    [SerializeField] private Vector3 initialDestination;

    private GameController gameController;
    private CharacterVitals vitals;
    private CharacterBlackboard blackboard;
    private CharacterAnimation characterAnimation;
    private NavMeshAgent navMeshAgent;
    private CapsuleCollider collider;
    private Renderer[] renderers;
    private float nextThrowTrashTime;

    public CharacterVitals Vitals => vitals;
    public CharacterBlackboard Blackboard => blackboard;

    public Vector3 Position
    {
        get { return transform.position; }
    }

    public bool IsAvailable
    {
        get { return isActiveAndEnabled && collider.enabled; }
    }

    public bool ShouldThrowTrash
    {
        get { return trashBehaviour == CityCharacterTrashBehaviour.Throw && Time.time > nextThrowTrashTime; }
    }

    public CityCharacterTrashBehaviour TrashBehaviour
    {
        get { return trashBehaviour; }
    }

    private void Awake()
    {
        // Get dependencies.
        gameController = Finder.GameController;
        vitals = GetComponent<CharacterVitals>();
        blackboard = GetComponent<CharacterBlackboard>();
        characterAnimation = GetComponent<CharacterAnimation>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        collider = GetComponent<CapsuleCollider>();
        renderers = GetComponentsInChildren<Renderer>();

        // Init values.
        nextThrowTrashTime = PickNextThrowTrashTime();
        
        // Init starting position.
        // TODO : Placer le personnage à un endroit aléatoire.
    }

    // TODO : Enlever la méthode Start. Ce n'est que pour la courte démo quand vous partez le jeu la première fois.
    //        Enlever aussi le [SerializedField] "initialDestination" par la même occasion.
    private IEnumerator Start()
    {
        // Move.
        navMeshAgent.SetDestination(initialDestination);
        yield return new WaitUntil(() => Vector3.Distance(transform.position, initialDestination) < destinationReachedDistance);
        
        // Greet.
        characterAnimation.PlayGreetAnimation();
    }

    public void NavigateTo(IDestination destination)
    {
        IEnumerator Routine()
        {
            // The NavMeshAgent needs time to initialize. Wait for it to be ready before issuing a command.
            while (!navMeshAgent.isOnNavMesh) yield return null;

            // Issue the navigate command.
            navMeshAgent.SetDestination(destination.Position);
        }

        StartCoroutine(Routine());
    }

    public void CancelNavigate()
    {
        navMeshAgent.ResetPath();
    }

    public bool IsCloseTo(IDestination destination)
    {
        return Vector3.Distance(transform.position, destination.Position) < destinationReachedDistance;
    }

    public void PickUpTrash(Trash trash)
    {
        IEnumerator Routine()
        {
            // Cancel navigation.
            navMeshAgent.ResetPath();

            // Play the pick-up animation.
            characterAnimation.PlayPickUpAnimation();

            // Look at the trash.
            transform.LookAt(trash.transform.position, Vector3.up);

            // Wait for animation to end.
            yield return new WaitUntil(() => !characterAnimation.IsPlayingPickUpAnimation());

            // Make trash disappear (should be safe even if trash is cleaned by someone else).
            gameController.ObjectPools.Trash.Release(trash.gameObject);

            // Remove trash from memory.
            // TODO : Enlevez le déchet ramassé de la mémoire du personnage. Par exemple :
            //      blackboard.Trash = null;

#if UNITY_EDITOR
            // Append the character name to the trash GameObject name for debugging.
            trash.gameObject.name = $"Trash (picked up by {FullName})";
#endif
        }

        StartCoroutine(Routine());
    }

    public bool IsPickingTrash()
    {
        return characterAnimation.IsPlayingPickUpAnimation();
    }

    public void ThrowTrash()
    {
        IEnumerator Routine()
        {
            // Cancel navigation.
            navMeshAgent.ResetPath();

            // Play the throw animation.
            characterAnimation.PlayThrowAnimation();

            // Wait for animation to end.
            yield return new WaitUntil(() => !characterAnimation.IsPlayingThrowAnimation());

            // Put trash at feet.
            var trash = gameController.ObjectPools.Trash.Place(transform.position);
            
            // Pick next throw trash time.
            nextThrowTrashTime = PickNextThrowTrashTime();

#if UNITY_EDITOR
            // Append the character name to the trash GameObject name for debugging.
            trash.gameObject.name = $"Trash (thrown by {FullName})";
#endif
        }

        StartCoroutine(Routine());
    }

    public bool IsThrowingTrash()
    {
        return characterAnimation.IsPlayingThrowAnimation();
    }

    private float PickNextThrowTrashTime()
    {
        return Time.time + Random.Range(minThrowDelay, maxThrowDelay);
    }

    public void GreetCharacter(Character character)
    {
        IEnumerator Routine()
        {
            // Cancel navigation.
            navMeshAgent.ResetPath();

            // Play the greet animation.
            characterAnimation.PlayGreetAnimation();

            // Look at the other character.
            transform.LookAt(character.transform.position, Vector3.up);

            // Wait for animation to end.
            yield return new WaitUntil(() => !characterAnimation.IsPlayingGreetAnimation());

            // Remove friend from memory.
            // TODO : Enlevez l'ami salué de la mémoire du personnage. Par exemple :
            //      blackboard.Friend = null;
        }

        StartCoroutine(Routine());
    }

    public bool IsGreetingCharacter()
    {
        return characterAnimation.IsPlayingGreetAnimation();
    }

    public void MakeVisible(bool visibility = true)
    {
        // Collider makes this character detectable by other characters.
        collider.enabled = visibility;

        // NavMeshAgent ensure that this character is avoided by the others.
        navMeshAgent.enabled = visibility;

        // Renderers make this character visible to the player (in the camera).
        for (var i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = visibility;
        }
    }

    public void MakeInvisible()
    {
        MakeVisible(false);
    }

    public void ShowPointer(bool visibility = true)
    {
        pointer.SetActive(visibility);
    }
    
    public void HidePointer()
    {
        ShowPointer(false);
    }
}

public enum CityCharacterTrashBehaviour
{
    Ignore,
    PickUp,
    Throw
}