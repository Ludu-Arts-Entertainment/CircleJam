public class AudioManager : IManager
{
    private IAudioProvider _audioProvider;
    public IManager CreateSelf()
    {
        return new AudioManager();
    }
    SystemLocator _systemLocator;
    public void Initialize(GameInstaller gameInstaller, System.Action onReady)
    {
        _audioProvider = AudioProviderFactory.Create(gameInstaller.Customizer.AudioProvider);
        _audioProvider.Initialize(onReady);
        _systemLocator = gameInstaller.SystemLocator;
        _systemLocator.SettingManager.OnSettingChanged += OnSettingChanged;
    }

    private void OnSettingChanged(SettingType obj)
    {
        switch (obj)
        {
            case SettingType.Music:
                _systemLocator.SettingManager.GetSetting<float>(SettingType.Music, out var musicVolume);
                SetMusicVolume(musicVolume);
                break;
            case SettingType.Sound:
                _systemLocator.SettingManager.GetSetting<float>(SettingType.Sound, out var soundVolume);
                SetSoundVolume(soundVolume);
                break;
        }
    }

    public bool IsReady()
    {
        return _audioProvider != null;
    }
    
    public void PlaySound(string soundName)
    {
        _audioProvider.PlaySound(soundName);
    }
    public void PlayMusic(string musicName)
    {
        _audioProvider.PlayMusic(musicName);
    }

    public void PauseMusic(string musicName)
    {
        _audioProvider.PauseMusic(musicName);
    } 
    public void ResumeMusic(string musicName)
    {
        _audioProvider.ResumeMusic(musicName);
    }

    public void StopMusic()
    {
        _audioProvider.StopMusic();
    }

    private void SetMusicVolume(float volume)
    {
        _audioProvider.SetMusicVolume(volume);
    }
    private void SetSoundVolume(float volume)
    {
        _audioProvider.SetSoundVolume(volume);
    }
}
