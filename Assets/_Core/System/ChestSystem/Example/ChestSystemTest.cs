using UnityEngine;

public class ChestSystemTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            //GameInstaller.Instance.SystemLocator.UIManager.Show(UITypes.ChestPanel, new ChestPanelArgs(ChestType.BasicChest,50));
        }
    }
}
