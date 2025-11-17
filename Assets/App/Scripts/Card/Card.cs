using UnityEngine;

public class Card : MonoBehaviour
{
    //[Header("Settings")]
    [Header("References")]
    [SerializeField] SpriteRenderer graphics;
    Card[] neighbours;

    SSO_CardData cardData;
    
    //[Header("Input")]
    //[Header("Output")]

    public void Setup(SSO_CardData data)
    {
        cardData = data;
        graphics.sprite = cardData.cardVisual;
    }

    public void ChangeData(SSO_CardData data)
    {
        cardData = data;
        graphics.sprite = cardData.cardVisual;
    }

    public void SetNeighbours(Card[] neighbours)
    {
        this.neighbours = neighbours;
    }

    public SSO_CardData GetData()
    {
        return cardData;
    }

    public SpriteRenderer GetGraphics()
    {
        return graphics;
    }

    public Card[] GetNeighbours()
    {
        return neighbours;
    }
}