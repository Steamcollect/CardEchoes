using UnityEngine;

[CreateAssetMenu(fileName = "SSO_CardData_Swamp", menuName = "SSO/Cards/SSO_CardData_Swamp")]
public class SSO_CardData_Swamp : SSO_CardData
{
    public override void ApplyEffectToNeighbour(Card neighbour)
    {
        SSO_CardData data = neighbour.GetData();

        if (data is SSO_CardData_Swamp) { return; }
        else if (data is SSO_CardData_Water)
        {
            neighbour.ChangeData(cardsAvailable.Water);
        } else if (data is SSO_CardData_Minerals)
        {
            neighbour.ChangeData(cardsAvailable.Plant);
        } else if (data is SSO_CardData_Plants)
        {
            neighbour.ChangeData(cardsAvailable.Tree);
        } else if (data is SSO_CardData_Trees)
        {
            neighbour.ChangeData(cardsAvailable.Mineral);
        }
    }
}