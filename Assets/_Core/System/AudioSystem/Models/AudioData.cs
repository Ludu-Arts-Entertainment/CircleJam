#if !AudioManager_Modified
using UnityEngine;

[System.Serializable]
public class AudioData
{
    public string Id;
    public AudioClip Clip;
    public bool IsMusic;
    public bool IsLoop;
    public float Pitch = 1f;
}
#endif