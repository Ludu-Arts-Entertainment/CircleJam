using System;
using System.Globalization;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class NumberAnimatedTMP_Text : TMP_Text
{
    public string Format;
    public float Current { get; private set;}
    public void SetCurrent(float current, bool roundToInt = true)
    {
        Current = current;
        if (!string.IsNullOrEmpty(Format))
        {
            SetText(string.Format(Format, roundToInt?(int)Current:Current, CultureInfo.InvariantCulture));
            return;
        }
        SetText((roundToInt?(int)Current:Current).ToString(CultureInfo.InvariantCulture)); 
    }

    public void UpdateValue(float to, float duration = 0.5f, bool roundToInt = true)
    {
        if (to > int.MaxValue)
        {
            SetCurrent(to);
        }
        else
        {
            DOTween.To(
                getter: () => Current,
                setter: current => SetCurrent(current, roundToInt),
                endValue: to,
                duration);
        }
    }
}