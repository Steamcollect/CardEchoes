using UnityEngine;

[CreateAssetMenu(fileName = "SSO_CardData_Plants", menuName = "SSO/Cards/SSO_CardDataPlants")]
public class SSO_CardData_Plants : SSO_CardData
{
    public override void ApplyEffectToNeighbour(Card card)
    {
        Card[] neighbours = card.GetNeighbours();

        int currentPriority = 100;
        SSO_CardData currentData = card.GetData();

        for (int i = 0; i < neighbours.Length; i++)
        {
            if (neighbours[i].GetData() is SSO_CardData_Water && currentPriority > 1)
            {
                currentPriority = 1;
                currentData = cardsAvailable.Tree;
            }
            else if (neighbours[i].GetData() is SSO_CardData_Trees && currentPriority > 2)
            {
                currentPriority = 2;
                currentData = cardsAvailable.Mineral;
            }
            else if (neighbours[i].GetData() is SSO_CardData_Minerals && currentPriority > 3)
            {
                currentPriority = 3;
                currentData = cardsAvailable.Water;
            }
            else if (neighbours[i].GetData() is SSO_CardData_Swamp && currentPriority > 4)
            {
                currentPriority = 4;
                currentData = cardsAvailable.Tree;
            }
            else if (neighbours[i].GetData() is SSO_CardData_Plants && currentPriority > 5)
            {
                currentPriority = 5;
                currentData = cardsAvailable.Plant;
            }
        }

        card.ChangeData(currentData);
    }
}