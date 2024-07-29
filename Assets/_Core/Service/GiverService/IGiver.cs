using System;
using System.Collections.Generic;
using UnityEngine;

public interface IGiver
{
    void Give(ProductBlock productBlock, Action onComplete, Action onFail = null);
}
public abstract class BaseGiver : ScriptableObject, IGiver
{
    public abstract void Give(ProductBlock productBlock, Action onComplete, Action onFail = null);
} 