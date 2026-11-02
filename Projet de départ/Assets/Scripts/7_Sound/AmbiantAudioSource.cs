using UnityEngine;

// Sons ambiants (ajusté au zoom de la caméra).
//
// Vous n'avez pas à toucher cette classe pour le travail.
public class AmbiantAudioSource : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private AudioClip audioClip;
    [SerializeField, Min(0)] private float volume = 1f;

    [Header("Camera zoom")]
    [SerializeField] private int minZoom = 2;
    [SerializeField] private int maxZoom = 15;
    [SerializeField, Min(0)] private float zoomFalloff = 1f;
    [SerializeField] private AnimationCurve zoomVolumeCurve = new(new Keyframe(0, 0, 0, 0), new Keyframe(1, 0.7f, 2, 2));

    private OrbitCamera camera;
    private AudioSource audioSource;

    private void Awake()
    {
        camera = Finder.Camera;
        audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = audioClip;
        audioSource.volume = 0;
        audioSource.loop = true;
    }

    private void OnEnable()
    {
        if (audioClip) audioSource.Play();
    }

    private void OnDisable()
    {
        audioSource.Stop();
    }

    private void Update()
    {
        // Get normalized zoom level.
        var normalizedZoom = Mathf.InverseLerp(maxZoom, minZoom, camera.Zoom * (1 / zoomFalloff));

        // Set volume from normalized zoom level. If zoomed far away, volume should decrease.
        audioSource.volume = volume * zoomVolumeCurve.Evaluate(normalizedZoom);
    }
}