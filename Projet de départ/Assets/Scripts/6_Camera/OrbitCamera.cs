using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

// Caméra orbitale (style Sims City).
//
// Vous n'avez pas à toucher cette classe pour le travail.
public class OrbitCamera : MonoBehaviour
{
    [Header("Initialisation")]
    [SerializeField, Min(1)] private float initialZoom = 11;

    [Header("Inputs")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference rotateAction;
    [SerializeField] private InputActionReference zoomAction;
    [SerializeField] private InputActionReference runAction;

    [Header("Position")]
    [SerializeField, Min(1)] private float distance = 45f;
    [SerializeField, Range(0, 90)] private float verticalAngle = 30f;
    [SerializeField, Range(0, 360)] private float horizontalAngle = 45f;
    [SerializeField, Min(1)] private int rotationCount = 4;

    [Header("Movement")]
    [SerializeField, Min(0)] private float movementSpeed = 2f;
    [SerializeField, Min(0)] private float zoomSpeed = 50f;
    [SerializeField, Min(0)] private float rotationSpeed = 5f;
    [SerializeField, Min(0)] private float runMovementSpeedMultiplier = 2f;
    [SerializeField, Min(0)] private float runZoomSpeedMultiplier = 2f;
    [SerializeField, Min(0)] private float runRotationSpeedMultiplier = 2f;
    [SerializeField, Min(0)] private float movementZoomMultiplier = 0.1f;
    [SerializeField, Min(0)] private float minZoom = 2;
    [SerializeField, Min(0)] private float maxZoom = 15;

    private CinemachineCamera virtualCamera;

    private Vector3 tracking;
    private int panning;
    private float panningAngle;
    private float zoom;

    public float Zoom => zoom;
    public float MinZoom => minZoom;
    public float MaxZoom => maxZoom;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineCamera>();
        virtualCamera.Lens.ModeOverride = LensSettings.OverrideModes.Orthographic;
    }

    private void Start()
    {
        tracking = Vector3.zero;
        panning = 0;
        panningAngle = 0f;
        zoom = initialZoom;
    }

    private void Update()
    {
        var cameraInputs = ReadInputs();

        UpdateTracking(cameraInputs);
        UpdatePanning(cameraInputs);
        UpdateZoom(cameraInputs);

        MoveCamera();
    }

    private CameraInputs ReadInputs()
    {
        var moveAction = this.moveAction.action;
        var rotateAction = this.rotateAction.action;
        var zoomAction = this.zoomAction.action;
        var runAction = this.runAction.action;

        return new CameraInputs
        {
            TrackingDirection = moveAction.ReadValue<Vector2>(),
            PanningDirection = rotateAction.triggered ? -rotateAction.ReadValue<float>().RoundToInt() : 0,
            ZoomDelta = zoomAction.ReadValue<float>(),
            IsRunning = runAction.IsPressed()
        };
    }

    private void UpdateTracking(CameraInputs cameraInputs)
    {
        var trackingDirection = cameraInputs.TrackingDirection;
        var isRunning = cameraInputs.IsRunning;

        var speed = movementSpeed * (isRunning ? runMovementSpeedMultiplier : 1f);
        speed += (maxZoom - zoom) * movementZoomMultiplier;

        tracking += trackingDirection * (speed * zoom * Time.unscaledDeltaTime);
    }

    private void UpdatePanning(CameraInputs cameraInputs)
    {
        var panningDirection = cameraInputs.PanningDirection;
        var isRunning = cameraInputs.IsRunning;

        var speed = rotationSpeed * (isRunning ? runRotationSpeedMultiplier : 1f);

        panning += panningDirection;
        panningAngle = Mathf.Lerp(panningAngle, panning * (360f / rotationCount), speed * Time.unscaledDeltaTime);
    }

    private void UpdateZoom(CameraInputs cameraInputs)
    {
        var zoomDelta = cameraInputs.ZoomDelta;
        var isRunning = cameraInputs.IsRunning;

        var speed = zoomSpeed * (isRunning ? runZoomSpeedMultiplier : 1f);

        zoom = Mathf.Clamp(zoom - zoomDelta * speed * Time.unscaledDeltaTime, minZoom, maxZoom);
    }

    private void MoveCamera()
    {
        var trackingRotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0);
        var panningRotation = Quaternion.Euler(0, panningAngle, 0);
        var rotation = panningRotation * trackingRotation;
        var trackingDistance = new Vector3(0, 0, -distance);

        virtualCamera.transform.position = rotation * (tracking + trackingDistance);
        virtualCamera.transform.rotation = rotation;
        virtualCamera.Lens.OrthographicSize = zoom;
    }

    private struct CameraInputs
    {
        public Vector3 TrackingDirection;
        public int PanningDirection;
        public float ZoomDelta;
        public bool IsRunning;
    }
}