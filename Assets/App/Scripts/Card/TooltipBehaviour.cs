using UnityEngine;

public class TooltipBehaviour : MonoBehaviour
{
    //[Header("Settings")]
    //[Header("References")]
    //[Header("Input")]
    //[Header("Output")]

    public static TooltipBehaviour instance;

    [SerializeField] private GameObject tooltip;

    private void Awake()
    {
        instance = this;
    }

    public void ShowTooltip()
    {
        tooltip.SetActive(true);
    }

    public void HideToolTip()
    {
        tooltip.SetActive(false);
    }
}