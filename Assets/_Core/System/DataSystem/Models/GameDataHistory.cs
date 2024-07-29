using System.Collections.Generic;

public class GameDataHistory
{
    /// <summary>
    /// This Key-Value pair is used to store the revision number of each device of player with same account. 
    /// </summary>
    /// <typeparam name="string">is represents Device Unique Identifier</typeparam>
    /// <typeparam name="uint">is represents Data Version Number</typeparam>
    public Dictionary<string,uint> Records = new Dictionary<string, uint>();
}