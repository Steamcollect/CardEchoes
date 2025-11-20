using TMPro;
using UnityEngine;

public class IntroductionPanel : MonoBehaviour
{
    [SerializeField, TextArea] string[] introductionTexts;
    int i = 0;

    [Space(10)]
    [SerializeField] GameObject introductionPanelGO;
    [SerializeField] TMP_Text introductionTxt;
    [SerializeField] Animator cardsContentAnim;

    private void Start()
    {
        if(introductionTexts.Length <= 0)
        {
            cardsContentAnim.SetTrigger("Show");
            introductionPanelGO.SetActive(false);
        }
        else
        {
            introductionTxt.text = introductionTexts[i];
            introductionPanelGO.SetActive(true);
        }
    }

    public void NextButton()
    {
        i++;
        if (i >= introductionTexts.Length)
        {
            cardsContentAnim.SetTrigger("Show");
            introductionPanelGO.SetActive(false);
        }
        else
        {
            introductionTxt.text = introductionTexts[i];
        }
    }
}
