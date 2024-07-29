public interface IPausable
{
    public bool IsPaused { get; set; }
    public void Pause();
    public void Resume();
}
