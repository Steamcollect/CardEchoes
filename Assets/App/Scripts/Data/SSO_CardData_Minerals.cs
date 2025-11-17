using UnityEngine;

[CreateAssetMenu(fileName = "SSO_CardData_Minerals", menuName = "SSO/Cards/SSO_CardData_Minerals")]
public class SSO_CardData_Minerals : SSO_CardData
{
    public override void ApplyEffectToNeighbour(Card neighbour)
    {
        SSO_CardData data = neighbour.GetData();

        if (data is SSO_CardData_Minerals) { return; }
        else if (data is SSO_CardData_Water)
        {
            neighbour.ChangeData(cardsAvailable.Plant);
        } else if (data is SSO_CardData_Plants)
        {
            neighbour.ChangeData(cardsAvailable.Water);
        } else if (data is SSO_CardData_Trees)
        {
            neighbour.ChangeData(cardsAvailable.Swamp);
        } else if (data is SSO_CardData_Swamp)
        {
            neighbour.ChangeData(cardsAvailable.Plant);
        }
    }
}