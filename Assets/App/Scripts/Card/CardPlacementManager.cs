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

    [HideInInspector] public bool canPlaceCard = true;

    [Space(10)]
    [SerializeField] Vector2Int minMaxXCadPos;
    [SerializeField] Vector2Int minMaxYCadPos;
    Vector2Int currentCardHandleGridPos;

    [Space(10)]
    [SerializeField, Inline] StartCard[] defaultCardsData;
    [System.Serializable]
    public struct StartCard
    {
        public Vector2Int position;
        public SSO_CardData cardData;
    }

    [Space(10)]
    [SerializeField] LayerMask defaultLayerMask;
    [SerializeField] LayerMask uxLayerMask;

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

    [Tab("Audio")]
    [SerializeField] Sound tileSwapSound;
    [SerializeField] Sound tileAnim1Sound;
    [SerializeField] Sound tilePlaceTreeSound;
    [SerializeField] Sound tilePlaceWaterSound;
    [SerializeField] Sound tilePlaceSwampSound;
    [SerializeField] Sound tilePlacePlantSound;
    [SerializeField] Sound tilePlaceMineralsSound;
    [SerializeField] Sound tilePlaceFireSound;

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
            Card card = Instantiate(startCard.cardData.cardPrefabs.GetRandom(), transform);
            card.Setup(startCard.cardData);
            card.transform.rotation = Quaternion.Euler(anim2CardRot);
            card.transform.position = new Vector3(startCard.position.x * gridSize, 0, startCard.position.y * gridSize);
            card.SetActiveRecto(true);
            card.SetActiveVerso(false);
            cards.Add(startCard.position, card);
        }

        foreach (var kvp in cards)
        {
            Vector2Int pos = kvp.Key;
            Card card = kvp.Value;

            Card[] neighbours = new Card[4];

            // directions indexées
            Vector2Int[] dirs = {
        new Vector2Int(-gridSize, 0), // 0 = gauche
        new Vector2Int(0, gridSize),  // 1 = haut
        new Vector2Int(gridSize, 0),  // 2 = droite
        new Vector2Int(0, -gridSize)  // 3 = bas
    };

            // assignation des voisins s'ils existent
            for (int i = 0; i < 4; i++)
            {
                Vector2Int nPos = pos + dirs[i];

                if (cards.TryGetValue(nPos, out Card neighbour))
                {
                    neighbours[i] = neighbour;
                }
            }

            card.SetNeighbours(neighbours);
        }

        // Mise à jour des voisins inverses (2e passe nécessaire)
        foreach (var kvp in cards)
        {
            Vector2Int pos = kvp.Key;
            Card card = kvp.Value;

            Card[] neighbours = card.GetNeighbours();

            for (int i = 0; i < 4; i++)
            {
                Card neighbour = neighbours[i];
                if (neighbour == null) continue;

                Card[] neighNeighbours = neighbour.GetNeighbours();
                if (neighNeighbours == null)
                    neighNeighbours = new Card[4];

                int opposite = (i + 2) % 4; // 0↔2 gauche<->droite, 1↔3 haut<->bas

                neighNeighbours[opposite] = card;
                neighbour.SetNeighbours(neighNeighbours);
            }
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

            currentCardHandleGridPos = GetCorrectPosition(gridPos);

            Vector3 worldPos = new Vector3(currentCardHandleGridPos.x * gridSize, 0, currentCardHandleGridPos.y * gridSize);

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
            new Vector3(currentCardHandleGridPos.x * gridSize, .12f, currentCardHandleGridPos.y * gridSize),
            ref cursorVelocity,
            cursorMoveTime
        );
    }

    public Vector2Int GetCorrectPosition(Vector2Int targetPosition)
    {
        // 1. Clamp dans les limites
        Vector2Int clamped = new Vector2Int(
            Mathf.Clamp(targetPosition.x, minMaxXCadPos.x, minMaxXCadPos.y),
            Mathf.Clamp(targetPosition.y, minMaxYCadPos.x, minMaxYCadPos.y)
        );

        // 2. Construire l'ensemble des positions autorisées :
        //    - toutes les positions qui contiennent une carte
        //    - toutes les cases adjacentes (haut / bas / gauche / droite) à ces cartes
        HashSet<Vector2Int> allowedPositions = new HashSet<Vector2Int>();

        Vector2Int[] dirs =
        {
        new Vector2Int(-gridSize, 0),  // gauche
        new Vector2Int(gridSize, 0),   // droite
        new Vector2Int(0, gridSize),   // haut
        new Vector2Int(0, -gridSize)   // bas
    };

        foreach (var kvp in cards)
        {
            Vector2Int pos = kvp.Key;

            // La case de la carte elle-même est autorisée
            if (IsInsideBounds(pos))
                allowedPositions.Add(pos);

            // Et ses 4 voisins non diagonaux aussi
            foreach (var d in dirs)
            {
                Vector2Int nPos = pos + d;
                if (IsInsideBounds(nPos))
                    allowedPositions.Add(nPos);
            }
        }

        // Si bizarrement aucune position n'est autorisée (aucune carte), fallback : clamp simple
        if (allowedPositions.Count == 0)
            return clamped;

        // 3. Si la position clamped est dans la zone autorisée → on la garde
        if (allowedPositions.Contains(clamped))
            return clamped;

        // 4. Sinon, on cherche la position autorisée la plus proche (distance de Manhattan)
        Vector2Int bestPos = clamped;
        int bestDist = int.MaxValue;

        foreach (var pos in allowedPositions)
        {
            int dist = Mathf.Abs(pos.x - clamped.x) + Mathf.Abs(pos.y - clamped.y);
            if (dist < bestDist)
            {
                bestDist = dist;
                bestPos = pos;
            }
        }

        return bestPos;
    }

    private bool IsInsideBounds(Vector2Int pos)
    {
        return pos.x >= minMaxXCadPos.x && pos.x <= minMaxXCadPos.y
            && pos.y >= minMaxYCadPos.x && pos.y <= minMaxYCadPos.y;
    }

    public void HandleNewCard(SSO_CardData card, CardControllerUI ui)
    {
        if (currentCardHandle) Destroy(currentCardHandle.gameObject);

        cursorGO.SetActive(true);

        currentCardHandleUI = ui;
        currentCardHandle = Instantiate(card.cardPrefabs.GetRandom(), transform);
        currentCardHandle.Setup(card);
        currentCardHandle.SetActiveVerso(true);
        currentCardHandle.SetActiveRecto(false);

        currentCardHandle.SetLayer(uxLayerMask);

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

            currentCardHandleGridPos = GetCorrectPosition(gridPos);

            Vector3 worldPos = new Vector3(currentCardHandleGridPos.x * gridSize, 0, currentCardHandleGridPos.y * gridSize);
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

        canPlaceCard = false;

        cursorGO.SetActive(false);

        GameObject objToDestroy = null;
        if (cards.ContainsKey(currentCardHandleGridPos))
        {
            objToDestroy = cards[currentCardHandleGridPos].gameObject;
            cards[currentCardHandleGridPos] = currentCardHandle;
        }
        else
            cards.Add(currentCardHandleGridPos, currentCardHandle);

        Card[] neighbours = new Card[4];

        Vector2Int[] dirs = {
            new Vector2Int(-gridSize, 0),
            new Vector2Int(0, gridSize),
            new Vector2Int(gridSize, 0),
            new Vector2Int(0, -gridSize)
        };

        for (int i = 0; i < 4; i++)
        {
            Vector2Int nPos = currentCardHandleGridPos + dirs[i];
            if (cards.ContainsKey(nPos))
                neighbours[i] = cards[nPos];
        }

        currentCardHandle.SetNeighbours(neighbours);

        // mise à jour des voisins
        for (int i = 0; i < 4; i++)
        {
            Card neighbour = neighbours[i];
            if (neighbour == null) continue;

            Card[] nNeigh = neighbour.GetNeighbours();
            int opposite = (i + 2) % 4;
            nNeigh[opposite] = currentCardHandle;
            neighbour.SetNeighbours(nNeigh);
        }

        currentCardHandle.SetNeighbours(neighbours);
        StartCoroutine(CardHandlePlacementAnim(objToDestroy));
    }
    IEnumerator CardHandlePlacementAnim(GameObject objToDestroy)
    {
        Transform cardHandle = currentCardHandle.transform;
        Card cCard = currentCardHandle;
        SSO_CardData data = cCard.GetData();
        currentCardHandle = null;

        cCard.SetActiveRecto(true);
        AudioManager.Instance.PlayClipAt(tileAnim1Sound, Vector3.zero);
        cardHandle.DOMove(new Vector3(currentCardHandleGridPos.x * gridSize, 0, currentCardHandleGridPos.y * gridSize) + tilePosOffset, anim1Time);
        cardHandle.DORotate(anim1ShowCardRot, anim1Time).OnComplete(() =>
        {
            cardHandle.DOMove(cardHandle.position + anim2CardPosOffset, anim2Time);
            cardHandle.DORotate(anim2CardRot, anim2Time).OnComplete(() =>
            {
                cCard.SetLayer(defaultLayerMask);
                cCard.SetActiveVerso(false);
                AudioManager.Instance.PlayClipAt(tileAnim1Sound, Vector3.zero);
                cardHandle.DOMove(new Vector3(currentCardHandleGridPos.x, 0, currentCardHandleGridPos.y), anim3Time).OnComplete(() =>
                {
                    if (data is SSO_CardData_Minerals)
                    {
                        AudioManager.Instance.PlayClipAt(tilePlaceMineralsSound, Vector3.zero);
                    } else if (data is SSO_CardData_Plants)
                    {
                        AudioManager.Instance.PlayClipAt(tilePlacePlantSound, Vector3.zero);
                    } else if (data is SSO_CardData_Swamp)
                    {
                        AudioManager.Instance.PlayClipAt(tilePlaceSwampSound, Vector3.zero);
                    } else if (data is SSO_CardData_Trees)
                    {
                        AudioManager.Instance.PlayClipAt(tilePlaceTreeSound, Vector3.zero);
                    } else if (data is SSO_CardData_Water)
                    {
                        AudioManager.Instance.PlayClipAt(tilePlaceWaterSound, Vector3.zero);
                    } else if (data is SSO_CardData_Fire)
                    {
                        AudioManager.Instance.PlayClipAt(tilePlaceFireSound, Vector3.zero);
                    }
                });
            });
        });

        yield return new WaitForSeconds(anim1Time + anim2Time + anim3Time);

        CameraController.Instance.Shake();

        InventoryManager.Instance.RemoveCard(currentCardHandleUI);
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
            int cardFoundCount = 0;
            for (int dx = -iterations; dx <= iterations; dx++)
            {
                int dy = iterations - Mathf.Abs(dx);

                Vector2Int pos1 = new Vector2Int(startingPos.x + dx, startingPos.y + dy);

                
                if (cards.ContainsKey(pos1))
                {
                    cardFoundCount++;
                    card = cards[pos1];
                    card.GetData().ApplyEffectToNeighbour(card, transform);
                }

                if (dy != 0)
                {
                    Vector2Int pos2 = new Vector2Int(startingPos.x + dx, startingPos.y + -dy);

                    if (cards.ContainsKey(pos2))
                    {
                        cardFoundCount++;
                        card = cards[pos2];
                        card.GetData().ApplyEffectToNeighbour(card, transform);
                    }
                }
            }

            if (cardFoundCount <= 0) break;

            AudioManager.Instance.PlayClipAt(tileSwapSound, Vector3.zero);
            iterations++;
            yield return new WaitForSeconds(timeBetweenWave);
        }
        while(iterations <= maxIteration);

        if (ScoreManager.Instance.CheckVictoryCondition(cards))
        {
            Debug.Log("You Win");
            yield break;
        }

        InventoryManager.Instance.AddNewCard();

        canPlaceCard = true;
    }

    public void ReplaceCardInDictionary(Vector2Int pos, Card newCard)
    {
        cards[pos] = newCard;
    }

    public void OnCardReplaced(Card oldCard, Card newCard)
    {
        // On cherche la clé (position) associée à l'ancienne carte
        Vector2Int keyToUpdate = default;
        bool found = false;

        foreach (var kvp in cards)
        {
            if (kvp.Value == oldCard)
            {
                keyToUpdate = kvp.Key;
                found = true;
                break;
            }
        }

        // Si on l'a trouvée, on met à jour la valeur dans le dictionnaire
        if (found)
        {
            cards[keyToUpdate] = newCard;
        }
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