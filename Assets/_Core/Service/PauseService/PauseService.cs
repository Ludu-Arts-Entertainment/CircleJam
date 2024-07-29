using System.Collections.Generic;

public static class PauseService
{
    private static HashSet<IPausable> _pausables = new();

    private static int _pauseRequestCount;

    private static bool IsPaused;

    public static void Register(IPausable pausable)
    {
        _pausables.Add(pausable);
        pausable.IsPaused = IsPaused;
    }

    public static void UnRegister(IPausable pausable)
    {
        _pausables.Remove(pausable);
    }

    public static void Pause()
    {
        IsPaused = true;

        foreach (var pausable in _pausables)
        {
            pausable.Pause();
        }

        _pauseRequestCount++;
    }

    public static void Resume()
    {
        _pauseRequestCount--;

        if (_pauseRequestCount > 0)
        {
            return;
        }

        _pauseRequestCount = 0;

        IsPaused = false;

        foreach (var pausable in _pausables)
        {
            pausable.Resume();
        }
    }

    public static void Clear()
    {
        _pauseRequestCount = 0;
        _pausables.Clear();
    }
}
