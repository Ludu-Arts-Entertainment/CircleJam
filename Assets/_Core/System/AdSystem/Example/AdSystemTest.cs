using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdSystemTest : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GameInstaller.Instance.SystemLocator.AdManager.ShowBanner();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            GameInstaller.Instance.SystemLocator.AdManager.HideBanner();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameInstaller.Instance.SystemLocator.AdManager.ShowInterstitial();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            GameInstaller.Instance.SystemLocator.AdManager.ShowRewarded(()=> { Debug.Log("Rewarded"); });
        }
    }
}
