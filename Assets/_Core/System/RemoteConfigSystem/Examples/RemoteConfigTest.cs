using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteConfigTest : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log(GameInstaller.Instance.SystemLocator.RemoteConfigManager.GetObject<AdConfig>().BannerAdEnabled);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log(JsonHelper.ToJson(GameInstaller.Instance.SystemLocator.RemoteConfigManager.GetObject<MailConfig>()));
        }
    }
}
