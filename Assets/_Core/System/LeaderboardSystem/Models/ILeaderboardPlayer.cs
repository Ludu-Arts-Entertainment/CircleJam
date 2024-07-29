using System.Collections.Generic;

public interface ILeaderboardPlayer
{
   string Id { get; set; }
   string Username { get; set; }
   string AvatarUrl { get; set; }
   uint Rank { get; set; }
   Dictionary<string, string> Stats { get; set; }
   void AddStat(string key, string value);
}
