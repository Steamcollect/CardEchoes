using UnityEngine;

[CreateAssetMenu(fileName = "SSO_CardData_Trees", menuName = "SSO/Cards/SSO_CardData_Trees")]
public class SSO_CardData_Trees : SSO_CardData
{
    public override void ApplyEffectToNeighbour(Card card, Transform content)
    {
        Card[] neighbours = card.GetNeighbours();

        int currentPriority = 100;
        SSO_CardData currentData = card.GetData();

        for (int i = 0; i < neighbours.Length; i++)
        {
            if (neighbours[i] == null)
                continue;

            if (neighbours[i].GetData() is SSO_CardData_Fire && currentPriority > 1)
            {
                currentPriority = 1;
                currentData = cardsAvailable.Fire;
            }
                   
        }

        if (currentData != cardsAvailable.Tree)
            card = ReplaceCards(card, currentData, content);

        card.WaveShake();
    }
}