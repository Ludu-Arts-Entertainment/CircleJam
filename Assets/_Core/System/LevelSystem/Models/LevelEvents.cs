#if !LevelManager_Modified
public partial class Events
{
    public struct OnLevelLoaded : IEvent
    {
        public int LevelIndex;
        public OnLevelLoaded(int levelIndex)
        {
            LevelIndex = levelIndex;
        }
    }
}
#endif
