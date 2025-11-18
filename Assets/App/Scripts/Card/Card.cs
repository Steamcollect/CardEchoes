using DG.Tweening;
using UnityEngine;

public class Card : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float shakeAngle = 15;
    [SerializeField] float shakeDuration = 0.2f;
    [SerializeField] float yOffset;

    [Header("References")]
    [SerializeField] MeshRenderer iconGraphics;
    [SerializeField] MeshRenderer visualGraphics;
    Card[] neighbours;

    SSO_CardData cardData;
    
    //[Header("Input")]
    //[Header("Output")]

    public void Setup(SSO_CardData data)
    {
        cardData = data;

        iconGraphics.material = new Material(iconGraphics.material);
        iconGraphics.material.mainTexture = cardData.cardTextureIcon;

        visualGraphics.material = new Material(visualGraphics.material);
        visualGraphics.material.mainTexture = cardData.cardTextureVisual;
    }

    public void ChangeData(SSO_CardData data)
    {
        transform.DOMoveY(transform.position.y + yOffset, shakeDuration / 2).SetLoops(2, LoopType.Yoyo);
        transform.DOPunchRotation(Vector3.up * shakeAngle, shakeDuration, 20, 1);
        cardData = data;
        iconGraphics.material.mainTexture = cardData.cardTextureIcon;
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
        return iconGraphics;
    }

    public Card[] GetNeighbours()
    {
        return neighbours;
    }
}