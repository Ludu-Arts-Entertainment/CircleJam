using System.Collections.Generic;

public enum AudioProviderEnums
{
    BasicAudioProvider
}
public static class AudioProviderFactory
{
    private static Dictionary<AudioProviderEnums, IAudioProvider> _audioProviderDictionary = new ()
    {
        {AudioProviderEnums.BasicAudioProvider , new BasicAudioProvider()},
    };
    
    public static IAudioProvider Create(AudioProviderEnums providerEnum)
    {
        return _audioProviderDictionary.TryGetValue(providerEnum, out var provider) ? provider.CreateSelf() : null;
    }
}