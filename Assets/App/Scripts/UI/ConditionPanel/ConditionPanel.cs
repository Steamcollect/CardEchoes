using System.Collections.Generic;
using UnityEngine;

public class ConditionPanel : MonoBehaviour
{
    [SerializeField] ConditionSlider waterSlider;
    [SerializeField] ConditionSlider swampSlider;
    [SerializeField] ConditionSlider mudSlider;
    [SerializeField] ConditionSlider plantSlider;
    [SerializeField] ConditionSlider forestSlider;
    [SerializeField] ConditionString cardCountStr;

    SSO_ScoreTargetData targetData;

    public static ConditionPanel Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SetTarget(SSO_ScoreTargetData targetData)
    {
        this.targetData = targetData;

        waterSlider.SetTargetValue(targetData.Water_minMaxTargetPercenage * .01f);
        swampSlider.SetTargetValue(targetData.Swamp_minMaxTargetPercenage * .01f);
        mudSlider.SetTargetValue(targetData.Mineral_minMaxTargetPercenage * .01f);
        plantSlider.SetTargetValue(targetData.Plant_minMaxTargetPercenage * .01f);
        forestSlider.SetTargetValue(targetData.Tree_minMaxTargetPercenage * .01f);
    }

    public void UpdateSliders(Dictionary<Vector2Int, Card> cards)
    {
        int total = cards.Count;
        if (total == 0) return;

        int waterCount = 0;
        int swampCount = 0;
        int mudCount = 0;
        int plantCount = 0;
        int forestCount = 0;

        foreach (var kvp in cards)
        {
            SSO_CardData data = kvp.Value.GetData();

            if (data is SSO_CardData_Water) waterCount++;
            else if (data is SSO_CardData_Swamp) swampCount++;
            else if (data is SSO_CardData_Minerals) mudCount++;
            else if (data is SSO_CardData_Plants) plantCount++;
            else if (data is SSO_CardData_Trees) forestCount++;
        }

        // Calcul des pourcentages (0 → 1)
        float waterPct = (float)waterCount / total;
        float swampPct = (float)swampCount / total;
        float mudPct = (float)mudCount / total;
        float plantPct = (float)plantCount / total;
        float forestPct = (float)forestCount / total;

        // Mise à jour des sliders
        waterSlider.SetPointerPosition(waterPct);
        swampSlider.SetPointerPosition(swampPct);
        mudSlider.SetPointerPosition(mudPct);
        plantSlider.SetPointerPosition(plantPct);
        forestSlider.SetPointerPosition(forestPct);

        cardCountStr.SetText(total + "/" + targetData.minimumCardCount);
    }
}
