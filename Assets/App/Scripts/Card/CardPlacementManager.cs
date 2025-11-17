using UnityEngine;
using UnityEngine.InputSystem;

public class CardPlacementManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float gridSize = 1;

    //[Header("References")]
    Camera cam;

    Card currentCardHandle;

    [Header("Input")]
    [SerializeField] InputActionReference mousePosition;
    [SerializeField] InputActionReference clearCardHandle;

    //[Header("Output")]

    public static CardPlacementManager Instance;

    private void OnEnable()
    {
        clearCardHandle.action.started += ClearCardHandle;
    }
    private void OnDisable()
    {
        clearCardHandle.action.started -= ClearCardHandle;
    }

    private void Awake()
    {
        cam = Camera.main;

        Instance = this;
    }

    private void Update()
    {
        MoveCurrentCardHandle();
    }

    void MoveCurrentCardHandle()
    {
        if (!currentCardHandle) return;

        Vector2 mousePos = mousePosition.action.ReadValue<Vector2>();
        Vector2 gridPos = ((Vector2)Vector2Int.RoundToInt(cam.ScreenToWorldPoint(mousePos))) * gridSize;

        currentCardHandle.transform.position = gridPos;
    }

    public void HandleNewCard(SSO_CardData card)
    {
        if (currentCardHandle) Destroy(currentCardHandle.gameObject);

        currentCardHandle = Instantiate(card.cardPrefab, transform);
    }

    void ClearCardHandle(InputAction.CallbackContext context)
    {
        if(currentCardHandle) 
            Destroy(currentCardHandle.gameObject);
    }
}