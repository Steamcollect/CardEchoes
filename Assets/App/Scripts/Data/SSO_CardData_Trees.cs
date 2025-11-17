using UnityEngine;

[CreateAssetMenu(fileName = "SSO_CardData_Trees", menuName = "SSO/Cards/SSO_CardData_Trees")]
public class SSO_CardData_Trees : SSO_CardData
{
    public override void ApplyEffectToNeighbour(Card neighbour)
    {
        SSO_CardData data = neighbour.GetData();

        if (data is SSO_CardData_Trees) { return; }
        else if (data is SSO_CardData_Water)
        {
            neighbour.ChangeData(cardsAvailable.Swamp);
        } else if (data is SSO_CardData_Minerals)
        {
            neighbour.ChangeData(cardsAvailable.Swamp);
        } else if (data is SSO_CardData_Plants)
        {
            neighbour.ChangeData(cardsAvailable.Mineral);
        } else if (data is SSO_CardData_Swamp)
        {
            neighbour.ChangeData(cardsAvailable.Mineral);
        }
    }
}