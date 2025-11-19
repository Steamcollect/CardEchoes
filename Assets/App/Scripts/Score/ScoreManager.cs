using DG.Tweening;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float scaleValue;
    [SerializeField] float bumpDuration;
    int currentScore = 0;

    [Header("References")]
    [SerializeField] TMP_Text scoreTxt;
    
    //[Header("Input")]
    //[Header("Output")]

    public static ScoreManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        scoreTxt.text = currentScore.ToString();

        float currentScale = transform.localScale.x;

        transform.DOKill();
        transform.DOScale(currentScale * scaleValue, bumpDuration).OnComplete(() =>
        {
            transform.DOScale(1, bumpDuration).OnComplete(() =>
            {
                transform.localScale = Vector3.one;
            });
        });
    }
}