using UnityEngine;

public class SSO_CardData : ScriptableObject
{
    public string cardName;
    [TextArea] public string cardDescription;
    public Texture cardVisualT;
    public Sprite cardVisualS;

    public SSO_CardsAvailable cardsAvailable;

    public virtual void ApplyEffectToNeighbour(Card card) { }
}