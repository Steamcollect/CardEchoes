using System.Collections.Generic;
using ToolBox.Utils;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float maxCardAngle = 7;
    [SerializeField] float maxCardYOffset = 20;

    [SerializeField] AnimationCurve angleModifierCurve;
    [SerializeField] AnimationCurve posModifierCurve;

    [Space(10)]
    [SerializeField] int startingCardCount;
    [SerializeField] List<SSO_CardData> cardsDataAvailable = new();

    [Header("References")]
    [SerializeField] CardControllerUI cardUIPrefabs;
    [SerializeField] Transform cardsContent;

    List<CardControllerUI> currentCards = new List<CardControllerUI>();

    //[Header("Input")]
    //[Header("Output")]

    public static InventoryManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < startingCardCount; i++)
        {
            AddNewCard();
        }
    }

    public void AddNewCard()
    {
        // 1. Récupérer toutes les cartes déjà placées sur le board
        HashSet<SSO_CardData> placed = new HashSet<SSO_CardData>(GetAllPlacedCardData());

        // 2. Filtrer les cartes disponibles : on enlève celles déjà placées
        List<SSO_CardData> pool = new List<SSO_CardData>();
        foreach (var c in cardsDataAvailable)
        {
            if (!placed.Contains(c))
                pool.Add(c);
        }

        // 3. Si aucune carte neuve n’est dispo -> fallback : prendre dans toutes les cartes disponibles
        if (pool.Count == 0)
            pool.AddRange(cardsDataAvailable);

        // 4. Choisir une carte parmi le pool valide
        SSO_CardData chosen = pool.GetRandom();

        // 5. Créer la carte UI et la setup avec cette carte
        CardControllerUI newCardUI = Instantiate(cardUIPrefabs, cardsContent);
        newCardUI.Setup(chosen);
        currentCards.Add(newCardUI);

        // Update visuals
        int total = cardsContent.childCount;
        int midIndex = (total - 1) / 2;
        if (total == 0) return;

        for (int i = 0; i < total; i++)
        {
            Transform child = cardsContent.GetChild(i);

            if(child.TryGetComponent(out CardControllerUI card))
            {
                float t = (total == 1) ? 0f : (float)i / (total - 1);

                float angle = (i > midIndex ? -1 : 1) * angleModifierCurve.Evaluate(t) * maxCardAngle;
                float yOffset = posModifierCurve.Evaluate(t) * maxCardYOffset;

                card.UpdateVisual(angle, yOffset);
            }
        }
    }

    IEnumerable<SSO_CardData> GetAllPlacedCardData()
    {
        foreach (CardControllerUI kvp in currentCards)
            yield return kvp.GetData();
    }

    public void RemoveCard(CardControllerUI card)
    {
        currentCards.Remove(card);
        Destroy(card.gameObject);
    }

    public void AddCard(SSO_CardData card)
    {
        cardsDataAvailable.Add(card);
    }
}