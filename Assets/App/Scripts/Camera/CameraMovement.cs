using UnityEngine;
using UnityEngine.InputSystem;
public class CameraMovement : MonoBehaviour
{

    [Header("References")]

    [SerializeField] private InputActionReference dragInput;

    [Header("Parameters")]

    [SerializeField] private Vector3 Origin;
    [SerializeField] private Vector3 Difference;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private bool isDragging;

    [Header("Camera Limits")]

    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    private void OnEnable()
    {
        dragInput.action.started += ctx => Origin = GetMousePosition;
        dragInput.action.performed += ctx => isDragging = true;
        dragInput.action.canceled += ctx => isDragging = false;
    }
    private void Awake()
    {
        mainCamera = Camera.main;
    }
    
    private void LateUpdate()
    {
        if (!isDragging) return;
        Difference = GetMousePosition - transform.position;
        Vector3 targetPosition = Origin - Difference;

        targetPosition.x = Mathf.Clamp (targetPosition.x = minX, maxX, 0);
        targetPosition.y = Mathf.Clamp (targetPosition.y = minY, maxY, 0);

        transform.position = targetPosition;
    }

    private Vector3 GetMousePosition => mainCamera.ScreenToWorldPoint((Vector3)Mouse.current.position.ReadValue());

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector2(minX, maxY), new Vector2(maxX, maxY));
        Gizmos.DrawLine(new Vector2(minX, minY), new Vector2(maxX, minY));
        Gizmos.DrawLine(new Vector2(minX, minY), new Vector2(minX, maxY));
        Gizmos.DrawLine(new Vector2(maxX, minY), new Vector2(maxX, maxY));
        
    }
}
