using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class FloatingText : MonoBehaviour
{
    [Header("Meta Data")]
    [SerializeField] private string poolId;
    [SerializeField] private TextMeshProUGUI text;
    
    public void SetText(string text)
    {
        this.text.text = text;
    }

    public abstract void StartAnimation();

    protected abstract void ResetAnimation();

    protected void DestroySelf()
    {
        GameInstaller.Instance.SystemLocator.PoolManager.Destroy(poolId, this);
    }
}
