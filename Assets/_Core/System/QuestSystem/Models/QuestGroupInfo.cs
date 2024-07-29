using System;
using System.Collections.Generic;

public record QuestGroupInfo
{
    public DateTime StartTime;
    public DateTime EndTime;
    public DateTime DeleteTime;
    public List<Quest> List;

    public QuestGroupInfo(DateTime startTime, DateTime endTime, List<Quest> list, DateTime deleteTime = default)
    {
        this.StartTime = startTime;
        this.EndTime = endTime;
        this.List = list;
        this.DeleteTime = deleteTime == default ? endTime.AddDays(5) : deleteTime;
    }
}