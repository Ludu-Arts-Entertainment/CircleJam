using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioContainer", menuName = "ScriptableObjects/AudioContainer", order = 1)]
public class AudioContainer : ScriptableObject
{
    public List<AudioData> AudioDataList;
}
