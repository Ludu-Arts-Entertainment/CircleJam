using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrefabBaseLevelProvider : ILevelProvider
{
    public ILevelData CurrentLevelData { get; private set; }
    private LevelContainer _levelContainer;
    private List<PrefabBaseLevelData> _activeLevelDataList = new List<PrefabBaseLevelData>();
    private LevelController _levelController;
    private Transform _levelRoot;
    private SystemLocator _systemLocator;
    private int _startLoopLevelIndex = 1; // 1 is the first level not 0
    private readonly string[] _activeLevelNames = {}; // { "Level 0", "Level 3" } active level name list

    public ILevelProvider CreateSelf()
    {
        return new PrefabBaseLevelProvider();
    }

    public void Initialize(Action onReady)
    {
        _levelContainer = Resources.Load<LevelContainer>("LevelContainer");
        if (_levelContainer == null)
        {
            Debug.LogWarning("LevelContainer not found");
            return;
        }
        _activeLevelDataList.AddRange(_activeLevelNames.Length ! > 0
            ? _levelContainer.LevelDataList.SelectMany(x => _activeLevelNames.Where(y => x.Name == y).Select(z => x))
                .ToList()
            : _levelContainer.LevelDataList);

        _levelRoot = new GameObject("LevelRoot").transform;
        _systemLocator = GameInstaller.Instance.SystemLocator;
        onReady?.Invoke();
    }

    public void LoadLevel(int levelIndex)
    {
        if (CurrentLevelData != null)
        {
            DisposeLevel();
        }

        var index = (levelIndex - 1) % _activeLevelDataList.Count;
        if (levelIndex > _activeLevelDataList.Count)
        {
            if (_startLoopLevelIndex > _activeLevelDataList.Count)
            {
                _startLoopLevelIndex = 1;
            }

            var temp = _activeLevelDataList.Count - _startLoopLevelIndex + 1;
            index = (levelIndex - _startLoopLevelIndex) % temp + _startLoopLevelIndex - 1;
        }

        CurrentLevelData = _activeLevelDataList[index];
        _levelController = _systemLocator.PoolManager.Instantiate<LevelController>(((PrefabBaseLevelData)CurrentLevelData).PrefabPoolId,
            Vector3.zero,
            Quaternion.identity, _levelRoot);
        _levelController.LoadLevel((CurrentLevelData as PrefabBaseLevelData).LevelData);
        _systemLocator.EventManager.Trigger(new Events.OnLevelLoaded(levelIndex));
    }

    public void DisposeLevel()
    {
        if (CurrentLevelData == null) return;
        
        GameInstaller.Instance.SystemLocator.GoalManager.Reset();
        GameInstaller.Instance.SystemLocator.GridManager.ResetGrid();
        GameInstaller.Instance.SystemLocator.PoolManager.Destroy(((PrefabBaseLevelData)CurrentLevelData).PrefabPoolId, _levelController);

        CurrentLevelData = null;
        _levelController = null;
    }
}