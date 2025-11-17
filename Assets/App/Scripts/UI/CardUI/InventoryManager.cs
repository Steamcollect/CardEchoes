using ToolBox.Utils;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int startingCardCount;
    [SerializeField] SSO_CardData[] cardsDataAvailable;

    [Header("References")]
    [SerializeField] CardControllerUI cardUIPrefabs;
    [SerializeField] Transform cardsContent;

    //[Header("Input")]
    //[Header("Output")]

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
    }
}