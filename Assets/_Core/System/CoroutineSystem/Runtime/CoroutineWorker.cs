using System.Collections.Generic;
using UnityEngine;

public class CoroutineWorker
{
    public List<string> RunningRoutines => CoroutineController.RoutineKeys;
    public static GameInstaller Instance => GameInstaller.Instance;
}