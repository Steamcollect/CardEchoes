using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] SSO_CardData[] startingCards;
    
    [Header("References")]
    [SerializeField] CardControllerUI cardUIPrefabs;
    [SerializeField] Transform cardsContent;

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        foreach (SSO_CardData card in startingCards)
        {
            AddNewCard(card);
        }
    }

    public void AddNewCard(SSO_CardData cardData)
    {
        CardControllerUI newCardUI = Instantiate(cardUIPrefabs, cardsContent);
        newCardUI.Setup(cardData);
    }
}