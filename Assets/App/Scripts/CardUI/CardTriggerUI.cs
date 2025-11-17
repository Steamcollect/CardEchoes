using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class CardTriggerUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    //[Header("Settings")]
    //[Header("References")]
    //[Header("Input")]
    [Header("Output")]
    public Action _OnPointerEnter, _OnPointerExit, _OnPointerClick;

    public void OnPointerDown(PointerEventData eventData)
    {
        CardPlacementManager.Instance?.SetCanPlaceCard(false);        
        _OnPointerClick?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _OnPointerEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _OnPointerExit?.Invoke();
    }
}