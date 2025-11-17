using UnityEngine;

public class CardControllerUI : MonoBehaviour
{
    //[Header("Settings")]
    [Header("References")]
    [SerializeField] CardMovementUI cardMovementUI;

    //[Header("Input")]
    //[Header("Output")]

    void Setup()
    {
        cardMovementUI._OnPointerEnter += ShowTooltip;
        cardMovementUI._OnPointerExit += HidetoolTip;
    }

    void ShowTooltip()
    {

    }
    void HidetoolTip()
    {

    }
}