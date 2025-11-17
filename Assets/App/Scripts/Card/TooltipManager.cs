using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    //[Header("Settings")]
    //[Header("References")]

    [SerializeField] private TMP_Text cardNameText, cardDescriptionText;

    [SerializeField] private LayoutElement cardLayout;
    [SerializeField] private int maxCharacter;

    //[Header("Input")]
    //[Header("Output")]

    public void SetText(string description, string name = "")
    {
        if (name == "")
        {
            cardNameText.gameObject.SetActive(false);
        } else
        {
            cardNameText.gameObject.SetActive(true);
            cardNameText.text = name;
            
        }

        cardDescriptionText.text = description;

        int nameLength = cardNameText.text.Length;
        int descriptionLength = cardDescriptionText.text.Length;
        cardLayout.enabled = (nameLength > maxCharacter || descriptionLength > maxCharacter) ? true : false;
    }
}