using System.Collections.Generic;

public enum InputProviderEnums
{
    LeanTouchInputProvider
}
public static class InputProviderFactory
{
    private static Dictionary<InputProviderEnums, IInputProvider> _inputProviderDictionary = new ()
    {
        {InputProviderEnums.LeanTouchInputProvider , new LeanTouchInputProvider()},
    };
    
    public static IInputProvider Create(InputProviderEnums providerEnum)
    {
        return _inputProviderDictionary.TryGetValue(providerEnum, out var provider) ? provider.CreateSelf() : null;
    }
}