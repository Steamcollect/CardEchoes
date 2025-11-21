using DG.Tweening;
using UnityEngine;

public class ConditionSlider : MonoBehaviour
{
    [Header("Settings")]
    float cursorValue;
    [SerializeField] float pointerMoveTime;

    [Header("References")]
    [SerializeField] RectTransform parentRect;
    [SerializeField] RectTransform pointerRect;
    [SerializeField] RectTransform targetRect;
    
    //[Header("Input")]
    //[Header("Output")]

    public void SetTargetValue(Vector2 targetValue)
    {
        float width = parentRect.rect.width;
        float leftPx = targetValue.x * width;
        float rightPx = targetValue.y * width;

        targetRect.offsetMin = new Vector2(leftPx, targetRect.offsetMin.y);
        targetRect.offsetMax = new Vector2(-(width - rightPx), targetRect.offsetMax.y);
    }

    public void SetPointerPosition(float value)
    {
        float newValue = cursorValue * parentRect.rect.width;
        cursorValue = value;
        DOTween.To(() => newValue, x => newValue = x, value * parentRect.rect.width, pointerMoveTime).OnUpdate(() => 
        {
            pointerRect.anchoredPosition = new Vector2(newValue, targetRect.anchoredPosition.y);
        });        
    }
}