using UnityEngine;

public class AudioSystemTest : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GameInstaller.Instance.SystemLocator.AudioManager.PlaySound("ExampleSound");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            GameInstaller.Instance.SystemLocator.AudioManager.PlayMusic("ExampleMusic");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            GameInstaller.Instance.SystemLocator.AudioManager.PauseMusic("ExampleMusic");
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            GameInstaller.Instance.SystemLocator.AudioManager.ResumeMusic("ExampleMusic");
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            GameInstaller.Instance.SystemLocator.AudioManager.StopMusic();
        }
    }
}
