using ToolBox.Utils;
using Unity.VisualScripting;
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

            if (neighbours[i].GetData() is SSO_CardData_Trees && currentPriority > 1)
            {
                currentPriority = 1;
                currentData = cardsAvailable.Swamp;
                Debug.Log("Water Priority 1");
                break;
            }
           /*else if (neighbours[i].GetData() is SSO_CardData_Swamp && currentPriority > 2)
           {
               currentPriority = 2;
               currentData = cardsAvailable.Water;
               Debug.Log("Water Priority 2");
               break;
           }
           else if (neighbours[i].GetData() is SSO_CardData_Water && currentPriority > 3)
           {
               currentPriority = 3;
               currentData = cardsAvailable.Water;
               Debug.Log("Water Priority 3");
           }
           else if (neighbours[i].GetData() is SSO_CardData_Plants && currentPriority > 4)
           {
               currentPriority = 4;
               currentData = cardsAvailable.Water;
               Debug.Log("Water Priority 4");
           }
           else if (neighbours[i].GetData() is SSO_CardData_Minerals && currentPriority > 5)
           {
               currentPriority = 5;
               currentData = cardsAvailable.Water;
               Debug.Log("Water Priority 5");
           }*/
            return;
        }

        if (currentData != cardsAvailable.Water)
        {
            card = ReplaceCards(card, currentData, content);
        }
            card.WaveShake();
    }
}