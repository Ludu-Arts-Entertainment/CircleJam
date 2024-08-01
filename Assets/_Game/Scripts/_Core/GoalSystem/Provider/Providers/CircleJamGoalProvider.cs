using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CircleJamGoalProvider : IGoalProvider
{
    private Dictionary<GoalColors, List<CharacterController>> _charactersByColor = new();

    public int CurrentGoalCount => _currentGoalCount;
    private int _currentGoalCount;
    
    public List<GoalColors> LeveledGoalColors => leveledGoalColors;
    private List<GoalColors> leveledGoalColors = new List<GoalColors>();
    public IGoalProvider CreateSelf()
    {
        return new CircleJamGoalProvider();
    }

    public void Initialize(Action onReady)
    {
        onReady?.Invoke();
        _charactersByColor.Clear();

        leveledGoalColors.Clear();
        leveledGoalColors.Add(GoalColors.Blue);
        leveledGoalColors.Add(GoalColors.Green);

        GameInstaller.Instance.SystemLocator.EventManager.Subscribe<Events.CharacterCreated>(OnCharacterCreated);
        GameInstaller.Instance.SystemLocator.EventManager.Subscribe<Events.GridUpdated>(OnGridUpdated);
    }

    public void UpdateLeveledGoal()
    {
        leveledGoalColors.Clear();
        leveledGoalColors.Add(GoalColors.Blue);
        leveledGoalColors.Add(GoalColors.Green);
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
        List<GoalColors> goalColors = _charactersByColor.Keys.ToList();
        foreach(var characters in _charactersByColor.Values)
        {   
            var idx = characters[0].CurrentGridNode.GridIdx;
            foreach(var character in characters)
            {
                //Debug.Log(character.CurrentGridNode.GridIdx);
                if(character.CurrentGridNode.GridIdx != idx)
                {
                    goalColors.Remove(character.Color);
                }
            }
        }

        foreach(var goalColor in goalColors)
        {
            if(goalColor == leveledGoalColors.First())
            {
                foreach(var character in _charactersByColor[goalColor])
                {
                    GameInstaller.Instance.SystemLocator.PoolManager.Destroy("Character", character);
                }

                _currentGoalCount -= _charactersByColor[goalColor].Count;

                leveledGoalColors.Remove(goalColor);

                Debug.Log($"{goalColor} goal is completed");
                GameInstaller.Instance.SystemLocator.EventManager.Trigger(new Events.GoalUpdated(_charactersByColor[goalColor].Count, true));
                
                _charactersByColor[goalColor].Clear();
                _charactersByColor.Remove(goalColor);

                break;
            }
        }

        if(_charactersByColor.Count == 0)
        {
            GameInstaller.Instance.SystemLocator.UIManager.Show(UITypes.WinPopup, null);
        }
    }

    public void Reset()
    {
        foreach(var goalColor in _charactersByColor.Values)
        {
            foreach(var character in goalColor)
            {
                GameInstaller.Instance.SystemLocator.PoolManager.Destroy("Character", character);
            }
        }

        _charactersByColor.Clear();
    }
}

public partial class Events
{
    public struct CharacterCreated : IEvent
    {
        public GoalColors CharacterColor;
        public CharacterController Character;

        public CharacterCreated(GoalColors characterColor, CharacterController character)
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

public enum GoalColors
{
    None = 0,
    Red = 1,
    Blue = 2,
    Green = 3,
    Yellow = 4,
    Pink = 5,
}