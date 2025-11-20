using UnityEngine;

[CreateAssetMenu(fileName = "SSO_CardData_Fire", menuName = "SSO/Cards/SSO_CardData_Fire")]
public class SSO_CardData_Fire : SSO_CardData
{
    public override void ApplyEffectToNeighbour(Card card, Transform content)
    {
        card = ReplaceCards(card, card.GetData().cardsAvailable.Mineral, content);
        card.WaveShake();
    }
}