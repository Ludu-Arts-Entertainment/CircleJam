using System;

[Serializable]
public class MailConfig : IConfig
{
    public DailyGiftConfig DailyGiftConfig = new ();
}