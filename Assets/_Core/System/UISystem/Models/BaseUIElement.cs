using UnityEngine;

public abstract class BaseUIElement : MonoBehaviour
{
    public virtual void Process()
    { }
    public abstract void Initialize();
    public abstract void Dispose();
}