using System;
using UnityEngine;
public class AnalyticsManagerTest : MonoBehaviour
{
    private int _level = 0;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GameInstaller.Instance.SystemLocator.AnalyticsManager.SendEvent(new AnalyticEvents.BasicEvent()
            {
                level = _level
            });
            _level++;
        }
    }
}