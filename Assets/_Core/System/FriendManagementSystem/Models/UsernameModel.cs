using System;

public class UsernameModel
{
    private string _username;
    private string _uniqueNumber;
    private const string _middleChar = "#";

    public UsernameModel(string rawUsername)
    {
        var lastSplitIndex = rawUsername.LastIndexOf(_middleChar, StringComparison.Ordinal);
        if (lastSplitIndex !=-1) rawUsername = rawUsername.Remove(lastSplitIndex,1);
        Parse(rawUsername);
    }
    public void Parse(string rawUsername)
    {
        try
        {
            _username = rawUsername[..^6];
        }
        catch (Exception e)
        {
            _username = "Guest";
        }

        try
        {
            _uniqueNumber = rawUsername[^6..];
        }
        catch (Exception e)
        {
            _uniqueNumber = "000000";
        }
    }
    public string ShortUsername
    {
        get { return _username; }
        set { _username = value; }
    }

    public string LongUsername
    {
        get { return _username + _middleChar + _uniqueNumber; }
    }

    public string UniqueNumber
    {
        get { return _uniqueNumber; }
        set { _uniqueNumber = value; }
    }

    public string MiddleChar
    {
        get { return _middleChar; }
    }

}
