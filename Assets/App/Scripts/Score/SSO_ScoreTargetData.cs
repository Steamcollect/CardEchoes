using UnityEngine;

[CreateAssetMenu(fileName = "SSO_ScoreTargetData", menuName = "SSO/Gameplay/SSO_ScoreTargetData")]
public class SSO_ScoreTargetData : ScriptableObject
{
    public Vector2 Tree_minMaxTargetPercenage;
    public Vector2 Swamp_minMaxTargetPercenage;
    public Vector2 Plant_minMaxTargetPercenage;
    public Vector2 Water_minMaxTargetPercenage;
    public Vector2 Mineral_minMaxTargetPercenage;

    public int minimumCardCount;
}