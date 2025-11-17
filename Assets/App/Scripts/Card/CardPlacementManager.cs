using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardPlacementManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float gridSize = 1;
    bool canPlaceCard = true;

    //[Header("References")]
    Camera cam;

    Dictionary<Vector2 /*Position*/, Card> cards = new Dictionary<Vector2 , Card>();

    CardControllerUI currentCardHandleUI;
    Card currentCardHandle;

    [Header("Input")]
    [SerializeField] InputActionReference mousePosition;
    [SerializeField] InputActionReference placeCardHandle;
    [SerializeField] InputActionReference clearCardHandle;

    //[Header("Output")]

    public static CardPlacementManager Instance;

    private void OnEnable()
    {
        clearCardHandle.action.started += ClearCardHandle;
        placeCardHandle.action.canceled += PlaceCardHandle;
    }
    private void OnDisable()
    {
        clearCardHandle.action.started -= ClearCardHandle;
        placeCardHandle.action.canceled -= PlaceCardHandle;
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

    public void HandleNewCard(SSO_CardData card, CardControllerUI ui)
    {
        if (currentCardHandle) Destroy(currentCardHandle.gameObject);

        currentCardHandleUI = ui;
        currentCardHandle = Instantiate(card.cardPrefab, transform);
    }

    void PlaceCardHandle(InputAction.CallbackContext context)
    {
        if (!currentCardHandle || !canPlaceCard)
        {
            canPlaceCard = true;
            return;
        }

        cards.Add(currentCardHandle.transform.position, currentCardHandle);
        Destroy(currentCardHandleUI.gameObject);

        currentCardHandle = null;
    }
    void ClearCardHandle(InputAction.CallbackContext context)
    {
        if(currentCardHandle) 
            Destroy(currentCardHandle.gameObject);
    }

    public void SetCanPlaceCard(bool value) { canPlaceCard = value; }
}