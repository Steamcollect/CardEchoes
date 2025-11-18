using System.Collections;
using System.Collections.Generic;
using ToolBox.Dev;
using UnityEngine;
using UnityEngine.InputSystem;
using static CardPlacementManager;

public class CardPlacementManager : MonoBehaviour
{
    [Header("Settings")]
    public int gridSize = 1;
    bool canPlaceCard = true;
    [SerializeField] float timeBetweenWave = .2f;

    [Space(10)]
    [SerializeField] Vector2Int minMaxXCadPos;
    [SerializeField] Vector2Int minMaxYCadPos;
    Vector2Int currentCardHandleGridPos;

    [Header("References")]
    [SerializeField] Card cardPrefab;
    [SerializeField, Inline] StartCard[] defaultCardsData;

    [System.Serializable]
    public struct StartCard
    {
        public Vector2Int position;
        public SSO_CardData cardData;
    }

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

    private void Start()
    {
        foreach (StartCard startCard in defaultCardsData)
        {
            Card card = Instantiate(cardPrefab, transform);
            card.Setup(startCard.cardData);
            card.transform.position = new Vector3(startCard.position.x * gridSize, 0, startCard.position.y * gridSize);
            cards.Add(startCard.position, card);
        }

        // Setup neighbours
        foreach (var kvp in cards)
        {
            Vector2Int pos = kvp.Key;
            Card card = kvp.Value;

            List<Card> neighbours = new List<Card>();

            Vector2Int[] dirs = {
            Vector2Int.up * gridSize,
            Vector2Int.down * gridSize,
            Vector2Int.left * gridSize,
            Vector2Int.right * gridSize
        };

            foreach (var dir in dirs)
            {
                Vector2Int nPos = pos + dir;
                if (cards.ContainsKey(nPos))
                {
                    Card neighbour = cards[nPos];
                    neighbours.Add(neighbour);

                    // Ajouter la carte courante comme voisin de son voisin
                    List<Card> nList = new List<Card>(neighbour.GetNeighbours() ?? new Card[0]);
                    if (!nList.Contains(card))
                        nList.Add(card);
                    neighbour.SetNeighbours(nList.ToArray());
                }
            }

            card.SetNeighbours(neighbours.ToArray());
        }
    }


    private void Update()
    {
        MoveCurrentCardHandle();
    }

    void MoveCurrentCardHandle()
    {
        if (!currentCardHandle) return;

        Vector2 mousePos = mousePosition.action.ReadValue<Vector2>();

        // Ray depuis la caméra
        Ray ray = cam.ScreenPointToRay(mousePos);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // Plan Y = 0

        float enter;
        if (groundPlane.Raycast(ray, out enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            Vector2Int gridPos = new Vector2Int(
                Mathf.RoundToInt(hitPoint.x / gridSize),
                Mathf.RoundToInt(hitPoint.z / gridSize)
            );

            gridPos.x = Mathf.Clamp(gridPos.x, minMaxXCadPos.x, minMaxXCadPos.y);
            gridPos.y = Mathf.Clamp(gridPos.y, minMaxYCadPos.x, minMaxYCadPos.y);

            currentCardHandleGridPos = gridPos;

            Vector3 worldPos = new Vector3(gridPos.x * gridSize, 0, gridPos.y * gridSize);
            currentCardHandle.transform.position = worldPos;

            currentCardHandle.GetGraphics().sortingOrder = -gridPos.y;
        }
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

        Vector2Int[] dirs = {
            Vector2Int.up * gridSize,
            Vector2Int.down * gridSize,
            Vector2Int.left * gridSize,
            Vector2Int.right * gridSize
        };

        foreach (var dir in dirs)
        {
            Vector2Int nPos = currentCardHandleGridPos + dir;
            if (cards.ContainsKey(nPos))
            {
                Card neighbour = cards[nPos];
                neighbours.Add(neighbour);

                List<Card> nList = new List<Card>(neighbour.GetNeighbours() ?? new Card[0]);
                nList.Add(currentCardHandle);
                neighbour.SetNeighbours(nList.ToArray());
            }
        }

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

            int distance = Mathf.Abs(pos.x - startingPos.x) + Mathf.Abs(pos.y - startingPos.y);

            if (distance > maxIteration)
                maxIteration = distance;
        }
        if (maxIteration <= 0)
        {
            InventoryManager.Instance.AddNewCard();
            yield break;
        }

        int iterations = 1;

        Card card;
        do
        {
            for (int dx = -iterations; dx <= iterations; dx++)
            {
                int dy = iterations - Mathf.Abs(dx);

                Vector2Int pos1 = new Vector2Int(startingPos.x + dx, startingPos.y + dy);

                
                if (cards.ContainsKey(pos1))
                {
                    card = cards[pos1];
                    card.GetData().ApplyEffectToNeighbour(card);
                }

                if (dy != 0)
                {
                    Vector2Int pos2 = new Vector2Int(startingPos.x + dx, startingPos.y + -dy);

                    if (cards.ContainsKey(pos2))
                    {
                        card = cards[pos2];
                        card.GetData().ApplyEffectToNeighbour(card);
                    }
                }
            }

            iterations++;
            yield return new WaitForSeconds(timeBetweenWave);
        }
        while(iterations <= maxIteration);

        InventoryManager.Instance.AddNewCard();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(minMaxXCadPos.x * gridSize, 0, minMaxYCadPos.x * gridSize),
                        new Vector3(minMaxXCadPos.y * gridSize, 0, minMaxYCadPos.x * gridSize));
        Gizmos.DrawLine(new Vector3(minMaxXCadPos.y * gridSize, 0, minMaxYCadPos.y * gridSize),
                        new Vector3(minMaxXCadPos.y * gridSize, 0, minMaxYCadPos.x * gridSize));
        
        Gizmos.DrawLine(new Vector3(minMaxXCadPos.x * gridSize, 0, minMaxYCadPos.y * gridSize),
                        new Vector3(minMaxXCadPos.x * gridSize, 0, minMaxYCadPos.x * gridSize));
        
        Gizmos.DrawLine(new Vector3(minMaxXCadPos.x * gridSize, 0, minMaxYCadPos.y * gridSize),
                        new Vector3(minMaxXCadPos.y * gridSize, 0, minMaxYCadPos.y * gridSize));

        Gizmos.color = Color.red;
        foreach (StartCard card in defaultCardsData)
        {
            Gizmos.DrawSphere(new Vector3(card.position.x * gridSize, 0, card.position.y * gridSize), .1f);
        }
    }
}