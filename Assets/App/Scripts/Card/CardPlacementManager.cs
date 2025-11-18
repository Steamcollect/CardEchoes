using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardPlacementManager : MonoBehaviour
{
    [Header("Settings")]
    public int gridSize = 1;
    bool canPlaceCard = true;
    [SerializeField] float timeBetweenWave = .2f;

    Vector2Int currentCardHandleGridPos;

    [Header("References")]
    [SerializeField] Card cardPrefab;

    Camera cam;

    Dictionary<Vector2Int /*Position*/, Card> cards = new Dictionary<Vector2Int, Card>();

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
        currentCardHandleGridPos = Vector2Int.RoundToInt(cam.ScreenToWorldPoint(mousePos)) * gridSize;
        currentCardHandle.GetGraphics().sortingOrder = -currentCardHandleGridPos.y;

        currentCardHandle.transform.position = (Vector2)currentCardHandleGridPos;
    }

    public void HandleNewCard(SSO_CardData card, CardControllerUI ui)
    {
        if (currentCardHandle) Destroy(currentCardHandle.gameObject);

        currentCardHandleUI = ui;
        currentCardHandle = Instantiate(cardPrefab, transform);
        currentCardHandle.Setup(card);
    }

    void PlaceCardHandle(InputAction.CallbackContext context)
    {
        if (!currentCardHandle || !canPlaceCard)
        {
            canPlaceCard = true;
            return;
        }

        if(cards.ContainsKey(currentCardHandleGridPos))
        {
            Destroy(cards[currentCardHandleGridPos].gameObject);
            cards[currentCardHandleGridPos] = currentCardHandle;
        }
        else
            cards.Add(currentCardHandleGridPos, currentCardHandle);

        List<Card> neighbours = new List<Card>();
        if (cards.ContainsKey(currentCardHandleGridPos + Vector2Int.up * gridSize)) neighbours.Add(cards[currentCardHandleGridPos + Vector2Int.up]);
        if (cards.ContainsKey(currentCardHandleGridPos + Vector2Int.down * gridSize)) neighbours.Add(cards[currentCardHandleGridPos + Vector2Int.down]);
        if (cards.ContainsKey(currentCardHandleGridPos + Vector2Int.left * gridSize)) neighbours.Add(cards[currentCardHandleGridPos + Vector2Int.left]);
        if (cards.ContainsKey(currentCardHandleGridPos + Vector2Int.right * gridSize)) neighbours.Add(cards[currentCardHandleGridPos + Vector2Int.right]);
        currentCardHandle.SetNeighbours(neighbours.ToArray());

        Destroy(currentCardHandleUI.gameObject);
        currentCardHandle = null;

        StartCoroutine(CheckCardsNeighbour(currentCardHandleGridPos));
    }
    void ClearCardHandle(InputAction.CallbackContext context)
    {
        if(currentCardHandle) 
            Destroy(currentCardHandle.gameObject);
    }

    public void SetCanPlaceCard(bool value) { canPlaceCard = value; }

    IEnumerator CheckCardsNeighbour(Vector2Int startingPos)
    {
        int maxIteration = 0;
        foreach (var kvp in cards)
        {
            Vector2Int pos = kvp.Key;

            int value = Mathf.Max(Mathf.Abs(pos.x), Mathf.Abs(pos.y));

            if (value > maxIteration)
                maxIteration = value;
        }
        if(maxIteration <= 0) yield break;

        int iterations = 1;
        do
        {
            for (int dx = -iterations; dx <= iterations; dx++)
            {
                int dy = iterations - Mathf.Abs(dx);

                Vector2Int pos1 = new Vector2Int(startingPos.x + dx, startingPos.y + dy);
                Card card = cards[pos1];
                card.GetData().ApplyEffectToNeighbour(card);

                if (dy != 0)
                {
                    Vector2Int pos2 = new Vector2Int(startingPos.x + dx, startingPos.y + -dy);
                    card = cards[pos2];
                    card.GetData().ApplyEffectToNeighbour(card);
                }
            }

            yield return new WaitForSeconds(timeBetweenWave);
        }
        while(iterations <= maxIteration);
    }
}