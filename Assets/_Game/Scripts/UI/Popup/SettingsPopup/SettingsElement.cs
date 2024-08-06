using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsElement : MonoBehaviour
{
    [SerializeField] private SettingType settingType;
    [SerializeField] private GameObject disableImage;

    private Button button;
    private Button Button => button ??= GetComponent<Button>();
    private bool isEnable = true;

    private void Awake() 
    {
        GameInstaller.Instance.SystemLocator.SettingManager.GetSetting<float>(settingType, out var valueFloat);
        isEnable = Math.Abs(1-valueFloat) < .01f;
        disableImage.SetActive(!isEnable);
    }

    private void OnEnable() 
    {
        Button.onClick.AddListener(OnClick);
        GameInstaller.Instance.SystemLocator.SettingManager.OnSettingChanged += OnSettingChanged;

    }

    private void OnDisable() 
    {
        Button.onClick.RemoveListener(OnClick);
    }

    private void OnSettingChanged(SettingType type)
    {
        GameInstaller.Instance.SystemLocator.SettingManager.GetSetting<float>(settingType, out var valueFloat);
        isEnable = Math.Abs(1-valueFloat) < .01f;
        disableImage.SetActive(!isEnable);
    }

    private void OnClick() 
    {
        GameInstaller.Instance.SystemLocator.HapticManager.Light();
        
        isEnable = !isEnable;
        disableImage.SetActive(!isEnable);
        GameInstaller.Instance.SystemLocator.SettingManager.SetSetting(settingType, isEnable ? 1 : 0);
    }
}