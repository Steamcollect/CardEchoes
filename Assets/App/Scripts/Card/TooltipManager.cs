using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    //[Header("Settings")]
    //[Header("References")]

    [SerializeField] private SSO_CardData cardData;
    [SerializeField] private TMP_Text cardNameText, cardDescriptionText;

    [SerializeField] private LayoutElement cardLayout;
    [SerializeField] private int maxCharacter;

    //[Header("Input")]
    //[Header("Output")]


    private void Update()
    {
        int nameLength = cardNameText.text.Length;
        int descriptionLength = cardDescriptionText.text.Length;
        cardLayout.enabled = (nameLength > maxCharacter || descriptionLength > maxCharacter) ? true : false;
    }
}