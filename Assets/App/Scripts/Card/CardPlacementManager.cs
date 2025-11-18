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

        // Ray depuis la caméra
        Ray ray = cam.ScreenPointToRay(mousePos);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // Plan Y = 0

        float enter;
        if (groundPlane.Raycast(ray, out enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            // Convertir en grille (x, y mais ta grille est X/Z)
            Vector2Int gridPos = new Vector2Int(
                Mathf.RoundToInt(hitPoint.x / gridSize),
                Mathf.RoundToInt(hitPoint.z / gridSize)
            );

            currentCardHandleGridPos = gridPos;

            // Set position réelle dans le monde
            Vector3 worldPos = new Vector3(gridPos.x * gridSize, 0, gridPos.y * gridSize);
            currentCardHandle.transform.position = worldPos;

            // Tri d’affichage si tu veux toujours l’ordre par profondeur
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

    IEnumerator CheckCardsNeighbour(Vector2Int startingPoint)
    {
        // Ensemble des positions déjà visitées (pour ne jamais repasser deux fois sur une carte)
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        // Liste des cartes à traiter pour cette vague
        List<Vector2Int> currentWave = new List<Vector2Int>();

        // Ajout des cartes adjacentes au point initial
        Vector2Int[] dirs = new Vector2Int[]
        {
        Vector2Int.up * gridSize,
        Vector2Int.down * gridSize,
        Vector2Int.left * gridSize,
        Vector2Int.right * gridSize
        };

        foreach (var dir in dirs)
        {
            Vector2Int pos = startingPoint + dir;
            if (cards.ContainsKey(pos))
            {
                currentWave.Add(pos);
                visited.Add(pos);
            }
        }

        // --- Début des vagues BFS ---
        while (currentWave.Count > 0)
        {
            List<Vector2Int> nextWave = new List<Vector2Int>();

            foreach (var pos in currentWave)
            {
                Card card = cards[pos];

                // Étape 1 : appliquer effet
                card.GetData().ApplyEffectToNeighbour(card);

                // Étape 2 : ajouter toutes les cartes adjacentes non visitées
                foreach (var dir in dirs)
                {
                    Vector2Int newPos = pos + dir;

                    if (cards.ContainsKey(newPos) && !visited.Contains(newPos))
                    {
                        visited.Add(newPos);
                        nextWave.Add(newPos);
                    }
                }
            }

            // On passe à la vague suivante
            currentWave = nextWave;

            // Attente entre chaque vague
            yield return new WaitForSeconds(timeBetweenWave);
        }
    }

}