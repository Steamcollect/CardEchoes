using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class CardMovementUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //[Header("Settings")]
    //[Header("References")]
    //[Header("Input")]
    [Header("Output")]
    public Action _OnPointerEnter, _OnPointerExit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipBehaviour.instance.ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipBehaviour.instance.HideToolTip();
    }
}