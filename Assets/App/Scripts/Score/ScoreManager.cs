using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    //[Header("Settings")]
    [Header("References")]
    [SerializeField] SSO_ScoreTargetData scoreTargetData;
    
    //[Header("Input")]
    //[Header("Output")]

    public static ScoreManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public bool CheckVictoryCondition(Dictionary<Vector2Int, Card> cards)
    {
        int totalCards = cards.Count;

        // Vérifie qu'on a le minimum de cartes posées avant de tester les pourcentages
        if (totalCards < scoreTargetData.minimumCardCount)
            return false;

        int treeCount = 0;
        int swampCount = 0;
        int plantCount = 0;
        int waterCount = 0;
        int mineralCount = 0;

        // Comptage des cartes par type
        foreach (var kvp in cards)
        {
            SSO_CardData data = kvp.Value.GetData();

            if (data is SSO_CardData_Trees) treeCount++;
            else if (data is SSO_CardData_Swamp) swampCount++;
            else if (data is SSO_CardData_Plants) plantCount++;
            else if (data is SSO_CardData_Water) waterCount++;
            else if (data is SSO_CardData_Minerals) mineralCount++;
        }

        // Calcul des pourcentages
        float treePerc = (float)treeCount / totalCards * 100f;
        float swampPerc = (float)swampCount / totalCards * 100f;
        float plantPerc = (float)plantCount / totalCards * 100f;
        float waterPerc = (float)waterCount / totalCards * 100f;
        float mineralPerc = (float)mineralCount / totalCards * 100f;

        // Vérification des min/max
        if (treePerc < scoreTargetData.Tree_minMaxTargetPercenage.x || treePerc > scoreTargetData.Tree_minMaxTargetPercenage.y) return false;
        if (swampPerc < scoreTargetData.Swamp_minMaxTargetPercenage.x || swampPerc > scoreTargetData.Swamp_minMaxTargetPercenage.y) return false;
        if (plantPerc < scoreTargetData.Plant_minMaxTargetPercenage.x || plantPerc > scoreTargetData.Plant_minMaxTargetPercenage.y) return false;
        if (waterPerc < scoreTargetData.Water_minMaxTargetPercenage.x || waterPerc > scoreTargetData.Water_minMaxTargetPercenage.y) return false;
        if (mineralPerc < scoreTargetData.Mineral_minMaxTargetPercenage.x || mineralPerc > scoreTargetData.Mineral_minMaxTargetPercenage.y) return false;

        // Toutes les conditions sont remplies -> VICTOIRE !
        return true;
    }

}