using UnityEngine;

public class CardControllerUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] SSO_CardData cardData;

    [Header("References")]
    [SerializeField] CardTriggerUI trigger;

    //[Header("Input")]
    //[Header("Output")]

    void Start()
    {
        trigger._OnPointerEnter += ShowTooltip;
        trigger._OnPointerExit += HidetoolTip;
        trigger._OnPointerClick += OnClick;
    }

    void ShowTooltip()
    {
        TooltipBehaviour.instance?.ShowTooltip();
    }
    void HidetoolTip()
    {
        TooltipBehaviour.instance?.HideToolTip();
    }

    void OnClick()
    {
        CardPlacementManager.Instance?.HandleNewCard(cardData);
    }

    #region Getter
    public CardTriggerUI GetTrigger() { return trigger; }
    #endregion
}