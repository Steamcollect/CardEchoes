using UnityEngine;
using UnityEngine.UI;

public class CardControllerUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] SSO_CardData cardData;

    [Header("References")]
    [SerializeField] Image image;
    [SerializeField] CardTriggerUI trigger;

    //[Header("Input")]
    //[Header("Output")]

    public void Setup(SSO_CardData data)
    {
        cardData = data;
        image.sprite = data.cardVisualS;

        trigger._OnPointerEnter += ShowTooltip;
        trigger._OnPointerExit += HidetoolTip;
        trigger._OnPointerClick += OnClick;
    }

    void ShowTooltip()
    {
        TooltipBehaviour.Instance?.ShowTooltip(cardData, transform.position);
    }
    void HidetoolTip()
    {
        TooltipBehaviour.Instance?.HideToolTip();
    }

    void OnClick()
    {
        CardPlacementManager.Instance?.HandleNewCard(cardData, this);
    }

    #region Getter
    public CardTriggerUI GetTrigger() { return trigger; }
    #endregion
}