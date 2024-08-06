using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : PopupBase
{
    private const string privacyPolicyUrl = "https://www.luduarts.com/privacy";

    [SerializeField] private Button closeButton, privacyPolicyButton;
    protected override void OnShown()
    {
        base.OnShown();
        closeButton.onClick.AddListener(ClosePanel);
        privacyPolicyButton.onClick.AddListener(OpenPrivacyPolicy);
    }

    protected override void OnHidden()
    {
        base.OnHidden();
        closeButton.onClick.RemoveListener(ClosePanel);
        privacyPolicyButton.onClick.RemoveListener(OpenPrivacyPolicy);
    }

    private void OpenPrivacyPolicy()
    {
        Application.OpenURL(privacyPolicyUrl);
    }

    private void ClosePanel()
    {
        GameInstaller.Instance.SystemLocator.UIManager.Hide(UITypes.SettingsPopup);
    }
}
public partial class UITypes
{
    public const string SettingsPopup = "SettingsPopup";
}