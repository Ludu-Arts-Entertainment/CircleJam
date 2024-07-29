public interface IAudioProvider
{
    IAudioProvider CreateSelf();
    void Initialize(System.Action onReady);
    void PlaySound(string soundName);
    void PlayMusic(string musicName);
    void PauseMusic(string musicName);
    void ResumeMusic(string musicName);
    void StopMusic();
    void SetMusicVolume(float volume);
    void SetSoundVolume(float volume);
}
