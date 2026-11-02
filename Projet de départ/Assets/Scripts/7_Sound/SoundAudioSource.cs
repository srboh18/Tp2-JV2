using UnityEngine;

// Sons (ajusté à la position et au zoom de la caméra).
//
// Vous n'avez pas à toucher cette classe pour le travail.
public class SoundAudioSource : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private AudioClip audioClip;
    [SerializeField, Min(0)] private float volume = 1f;

    [Header("Camera position")]
    [SerializeField, Min(0)] private float positionFalloff = 1f;
    [SerializeField] private AnimationCurve positionCurve = new(new Keyframe(0, 0, 0, 0), new Keyframe(1, 0.7f, 2, 2));

    [Header("Camera zoom")]
    [SerializeField] private int minZoom = 2;
    [SerializeField] private int maxZoom = 15;
    [SerializeField, Min(0)] private float zoomFalloff = 1f;
    [SerializeField] private AnimationCurve zoomCurve = new(new Keyframe(0, 0, 0, 0), new Keyframe(1, 0.7f, 2, 2));

    private Camera camera;
    private OrbitCamera orbitCamera;
    private AudioSource audioSource;

    private void Awake()
    {
        camera = Camera.main!;
        orbitCamera = Finder.Camera;
        audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = audioClip;
        audioSource.volume = 0;
        audioSource.loop = false;
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
        // How much the object position affect the volume output ?
        var positionFactor = GetPositionFactor();
        var positionVolumeFactor = positionCurve.Evaluate(positionFactor);
        
        // How much the camera zoom affect the volume output ?
        var zoomFactor = GetZoomFactor();
        var zoomVolumeFactor = zoomCurve.Evaluate(zoomFactor);
        
        // Set volume from normalized zoom level. If zoomed far away, volume should decrease.
        audioSource.volume = volume * positionVolumeFactor * zoomVolumeFactor;
    }

    private float GetPositionFactor()
    {
        // Get viewport visibility.
        var viewPosition = camera.WorldToViewportPoint(transform.position);
        
        // Calculate how much the object is inside the camera. A value of 0 means inside, and anything else is outside.
        var deltaX = Mathf.Max(0f, Mathf.Abs(viewPosition.x - 0.5f) - 0.5f);
        var deltaY = Mathf.Max(0f, Mathf.Abs(viewPosition.y - 0.5f) - 0.5f);
        var visibility = Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY);
    
        // Return value.
        return Mathf.Clamp01(1 - visibility * positionFalloff);
    }

    private float GetZoomFactor()
    {
        return Mathf.InverseLerp(maxZoom, minZoom, orbitCamera.Zoom * (1 / zoomFalloff));
    }

    public void PlayOneShot(AudioClip audioClip, float volumeScale = 1f)
    {
        audioSource.PlayOneShot(audioClip, volumeScale);
    }
}