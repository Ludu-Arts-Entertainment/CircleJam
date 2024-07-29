using UnityEngine;

public class QuestSystemTest : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Active Count "+GameInstaller.Instance.SystemLocator.QuestManager.GetQuests(QuestState.Active).Count);
            Debug.Log("Group Id: 0 Active Count "+GameInstaller.Instance.SystemLocator.QuestManager.GetQuests(QuestState.Active,"basic_game_quest_group").Count);
            Debug.Log("Claimed Count "+GameInstaller.Instance.SystemLocator.QuestManager.GetQuests(QuestState.Claimed).Count);
        }
    }
}
