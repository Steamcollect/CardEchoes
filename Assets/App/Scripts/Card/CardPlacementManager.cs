using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ToolBox.Dev;
using ToolBox.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardPlacementManager : MonoBehaviour
{
    [Tab("Grid")]
    public int gridSize = 1;
    [SerializeField] float timeBetweenWave = .2f;

    [Space(10)]
    [SerializeField] Vector2Int minMaxXCadPos;
    [SerializeField] Vector2Int minMaxYCadPos;
    Vector2Int currentCardHandleGridPos;

    [Space(10)]
    [SerializeField] Card cardPrefab;
    [SerializeField, Inline] StartCard[] defaultCardsData;
    [System.Serializable]
    public struct StartCard
    {
        public Vector2Int position;
        public SSO_CardData cardData;
    }

    [Tab("Cursor")]
    [SerializeField] float cursorRotationSpeed;
    [SerializeField] float cursorMoveTime;
    Vector3 cursorVelocity;

    [Space(10)]
    [SerializeField] Vector3 tilePosOffset;
    [SerializeField] Vector3 tileRotationOffset;
    [SerializeField] float currentCardHandleMoveTime;
    Vector3 currentCardHandleVelocity;

    [Space(10)]
    [SerializeField] float cardHandleRotationAmount;
    [SerializeField] float cardHandleRotationVelocityMult;
    [SerializeField] float cardHandleRotationTime;

    float rotationVelocity;
    float rotationDelta;

    [Space(10)]
    [SerializeField] GameObject cursorGO;

    [Tab("Card Anim")]
    [SerializeField] float anim1Time;
    [SerializeField] Vector3 anim1ShowCardRot;

    [Space(10)]
    [SerializeField] float anim2Time;
    [SerializeField] Vector3 anim2CardPosOffset;
    [SerializeField] Vector3 anim2CardRot;

    [Space(10)]
    [SerializeField] float anim3Time;

    Camera cam;

    Dictionary<Vector2Int /*Position*/, Card> cards = new Dictionary<Vector2Int, Card>();

    CardControllerUI currentCardHandleUI;
    Card currentCardHandle;

    [Tab("References")]
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
        cursorGO.SetActive(false);

        foreach (StartCard startCard in defaultCardsData)
        {
            Card card = Instantiate(cardPrefab, transform);
            card.Setup(startCard.cardData);
            card.transform.rotation = Quaternion.Euler(anim2CardRot);
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
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

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

            currentCardHandle.transform.position = Vector3.SmoothDamp(
                currentCardHandle.transform.position,
                worldPos + tilePosOffset,
                ref currentCardHandleVelocity,
                currentCardHandleMoveTime);

            float targetRot = -((currentCardHandleVelocity * Time.deltaTime) / Mathf.Min(.001f, cardHandleRotationVelocityMult * cardHandleRotationAmount)).x;
            rotationDelta = Mathf.SmoothDamp(rotationDelta, targetRot, ref rotationVelocity, cardHandleRotationTime);

            float angle = Mathf.Clamp(rotationDelta, -60, 60);
            currentCardHandle.transform.localEulerAngles = 
                new Vector3(tileRotationOffset.x, tileRotationOffset.y + angle, tileRotationOffset.z);
        }

        // Cursor
        cursorGO.transform.eulerAngles += Vector3.up * cursorRotationSpeed * Time.deltaTime;
        cursorGO.transform.position = Vector3.SmoothDamp(
            cursorGO.transform.position,
            new Vector3(currentCardHandleGridPos.x * gridSize, .1f, currentCardHandleGridPos.y * gridSize),
            ref cursorVelocity,
            cursorMoveTime
        );
    }

    public void HandleNewCard(SSO_CardData card, CardControllerUI ui)
    {
        if (currentCardHandle) Destroy(currentCardHandle.gameObject);

        cursorGO.SetActive(true);

        currentCardHandleUI = ui;
        currentCardHandle = Instantiate(cardPrefab, transform);
        currentCardHandle.Setup(card);

        Vector2 mousePos = mousePosition.action.ReadValue<Vector2>();

        // Ray depuis la caméra
        Ray ray = cam.ScreenPointToRay(mousePos);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

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
            currentCardHandle.transform.position = worldPos + tilePosOffset;
        }

        cursorGO.transform.position =
            new Vector3(currentCardHandleGridPos.x * gridSize, .1f, currentCardHandleGridPos.y * gridSize);
    }

    void PlaceCardHandle(InputAction.CallbackContext context)
    {
        if (!currentCardHandle)
        {
            return;
        }

        cursorGO.SetActive(false);

        GameObject objToDestroy = null;
        if (cards.ContainsKey(currentCardHandleGridPos))
        {
            objToDestroy = cards[currentCardHandleGridPos].gameObject;
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

        StartCoroutine(CardHandlePlacementAnim(objToDestroy));
    }
    IEnumerator CardHandlePlacementAnim(GameObject objToDestroy)
    {
        Transform cardHandle = currentCardHandle.transform;
        currentCardHandle = null;

        cardHandle.DOMove(new Vector3(currentCardHandleGridPos.x * gridSize, 0, currentCardHandleGridPos.y * gridSize) + tilePosOffset, anim1Time);
        cardHandle.DORotate(anim1ShowCardRot, anim1Time).OnComplete(() =>
        {
            cardHandle.DOMove(cardHandle.position + anim2CardPosOffset, anim2Time);
            cardHandle.DORotate(anim2CardRot, anim2Time).OnComplete(() =>
            {
                cardHandle.DOMove(new Vector3(currentCardHandleGridPos.x, 0, currentCardHandleGridPos.y), anim3Time);
            });
        });

        yield return new WaitForSeconds(anim1Time + anim2Time + anim3Time);

        CameraController.Instance.Shake();

        Destroy(currentCardHandleUI.gameObject);
        if(objToDestroy) Destroy(objToDestroy);

        StartCoroutine(CheckCardsNeighbour(currentCardHandleGridPos));
    }

    void ClearCardHandle(InputAction.CallbackContext context)
    {
        if(currentCardHandle)
        {
            Destroy(currentCardHandle.gameObject);
            cursorGO.SetActive(false);
        }
    }

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