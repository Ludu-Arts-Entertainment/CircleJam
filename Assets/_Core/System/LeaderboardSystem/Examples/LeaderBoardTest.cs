using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderBoardTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            GameInstaller.Instance.SystemLocator.LeaderboardManager.SendRequestForGetLeaderboard("TrophyCount", 0, 49,
                (result) =>
                {
                    Debug.Log(result);
                }, (error) => { Debug.LogError(error); });
        }
    }
}