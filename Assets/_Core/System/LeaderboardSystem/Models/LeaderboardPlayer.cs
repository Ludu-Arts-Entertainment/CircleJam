using System.Collections.Generic;

public class LeaderboardPlayer : ILeaderboardPlayer
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string AvatarUrl { get; set; }
    public uint Rank { get; set; }
    public Dictionary<string, string> Stats { get; set; }

    public LeaderboardPlayer(string id, string username, string avatarUrl, uint rank)
    {
        Id = id;
        Username = username;
        AvatarUrl = avatarUrl;
        Rank = rank;
        Stats = new Dictionary<string, string>();
    }
    
    public void AddStat(string key, string value)
    {
        Stats.Add(key, value);
    }
}
