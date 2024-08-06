using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowUIButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private string UIName;

    private void OnEnable() 
    {
        button.onClick.AddListener(ShowPanel);
    }

    private void OnDisable() 
    {
        button.onClick.RemoveListener(ShowPanel);
    }

    private void ShowPanel()
    {
        GameInstaller.Instance.SystemLocator.UIManager.Show(UIName, null);
    }
}
