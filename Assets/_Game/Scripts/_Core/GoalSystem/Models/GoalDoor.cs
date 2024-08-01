using System.Collections.Generic;
using UnityEngine;

public class GoalDoor : MonoBehaviour
{
    [SerializeField] private List<MeshRenderer> coloredDoors;

    private void OnEnable() 
    {
        GameInstaller.Instance.SystemLocator.EventManager.Subscribe<Events.OnGoalColorUpdated>(OnGoalColorUpdated);
    }

    private void OnDisable() 
    {
        GameInstaller.Instance.SystemLocator.EventManager.Unsubscribe<Events.OnGoalColorUpdated>(OnGoalColorUpdated);
    }

    private void OnGoalColorUpdated(Events.OnGoalColorUpdated updated)
    {
        //Farklı bir material kullanılana kadar karakter datasından çekilecektir.
        var characterData = CharacterDataService.GetCharacterByColor(updated.GoalColor);
        foreach(var door in coloredDoors)
        {
            door.material = characterData.Material;
        }
    }
}
