using UnityEngine;

public class Card : MonoBehaviour
{
    //[Header("Settings")]
    [Header("References")]
    [SerializeField] SpriteRenderer graphics;
    
    SSO_CardData cardData;
    
    //[Header("Input")]
    //[Header("Output")]

    public void Setup(SSO_CardData data)
    {
        cardData = data;
        graphics.sprite = cardData.cardVisual;
    }

    public SSO_CardData GetData()
    {
        return cardData;
    }

    public SpriteRenderer GetGraphics()
    {
        return graphics;
    }
}