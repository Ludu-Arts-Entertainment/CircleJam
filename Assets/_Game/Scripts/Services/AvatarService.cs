using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarService
{
    private static bool _isReady;
    private static AvatarContainer _avatarContainer;
    private static void Initialize()
    {
        _avatarContainer = Resources.Load<AvatarContainer>("AvatarContainer");
        _isReady = true;
    }
    
    public static PlayerAvatarModel GetAvatarById(string id)
    {
        if (_isReady) return _avatarContainer.GetAvatarById(id);
        Initialize();
        return _avatarContainer.GetAvatarById(id);
    }
    public static List<PlayerAvatarModel> GetAvatarsByLevel(int level)
    {
        if (_isReady) return _avatarContainer.GetAvatarsByLevel(level);
        Initialize();
        return _avatarContainer.GetAvatarsByLevel(level);
    }
}

