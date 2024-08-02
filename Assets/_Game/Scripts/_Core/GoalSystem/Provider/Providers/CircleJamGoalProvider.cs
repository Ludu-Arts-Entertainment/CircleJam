using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CircleJamGoalProvider : IGoalProvider
{
    private Dictionary<GoalColor, List<CharacterController>> _charactersByColor = new();

    public int CurrentGoalCount => _currentGoalCount;
    private int _currentGoalCount;
    
    public List<GoalColor> LeveledGoalColors => leveledGoalColors;
    private List<GoalColor> leveledGoalColors = new List<GoalColor>();

    private SystemLocator _systemLocator;
    private bool _isGoalColorOrderEnable;
    public IGoalProvider CreateSelf()
    {
        return new CircleJamGoalProvider();
    }

    public async void Initialize(Action onReady)
    {
        _systemLocator = GameInstaller.Instance.SystemLocator;
        _charactersByColor.Clear();

        leveledGoalColors.Clear();
        leveledGoalColors.Add(GoalColor.Blue);
        leveledGoalColors.Add(GoalColor.Green);

        _systemLocator.EventManager.Subscribe<Events.CharacterCreated>(OnCharacterCreated);
        _systemLocator.EventManager.Subscribe<Events.GridUpdated>(OnGridUpdated);

#if RemoteConfigManager_Enabled
        await UniTask.WaitUntil(()=>GameInstaller.Instance.ManagerDictionary.ContainsKey(ManagerEnums.RemoteConfigManager));
        var goalConfig = _systemLocator.RemoteConfigManager.GetObject<GoalConfig>();
        if(goalConfig != null)
            _isGoalColorOrderEnable = goalConfig.IsGoalColorOrderEnable;
#endif
        onReady?.Invoke();
    }

    public void UpdateLeveledGoal()
    {
        leveledGoalColors.Clear();
        leveledGoalColors.Add(GoalColor.Blue);
        leveledGoalColors.Add(GoalColor.Green);
    }

    private void OnCharacterCreated(Events.CharacterCreated created)
    {
        if(!_charactersByColor.ContainsKey(created.CharacterColor))
        {
            _charactersByColor.Add(created.CharacterColor, new List<CharacterController>());
        }
        _charactersByColor[created.CharacterColor].Add(created.Character);
        _currentGoalCount++;
    }

    private void OnGridUpdated(Events.GridUpdated updated)
    {
        //aynı renkteki karakterlerin current grid'lerinin index'i aynı ise goal'den onları sil
        List<GoalColor> goalColors = _charactersByColor.Keys.ToList();
        foreach(var characters in _charactersByColor.Values)
        {   
            var idx = characters[0].CurrentGridNode.GridNodeData.GridIdx;
            foreach(var character in characters)
            {
                var gridNodeData = character.CurrentGridNode.GridNodeData;
                if((gridNodeData.CircleLevel > 0 && _systemLocator.GridManager.CheckAnyObstacle(gridNodeData.CircleLevel - 1, idx)) 
                    || gridNodeData.GridIdx != idx)
                {
                    goalColors.Remove(character.Color); 
                }
            }
        }

        foreach(var goalColor in goalColors)
        {
            if(goalColor == leveledGoalColors.First() || !_isGoalColorOrderEnable)
            {
                foreach(var character in _charactersByColor[goalColor])
                {
                    _systemLocator.PoolManager.Destroy("Character", character);
                }

                _currentGoalCount -= _charactersByColor[goalColor].Count;

                leveledGoalColors.Remove(goalColor);

                Debug.Log($"{goalColor} goal is completed");
                _systemLocator.EventManager.Trigger(new Events.GoalUpdated(_charactersByColor[goalColor].Count, true));
                
                _charactersByColor[goalColor].Clear();
                _charactersByColor.Remove(goalColor);

                break;
            }
        }

        if(_charactersByColor.Count == 0)
        {
            _systemLocator.UIManager.Show(UITypes.WinPopup, null);
        }
    }

    public void Reset()
    {
        foreach(var goalColor in _charactersByColor.Values)
        {
            foreach(var character in goalColor)
            {
                _systemLocator.PoolManager.Destroy("Character", character);
            }
        }

        _charactersByColor.Clear();
    }
}

public partial class Events
{
    public struct CharacterCreated : IEvent
    {
        public GoalColor CharacterColor;
        public CharacterController Character;

        public CharacterCreated(GoalColor characterColor, CharacterController character)
        {
            CharacterColor = characterColor;
            Character = character;
        }
    }

    public struct GoalUpdated : IEvent
    {
        public int Amount;
        public bool WithAnimation;

        public GoalUpdated(int amount, bool withAnimation)
        {
            Amount = amount;
            WithAnimation = withAnimation;
        }
    }
}

public enum GoalColor
{
    None = 0,
    Red = 1,
    Blue = 2,
    Green = 3,
    Yellow = 4,
    Pink = 5,
}