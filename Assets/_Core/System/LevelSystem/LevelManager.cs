using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : IManager
{
    private ILevelProvider _levelProvider;
    public ILevelData CurrentLevelData => _levelProvider.CurrentLevelData;
    public IManager CreateSelf()
    {
        return new LevelManager();
    }

    public int CurrentLevelIndex { get; private set; }

    public void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        var stateData =
            GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string, ulong>>(GameDataType.State);
        if (stateData.TryGetValue("CurrentLevel", out var value))
        {
            CurrentLevelIndex = (int)value;
        }
        else
        {
            stateData.Add("CurrentLevel", 1);
            CurrentLevelIndex = 1;
            GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.State, stateData);
            GameInstaller.Instance.SystemLocator.DataManager.SaveData();
        }

        _levelProvider = LevelProviderFactory.Create(LevelProviderEnums.PrefabBaseLevelProvider);
        _levelProvider.Initialize(onReady);
    }

    public bool IsReady()
    {
        return _levelProvider != null;
    }

    public void LoadLevel(int levelIndex = -1)
    {
        if (levelIndex == -1)
        {
            _levelProvider.LoadLevel(CurrentLevelIndex);
            return;
        }

        _levelProvider.LoadLevel(levelIndex);
    }

    public void DisposeLevel()
    {
        _levelProvider.DisposeLevel();
    }

    public void LevelComplete()
    {
        CurrentLevelIndex++;
        var stateData =
            GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string, ulong>>(GameDataType.State);
        stateData["CurrentLevel"] = (ulong)CurrentLevelIndex;
        GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.State, stateData);
        GameInstaller.Instance.SystemLocator.DataManager.SaveData();
    }
}