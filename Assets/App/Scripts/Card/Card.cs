using JetBrains.Annotations;
using UnityEngine;

public class Card : MonoBehaviour
{
    //[Header("Settings")]
    [Header("References")]
    [SerializeField] MeshRenderer graphics;
    Card[] neighbours;

    SSO_CardData cardData;
    
    //[Header("Input")]
    //[Header("Output")]

    public void Setup(SSO_CardData data)
    {
        cardData = data;

        graphics.material = new Material(graphics.material);
        graphics.material.mainTexture = cardData.cardVisualT;
    }

    public void ChangeData(SSO_CardData data)
    {
        cardData = data;
        graphics.material.mainTexture = cardData.cardVisualT;
    }

    public void SetNeighbours(Card[] neighbours)
    {
        this.neighbours = neighbours;
    }

    public SSO_CardData GetData()
    {
        return cardData;
    }

    public MeshRenderer GetGraphics()
    {
        return graphics;
    }

    public Card[] GetNeighbours()
    {
        return neighbours;
    }
}