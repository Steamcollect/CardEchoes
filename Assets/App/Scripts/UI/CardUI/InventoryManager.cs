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
    [SerializeField] SSO_CardData[] cardsDataAvailable;

    [Header("References")]
    [SerializeField] CardControllerUI cardUIPrefabs;
    [SerializeField] Transform cardsContent;

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
        CardControllerUI newCardUI = Instantiate(cardUIPrefabs, cardsContent);
        newCardUI.Setup(cardsDataAvailable.GetRandom());

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
}