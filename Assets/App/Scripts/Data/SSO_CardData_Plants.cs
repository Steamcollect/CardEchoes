using UnityEngine;

[CreateAssetMenu(fileName = "SSO_CardData_Plants", menuName = "SSO/Cards/SSO_CardDataPlants")]
public class SSO_CardData_Plants : SSO_CardData
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
                currentData = cardsAvailable.Tree;
            }
            else if(neighbours[i].GetData() is SSO_CardData_House && currentPriority > 2)
            {
                currentPriority = 2;
                currentData = cardsAvailable.House;
            }            
        }

        if (currentData != cardsAvailable.Plant)
            card = ReplaceCards(card, currentData, content);

        card.WaveShake();
    }
}