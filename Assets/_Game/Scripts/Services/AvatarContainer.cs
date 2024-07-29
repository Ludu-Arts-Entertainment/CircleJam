using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AvatarContainer", menuName = "ScriptableObjects/Service/AvatarContainer", order = 0)]
public class AvatarContainer : ScriptableObject
{
    public List<PlayerAvatarModel> Avatars = new List<PlayerAvatarModel>();

    public PlayerAvatarModel GetAvatarById(string id)
    {
        foreach (var player in Avatars)
        {
            if (player.id == id)
            {
                return player;
            }
        }

        return new PlayerAvatarModel
        {
            avatar = null,
            id = id,
            requiredLevel = 0
        };
    }

    public List<PlayerAvatarModel> GetAvatarsByLevel(int level)
    {
        return Avatars.FindAll(x => x.requiredLevel <= level);
    }
}

[Serializable]
public class PlayerAvatarModel
{
    public string id;
    public Sprite avatar;
    public int requiredLevel;
}