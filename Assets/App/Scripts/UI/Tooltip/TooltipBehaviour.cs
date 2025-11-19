using UnityEngine;

public class TooltipBehaviour : MonoBehaviour
{
    //[Header("Settings")]

    [SerializeField] Vector3 offsetTooltip;
    //[Header("References")]

    public static TooltipBehaviour Instance;
    [SerializeField] private TooltipManager tooltip;

    //[Header("Input")]
    //[Header("Output")]

    private void Awake()
    {
        Instance = this;
    }

    public void ShowTooltip(SSO_CardData cardData, Vector3 triggerPosition)
    {
        tooltip.SetText(cardData.cardDescription, cardData.cardName);
        tooltip.gameObject.SetActive(true);
        RectTransform tooltipRect = tooltip.gameObject.GetComponent<RectTransform>();
        float heightRect = tooltipRect.rect.height / 2;
        Vector2 offsetPosition = (triggerPosition + new Vector3(0, heightRect, 0)) + offsetTooltip;
        tooltip.transform.position = offsetPosition;
    }

    public void HideToolTip()
    {
        tooltip.gameObject.SetActive(false);
    }
}