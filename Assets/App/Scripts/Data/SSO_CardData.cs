using UnityEngine;

[CreateAssetMenu(fileName = "SSO_CardData", menuName = "SSO/Card/SSO_CardData")]
public class SSO_CardData : ScriptableObject
{
    public string cardName;
    [TextArea] public string cardDescription;
    public Sprite cardIcon;

    [Space(10)]
    public Card cardPrefab;
}