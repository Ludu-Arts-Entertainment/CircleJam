using System;
using UnityEngine;

public interface IPayer
{
    void Pay(ListOfProductBlock product, Action onComplete, Action onFail);
    void Pay(PriceBlock priceBlock, Action onComplete, Action onFail);
    bool CanPay(PriceBlock priceBlock);
}
public abstract class BasePayer : ScriptableObject, IPayer
{
    public abstract void Pay(ListOfProductBlock product, Action onComplete, Action onFail);
    public abstract void Pay(PriceBlock priceBlock, Action onComplete, Action onFail);
    public abstract bool CanPay(PriceBlock priceBlock);
} 