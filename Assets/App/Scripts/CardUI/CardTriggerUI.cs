using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class CardTriggerUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    //[Header("Settings")]
    //[Header("References")]
    //[Header("Input")]
    [Header("Output")]
    public Action _OnPointerEnter, _OnPointerExit, _OnPointerClick;

    public void OnPointerClick(PointerEventData eventData)
    {
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