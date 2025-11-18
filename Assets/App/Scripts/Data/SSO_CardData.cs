using UnityEngine;

public class SSO_CardData : ScriptableObject
{
    public string cardName;
    [TextArea] public string cardDescription;
    public Sprite cardUIIcon;
    public Texture cardTextureIcon;
    public Texture cardTextureVisual;

    public SSO_CardsAvailable cardsAvailable;

    public virtual void ApplyEffectToNeighbour(Card card) { }
}