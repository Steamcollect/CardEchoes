using UnityEngine;

[CreateAssetMenu(fileName = "SSO_CardData_Swamp", menuName = "SSO/Cards/SSO_CardData_Swamp")]
public class SSO_CardData_Swamp : SSO_CardData
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

            if (neighbours[i].GetData() is SSO_CardData_Water && currentPriority > 1)
            {
                currentPriority = 1;
                currentData = cardsAvailable.Water;
            }
            else if (neighbours[i].GetData() is SSO_CardData_Trees && currentPriority > 2)
            {
                currentPriority = 2;
                currentData = cardsAvailable.Plant;
            }
        }

        if (currentData != cardsAvailable.Swamp)
        {
            card = ReplaceCards(card, currentData, content);
        }

        card.WaveShake();
        }
}