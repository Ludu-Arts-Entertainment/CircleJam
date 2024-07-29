using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SystemLocator
{
    private AudioManager _audioManager;

    public AudioManager AudioManager => _audioManager ??=
        GameInstaller.Instance.ManagerDictionary[ManagerEnums.AudioManager] as AudioManager;
}