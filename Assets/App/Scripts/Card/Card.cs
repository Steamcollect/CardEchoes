using DG.Tweening;
using UnityEngine;

public class Card : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float shakeAngle = 15;
    [SerializeField] float shakeDuration = 0.2f;
    [SerializeField] float yOffset;

    [Header("References")]
    [SerializeField] GameObject rectoGO;
    [SerializeField] GameObject versoGO;
    Card[] neighbours;

    SSO_CardData data;

    //[Header("Input")]
    //[Header("Output")]

    public void Setup(SSO_CardData data)
    {
        this.data = data;
    }

    public void SetLayer(LayerMask layerMask)
    {
        int maskValue = layerMask.value;

        if (maskValue == 0 || (maskValue & (maskValue - 1)) != 0)
        {
            Debug.LogError($"LayerMask {layerMask} n’a pas exactement un seul layer actif !");
            return;
        }

        int layer = Mathf.RoundToInt(Mathf.Log(maskValue, 2));

        ApplyLayerRecursively(transform, layer);
    }

    private void ApplyLayerRecursively(Transform t, int layer)
    {
        t.gameObject.layer = layer;

        foreach (Transform child in t)
            ApplyLayerRecursively(child, layer);
    }

    public void WaveShake()
    {
        transform.DOMoveY(transform.position.y + yOffset, shakeDuration / 2).SetLoops(2, LoopType.Yoyo);
        transform.DOPunchRotation(Vector3.up * shakeAngle, shakeDuration, 20, 1);
    }

    public void SetNeighbours(Card[] neighbours)
    {
        this.neighbours = neighbours;
    }

    public SSO_CardData GetData()
    {
        return data;
    }

    public void SetActiveRecto(bool active) { rectoGO.SetActive(active); }
    public void SetActiveVerso(bool active) { versoGO.SetActive(active); }

    public Card[] GetNeighbours()
    {
        return neighbours;
    }
}