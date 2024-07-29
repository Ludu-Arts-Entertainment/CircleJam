using UnityEngine;

public class SettingSystemTest : MonoBehaviour
{
    private void Start()
    {
        GameInstaller.Instance.SystemLocator.SettingManager.OnSettingChanged += OnSettingChanged;
        GameInstaller.Instance.SystemLocator.SettingManager.GetSetting<float>(SettingType.Sound, out var valueFloat);
        Debug.Log($"Setting: {SettingType.Sound} {valueFloat}");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameInstaller.Instance.SystemLocator.SettingManager.GetSetting<float>(SettingType.Sound, out var valueFloat);
            GameInstaller.Instance.SystemLocator.SettingManager.SetSetting(SettingType.Sound,valueFloat==1?0:1);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            GameInstaller.Instance.SystemLocator.SettingManager.GetSetting<float>(SettingType.Music, out var valueFloat);
            GameInstaller.Instance.SystemLocator.SettingManager.SetSetting(SettingType.Music,valueFloat==1?0:1);
        }
    }
    private void OnSettingChanged(SettingType obj)
    {
        if (obj == SettingType.Sound)
        {
            GameInstaller.Instance.SystemLocator.SettingManager.GetSetting<float>(obj, out var value);
            Debug.Log($"Setting Changed {obj} {value}");
        }else if (obj == SettingType.Music)
        {
            GameInstaller.Instance.SystemLocator.SettingManager.GetSetting<float>(obj, out var value);
            Debug.Log($"Setting Changed {obj} {value}");
        }
    }
}
