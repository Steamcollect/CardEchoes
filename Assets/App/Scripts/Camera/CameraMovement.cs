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
        transform.position = Origin - Difference;
    }

    private Vector3 GetMousePosition => mainCamera.ScreenToWorldPoint((Vector3)Mouse.current.position.ReadValue());
}
