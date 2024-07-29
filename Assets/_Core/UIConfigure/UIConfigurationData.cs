using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UIConfigurationData", menuName = "UIConfigurationData")]
public class UIConfigurationData : ScriptableObject
{
    public List<ColorData> ColorDataList;
}


public enum ColorType
{
    Primary,
    Secondary,
    Color1,
    Color2,
    Color3,
    Color4,
    Color5,
    Color6,
    Color7,
    Color8,
    Color9,
    Color10,
}

public static class ColorTypeExtension
{
    public static Color Get(this ColorType colorType)
    {
        var color = GameInstaller.Instance.UIConfigurationData.ColorDataList.Find(x => x.Key == colorType)?.Value;
        return color ?? Color.white;
    }
} 

[Serializable]
public class ColorData
{
    public ColorType Key;
    public Color Value;
}