using UnityEngine;

public interface IRequirement
{
    bool Check(float minValue, float maxValue);
}
public abstract class BaseRequirement : ScriptableObject, IRequirement
{
    public abstract bool Check(float minValue, float maxValue);
}