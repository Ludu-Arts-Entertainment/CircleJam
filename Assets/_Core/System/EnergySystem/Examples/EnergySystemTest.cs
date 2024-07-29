using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnergySystemTest : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            //GameInstaller.Instance.SystemLocator.UIManager.Switch(UITypes.MainMenuPanel, null);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            GameInstaller.Instance.SystemLocator.EnergyManager.Add(1);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            GameInstaller.Instance.SystemLocator.EnergyManager.Use(1);
        }
    }
}
