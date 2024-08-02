using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class GoalDoor : MonoBehaviour
{
    [SerializeField] private List<MeshRenderer> coloredDoors;
    [SerializeField] private float openPosition = 1.9f;

    private void OnEnable() 
    {
        GameInstaller.Instance.SystemLocator.EventManager.Subscribe<Events.GoalUpdated>(OnGoalUpdated);
    }

    private void OnDisable() 
    {
        GameInstaller.Instance.SystemLocator.EventManager.Unsubscribe<Events.GoalUpdated>(OnGoalUpdated);
    }

    private void OnGoalUpdated(Events.GoalUpdated updated)
    {
        if(updated.WithAnimation)
        {
            coloredDoors[0].transform.DOLocalMoveX(-openPosition, 0.5f).SetEase(Ease.OutQuart).OnComplete(()=>
            {
                coloredDoors[0].transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutQuart);
            });

            coloredDoors[1].transform.DOLocalMoveX(openPosition, 0.5f).SetEase(Ease.OutQuart).OnComplete(()=>
            {
                coloredDoors[1].transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutQuart);
                UpdateDoorColor();
            });
        }
        else
        {
            UpdateDoorColor();
        }
    }

    private void UpdateDoorColor()
    {
        var goalColorOrderEnable = GameInstaller.Instance.SystemLocator.RemoteConfigManager.GetObject<GoalConfig>();

        if(GameInstaller.Instance.SystemLocator.GoalManager.LeveledGoalColors.Count > 0 && goalColorOrderEnable.IsGoalColorOrderEnable)
        {
            //Kendi datası olursa oradan alınacak.
            var characterData = CharacterDataService.GetCharacterByColor(GameInstaller.Instance.SystemLocator.GoalManager.LeveledGoalColors.First());
            coloredDoors[0].material = characterData.Material;
            coloredDoors[1].material = characterData.Material;
        }
    }
}
