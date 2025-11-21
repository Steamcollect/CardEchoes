using TMPro;
using UnityEngine;

public class ConditionString : MonoBehaviour
{
    [SerializeField] TMP_Text txt;

    public void SetText(string text)
    {
        txt.text = text;
    }
}