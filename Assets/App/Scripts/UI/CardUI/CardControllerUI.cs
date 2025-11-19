using UnityEngine;
using UnityEngine.UI;

public class CardControllerUI : MonoBehaviour
{
    //[Header("Settings")]
    [Header("References")]
    [SerializeField] Image image;
    [SerializeField] CardTriggerUI trigger;

    SSO_CardData cardData;

    //[Header("Input")]
    //[Header("Output")]

    public void Setup(SSO_CardData data)
    {
        cardData = data;
        image.sprite = data.cardUIIcon;

        trigger._OnPointerEnter += ShowTooltip;
        trigger._OnPointerExit += HidetoolTip;
        trigger._OnPointerClick += OnClick;
    }

    public void UpdateVisual(float angle, float yOffset)
    {
        image.transform.rotation = Quaternion.Euler(0, 0, angle);
        image.rectTransform.offsetMin = new Vector2(image.rectTransform.offsetMin.x, yOffset);
        image.rectTransform.offsetMax = new Vector2(image.rectTransform.offsetMin.x, yOffset);
    }

    void ShowTooltip()
    {
        TooltipBehaviour.Instance?.ShowTooltip(cardData, trigger.transform.position);
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