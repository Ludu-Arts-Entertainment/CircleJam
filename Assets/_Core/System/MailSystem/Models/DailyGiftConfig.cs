using System;

[Serializable]
public class DailyGiftConfig
{
    public long Amount = -1;
    public long ExpireTime  = 86400;
    public long SendInterval = 86400;
    public string Title = $"{0} sent a surprise gift for you!";
    public string Message = "You have received a daily gift";
    public ProductBlock[] Products = new ProductBlock[]
    {
        new (){amount = 10, type = ProductBlockType.Currency, subType = ProductBlockSubType.Coin}
    };
}