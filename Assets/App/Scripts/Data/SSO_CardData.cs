using Unity.VisualScripting;
using UnityEngine;

public class SSO_CardData : ScriptableObject
{
    public string cardName;
    [TextArea] public string cardDescription;
    public Sprite cardVisual;

    public virtual void ApplyEffectToNeighbour(Card neighbour) { }
}