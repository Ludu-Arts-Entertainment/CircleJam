#if !RemoteConfigManager_Modified
using System;
using System.Collections.Generic;
using UnityEngine;

public class DefaultRemoteData
{
    private LocalConfigData _localConfigData;
    private LocalConfigData LocalConfigData
    {
        get
        {
            if (_localConfigData == null)
            {
                _localConfigData = Resources.Load<LocalConfigData>("LocalConfigData");
                if (_localConfigData == null)
                {
                    Debug.LogWarning("LocalConfigData not found in Resources");
                    return ScriptableObject.CreateInstance<LocalConfigData>();
                }
            }
            return _localConfigData;
        }
    }

    public Dictionary<Type, object> RemoteConfigDictionary
    {
        get
        {
            if (_remoteConfigDictionary != null) return _remoteConfigDictionary;
            _remoteConfigDictionary = new Dictionary<Type, object>();
            foreach (var config in LocalConfigData.ListOfConfigs)
            {
                _remoteConfigDictionary.Add(config.GetType(), TypeUtilities.CopyClass(config));
            }
            return _remoteConfigDictionary;
        }
    }

    private Dictionary<Type, object> _remoteConfigDictionary;
}
#endif
