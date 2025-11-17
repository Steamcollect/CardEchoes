using UnityEngine;

public class TooltipBehaviour : MonoBehaviour
{
    //[Header("Settings")]
    //[Header("References")]
    //[Header("Input")]
    //[Header("Output")]

    public static TooltipBehaviour Instance;

    [SerializeField] private TooltipManager tooltip;

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
        Vector3 offsetPosition = triggerPosition + new Vector3(0, heightRect, 0);
        tooltip.transform.position = offsetPosition;
    }

    public void HideToolTip()
    {
        tooltip.gameObject.SetActive(false);
    }
}