using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 minMaxX = new Vector2(-10, 10);
    [SerializeField] private Vector2 minMaxY = new Vector2(-10, 10);
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float smoothTime = 0.15f;

    [Header("References")]
    [SerializeField] private InputActionReference moveInput;

    private Vector3 velocity = Vector3.zero;   // utilisé par SmoothDamp

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector2 input = moveInput.action.ReadValue<Vector2>();

        // Direction caméra (X = gauche/droite, Z = haut/bas)
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
}