using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardTriggerUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Header("Settings")]
    [SerializeField] float shakeAngle = 15;
    [SerializeField] float shakeDuration = 0.2f;
    [SerializeField] float hoverScale = 1.1f;

    //[Header("References")]
    //[Header("Input")]
    [Header("Output")]
    public Action _OnPointerEnter, _OnPointerExit, _OnPointerClick;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (CardPlacementManager.Instance?.canPlaceCard == false) return;
        _OnPointerClick?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOPunchRotation(Vector3.forward * shakeAngle, shakeDuration, 20, 1);
        transform.DOScale(hoverScale, shakeDuration).SetEase(Ease.OutBack);
        _OnPointerEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (CardPlacementManager.Instance?.canPlaceCard == false) return;

        transform.DOScale(1, shakeDuration).SetEase(Ease.OutBack);
        _OnPointerExit?.Invoke();
    }
}