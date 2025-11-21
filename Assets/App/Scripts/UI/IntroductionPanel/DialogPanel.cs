using DG.Tweening;
using TMPro;
using ToolBox.Dev;
using UnityEngine;

public class DialogPanel : MonoBehaviour
{
    [SerializeField, TextArea] string[] introductionTexts;
    [SerializeField, TextArea] string[] conditionWinTexts;
    int i = 0;

    [Space(10)]
    [SerializeField] GameObject introductionPanelGO;
    [SerializeField] TMP_Text introductionTxt;
    [SerializeField] Animator cardsContentAnim;
    [SerializeField] Sound buttonSound;

    public static DialogPanel Instance;

    private void Awake()
    {
        Instance = this;
    }

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
            introductionPanelGO.transform.DOScale(1.1f, .08f).SetLoops(2, LoopType.Yoyo);
        }
    }

    public void SetConditionWinPanel()
    {
        i = 0;
        introductionTexts = conditionWinTexts;
        introductionTxt.text = introductionTexts[i];
        introductionPanelGO.SetActive(true);
        introductionPanelGO.transform.DOScale(1.1f, .08f).SetLoops(2, LoopType.Yoyo);
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
        AudioManager.Instance.PlayClipAt(buttonSound, Vector3.zero);
    }
}
