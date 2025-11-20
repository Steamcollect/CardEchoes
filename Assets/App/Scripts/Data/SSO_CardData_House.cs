using UnityEngine;

[CreateAssetMenu(fileName = "SSO_CardData_House", menuName = "SSO/Cards/SSO_CardData_House")]
public class SSO_CardData_House : SSO_CardData
{
    public override void ApplyEffectToNeighbour(Card card, Transform content)
    {
        Card[] neighbours = card.GetNeighbours();

        int currentPriority = 100;
        SSO_CardData currentData = card.GetData();

        for (int i = 0; i < neighbours.Length; i++)
        {
            if (neighbours[i].GetData() is SSO_CardData_Fire && currentPriority > 1)
            {
                currentPriority = 1;
                currentData = cardsAvailable.Fire;
                break;
            }
        }

        if (currentData != cardsAvailable.House)
            card = ReplaceCards(card, currentData, content);

        card.WaveShake();
    }
}