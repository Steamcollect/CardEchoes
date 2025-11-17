using UnityEngine;

[CreateAssetMenu(fileName = "SSO_CardData_Water", menuName = "SSO/Cards/SSO_CardData_Water")]
public class SSO_CardData_Water : SSO_CardData
{
    public override void ApplyEffectToNeighbour(Card neighbour)
    {
        SSO_CardData data = neighbour.GetData();

        if(data is SSO_CardData_Water) { return; }
        else if (data is SSO_CardData_Minerals)
        {
            neighbour.ChangeData(cardsAvailable.Plant);
        }
    }
}