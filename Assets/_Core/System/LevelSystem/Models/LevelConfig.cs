using System;

[Serializable]
public class LevelConfig : IConfig
{
    public string[] ActiveLevelNames;
    public int StartLoopLevelIndex;
}
