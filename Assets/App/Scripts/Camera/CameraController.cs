using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 minMaxX = new Vector2(-10, 10);
    [SerializeField] private Vector2 minMaxY = new Vector2(-10, 10);
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float smoothTime = 0.15f;

    [Space(10)]
    [SerializeField] float shakeAngle = 15;
    [SerializeField] float shakeDuration = 0.2f;

    [Header("Zoom Settings")]
    [SerializeField] private InputActionReference zoomInput;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float maxZoomDistance = 20f;

    private float currentZoom = 0f;
    private Vector3 initialPosition;

    [Header("References")]
    [SerializeField] private InputActionReference moveInput;

    private Vector3 velocity = Vector3.zero;

    public static CameraController Instance;

    private void Awake()
    {
        Instance = this;
        initialPosition = transform.position;
    }

    private void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    private void HandleMovement()
    {
        Vector2 input = moveInput.action.ReadValue<Vector2>().normalized;

        Vector3 targetPosition = transform.position +
            new Vector3(input.x, 0, input.y) * moveSpeed * Time.deltaTime;

        // Clamp dans les limites
        targetPosition.x = Mathf.Clamp(targetPosition.x, minMaxX.x, minMaxX.y);
        targetPosition.z = Mathf.Clamp(targetPosition.z, minMaxY.x, minMaxY.y);

        // Smooth movement
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }

    private void HandleZoom()
    {
        float scrollValue = zoomInput.action.ReadValue<Vector2>().y;

        if (Mathf.Abs(scrollValue) > 0.01f)
        {
            // Mise à jour du zoom souhaité
            currentZoom += scrollValue * zoomSpeed;

            // Clamp pour ne pas dépasser la distance max avant/arrière
            currentZoom = Mathf.Clamp(currentZoom, -maxZoomDistance, maxZoomDistance);

            // Nouvelle position = position initiale + forward * zoom
            transform.position = initialPosition + transform.forward * currentZoom;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        // 4 coins du rectangle de limites
        Vector3 bottomLeft = new Vector3(minMaxX.x, 0, minMaxY.x);
        Vector3 bottomRight = new Vector3(minMaxX.y, 0, minMaxY.x);
        Vector3 topLeft = new Vector3(minMaxX.x, 0, minMaxY.y);
        Vector3 topRight = new Vector3(minMaxX.y, 0, minMaxY.y);

        // tracer le rectangle
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }

    public void Shake()
    {
        transform.DOPunchRotation(transform.forward * shakeAngle, shakeDuration, 20, 1);
    }
}