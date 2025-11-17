using UnityEngine;

[CreateAssetMenu(fileName = "SSO_CardData_Plants", menuName = "SSO/Cards/SSO_CardDataPlants")]
public class SSO_CardData_Plants : SSO_CardData
{
    public override void ApplyEffectToNeighbour(Card neighbour)
    {
        SSO_CardData data = neighbour.GetData();

        if (data is SSO_CardData_Plants) { return; }
        else if (data is SSO_CardData_Water)
        {
            neighbour.ChangeData(cardsAvailable.Tree);
        } else if (data is SSO_CardData_Minerals)
        {
            neighbour.ChangeData(cardsAvailable.Water);
        } else if (data is SSO_CardData_Trees)
        {
            neighbour.ChangeData(cardsAvailable.Mineral);
        } else if (data is SSO_CardData_Swamp)
        {
            neighbour.ChangeData(cardsAvailable.Tree);
        }
    }
}