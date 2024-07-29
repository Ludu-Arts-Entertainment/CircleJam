using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAudioProvider : IAudioProvider
{
    private float _musicVolume = 1f;
    private float _soundVolume = 1f;
    private Dictionary<string, AudioData> _audioDataList = new ();

    private AudioSource _playingMusicSource;
    private string _playingMusicName;
    private List<(string, AudioSource)> _playingSoundSources = new ();
    
    private List<AudioSource> _freeAudioSources = new ();
    private Transform _audioSourceParent;
    public IAudioProvider CreateSelf()
    {
        return new BasicAudioProvider();
    }

    public void Initialize(Action onReady)
    {
        GameInstaller.Instance.SystemLocator.SettingManager.GetSetting(SettingType.Sound, out _soundVolume);
        GameInstaller.Instance.SystemLocator.SettingManager.GetSetting(SettingType.Music, out _musicVolume);
        _audioSourceParent = new GameObject("AudioSourceParent").transform;
        GameObject.DontDestroyOnLoad(_audioSourceParent);
        var audioContainers = Resources.LoadAll<AudioContainer>("AudioContainer");
        foreach (var audioContainer in audioContainers)
        {
            foreach (var pair in audioContainer.AudioDataList)
            {
                _audioDataList.Add(pair.Id, pair);
            }
        }
        onReady?.Invoke();
    }
    
    public void PlaySound(string soundName)
    {
        if (!_audioDataList.TryGetValue(soundName, out var audioData))
        {
            Debug.LogError($"AudioData not found: {soundName}");
            return;
        }
        var audioSource = GetFreeAudioSource();
        audioSource.loop = false;
        audioSource.clip = audioData.Clip;
        audioSource.pitch = audioData.Pitch;
        audioSource.volume = _soundVolume;
        audioSource.PlayOneShot(audioData.Clip);
        _playingSoundSources.Add((soundName, audioSource));
        CoroutineController.StartCoroutine($"RecycleRoutine_-{soundName}-{Time.deltaTime}-{UnityEngine.Random.Range(0,10000)}",SoundRecycle(audioSource));
    }

    public void PlayMusic(string musicName)
    {
        if (!_audioDataList.TryGetValue(musicName, out var audioData))
        {
            Debug.LogError($"AudioData not found: {musicName}");
            return;
        }

        if (audioData.IsMusic && _playingMusicName == musicName)
        {
            return;
        }
        StopMusic();
        if (CoroutineController.IsCoroutineRunning($"RecycleRoutine_{_playingMusicName}"))
        {
            CoroutineController.StopCoroutine($"RecycleRoutine_{_playingMusicName}");
        }
        
        _playingMusicName = musicName;
        var audioSource = GetFreeAudioSource();
        audioSource.clip = audioData.Clip;
        audioSource.pitch = audioData.Pitch;
        audioSource.volume = _musicVolume;
        audioSource.loop = audioData.IsLoop;
        audioSource.Play();
        _playingMusicSource = audioSource;
        if (!audioData.IsLoop)
        {
            CoroutineController.StartCoroutine($"RecycleRoutine_{musicName}",MusicRecycle(audioSource));
        }
    }

    public void PauseMusic(string musicName)
    {
        if (_playingMusicSource!=null)
        {
            _playingMusicSource.Pause();
        }
    }
    public void ResumeMusic(string musicName)
    {
        if (_playingMusicSource != null)
        {
            _playingMusicSource.UnPause();
            return;
        }
        PlayMusic(musicName);
    }
    public void StopMusic()
    {
        if (_playingMusicSource != null)
        {
            _playingMusicSource.Stop();
            _freeAudioSources.Add(_playingMusicSource);
            _playingMusicSource = null;
        }
    }

    public void SetMusicVolume(float volume)
    {
        _musicVolume = volume;
        _playingMusicSource.volume = _musicVolume;
    }

    public void SetSoundVolume(float volume)
    {
        _soundVolume = volume;
        foreach (var pair in _playingSoundSources)
        {
            pair.Item2.volume = _musicVolume;
        }
    }
    private AudioSource GetFreeAudioSource()
    {
        if (_freeAudioSources.Count>0)
        {
            var source = _freeAudioSources[0];
            _freeAudioSources.RemoveAt(0);
            return source;
        }
        var newSourceObject = new GameObject("AudioSource");
        newSourceObject.transform.SetParent(_audioSourceParent);
        return newSourceObject.AddComponent<AudioSource>();
    }
    private IEnumerator SoundRecycle(AudioSource source)
    {
        yield return new WaitUntil(()=>!source.isPlaying);
        var index = _playingSoundSources.FindIndex(x => x.Item2 == source);
        _playingSoundSources.RemoveAt(index);
        _freeAudioSources.Add(source);
    }
    private IEnumerator MusicRecycle(AudioSource source)
    {
        yield return new WaitUntil(()=>!source.isPlaying);
        _playingMusicSource = null;
        _freeAudioSources.Add(source);
    }
}
