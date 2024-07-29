using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementProviderEnums
{
    CircleJamMovementProvider,
}
public static class MovementProviderFactory
{
    private static Dictionary<MovementProviderEnums, IMovementProvider> _movementProviderDictionary = new ()
    {
        {MovementProviderEnums.CircleJamMovementProvider, new CircleJamMovementProvider()}
    };
    
    public static IMovementProvider Create(MovementProviderEnums providerEnum)
    {
        return _movementProviderDictionary.TryGetValue(providerEnum, out var provider) ? provider.CreateSelf() : null;
    }
}