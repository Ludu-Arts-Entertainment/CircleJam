using System.Globalization;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class NumberAnimator
{
    private string format;
    public float Current
    {
        private set;
        get;
    }
    protected TMP_Text tmpText;
    private bool isPercentage;
    private Tween tween;

    public NumberAnimator(float current, TMP_Text tmpText, string format = "", bool isPercentage = false)
    {
        this.tmpText = tmpText;
        this.format = format;
        this.isPercentage = isPercentage;
        SetCurrent(current);
    }

    public void SetCurrent(float current, bool roundToInt = true)
    {
        Current = current;
        string value = "";
        if (!string.IsNullOrEmpty(format))
        {
            value = string.Format(format, roundToInt?(int)Current:Current, CultureInfo.InvariantCulture);
            tmpText.text = isPercentage ?  "%" + value : value;
            return;
        }

        value = (roundToInt?(int)Current:Current).ToString(CultureInfo.InvariantCulture);
        tmpText.text = isPercentage ?  "%" + value : value;
    }

    public void UpdateValue(float to, float duration = 0.5f, bool roundToInt = true)
    {
        if (to > int.MaxValue)
        {
            SetCurrent(to);
        }
        else
        {
            tween?.Kill();
            tween = DOTween.To(
                getter: () => Current,
                setter: current => SetCurrent(current, roundToInt),
                endValue: to,
                duration);
        }
    }

    public void StopAnimation()
    {
        tween?.Kill();
        tmpText.DOKill();
        tmpText.transform.DOKill();
        tmpText.transform.localScale = Vector3.one;
    }
}