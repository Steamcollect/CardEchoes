using UnityEngine;

public class CardPlacementManager : MonoBehaviour
{
    //[Header("Settings")]
    //[Header("References")]
    Card currentCardHandle;

    //[Header("Input")]
    //[Header("Output")]

    public void HandleNewCard(SSO_CardData card)
    {
        if(currentCardHandle) Destroy(currentCardHandle.gameObject);

        currentCardHandle = Instantiate(card.cardPrefab, transform);
    }
}