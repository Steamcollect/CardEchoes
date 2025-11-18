using UnityEngine;

[CreateAssetMenu(fileName = "SSO_CardData_Swamp", menuName = "SSO/Cards/SSO_CardData_Swamp")]
public class SSO_CardData_Swamp : SSO_CardData
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
                currentData = neighbours[i].GetData();
            }
            else if (neighbours[i].GetData() is SSO_CardData_Plants && currentPriority > 2)
            {
                currentPriority = 2;
                currentData = neighbours[i].GetData();
            }
            else if (neighbours[i].GetData() is SSO_CardData_Minerals && currentPriority > 3)
            {
                currentPriority = 3;
                currentData = neighbours[i].GetData();
            }
            else if (neighbours[i].GetData() is SSO_CardData_Trees && currentPriority > 4)
            {
                currentPriority = 4;
                currentData = neighbours[i].GetData();
            }
            else if (neighbours[i].GetData() is SSO_CardData_Swamp && currentPriority > 5)
            {
                currentPriority = 5;
                currentData = neighbours[i].GetData();
            }
        }

        card.ChangeData(currentData);
    }
}