using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManagerTest : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GameInstaller.Instance.SystemLocator.LevelManager.LoadLevel();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            GameInstaller.Instance.SystemLocator.LevelManager.LevelComplete();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            GameInstaller.Instance.SystemLocator.LevelManager.DisposeLevel();
        }
    }
}
