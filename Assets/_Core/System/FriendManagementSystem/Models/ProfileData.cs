using System;
using System.Collections.Generic;

public partial class GameData
{
    public ProfileModel ProfileData = new();
}

[Serializable]
public class ProfileModel
{
    public string UserId;
    public UsernameModel Name;
    public string AvatarIndex;
    public string CountryCode;
    public string League;
    public int Level;
    public int Experience;
    public int UnlockedCards;
    public int TrophyCount;


    public ProfileModel()
    {
        Name = new UsernameModel("");
        AvatarIndex = string.Empty;
        CountryCode = string.Empty;
        UserId = string.Empty;
        League = string.Empty;
        Level = 0;
        Experience = 0;
        UnlockedCards = 0;
    }

    public ProfileModel(string name, string avatarIndex, string countryCode, string league, int level, int experience,
        int unlockedCards)
    {
        Name = new UsernameModel(name);
        AvatarIndex = avatarIndex;
        CountryCode = countryCode;
        League = league;
        Level = level;
        Experience = experience;
        UnlockedCards = unlockedCards;
    }
}