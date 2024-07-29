using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
public class PlayerPrefsDataProvider : IDataProvider
{
    private string _dataKey = "PlayerPrefsData.key";
    private DataManager _dataManager;
    
    public void Initialize(DataManager dataManager)
    {
        _dataManager = dataManager;
    }

    public IDataProvider CreateSelf()
    {
        return new PlayerPrefsDataProvider();
    }
    public async UniTask<IData> Load(IData data)
    {
        if (PlayerPrefs.HasKey(_dataKey))
        {
            data = JsonHelper.FromJson<IData>(PlayerPrefs.GetString(_dataKey),typeof(GameData));
        }
        else
        {
            data = Activator.CreateInstance(typeof(GameData)) as IData;
        }

        return data;
    }

    public async UniTask<IData> LoadAll(IData data)
    {
        return await Load(data);
    }

    public void Save(IData data)
    {
        
        PlayerPrefs.SetString(_dataKey, JsonHelper.ToJson(data));
        PlayerPrefs.Save();
    }

    public void SaveAll(IData data)
    {
        Save(data);
    }
}
