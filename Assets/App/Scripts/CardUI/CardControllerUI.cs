using UnityEngine;

public class CardControllerUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] SSO_CardData cardData;

    [Header("References")]
    [SerializeField] CardMovementUI cardMovementUI;

    //[Header("Input")]
    //[Header("Output")]

    void Setup()
    {
        cardMovementUI._OnPointerEnter += ShowTooltip;
        cardMovementUI._OnPointerExit += HidetoolTip;
    }

    void ShowTooltip()
    {
        TooltipBehaviour.instance.ShowTooltip();
    }
    void HidetoolTip()
    {
        TooltipBehaviour.instance.HideToolTip();
    }

    #region Getter
    public CardMovementUI GetMovement() { return cardMovementUI; }
    #endregion
}