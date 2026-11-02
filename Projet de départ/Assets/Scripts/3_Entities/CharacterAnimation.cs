using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

// Animations d'un personnage.
//
// Vous n'avez pas à toucher cette classe pour le travail.
public class CharacterAnimation : MonoBehaviour
{
    private static readonly int Blend = Animator.StringToHash("Blend");
    private static readonly int PickUp = Animator.StringToHash("PickUp");
    private static readonly int Throw = Animator.StringToHash("Throw");
    private static readonly int Greet = Animator.StringToHash("Greet");

    [Header("Movement")]
    [SerializeField] private float maximumSpeed = 5;

    [Header("Sounds")]
    [SerializeField] private AudioClip pickupAudioClip;
    [SerializeField, Range(0, 1)] private float pickupAudioClipVolume = 0.3f;
    [SerializeField] private AudioClip throwAudioClip;
    [SerializeField, Range(0, 1)] private float throwAudioClipVolume = 0.3f;
    [SerializeField] private AudioClip greetAudioClip;
    [SerializeField, Range(0, 1)] private float greetAudioClipVolume = 0.1f;
    [SerializeField] private AudioClip[] footstepAudioClips;
    [SerializeField, Range(0, 1)] private float footstepAudioClipVolume = 0.015f;

    private Animator animator;
    private NavMeshAgent agent;
    private SoundAudioSource soundAudioSource;
    private bool isAnimationPlaying;

    private void Awake()
    {
        // Get components.
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        soundAudioSource = GetComponent<SoundAudioSource>();

        // Prevent script from running if no Animator or NavMeshAgent is found.
        enabled = animator != null && agent != null;
    }

    private void Update()
    {
        // Get normalized speed from nav mesh agent.
        var normalizedSpeed = Mathf.InverseLerp(0, maximumSpeed, agent.velocity.magnitude);

        // Update animator with character speed.
        animator.SetFloat(Blend, normalizedSpeed);
    }

    public void PlayPickUpAnimation()
    {
        animator.SetTrigger(PickUp);
        agent.ResetPath();
        isAnimationPlaying = true;
    }

    public bool IsPlayingPickUpAnimation()
    {
        return isAnimationPlaying;
    }

    public void PlayThrowAnimation()
    {
        animator.SetTrigger(Throw);
        agent.ResetPath();
        isAnimationPlaying = true;
    }

    public bool IsPlayingThrowAnimation()
    {
        return isAnimationPlaying;
    }

    public void PlayGreetAnimation()
    {
        animator.SetTrigger(Greet);
        agent.ResetPath();
        isAnimationPlaying = true;
    }

    public bool IsPlayingGreetAnimation()
    {
        return isAnimationPlaying;
    }

    [UsedImplicitly]
    public void PlayPickupSound()
    {
        soundAudioSource.PlayOneShot(pickupAudioClip, pickupAudioClipVolume);
    }
    
    [UsedImplicitly]
    public void PlayThrowSound()
    {
        soundAudioSource.PlayOneShot(throwAudioClip, throwAudioClipVolume);
    }
    
    [UsedImplicitly]
    public void PlayGreetSound()
    {
        soundAudioSource.PlayOneShot(greetAudioClip, greetAudioClipVolume);
    }
    
    [UsedImplicitly]
    public void PlayFootstepSound()
    {
        soundAudioSource.PlayOneShot(footstepAudioClips.Random(), footstepAudioClipVolume);
    }

    [UsedImplicitly]
    public void NotifyAnimationEnd()
    {
        isAnimationPlaying = false;
    }
}