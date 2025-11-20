using ToolBox.Utils;
using UnityEngine;

[CreateAssetMenu(fileName = "SSO_CardData_Water", menuName = "SSO/Cards/SSO_CardData_Water")]
public class SSO_CardData_Water : SSO_CardData
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

            if (neighbours[i].GetData() is SSO_CardData_Water && currentPriority > 2)
            {
                currentPriority = 2;
                currentData = cardsAvailable.Water;
            }
            else if (neighbours[i].GetData() is SSO_CardData_Minerals && currentPriority > 1)
            {
                currentPriority = 1;
                currentData = cardsAvailable.Swamp;
            }
        }

        if (currentData != cardsAvailable.Water)
        {
            card = ReplaceCards(card, currentData, content);
        }
            

        card.WaveShake();
    }
}